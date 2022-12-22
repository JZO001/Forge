/* *********************************************************************
 * Date: 14 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Forge.Logging.Abstraction;
using Forge.Net.Synapse;
using Forge.Net.TerraGraf.Contexts;
using Forge.Net.TerraGraf.Formatters;
using Forge.Net.TerraGraf.Messaging;
using Forge.Net.TerraGraf.NetworkPeers;
using Forge.Shared;

namespace Forge.Net.TerraGraf.Connection
{

    /// <summary>
    /// This is a server which sends UDP detection broadcast messages
    /// </summary>
    internal sealed class BroadcastServer
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<BroadcastServer>();

        private static readonly Forge.Threading.ThreadPool mThreadPool = new Forge.Threading.ThreadPool("TerraGraf_Network_BroadcastServer");

        private readonly MessageFormatter<UdpBroadcastMessage> mMessageFormatter = new MessageFormatter<UdpBroadcastMessage>();

        private readonly IPEndPoint mBroadcastEp = null;

        private readonly System.Net.Sockets.UdpClient mUdpClient = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastServer"/> class.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="client">The client.</param>
        internal BroadcastServer(IPEndPoint endPoint, System.Net.Sockets.UdpClient client)
        {
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            if (client == null)
            {
                ThrowHelper.ThrowArgumentNullException("client");
            }
            mBroadcastEp = endPoint;
            mUdpClient = client;
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Begins the receive.
        /// </summary>
        internal void BeginReceive()
        {
            mUdpClient.BeginReceive(new AsyncCallback(Callback), null);
        }

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        internal void Shutdown()
        {
            if (mUdpClient != null)
            {
                mUdpClient.Close();
            }
        }

        #endregion

        #region Private method(s)

        private void Callback(IAsyncResult result)
        {
            try
            {
                UdpBroadcastMessage message = null;
                IPEndPoint ep = mBroadcastEp;
                using (MemoryStream ms = new MemoryStream(mUdpClient.EndReceive(result, ref ep)))
                {
                    ms.Position = 0;
                    message = mMessageFormatter.Read(ms);
                    ms.SetLength(0);
                }
                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("BROADCAST_SERVER, a broadcast message arrived from '{0}'.", message.SenderId));
                if (NetworkManager.Instance.InternalLocalhost.Id.Equals(message.SenderId))
                {
                    if (LOGGER.IsInfoEnabled) LOGGER.Info("BROADCAST_SERVER, this broadcast message arrived from me.");
                }
                else
                {
                    if (NetworkManager.Instance.NetworkContextRuleManager.CheckSeparation(NetworkManager.Instance.InternalLocalhost.NetworkContext.Name,
                        message.NetworkContextName))
                    {
                        // I can access this context, trying to connect...
                        INetworkPeerRemote peer = NetworkPeerContext.GetNetworkPeerById(message.SenderId);
                        if (peer == null || (peer != null && peer.Distance != 1))
                        {
                            // there is no direct connection with this peer
                            mThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ConnectionTask), message);
                        }
                        else
                        {
                            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("BROADCAST_SERVER, this is a known peer with direct network connection. No need to establish a new. PeerId: '{0}'.", peer.Id));
                        }
                    }
                    else
                    {
                        if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("BROADCAST_SERVER, network context separation is active between '{0}' and '{1}'.", NetworkManager.Instance.InternalLocalhost.NetworkContext.Name, message.NetworkContextName));
                    }
                }
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("BROADCAST_SERVER, failed to receive a broadcast message. Reason: {0}", ex.Message));
            }
            finally
            {
                BeginReceive();
            }
        }

        private static void ConnectionTask(object state)
        {
            UdpBroadcastMessage message = (UdpBroadcastMessage)state;
            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("BROADCAST_SERVER, processing message of sender, id: {0}", message.SenderId));
            if (message.TCPServers != null && message.TCPServers.Length > 0)
            {
                foreach (AddressEndPoint ep in message.TCPServers)
                {
                    if (ep.AddressFamily == AddressFamily.InterNetwork || (ep.AddressFamily == AddressFamily.InterNetworkV6 && NetworkManager.Instance.InternalConfiguration.Settings.EnableIPV6))
                    {
                        try
                        {
                            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("BROADCAST_SERVER, connecting to [{0}] with address [{1}] and port {2}.", message.SenderId, ep.Host, ep.Port));
                            Synapse.NetworkStream stream = NetworkManager.Instance.InternalNetworkManager.Connect(ep);
                            stream.SendBufferSize = NetworkManager.Instance.InternalConfiguration.Settings.DefaultLowLevelSocketSendBufferSize;
                            stream.ReceiveBufferSize = NetworkManager.Instance.InternalConfiguration.Settings.DefaultLowLevelSocketReceiveBufferSize;
                            stream.NoDelay = NetworkManager.Instance.InternalConfiguration.Settings.DefaultLowLevelNoDelay;
                            NetworkPeerRemote remotePeer = NetworkManager.Instance.ProcessConnection(stream, NetworkManager.Instance.InternalConfiguration.Settings.DefaultConnectionTimeoutInMS, true, !NetworkManager.Instance.InternalConfiguration.Settings.EnableMultipleConnectionWithNetworkPeers);
                            if (NetworkManager.Instance.IsShutdown)
                            {
                                stream.Close();
                                break;
                            }
                            if (remotePeer != null && remotePeer.Id.Equals(message.SenderId))
                            {
                                // connection established, no more trying to connect to
                                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("BROADCAST_SERVER, successfully connected to '{0}' on TCP server.", message.SenderId));
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("BROADCAST_SERVER, failed to connect to [{0}] with address [{1}] and port {2}. Reason: {3}", message.SenderId, ep.Host, ep.Port, ex.Message));
                        }
                    }
                    else
                    {
                        if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("BROADCAST_SERVER, connecting to [{0}] with address [{1}] not allowed. IPV6 protocol disabled.", message.SenderId, ep.Host));
                    }
                }
            }
            INetworkPeerRemote peer = NetworkPeerContext.GetNetworkPeerById(message.SenderId);
            if (peer == null || (peer != null && peer.Distance != 1))
            {
                if (message.NATGateways != null && message.NATGateways.Length > 0)
                {
                    foreach (AddressEndPoint ep in message.NATGateways)
                    {
                        if (ep.AddressFamily == AddressFamily.InterNetwork || (ep.AddressFamily == AddressFamily.InterNetworkV6 && NetworkManager.Instance.InternalConfiguration.Settings.EnableIPV6))
                        {
                            try
                            {
                                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("BROADCAST_SERVER, connecting to [{0}] with NAT address [{1}] and port {2}.", message.SenderId, ep.Host, ep.Port));
                                Synapse.NetworkStream stream = NetworkManager.Instance.InternalNetworkManager.Connect(ep);
                                stream.SendBufferSize = NetworkManager.Instance.InternalConfiguration.Settings.DefaultLowLevelSocketSendBufferSize;
                                stream.ReceiveBufferSize = NetworkManager.Instance.InternalConfiguration.Settings.DefaultLowLevelSocketReceiveBufferSize;
                                stream.NoDelay = NetworkManager.Instance.InternalConfiguration.Settings.DefaultLowLevelNoDelay;
                                NetworkPeerRemote remotePeer = NetworkManager.Instance.ProcessConnection(stream, NetworkManager.Instance.InternalConfiguration.Settings.DefaultConnectionTimeoutInMS, true, !NetworkManager.Instance.InternalConfiguration.Settings.EnableMultipleConnectionWithNetworkPeers);
                                if (NetworkManager.Instance.IsShutdown)
                                {
                                    stream.Close();
                                    break;
                                }
                                if (remotePeer != null && remotePeer.Id.Equals(message.SenderId))
                                {
                                    // connection established, no more trying to connect to
                                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("BROADCAST_SERVER, successfully connected to '{0}' on NAT address.", message.SenderId));
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("BROADCAST_SERVER, failed to connect to [{0}] with address [{1}] and port {2}. Reason: {3}", message.SenderId, ep.Host, ep.Port, ex.Message));
                            }
                        }
                        else
                        {
                            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("BROADCAST_SERVER, connecting to [{0}] with address [{1}] not allowed. IPV6 protocol disabled.", message.SenderId, ep.Host));
                        }
                    }
                }
            }
        }

        #endregion

    }
}
