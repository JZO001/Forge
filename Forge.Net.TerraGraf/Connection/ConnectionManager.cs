/* *********************************************************************
 * Date: 14 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Forge.Collections;
using Forge.Logging.Abstraction;
using Forge.Net.Synapse;
using Forge.Net.TerraGraf.Configuration;
using Forge.Net.TerraGraf.Formatters;
using Forge.Net.TerraGraf.Messaging;
using Forge.Net.TerraGraf.NetworkPeers;
#if IS_WINDOWS
using Forge.Net.Synapse.NATUPnP;
using NATUPNPLib;
#endif

namespace Forge.Net.TerraGraf.Connection
{

    /// <summary>
    /// Process and manage the connection tasks
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    internal sealed class ConnectionManager
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<ConnectionManager>();

        private Forge.Threading.ThreadPool mThreadPool = new Forge.Threading.ThreadPool("TerraGraf_Network_Connection");

        private List<ConnectionTask> mConnectionTasks = new List<ConnectionTask>();

        private Thread mDelayThread = null;

        private Semaphore mSemaphore = new Semaphore(0, int.MaxValue);

        private ListSpecialized<ConnectionTask> mDelayedConnectionTasks = new ListSpecialized<ConnectionTask>();

        private List<BroadcastServer> mBroadcastServers = new List<BroadcastServer>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionManager"/> class.
        /// </summary>
        internal ConnectionManager()
        {
            mDelayThread = new Thread(new ThreadStart(DelayThread));
            mDelayThread.Name = "ConnectionManager_DelayThread";
            mDelayThread.IsBackground = true;
            mDelayThread.Start();
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Initializes the TCP servers.
        /// </summary>
        internal void InitializeTCPServers()
        {
            List<TCPServer> servers = NetworkManager.Instance.InternalLocalhost.TCPServerCollection.TCPServers;
            servers.Clear();
            if (NetworkManager.Instance.InternalConfiguration.NetworkPeering.TCPServers.Auto)
            {
                // automatic network peer detection
                foreach (IPAddress a in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (a.AddressFamily == AddressFamily.InterNetwork || (a.AddressFamily == AddressFamily.InterNetworkV6 &&
                        NetworkManager.Instance.InternalConfiguration.Settings.EnableIPV6))
                    {
                        try
                        {
                            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("CONNECTION_MANAGER, starting TCP server on interface: {0}", a.ToString()));
                            long serverId = NetworkManager.Instance.InternalNetworkManager.StartServer(new AddressEndPoint(a.ToString(), 0));
                            AddressEndPoint serverEp = NetworkManager.Instance.InternalNetworkManager.GetServerEndPoint(serverId);
                            servers.Add(new TCPServer(serverId, serverEp, false));
                            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("CONNECTION_MANAGER, TCP server started on interface: {0}, port: {1}, server id: {2}", a.ToString(), serverEp.Port, serverId));
                        }
                        catch (Exception ex)
                        {
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("CONNECTION_MANAGER, failed to start server on interface '{0}' on any port. Reason: {1}", a.ToString(), ex.Message));
                        }
                    }
                }
            }
            else
            {
                // kézi beállítások
                foreach (AddressEndPoint ep in NetworkManager.Instance.InternalConfiguration.NetworkPeering.TCPServers.EndPoints)
                {
                    IPAddress a = null;
                    if (IPAddress.TryParse(ep.Host, out a))
                    {
                        if (a.AddressFamily == AddressFamily.InterNetwork || (a.AddressFamily == AddressFamily.InterNetworkV6 &&
                            NetworkManager.Instance.InternalConfiguration.Settings.EnableIPV6))
                        {
                            if (IPAddress.Equals(a, IPAddress.Any) || IPAddress.Equals(a, IPAddress.IPv6Any))
                            {
                                foreach (IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName()))
                                {
                                    if (ip.AddressFamily == AddressFamily.InterNetwork || (ip.AddressFamily == AddressFamily.InterNetworkV6 &&
                                        NetworkManager.Instance.InternalConfiguration.Settings.EnableIPV6))
                                    {
                                        try
                                        {
                                            long serverId = NetworkManager.Instance.InternalNetworkManager.StartServer(new AddressEndPoint(ip.ToString(), ep.Port));
                                            AddressEndPoint serverEp = NetworkManager.Instance.InternalNetworkManager.GetServerEndPoint(serverId);
                                            servers.Add(new TCPServer(serverId, serverEp, false));
                                        }
                                        catch (Exception ex)
                                        {
                                            if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("CONNECTION_MANAGER, failed to start server on interface '{0}' on any port. Reason: {1}", ip.ToString(), ex.Message));
                                        }
                                    }
                                }
                                foreach (IPAddress ip in Dns.GetHostAddresses("localhost"))
                                {
                                    if (ip.AddressFamily == AddressFamily.InterNetwork || (ip.AddressFamily == AddressFamily.InterNetworkV6 &&
                                        NetworkManager.Instance.InternalConfiguration.Settings.EnableIPV6))
                                    {
                                        try
                                        {
                                            NetworkManager.Instance.InternalNetworkManager.StartServer(new AddressEndPoint(ip.ToString(), ep.Port));
                                        }
                                        catch (Exception ex)
                                        {
                                            if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("CONNECTION_MANAGER, failed to start server on interface '{0}' on any port. Reason: {1}", ip.ToString(), ex.Message));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("CONNECTION_MANAGER, starting TCP server on interface: {0} and port {1}", a.ToString(), ep.Port));
                                    long serverId = NetworkManager.Instance.InternalNetworkManager.StartServer(new AddressEndPoint(a.ToString(), ep.Port));
                                    AddressEndPoint serverEp = NetworkManager.Instance.InternalNetworkManager.GetServerEndPoint(serverId);
                                    servers.Add(new TCPServer(serverId, serverEp, false));
                                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("CONNECTION_MANAGER, TCP server started on interface: {0}, port: {1}, server id: {2}", a.ToString(), serverEp.Port, serverId));
                                }
                                catch (Exception ex)
                                {
                                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("CONNECTION_MANAGER, failed to start server on interface '{0}' and port {1}. Reason: {2}", ep.Host, ep.Port.ToString(), ex.Message));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("CONNECTION_MANAGER, failed to resolve interface: {0}", ep.Host));
                    }
                }
            }
        }

        //#if NETCOREAPP3_1_OR_GREATER
#if IS_WINDOWS

        /// <summary>
        /// Initializes the NATUPnP service.
        /// </summary>
        internal void InitializeNATUPnPService()
        {
            try
            {
                if (NetworkManager.Instance.InternalConfiguration.NetworkPeering.NATUPnPSettings.IsEnabled)
                {
                    List<TCPServer> servers = NetworkManager.Instance.InternalLocalhost.TCPServerCollection.TCPServers;
                    if (servers.Count > 0)
                    {
                        IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
                        if (addresses != null)
                        {
                            IPAddress defAddress = null;
                            if (addresses.Length == 1)
                            {
                                defAddress = addresses[0];
                            }
                            else if (!string.IsNullOrEmpty(NetworkManager.Instance.InternalConfiguration.NetworkPeering.NATUPnPSettings.ProbeAddress))
                            {
                                // probe
                                string probeAddress = NetworkManager.Instance.InternalConfiguration.NetworkPeering.NATUPnPSettings.ProbeAddress;
                                string[] nameAndPort = probeAddress.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                string hostName = string.Empty;
                                int port = 80;
                                hostName = nameAndPort[0];
                                if (nameAndPort.Length > 1)
                                {
                                    port = int.Parse(nameAndPort[1]);
                                }

                                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(hostName, port);
                                try
                                {
                                    defAddress = ((IPEndPoint)client.Client.LocalEndPoint).Address;
                                }
                                finally
                                {
                                    client.Close();
                                }
                            }
                            if (defAddress != null)
                            {
                                // megnézem van-e nyitott TCP szerver ezen az interface-en
                                TCPServer selectedServer = null;
                                foreach (TCPServer server in servers)
                                {
                                    if (server.EndPoint.Host.Equals(IPAddress.Any.ToString()) || server.EndPoint.Host.Equals(IPAddress.IPv6Any.ToString()) ||
                                        server.EndPoint.Host.Equals(defAddress.ToString()))
                                    {
                                        selectedServer = server;
                                        break;
                                    }
                                }
                                if (selectedServer != null)
                                {
                                    // erre a TCP szerverre és a megadott interface-en keresztül lehet portot nyitni
                                    using (UPnPManager manager = new UPnPManager())
                                    {
                                        manager.Initialize();
                                        bool exist = false;
                                        foreach (IStaticPortMapping mapping in manager.GetStaticPortMappings())
                                        {
                                            if (mapping.ExternalPort == selectedServer.EndPoint.Port && mapping.InternalClient.Equals(selectedServer.EndPoint.Host))
                                            {
                                                exist = true;
                                                if (!mapping.Enabled)
                                                {
                                                    mapping.Enable(true);
                                                }
                                                break;
                                            }
                                        }
                                        if (!exist)
                                        {
                                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("CONNECTION_MANAGER, opening UPnP port {0} to local interface: {1}", selectedServer.EndPoint.Port, selectedServer.EndPoint.Host));
                                            IStaticPortMapping mapping = manager.AddNATUPnPPortMapping(selectedServer.EndPoint.Port, ProtocolEnum.TCP, selectedServer.EndPoint.Port, defAddress, true, NetworkManager.Configuration.Settings.ApplicationName);
                                            if (mapping == null)
                                            {
                                                if (LOGGER.IsErrorEnabled) LOGGER.Error("CONNECTION_MANAGER, failed to add UPnP port. Is there any UPnP service exist?");
                                            }
                                            else
                                            {
                                                exist = false;
                                                foreach (NATGateway gw in NetworkManager.Instance.InternalLocalhost.NATGatewayCollection.NATGateways)
                                                {
                                                    if (gw.EndPoint.Host.Equals(mapping.ExternalIPAddress) && gw.EndPoint.Port.Equals(mapping.ExternalPort))
                                                    {
                                                        // I have a server like this
                                                        exist = true;
                                                        break;
                                                    }
                                                }
                                                if (!exist)
                                                {
                                                    NetworkManager.Instance.InternalLocalhost.NATGatewayCollection.NATGateways.Add(new NATGateway(new AddressEndPoint(mapping.ExternalIPAddress, mapping.ExternalPort)));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error("CONNECTION_MANAGER, failed to initialize NATUPnP service.", ex);
            }
        }

#endif

        /// <summary>
        /// Initializes the TCP connections.
        /// </summary>
        internal void InitializeTCPConnections()
        {
            if (NetworkManager.Instance.InternalConfiguration.NetworkPeering.TCPConnections.EndPoints.Count > 0)
            {
                foreach (ConnectionEntry e in NetworkManager.Instance.InternalConfiguration.NetworkPeering.TCPConnections.EndPoints)
                {
                    ConnectionTask task = new ConnectionTask(e.EndPoint, e.ReconnectOnFailure, e.DelayBetweenAttempsInMS, e.ConnectionTimeout);
                    mConnectionTasks.Add(task);
                    mThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ProcessConnection), task);
                }
            }
        }

        /// <summary>
        /// Initializes the UDP detector.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal void InitializeUDPDetector()
        {
            if (NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.Enabled)
            {
                {
                    // UDP szerver indítása
                    List<int> listeningPorts = NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.BroadcastListeningPorts;
                    if (listeningPorts.Count > 0)
                    {
                        if (NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.DetectionMode == UDPDetectionModeEnum.Multicast)
                        {
                            Action<AddressFamily, string> initMulticastUdpClient = ((a, ip) =>
                            {
                                IPEndPoint broadcastEp = null;
                                System.Net.Sockets.UdpClient udpClient = null;
                                foreach (int port in listeningPorts)
                                {
                                    try
                                    {
                                        if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("CONNECTION_MANAGER, trying to initialize broadcast detector on port {0} ({1}).", port, a.ToString()));
                                        IPAddress multicastAddress = IPAddress.Parse(ip); // (239.0.0.222)
                                        broadcastEp = new IPEndPoint(a == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, port);
                                        udpClient = new System.Net.Sockets.UdpClient(a);

                                        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                                        udpClient.ExclusiveAddressUse = false;
                                        udpClient.Client.Bind(broadcastEp);
                                        udpClient.EnableBroadcast = true;
                                        udpClient.MulticastLoopback = true;
#if IS_WINDOWS
                                        udpClient.AllowNatTraversal(true);
#endif

                                        udpClient.JoinMulticastGroup(multicastAddress);

                                        if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("CONNECTION_MANAGER, broadcast detector initialized on port {0} ({1}).", port, a.ToString()));
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("CONNECTION_MANAGER, failed to initialize broadcast detector on port {0} ({1}). Reason: {2}", port, a.ToString(), ex.Message), ex);
                                    }
                                }
                                if (udpClient != null)
                                {
                                    BroadcastServer server = new BroadcastServer(broadcastEp, udpClient);
                                    mBroadcastServers.Add(server);
                                    server.BeginReceive();
                                }
                            });
                            if (!string.IsNullOrEmpty(NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.IPv4MulticastAddress))
                                initMulticastUdpClient(AddressFamily.InterNetwork, NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.IPv4MulticastAddress);
                            if (NetworkManager.Instance.InternalConfiguration.Settings.EnableIPV6 && !string.IsNullOrEmpty(NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.IPv6MulticastAddress))
                                initMulticastUdpClient(AddressFamily.InterNetworkV6, NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.IPv6MulticastAddress);
                        }

                        if (NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.DetectionMode == UDPDetectionModeEnum.Broadcast)
                        {
                            Action<AddressFamily> initUdpClient = (a =>
                            {
                                IPEndPoint broadcastEp = null;
                                System.Net.Sockets.UdpClient udpClient = null;
                                foreach (int port in listeningPorts)
                                {
                                    try
                                    {
                                        if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("CONNECTION_MANAGER, trying to initialize broadcast detector on port {0} ({1}).", port, a.ToString()));
                                        broadcastEp = new IPEndPoint(a == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, port);
                                        udpClient = new System.Net.Sockets.UdpClient(broadcastEp);
                                        udpClient.EnableBroadcast = true;
#if IS_WINDOWS
                                        udpClient.AllowNatTraversal(true);
#endif
                                        if (a == AddressFamily.InterNetwork) udpClient.DontFragment = true;
                                        if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("CONNECTION_MANAGER, broadcast detector initialized on port {0} ({1}).", port, a.ToString()));
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("CONNECTION_MANAGER, failed to initialize broadcast detector on port {0} ({1}). Reason: {2}", port, a.ToString(), ex.Message), ex);
                                    }
                                }
                                if (udpClient != null)
                                {
                                    BroadcastServer server = new BroadcastServer(broadcastEp, udpClient);
                                    mBroadcastServers.Add(server);
                                    server.BeginReceive();
                                }
                            });
                            initUdpClient(AddressFamily.InterNetwork);
                            if (NetworkManager.Instance.InternalConfiguration.Settings.EnableIPV6) initUdpClient(AddressFamily.InterNetworkV6);
                        }

                    }
                }
                {
                    // UDP üzenetek szétszórása a hálózatba
                    List<int> targetPorts = NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.BroadcastTargetPorts;
                    if (targetPorts.Count > 0)
                    {
                        AddressEndPoint[] addressEps = null;
                        AddressEndPoint[] ipEps = null;
                        if (NetworkManager.Instance.InternalLocalhost.NATGatewayCollection.NATGateways.Count > 0)
                        {
                            addressEps = new AddressEndPoint[NetworkManager.Instance.InternalLocalhost.NATGatewayCollection.NATGateways.Count];
                            for (int i = 0; i < addressEps.Length; i++)
                            {
                                addressEps[i] = NetworkManager.Instance.InternalLocalhost.NATGatewayCollection.NATGateways[i].EndPoint;
                            }
                        }
                        if (NetworkManager.Instance.InternalLocalhost.TCPServerCollection.TCPServers.Count > 0)
                        {
                            ipEps = new AddressEndPoint[NetworkManager.Instance.InternalLocalhost.TCPServerCollection.TCPServers.Count];
                            for (int i = 0; i < ipEps.Length; i++)
                            {
                                ipEps[i] = NetworkManager.Instance.InternalLocalhost.TCPServerCollection.TCPServers[i].EndPoint;
                            }
                        }
                        if (addressEps != null || ipEps != null)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                {
                                    UdpBroadcastMessage message = new UdpBroadcastMessage(NetworkManager.Instance.InternalLocalhost.Id,
                                        NetworkManager.Instance.InternalLocalhost.NetworkContext.Name, addressEps, ipEps);
                                    MessageFormatter<UdpBroadcastMessage> formatter = new MessageFormatter<UdpBroadcastMessage>();
                                    formatter.Write(ms, message);
                                    ms.Position = 0;
                                }

                                // multicast
                                if (NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.DetectionMode == UDPDetectionModeEnum.Multicast)
                                {
                                    // multicast
                                    foreach (int port in targetPorts)
                                    {
                                        Action<AddressFamily, string> sendMulticastUdpClient = ((a, ip) =>
                                        {
                                            using (System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient(a))
                                            {
                                                udpClient.MulticastLoopback = true;
                                                udpClient.EnableBroadcast = true;
#if IS_WINDOWS
                                                udpClient.AllowNatTraversal(true);
#endif
                                                try
                                                {
                                                    IPAddress multicastaddress = IPAddress.Parse(ip);
                                                    udpClient.JoinMulticastGroup(multicastaddress);
                                                    IPEndPoint remoteEp = new IPEndPoint(multicastaddress, port);
                                                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("CONNECTION_MANAGER, sending multicast message on port {0}. ({1})", port, a.ToString()));
                                                    udpClient.Send(ms.ToArray(), Convert.ToInt32(ms.Length), remoteEp);
                                                    udpClient.DropMulticastGroup(multicastaddress);
                                                }
                                                catch (Exception ex)
                                                {
                                                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("CONNECTION_MANAGER, failed to send multicast message ({0}). Reason: {1}", a.ToString(), ex.Message));
                                                }
                                            }
                                        });
                                        if (!string.IsNullOrEmpty(NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.IPv4MulticastAddress))
                                            sendMulticastUdpClient(AddressFamily.InterNetwork, NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.IPv4MulticastAddress);
                                        if (NetworkManager.Instance.InternalConfiguration.Settings.EnableIPV6 && !string.IsNullOrEmpty(NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.IPv6MulticastAddress))
                                            sendMulticastUdpClient(AddressFamily.InterNetworkV6, NetworkManager.Instance.InternalConfiguration.NetworkPeering.UDPDetection.IPv6MulticastAddress);
                                    }
                                }
                                else
                                {
                                    // broadcast
                                    using (System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient())
                                    {
                                        udpClient.MulticastLoopback = false;
                                        udpClient.EnableBroadcast = true;
#if IS_WINDOWS
                                        udpClient.AllowNatTraversal(true);
#endif
                                        udpClient.DontFragment = true;
                                        foreach (int port in targetPorts)
                                        {
                                            try
                                            {
                                                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("CONNECTION_MANAGER, sending broadcast message on port {0}.", port));
                                                udpClient.Send(ms.ToArray(), Convert.ToInt32(ms.Length), new IPEndPoint(IPAddress.Broadcast, port));
                                            }
                                            catch (Exception ex)
                                            {
                                                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("CONNECTION_MANAGER, failed to send broadcast message. Reason: {0}", ex.Message));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (LOGGER.IsInfoEnabled) LOGGER.Info("CONNECTION_MANAGER, both of list NAT gateways and TCP servers are empties for broadcast detection.");
                        }
                    }
                    else
                    {
                        if (LOGGER.IsInfoEnabled) LOGGER.Info("CONNECTION_MANAGER, no target udp port definied for broadcast detection.");
                    }
                }
            }
            else
            {
                if (LOGGER.IsInfoEnabled) LOGGER.Info("CONNECTION_MANAGER, broadcast detection disabled.");
            }
        }

        /// <summary>
        /// Networks the connection_ disconnect.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void NetworkConnection_Disconnect(Synapse.NetworkStream stream)
        {
            foreach (ConnectionTask task in mConnectionTasks)
            {
                if (task.NetworkStream != null && task.NetworkStream.Equals(stream))
                {
                    // ez a kapcsolat ment szét
                    if (task.ReconnectOnFailure)
                    {
                        // újra időzítés
                        QueueToReconnect(task);
                    }
                    else
                    {
                        lock (mConnectionTasks)
                        {
                            mConnectionTasks.Remove(task);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        internal void Shutdown()
        {
            if (mBroadcastServers.Count > 0)
            {
                Parallel.ForEach(mBroadcastServers, i => i.Shutdown());
            }
        }

        #endregion

        #region Private method(s)

        private void ProcessConnection(object state)
        {
            if (!NetworkManager.Instance.IsShutdown)
            {
                ConnectionTask task = (ConnectionTask)state;
                try
                {
                    task.NetworkStream = null;
                    Synapse.NetworkStream stream = NetworkManager.Instance.InternalNetworkManager.Connect(task.EndPoint); // dobhat hibát
                    stream.SendBufferSize = NetworkManager.Instance.InternalConfiguration.Settings.DefaultLowLevelSocketSendBufferSize;
                    stream.ReceiveBufferSize = NetworkManager.Instance.InternalConfiguration.Settings.DefaultLowLevelSocketReceiveBufferSize;
                    stream.NoDelay = NetworkManager.Instance.InternalConfiguration.Settings.DefaultLowLevelNoDelay;
                    task.NetworkStream = stream;
                    NetworkPeerRemote result = NetworkManager.Instance.ProcessConnection(stream, task.ConnectionTimeout, true, !NetworkManager.Instance.InternalConfiguration.Settings.EnableMultipleConnectionWithNetworkPeers);
                    if (result == null)
                    {
                        if (task.ReconnectOnFailure)
                        {
                            // újra időzítés
                            QueueToReconnect(task);
                        }
                        else
                        {
                            lock (mConnectionTasks)
                            {
                                mConnectionTasks.Remove(task);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // failed to resolve host
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("CONNECTION_MANAGER, failed to establish connection to a remote host. Reason: {0}", ex.Message));
                    if (task.ReconnectOnFailure)
                    {
                        // újra időzítés
                        QueueToReconnect(task);
                    }
                    else
                    {
                        lock (mConnectionTasks)
                        {
                            mConnectionTasks.Remove(task);
                        }
                    }
                }
            }
        }

        private void QueueToReconnect(ConnectionTask task)
        {
            if (!NetworkManager.Instance.IsShutdown)
            {
                lock (mDelayedConnectionTasks)
                {
                    task.Timeout = DateTime.Now.AddMilliseconds(task.DelayBetweenAttempsInMS);
                    mDelayedConnectionTasks.Add(task);
                }
                mSemaphore.Release();
            }
        }

        private void DelayThread()
        {
            int delay = 0;
            while (true)
            {
                mSemaphore.WaitOne(delay);
                delay = Timeout.Infinite;
                lock (mDelayedConnectionTasks)
                {
                    IEnumeratorSpecialized<ConnectionTask> e = mDelayedConnectionTasks.GetEnumerator();
                    while (e.MoveNext())
                    {
                        ConnectionTask task = e.Current;
                        long currentTicks = DateTime.Now.Ticks;
                        if (currentTicks > task.Timeout.Ticks)
                        {
                            // mehet újra
                            e.Remove();
                            mThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ProcessConnection), task);
                        }
                        else
                        {
                            // csökkentem a következő várakozási időt
                            int delayValue = Convert.ToInt32(TimeSpan.FromTicks(task.Timeout.Ticks - currentTicks).TotalMilliseconds);
                            if (delayValue < delay || delay == Timeout.Infinite)
                            {
                                delay = delayValue;
                            }
                        }
                    }
                }
            }
        }

        private sealed class ConnectionTask : ConnectionEntry
        {

            private Synapse.NetworkStream mNetworkStream = null;
            private DateTime mTimeout = DateTime.Now;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectionTask"/> class.
            /// </summary>
            /// <param name="endPoint">The end point.</param>
            /// <param name="reconnectOnFailure">if set to <c>true</c> [reconnect on failure].</param>
            /// <param name="delayBetweenAttemptsInMS">The delay between attempts in MS.</param>
            /// <param name="connectionTimeout">The connection timeout.</param>
            internal ConnectionTask(AddressEndPoint endPoint, bool reconnectOnFailure, int delayBetweenAttemptsInMS, int connectionTimeout)
                : base(endPoint, reconnectOnFailure, delayBetweenAttemptsInMS, connectionTimeout)
            {
            }

            /// <summary>
            /// Gets or sets the network stream.
            /// </summary>
            /// <value>
            /// The network stream.
            /// </value>
            internal Synapse.NetworkStream NetworkStream
            {
                get { return mNetworkStream; }
                set { mNetworkStream = value; }
            }

            /// <summary>
            /// Gets or sets the timeout.
            /// </summary>
            /// <value>
            /// The timeout.
            /// </value>
            internal DateTime Timeout
            {
                get { return mTimeout; }
                set { mTimeout = value; }
            }

        }

        #endregion

    }

}
