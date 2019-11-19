/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Forge.Collections;
using Forge.EventRaiser;
using Forge.Management;
using Forge.Net.Synapse;
using Forge.Net.Synapse.Firewall;
using Forge.Net.Synapse.NetworkServices;
using Forge.Net.TerraGraf.Configuration;
using Forge.Net.TerraGraf.Connection;
using Forge.Net.TerraGraf.Contexts;
using Forge.Net.TerraGraf.Messaging;
using Forge.Net.TerraGraf.NetworkInfo;
using Forge.Net.TerraGraf.NetworkPeers;
using log4net;

namespace Forge.Net.TerraGraf
{

    internal delegate INetworkPeerRemote NetworkManagerConnectDelegate(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory, int timeoutInMS);
    internal delegate bool NetworkManagerInternalConnectDelegate(NetworkPeerRemote networkPeer, bool localRequest, bool disconnectOnConnectionDuplication);

    /// <summary>
    /// Represents and manages the terragraf network
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public sealed class NetworkManager : ManagerSingletonBase<NetworkManager>
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger("Forge.Net.TerraGraf.NetworkManager");

        private static Mutex mMutex = null;

        private static string APPLICATION_ID = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mShutdown = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkPeerLocal mLocalHost = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<NetworkContext> mKnownNetworkContexts = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ConfigContainer mConfiguration = null;

        private NetworkContextRuleManager mNetworkContextRuleManager = null;

        private Synapse.NetworkManager mNetworkManager = null;

        private ConnectionManager mConnectionManager = null;

        private readonly Dictionary<long, MessageTask> mMessagesToAcknowledge = new Dictionary<long, MessageTask>(); // ack-ra váró üzenet task-ok

        private readonly Dictionary<string, Dictionary<long, int>> mUnknownSenderMessageCounter = new Dictionary<string, Dictionary<long, int>>(); // ismeretlen küldőktől nyilvántott üzenetek

        private readonly Dictionary<long, Socket> mBindedSockets = new Dictionary<long, Socket>();

        private readonly ReaderWriterLockSlim mBindedSocketsLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private readonly HashSet<NetworkConnection> mActiveNetworkConnections = new HashSet<NetworkConnection>(); // aktív, küldésre kiválasztott hálózati kapcsolatok

        private readonly Forge.Threading.DeadlockSafeLock mLockMessageProcessor = new Forge.Threading.DeadlockSafeLock("LockMessageProcessor"); // aki a gráf adatokhoz akar nyúlni, erre rá kell lockolnia

        private int mAsyncActiveConnectCount = 0;
        private AutoResetEvent mAsyncActiveConnectEvent = null;
        private NetworkManagerConnectDelegate mConnectDelegate = null;

        private static readonly object LOCK_CONNECT = new object();

        /// <summary>
        /// Occurs when [network peer discovered].
        /// </summary>
        public event EventHandler<NetworkPeerChangedEventArgs> NetworkPeerDiscovered;

        /// <summary>
        /// Occurs when [network peer distance changed].
        /// </summary>
        public event EventHandler<NetworkPeerDistanceChangedEventArgs> NetworkPeerDistanceChanged;

        /// <summary>
        /// Occurs when [network peer unaccessible].
        /// </summary>
        public event EventHandler<NetworkPeerChangedEventArgs> NetworkPeerUnaccessible;

        /// <summary>
        /// Occurs when [network peer context changed].
        /// </summary>
        public event EventHandler<NetworkPeerContextEventArgs> NetworkPeerContextChanged;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="InternalNetworkManager"/> class from being created.
        /// </summary>
        private NetworkManager()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        [DebuggerHidden]
        public ConfigContainer Configuration
        {
            get
            {
                DoInitializationCheck();
                return mConfiguration;
            }
        }

        /// <summary>
        /// Gets the localhost.
        /// </summary>
        /// <value>
        /// The localhost.
        /// </value>
        [DebuggerHidden]
        public INetworkPeerLocal Localhost
        {
            get
            {
                DoInitializationCheck();
                return mLocalHost;
            }
        }

        /// <summary>
        /// Gets the current network context.
        /// </summary>
        /// <value>
        /// The local network context.
        /// </value>
        [DebuggerHidden]
        public NetworkContext LocalNetworkContext
        {
            get
            {
                DoInitializationCheck();
                return mLocalHost.NetworkContext;
            }
        }

        /// <summary>
        /// Gets the active sockets.
        /// </summary>
        /// <value>
        /// The active sockets.
        /// </value>
        public ICollection<ISocketSafeHandle> ActiveSockets
        {
            get
            {
                ListSpecialized<ISocketSafeHandle> list = new ListSpecialized<ISocketSafeHandle>();
                SafeBindedSocketsAccess(() =>
                    {
                        foreach (Socket socket in mBindedSockets.Values)
                        {
                            list.Add(new SocketSafeHandle(socket));
                        }
                    });
                return list;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is shutdown.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is shutdown; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsShutdown
        {
            get { return mShutdown; }
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the internal known network contexts.
        /// </summary>
        /// <value>
        /// The internal known network contexts.
        /// </value>
        [DebuggerHidden]
        internal List<NetworkContext> InternalKnownNetworkContexts
        {
            get { return mKnownNetworkContexts; }
        }

        /// <summary>
        /// Gets the internal configuration.
        /// </summary>
        /// <value>
        /// The internal configuration.
        /// </value>
        /// <exception cref="Forge.InitializationException">Network has not been initialized.</exception>
        [DebuggerHidden]
        internal ConfigContainer InternalConfiguration
        {
            get
            {
                if (mConfiguration == null)
                {
                    throw new InitializationException("Network has not been initialized.");
                }
                return mConfiguration;
            }
        }

        /// <summary>
        /// Gets the network context rule manager.
        /// </summary>
        /// <value>
        /// The network context rule manager.
        /// </value>
        [DebuggerHidden]
        internal NetworkContextRuleManager NetworkContextRuleManager
        {
            get { return mNetworkContextRuleManager; }
        }

        /// <summary>
        /// Gets the network manager.
        /// </summary>
        /// <value>
        /// The internal network manager.
        /// </value>
        [DebuggerHidden]
        internal Synapse.NetworkManager InternalNetworkManager
        {
            get { return mNetworkManager; }
        }

        /// <summary>
        /// Gets the internal localhost.
        /// </summary>
        /// <value>
        /// The internal localhost.
        /// </value>
        [DebuggerHidden]
        internal NetworkPeerLocal InternalLocalhost
        {
            get { return mLocalHost; }
        }

        /// <summary>
        /// Gets the sync root.
        /// </summary>
        /// <value>
        /// The sync root.
        /// </value>
        internal Forge.Threading.DeadlockSafeLock SyncRoot
        {
            get { return mLockMessageProcessor; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>Manager State</returns>
        public override ManagerStateEnum Start()
        {
            Initialize(false);
            return this.ManagerState;
        }

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        /// <returns>Manager State</returns>
        public override ManagerStateEnum Stop()
        {
            DoInitializationCheck();
            if (!mShutdown)
            {
                OnStop(ManagerEventStateEnum.Before);
                mLockMessageProcessor.Lock();
                try
                {
                    mNetworkManager.Dispose();
                    mConnectionManager.Shutdown();
                    foreach (NetworkContext nc in new List<NetworkContext>(InternalKnownNetworkContexts))
                    {
                        foreach (INetworkPeerRemote peer in new List<INetworkPeerRemote>(nc.InternalNetworkPeers))
                        {
                            foreach (INetworkConnection c in new List<INetworkConnection>(peer.NetworkConnections))
                            {
                                c.Close();
                            }
                        }
                    }
                    mMutex.Dispose();
                }
                finally
                {
                    mLockMessageProcessor.Unlock();
                    this.ManagerState = ManagerStateEnum.Stopped;
                    mShutdown = true;
                    OnStop(ManagerEventStateEnum.After);
                }
            }
            return this.ManagerState;
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>Identifier of the endpoint</returns>
        public long StartServer(AddressEndPoint endPoint)
        {
            return StartServer(endPoint, mNetworkManager.ServerStreamFactory);
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="serverStreamFactory">The server stream factory.</param>
        /// <returns>Identifier of the endpoint</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)")]
        public long StartServer(AddressEndPoint endPoint, IServerStreamFactory serverStreamFactory)
        {
            DoInitializationCheck();
            DoShutdownCheck();
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            if (serverStreamFactory == null)
            {
                ThrowHelper.ThrowArgumentNullException("serverStreamFactory");
            }
            if (endPoint.Host.Equals(AddressEndPoint.Any) || endPoint.Host.Equals(AddressEndPoint.IPv6Any))
            {
                ThrowHelper.ThrowArgumentException(string.Format("Endpoint address {0} not allowed.", endPoint.Host), "endPoint");
            }

            long serverId = InternalStartTCPServer(endPoint, serverStreamFactory);

            mLockMessageProcessor.Lock();
            try
            {
                lock (mLocalHost.TCPServerCollection.TCPServers)
                {
                    mLocalHost.TCPServerCollection.TCPServers.Add(new TCPServer(serverId, mNetworkManager.GetServerEndPoint(serverId), true));
                }
                InternalSendTCPServers();
            }
            finally
            {
                mLockMessageProcessor.Unlock();
            }

            return serverId;
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <param name="serverId">The server id.</param>
        /// <returns>True, if the server stopped, otherwise False.</returns>
        public bool StopServer(long serverId)
        {
            DoInitializationCheck();
            DoShutdownCheck();
            bool result = false;

            AddressEndPoint ep = this.mNetworkManager.GetServerEndPoint(serverId);
            if (ep != null)
            {
                result = this.mNetworkManager.StopServer(serverId);
                mLockMessageProcessor.Lock();
                try
                {
                    foreach (TCPServer server in mLocalHost.TCPServerCollection.TCPServers)
                    {
                        if (server.EndPoint.Equals(ep))
                        {
                            mLocalHost.TCPServerCollection.TCPServers.Remove(server);
                            break;
                        }
                    }
                    mLocalHost.TCPServerCollection.StateId = mLocalHost.TCPServerCollection.StateId + 1;
                    TerraGrafPeerUpdateMessage message = new TerraGrafPeerUpdateMessage(mLocalHost.Id, mLocalHost.TCPServerCollection.BuildServerContainer());
                    InternalSendMessage(message, null);
                }
                finally
                {
                    mLockMessageProcessor.Unlock();
                }
            }

            return result;
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(AddressEndPoint endPoint, AsyncCallback callback, object state)
        {
            return BeginConnect(endPoint, mNetworkManager.ClientStreamFactory, Timeout.Infinite, callback, state);
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory, AsyncCallback callback, object state)
        {
            return BeginConnect(endPoint, clientStreamFactory, Timeout.Infinite, callback, state);
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="timeoutInMS">The timeout in MS.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(AddressEndPoint endPoint, int timeoutInMS, AsyncCallback callback, object state)
        {
            return BeginConnect(endPoint, mNetworkManager.ClientStreamFactory, timeoutInMS, callback, state);
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="timeoutInMS">The timeout in MS.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory, int timeoutInMS, AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveConnectCount);
            NetworkManagerConnectDelegate d = new NetworkManagerConnectDelegate(this.Connect);
            if (this.mAsyncActiveConnectEvent == null)
            {
                lock (LOCK_CONNECT)
                {
                    if (this.mAsyncActiveConnectEvent == null)
                    {
                        this.mAsyncActiveConnectEvent = new AutoResetEvent(true);
                    }
                }
            }
            this.mAsyncActiveConnectEvent.WaitOne();
            this.mConnectDelegate = d;
            return d.BeginInvoke(endPoint, clientStreamFactory, timeoutInMS, callback, state);
        }

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>NetworkPeerRemote</returns>
        public INetworkPeerRemote Connect(AddressEndPoint endPoint)
        {
            return Connect(endPoint, mNetworkManager.ClientStreamFactory);
        }

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <returns>NetworkPeerRemote</returns>
        public INetworkPeerRemote Connect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory)
        {
            return Connect(endPoint, clientStreamFactory, Timeout.Infinite);
        }

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="timeoutInMS">The timeout in MS.</param>
        /// <returns>NetworkPeerRemote</returns>
        public INetworkPeerRemote Connect(AddressEndPoint endPoint, int timeoutInMS)
        {
            return Connect(endPoint, mNetworkManager.ClientStreamFactory, timeoutInMS);
        }

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="timeoutInMS">The timeout in MS.</param>
        /// <returns>NetworkPeerRemote</returns>
        public INetworkPeerRemote Connect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory, int timeoutInMS)
        {
            DoInitializationCheck();
            DoShutdownCheck();
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            if (clientStreamFactory == null)
            {
                ThrowHelper.ThrowArgumentNullException("clientStreamFactory");
            }
            return InternalConnect(endPoint, clientStreamFactory, timeoutInMS, true, false);
        }

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>NetworkPeerRemote</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)")]
        public INetworkPeerRemote EndConnect(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (this.mConnectDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndConnect called multiple times.", "asyncResult");
            }
            try
            {
                return this.mConnectDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                this.mConnectDelegate = null;
                this.mAsyncActiveConnectEvent.Set();
                CloseAsyncActiveConnectEvent(Interlocked.Decrement(ref mAsyncActiveConnectCount));
            }
        }

        /// <summary>
        /// Creates the socket TCP.
        /// </summary>
        /// <returns>Socket</returns>
        public ISocket CreateSocketTcp()
        {
            DoInitializationCheck();
            DoShutdownCheck();
            return CreateSocket(ProtocolType.Tcp);
        }

        /// <summary>
        /// Creates the socket UDP.
        /// </summary>
        /// <returns>Socket</returns>
        public ISocket CreateSocketUdp()
        {
            DoInitializationCheck();
            DoShutdownCheck();
            return CreateSocket(ProtocolType.Udp);
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Trying to connect to a remote peer in the background.
        /// </summary>
        /// <param name="networkPeer">The network peer.</param>
        /// <param name="localRequest">if set to <c>true</c> [local request].</param>
        /// <param name="disconnectOnConnectionDuplication">if set to <c>true</c> [disconnect on connection duplication].</param>
        internal void InternalConnectAsync(NetworkPeerRemote networkPeer, bool localRequest, bool disconnectOnConnectionDuplication)
        {
            NetworkManagerInternalConnectDelegate d = new NetworkManagerInternalConnectDelegate(InternalConnect);
            d.BeginInvoke(networkPeer, localRequest, disconnectOnConnectionDuplication, new AsyncCallback(InternalConnectAsyncEnd), d);
        }

        /// <summary>
        /// Trying to connect to a remote peer.
        /// </summary>
        /// <param name="networkPeer">The network peer.</param>
        /// <param name="localRequest">if set to <c>true</c> [local request].</param>
        /// <param name="disconnectOnConnectionDuplication">if set to <c>true</c> [disconnect on connection duplication].</param>
        /// <returns>True, if the connection succeeded</returns>
        internal bool InternalConnect(NetworkPeerRemote networkPeer, bool localRequest, bool disconnectOnConnectionDuplication)
        {
            if (networkPeer == null)
            {
                ThrowHelper.ThrowArgumentNullException("networkPeer");
            }

            // itt megpróbálunk egy ismert kliens-hez csatlakozni a benne lévő kapcsolódási pontok alapján
            bool result = false;

            {
                TCPServer server = networkPeer.TCPServerCollection.SelectTCPServer();
                HashSet<TCPServer> triedServers = new HashSet<TCPServer>();
                while (!result && server != null)
                {
                    if (triedServers.Contains(server))
                    {
                        // ezt a szervert már próbáltuk és nem vált be előzőleg
                        break;
                    }
                    else
                    {
                        triedServers.Add(server);
                    }

                    AddressEndPoint ep = server.EndPoint;
                    server.IncAttempts();
                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, trying to connect remote network peer [{0}] on address [{1}] and port {2}.", networkPeer.Id, ep.Host.ToString(), ep.Port));
                    try
                    {
                        NetworkPeerRemote peer = ProcessConnection(mNetworkManager.Connect(ep, mNetworkManager.ClientStreamFactory), mConfiguration.Settings.DefaultConnectionTimeoutInMS, localRequest, disconnectOnConnectionDuplication);
                        if (peer != null && peer.Equals(networkPeer))
                        {
                            server.Success = true;
                            result = true;
                        }
                        else
                        {
                            server.Success = false;
                            server = networkPeer.TCPServerCollection.SelectTCPServer();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, failed to connect remote network peer [{0}] on address [{1}] and port {2}. Reason: {3}", networkPeer.Id, ep.Host.ToString(), ep.Port, ex.Message));
                        server.Success = false;
                        server = networkPeer.TCPServerCollection.SelectTCPServer();
                    }
                }
            }

            if (!result)
            {
                NATGateway gateway = networkPeer.NATGatewayCollection.SelectNATGateWay();
                HashSet<NATGateway> triedServers = new HashSet<NATGateway>();
                while (!result && gateway != null)
                {
                    if (triedServers.Contains(gateway))
                    {
                        // ezt a gatewayt már próbáltuk és nem vált be előzőleg
                        break;
                    }
                    else
                    {
                        triedServers.Add(gateway);
                    }

                    AddressEndPoint ep = gateway.EndPoint;
                    gateway.IncAttempts();
                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, trying to connect remote network peer [{0}] on address [{1}] and port {2}.", networkPeer.Id, ep.Host.ToString(), ep.Port));
                    try
                    {
                        NetworkPeerRemote peer = InternalConnect(ep, mNetworkManager.ClientStreamFactory, mConfiguration.Settings.DefaultConnectionTimeoutInMS, localRequest, disconnectOnConnectionDuplication);
                        if (peer != null && peer.Equals(networkPeer))
                        {
                            gateway.Success = true;
                            result = true;
                        }
                        else
                        {
                            gateway.Success = false;
                            gateway = networkPeer.NATGatewayCollection.SelectNATGateWay();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, failed to connect remote network peer [{0}] on address [{1}] and port {2}. Reason: {3}", networkPeer.Id, ep.Host.ToString(), ep.Port, ex.Message));
                        gateway.Success = false;
                        gateway = networkPeer.NATGatewayCollection.SelectNATGateWay();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Internals the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="localRequest">if set to <c>true</c> [local request].</param>
        /// <param name="disconnectOnConnectionDuplication">if set to <c>true</c> [disconnect on connection duplication].</param>
        /// <returns>NetworkPeerRemote</returns>
        internal NetworkPeerRemote InternalConnect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory, int timeout, bool localRequest, bool disconnectOnConnectionDuplication)
        {
            return ProcessConnection(mNetworkManager.Connect(endPoint, clientStreamFactory), timeout, localRequest, disconnectOnConnectionDuplication);
        }

        /// <summary>
        /// Processes the connection.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="localRequest">if set to <c>true</c> [local request].</param>
        /// <param name="disconnectOnConnectionDuplication">if set to <c>true</c> [disconnect on connection duplication].</param>
        /// <returns>NetworkPeerRemote</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal NetworkPeerRemote ProcessConnection(Synapse.NetworkStream stream, int timeout, bool localRequest, bool disconnectOnConnectionDuplication)
        {
            using (NetworkConnectionTask task = new NetworkConnectionTask(stream, localRequest, disconnectOnConnectionDuplication))
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, connection task created with id: {0}. Network stream id: {1}. Remote host: {2}", task.TaskId, task.NetworkConnection.NetworkStream.Id, task.NetworkConnection.NetworkStream.RemoteEndPoint.Host));
                NetworkConnection networkConnection = task.NetworkConnection;
                networkConnection.Disconnect += new EventHandler<EventArgs>(NetworkConnection_Disconnect);
                networkConnection.MessageSendBefore += new EventHandler<MessageSendEventArgs>(NetworkConnection_MessageSendBefore);
                networkConnection.MessageSendAfter += new EventHandler<MessageSendEventArgs>(NetworkConnection_MessageSendAfter);
                networkConnection.MessageArrived += new EventHandler<MessageArrivedEventArgs>(NetworkConnection_MessageArrived);

                if (localRequest)
                {
                    NegotiationMessage message = new NegotiationMessage(mLocalHost.Id, mLocalHost.NetworkContext.Name, mLocalHost.Version);
                    MessageTask messageTask = new MessageTask(message);
                    networkConnection.AddMessageTask(messageTask);
                }

                //TODO-DONE: ellenőrizni, hogy kapcsolat szakadáskor nem blokkolódik-e tovább

                networkConnection.Initialize(); // kezdődik az adatfogadás
                // várakozás a kapcsolódásra
                if (task.WaitForConnection(timeout))
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, connection timed out. TaskId: {0}. Network stream id: {1}", task.TaskId, networkConnection.NetworkStream.Id));
                }
                else if (task.NetworkConnection.IsConnected)
                {
                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, connection established. TaskId: {0}. Network stream id: {1}.", task.TaskId, networkConnection.NetworkStream.Id));
                }
                else
                {
                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, connection lost. TaskId: {0}. Network stream id: {1}.", task.TaskId, networkConnection.NetworkStream.Id));
                }

                NetworkPeerRemote result = task.RemoteHostPeer;
                mLockMessageProcessor.Lock();
                try
                {
                    networkConnection.ConnectionTask = null;
                    task.Dispose();
                }
                finally
                {
                    mLockMessageProcessor.Unlock();
                }

                return result;
            }
        }

        /// <summary>
        /// Internals the send message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exceptConnection">The except connection.</param>
        /// <returns>MessageTask</returns>
        internal MessageTask InternalSendMessage(TerraGrafMessageBase message, NetworkConnection exceptConnection)
        {
            return InternalSendMessage(message, exceptConnection, null);
        }

        /// <summary>
        /// Internals the send message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exceptConnection">The except connection.</param>
        /// <param name="sentEvent">The sent event.</param>
        /// <returns>MessageTask</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal MessageTask InternalSendMessage(TerraGrafMessageBase message, NetworkConnection exceptConnection, EventWaitHandle sentEvent)
        {
            if (message == null)
            {
                ThrowHelper.ThrowArgumentNullException("message");
            }

            MessageTask result = null;

            if (message.MessageType == MessageTypeEnum.Udp && string.IsNullOrEmpty(message.TargetId))
            {
                // broadcast message
                // TCP broadcast message-ek nem támogatottak
                if (mLocalHost.Id.Equals(message.SenderId) || !mConfiguration.Settings.BlackHole)
                {
                    // csak ha nem vagyok blackhole
                    mLockMessageProcessor.Lock();
                    try
                    {
                        string arrivedNetworkContext = string.Empty;
                        if (exceptConnection != null)
                        {
                            arrivedNetworkContext = exceptConnection.ConnectionTask == null ? exceptConnection.OwnerSession.RemotePeer.NetworkContext.Name : exceptConnection.ConnectionTask.RemoteNetworkContextName;
                        }
                        foreach (NetworkConnection c in mActiveNetworkConnections)
                        {
                            // 1. élő kapcsolat
                            // 2. kivétel kapcsolatra ne (jellemzően, ahonnan jött)
                            // 3. a feladónak szintén ne küldjük vissza
                            try
                            {
                                if (c.IsConnected &&
                                    !c.Equals(exceptConnection) &&
                                    (c.OwnerSession == null || !c.OwnerSession.RemotePeer.Id.Equals(message.SenderId)))
                                {
                                    string targetNetworkContext = c.ConnectionTask == null ? c.OwnerSession.RemotePeer.NetworkContext.Name : c.ConnectionTask.RemoteNetworkContextName;
                                    NetworkPeerRemote sourceNetworkPeer = message.SenderId.Equals(mLocalHost.Id) ? mLocalHost : (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(message.SenderId);
                                    if (mNetworkContextRuleManager.CheckSeparation(mLocalHost.NetworkContext.Name, targetNetworkContext))
                                    {
                                        if (sourceNetworkPeer == null || mNetworkContextRuleManager.CheckSeparation(sourceNetworkPeer.NetworkContext.Name, targetNetworkContext))
                                        {
                                            if (string.IsNullOrEmpty(arrivedNetworkContext) || (mNetworkContextRuleManager.CheckSeparation(arrivedNetworkContext, targetNetworkContext)))
                                            {
                                                NetworkPeerRemote connectionPeer = c.ConnectionTask == null ? c.OwnerSession.RemotePeer : null;
                                                if (ForwardSpecialDecision(message, sourceNetworkPeer, connectionPeer))
                                                {
                                                    c.AddMessageTask(new MessageTask(message));
                                                }
                                            }
                                            else
                                            {
                                                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, sending message between network contexts [{0}] and [{1}] is not allowed. Sender id: [{2}], message id: {3}.", targetNetworkContext, arrivedNetworkContext, message.SenderId, message.MessageId));
                                            }
                                        }
                                        else
                                        {
                                            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, sending message between network contexts [{0}] and [{1}] is not allowed. Sender id: [{2}], message id: {3}.", targetNetworkContext, sourceNetworkPeer.NetworkContext.Name, message.SenderId, message.MessageId));
                                        }
                                    }
                                    else
                                    {
                                        if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, sending message between network contexts [{0}] and [{1}] is not allowed. Sender id: [{2}], message id: {3}.", targetNetworkContext, mLocalHost.NetworkContext.Name, message.SenderId, message.MessageId));
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                if (LOGGER.IsDebugEnabled) LOGGER.Debug("NETWORK, a connection lost while trying to forward a broadcast message on it.");
                            }
                        }
                        if (mLocalHost.Id.Equals(message.SenderId))
                        {
                            if (sentEvent != null)
                            {
                                // broadcast üzeneteknél tudjuk mérni, hogy mennyi idő alatt ment ki a hálózaton a csomag (mindenkinek)
                                try
                                {
                                    sentEvent.Set();
                                }
                                catch (Exception) { }
                            }
                            if (message.MessageCode == MessageCodeEnum.SocketRawData ||
                                message.MessageCode == MessageCodeEnum.SocketClose ||
                                message.MessageCode == MessageCodeEnum.SocketOpenRequest ||
                                message.MessageCode == MessageCodeEnum.SocketOpenResponse)
                            {
                                // saját broadcast üzenetemet házon belülre is elküldöm
                                NetworkConnection_MessageArrived(null, new MessageArrivedEventArgs(message));
                            }
                        }
                    }
                    finally
                    {
                        mLockMessageProcessor.Unlock();
                    }
                }
            }
            else if (!string.IsNullOrEmpty(message.TargetId))
            {
                // TCP és UDP üzenetek, fix címzettel
                NetworkPeerRemote networkPeer = mLocalHost.Id.Equals(message.TargetId) ? mLocalHost : (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(message.TargetId);
                if (networkPeer == null)
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, a message provided with an unknown target network peer. PeerId: {0}, MessageCode: {1}, MessageId: {2}", message.SenderId, message.MessageCode, message.MessageId));
                }
                else
                {
                    if (mLocalHost.Id.Equals(message.SenderId) && mLocalHost.Id.Equals(message.TargetId))
                    {
                        // saját, házon belüli üzenet
                        if (message.MessageCode == MessageCodeEnum.SocketRawData ||
                            message.MessageCode == MessageCodeEnum.SocketClose ||
                            message.MessageCode == MessageCodeEnum.SocketOpenRequest ||
                            message.MessageCode == MessageCodeEnum.SocketOpenResponse ||
                            message.MessageCode == MessageCodeEnum.Acknowledge)
                        {
                            result = new MessageTask(message, sentEvent);
                            if (message.MessageType == MessageTypeEnum.Tcp)
                            {
                                lock (mMessagesToAcknowledge)
                                {
                                    if (!mMessagesToAcknowledge.ContainsKey(result.Message.MessageId))
                                    {
                                        mMessagesToAcknowledge.Add(result.Message.MessageId, result);
                                    }
                                }
                            }

                            NetworkConnection_MessageArrived(null, new MessageArrivedEventArgs(message));

                            if (sentEvent != null && message.MessageType == MessageTypeEnum.Udp)
                            {
                                sentEvent.Set();
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("Message type not supported.");
                        }
                    }
                    else
                    {
                        if (mLocalHost.Id.Equals(message.SenderId) || !mConfiguration.Settings.BlackHole)
                        {
                            // csak ha nem vagyok blackhole
                            mLockMessageProcessor.Lock();
                            try
                            {
                                NetworkConnection connection = networkPeer.Session.NetworkConnection;
                                if (connection != null)
                                {
                                    result = new MessageTask(message, sentEvent);
                                    connection.AddMessageTask(result);
                                }
                            }
                            finally
                            {
                                mLockMessageProcessor.Unlock();
                            }
                        }
                    }
                }
            }
            else
            {
                // hibás üzenet
                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, a message provided without target. PeerId: {0}, MessageCode: {1}, MessageId: {2}", message.SenderId, message.MessageCode, message.MessageId));
            }

            return result;
        }

        /// <summary>
        /// Gets the internal current network context.
        /// </summary>
        /// <value>
        /// The internal current network context.
        /// </value>
        internal NetworkContext InternalCurrentNetworkContext
        {
            get { return this.mLocalHost.NetworkContext; }
        }

        /// <summary>
        /// Blacks the hole.
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal void InternalBlackHole(bool state)
        {
            mLockMessageProcessor.Lock();
            try
            {
                if (mLocalHost.BlackHoleContainer.IsBlackHole != state)
                {
                    mLocalHost.BlackHoleContainer.StateId = mLocalHost.BlackHoleContainer.StateId + 1;
                    mLocalHost.BlackHoleContainer.IsBlackHole = state;
                    if (state)
                    {
                        // bekapcsolás
                        TerraGrafInformationMessage message = new TerraGrafInformationMessage(mLocalHost.Id, mLocalHost.BlackHoleContainer.BuildBlackHoleContainer());
                        InternalSendMessage(message, null);
                    }
                    else
                    {
                        // kikapcsolás
                        Dictionary<string, TerraGrafInformationMessage> infos = new Dictionary<string, TerraGrafInformationMessage>();
                        foreach (NetworkConnection c in mActiveNetworkConnections)
                        {
                            if (c.OwnerSession != null)
                            {
                                // továbbítás minden szomszédnak
                                NetworkPeerRemote peer = c.OwnerSession.RemotePeer;
                                TerraGrafInformationMessage message = null;
                                if (infos.ContainsKey(peer.NetworkContext.Name))
                                {
                                    message = infos[peer.NetworkContext.Name];
                                }
                                else
                                {
                                    message = new TerraGrafInformationMessage(mLocalHost.Id, BuildCompleteGrafSnapshot(peer.NetworkContext.Name), mLocalHost.BlackHoleContainer.BuildBlackHoleContainer());
                                    infos[peer.NetworkContext.Name] = message;
                                }
                                c.AddMessageTask(new MessageTask(message));
                            }
                        }
                        infos.Clear();
                    }
                }
            }
            finally
            {
                mLockMessageProcessor.Unlock();
            }
        }

        /// <summary>
        /// Internals the send localhost peer context.
        /// </summary>
        /// <param name="context">The context.</param>
        internal void InternalSendLocalhostPeerContext(NetworkPeerDataContext context)
        {
            mLockMessageProcessor.Lock();
            try
            {
                mLocalHost.InternalPeerContext.StateId = mLocalHost.InternalPeerContext.StateId + 1;
                mLocalHost.InternalPeerContext.PeerContext = context;
                TerraGrafPeerUpdateMessage message = new TerraGrafPeerUpdateMessage(mLocalHost.Id, mLocalHost.InternalPeerContext.BuildPeerContextContainer());
                InternalSendMessage(message, null);
            }
            finally
            {
                mLockMessageProcessor.Unlock();
            }
        }

        /// <summary>
        /// Start TCP server internally.
        /// </summary>
        /// <param name="ep">The ep.</param>
        /// <param name="serverStreamFactory">The server stream factory.</param>
        /// <returns>Identifier of the server</returns>
        internal long InternalStartTCPServer(AddressEndPoint ep, IServerStreamFactory serverStreamFactory)
        {
            return mNetworkManager.StartServer(ep, serverStreamFactory);
        }

        /// <summary>
        /// Send TCP servers.
        /// </summary>
        internal void InternalSendTCPServers()
        {
            mLockMessageProcessor.Lock();
            try
            {
                mLocalHost.TCPServerCollection.StateId = mLocalHost.TCPServerCollection.StateId + 1;
                TerraGrafPeerUpdateMessage message = new TerraGrafPeerUpdateMessage(mLocalHost.Id, mLocalHost.TCPServerCollection.BuildServerContainer());
                InternalSendMessage(message, null);
            }
            finally
            {
                mLockMessageProcessor.Unlock();
            }
        }

        /// <summary>
        /// Send NAT gateways.
        /// </summary>
        internal void InternalSendNATGateways()
        {
            mLockMessageProcessor.Lock();
            try
            {
                mLocalHost.NATGatewayCollection.StateId = mLocalHost.NATGatewayCollection.StateId + 1;
                TerraGrafPeerUpdateMessage message = new TerraGrafPeerUpdateMessage(mLocalHost.Id, mLocalHost.NATGatewayCollection.BuildNATGatewayContainer());
                InternalSendMessage(message, null);
            }
            finally
            {
                mLockMessageProcessor.Unlock();
            }
        }

        /// <summary>
        /// Register a new binded socket
        /// </summary>
        /// <param name="socket">The socket.</param>
        internal void InternalSocketRegister(Socket socket)
        {
            SafeBindedSocketsAccess(true, () =>
                {
                    mBindedSocketsLock.EnterWriteLock();
                    mBindedSockets[socket.LocalSocketId] = socket;
                    mBindedSocketsLock.ExitWriteLock();
                });
        }

        /// <summary>
        /// Unregister a socket
        /// </summary>
        /// <param name="socket">The socket.</param>
        internal void InternalSocketUnregister(Socket socket)
        {
            SafeBindedSocketsAccess(true, () =>
                {
                    mBindedSocketsLock.EnterWriteLock();
                    mBindedSockets.Remove(socket.LocalSocketId);
                    mBindedSocketsLock.ExitWriteLock();
                });
        }

        /// <summary>
        /// Does the initialization check.
        /// </summary>
        [DebuggerHidden]
        internal void DoInitializationCheck()
        {
            if (this.ManagerState != ManagerStateEnum.Started)
            {
                throw new InitializationException("Network has not been initialized.");
            }
        }

        [DebuggerHidden]
        internal void DoShutdownCheck()
        {
            if (mShutdown)
            {
                throw new InitializationException("Network has shutdown.");
            }
        }

        /// <summary>
        /// Called when [network peer context changed].
        /// </summary>
        /// <param name="peer">The peer.</param>
        /// <param name="asyncEvent">if set to <c>true</c> [async event].</param>
        internal void OnNetworkPeerContextChanged(INetworkPeerRemote peer, bool asyncEvent)
        {
            if (asyncEvent)
            {
                Raiser.CallDelegatorByAsync(NetworkPeerContextChanged, new object[] { this, new NetworkPeerContextEventArgs(peer) });
            }
            else
            {
                Raiser.CallDelegatorBySync(NetworkPeerContextChanged, new object[] { this, new NetworkPeerContextEventArgs(peer) });
            }
        }

        #endregion

        #region Private method(s)

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times"), MethodImpl(MethodImplOptions.Synchronized)]
        private void Initialize(bool debug)
        {
            if (this.ManagerState != ManagerStateEnum.Started)
            {
                OnStart(ManagerEventStateEnum.Before);
                try
                {
                    //{
                    //    #region License check

                    //    Uri uri = new Uri(GetType().Assembly.CodeBase);
                    //    string localPath = string.Format("{0}.cer", uri.LocalPath);
                    //    if (new FileInfo(localPath).Exists)
                    //    {
                    //        try
                    //        {
                    //            X509Certificate2 certificate = new X509Certificate2(new X509Certificate(localPath));

                    //            // dátum ellenőrzés
                    //            if (certificate.NotBefore.Ticks > DateTime.Now.Ticks || certificate.NotAfter.Ticks < DateTime.Now.Ticks)
                    //            {
                    //                throw new SecurityException();
                    //            }

                    //            if (!"2CC660BF".Equals(certificate.SerialNumber) || !"CN=JZO.TerraGraf".Equals(certificate.SubjectName.Name))
                    //            {
                    //                throw new SecurityException();
                    //            }

                    //            byte[] sample = new byte[] { 65, 181, 228, 63, 153, 91, 146, 90, 252, 107, 206, 196, 86, 54, 248, 249, 
                    //            216, 164, 211, 68, 170, 249, 66, 186, 108, 86, 152, 187, 91, 17, 154, 166, 160, 96, 129, 90, 14, 85, 
                    //            27, 40, 113, 224, 25, 242, 155, 227, 233, 186, 12, 102, 30, 201, 221, 121, 51, 211, 187, 30, 191, 169, 
                    //            119, 126, 95, 138, 239, 211, 0, 156, 216, 176, 100, 254, 124, 100, 254, 74, 169, 26, 178, 125, 204, 
                    //            101, 28, 235, 220, 29, 191, 71, 0, 165, 8, 219, 42, 64, 92, 154, 140, 127, 191, 243, 204, 93, 170, 
                    //            243, 214, 214, 130, 168, 221, 164, 79, 226, 248, 195, 20, 71, 85, 247, 189, 23, 119, 146, 143, 241, 
                    //            70, 98, 113, 196, 101, 221, 89, 169, 167, 226, 201, 157, 202, 143, 30, 168, 167, 182, 155, 113, 247, 
                    //            199, 57, 216, 124, 2, 176, 179, 6, 95, 180, 247, 101, 185, 132, 203, 193, 89, 127, 58, 175, 104, 251, 
                    //            104, 195, 229, 174, 133, 15, 39, 64, 175, 249, 138, 155, 209, 165, 236, 167, 193, 247, 239, 5, 158, 
                    //            250, 19, 104, 215, 9, 55, 54, 175, 157, 48, 48, 249, 184, 209, 146, 108, 138, 70, 185, 25, 18, 1, 176, 
                    //            219, 140, 172, 105, 253, 142, 146, 122, 189, 187, 106, 2, 255, 59, 189, 57, 37, 25, 170, 136, 197, 167, 
                    //            202, 251, 232, 254, 169, 253, 111, 91, 128, 123, 171, 123, 196, 245, 253, 157, 35, 17, 224, 203, 175, 
                    //            230, 118 };
                    //            byte[] expected = new byte[] { 22, 218, 92, 35, 246, 115, 194, 155, 107, 254, 51, 150, 208, 248, 116, 236, 
                    //            64, 106, 49, 2, 77, 59, 236, 241, 142, 135, 125, 142, 163, 209, 36, 62, 142, 164, 125, 148, 186, 245, 
                    //            55, 228, 31, 241, 222, 69, 85, 98, 70, 13, 129, 34, 33, 117, 79, 163, 114, 201, 229, 222, 236, 56, 75, 
                    //            100, 224, 114, 169, 250, 5, 57, 79, 221, 71, 64, 207, 160, 109, 14, 241, 137, 23, 118, 53, 46, 149, 43, 
                    //            117, 49, 42, 251, 36, 10, 131, 81, 71, 88, 148, 155, 238, 197, 138, 22, 178, 207, 26, 127, 219, 51, 56, 
                    //            185, 79, 163, 75, 108, 245, 53, 97, 19, 147, 53, 96, 78, 229, 41, 237, 38, 180, 77, 210, 38, 138, 115, 
                    //            7, 4, 118, 206, 253, 20, 254, 226, 179, 109, 119, 7, 125, 249, 215, 153, 227, 199, 108, 182, 181, 94, 
                    //            132, 111, 74, 99, 45, 69, 201, 221, 58, 58, 27, 84, 89, 93, 203, 203, 56, 70, 133, 134, 15, 97, 246, 89, 
                    //            149, 71, 147, 203, 197, 101, 102, 242, 173, 178, 5, 223, 170, 46, 74, 181, 246, 123, 42, 23, 57, 84, 240, 
                    //            226, 224, 96, 8, 94, 11, 244, 113, 237, 12, 52, 248, 78, 26, 153, 229, 146, 101, 227, 71, 158, 191, 140, 
                    //            104, 173, 151, 62, 77, 8, 13, 16, 173, 179, 247, 107, 177, 51, 29, 222, 242, 243, 81, 196, 108, 255, 47, 
                    //            116, 113, 54, 13, 185, 123, 226, 227, 4, 198, 84, 73, 115, 95, 167, 251, 55, 94, 200, 96, 32, 102, 111, 
                    //            9, 188, 13, 69 };
                    //            byte[] encryptedData = null;
                    //            byte[] IV = new byte[16];
                    //            byte[] Key = new byte[32];

                    //            Buffer.BlockCopy(certificate.PublicKey.EncodedKeyValue.RawData, 0, IV, 0, IV.Length);
                    //            Buffer.BlockCopy(certificate.PublicKey.EncodedKeyValue.RawData, certificate.PublicKey.EncodedKeyValue.RawData.Length - Key.Length, Key, 0, Key.Length);

                    //            using (RijndaelManaged r = new RijndaelManaged())
                    //            {
                    //                r.IV = IV;
                    //                r.Key = Key;
                    //                using (ICryptoTransform encryptor = r.CreateEncryptor())
                    //                {
                    //                    using (MemoryStream ms = new MemoryStream())
                    //                    {
                    //                        using (CryptoStream csEncrypt = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    //                        {
                    //                            csEncrypt.Write(sample, 0, sample.Length);
                    //                            csEncrypt.FlushFinalBlock();
                    //                            encryptedData = ms.ToArray();
                    //                        }
                    //                    }
                    //                }
                    //            }

                    //            if (expected.Length == encryptedData.Length)
                    //            {
                    //                for (int i = 0; i < expected.Length; i++)
                    //                {
                    //                    if (!expected[i].Equals(encryptedData[i]))
                    //                    {
                    //                        throw new SecurityException();
                    //                    }
                    //                }
                    //            }

                    //            #region Expected generator
                    //            //using (FileStream fs = File.Create("output.txt"))
                    //            //{
                    //            //    System.Text.StringBuilder sb = new System.Text.StringBuilder("byte[] expected = new byte[] {");
                    //            //    bool writeMark = false;
                    //            //    foreach (byte b in encryptedData)
                    //            //    {
                    //            //        if (writeMark)
                    //            //        {
                    //            //            sb.Append(", ");
                    //            //        }
                    //            //        else
                    //            //        {
                    //            //            sb.Append(" ");
                    //            //            writeMark = true;
                    //            //        }
                    //            //        sb.Append(((int)b).ToString());
                    //            //    }
                    //            //    sb.Append("};");
                    //            //    byte[] data = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                    //            //    fs.Write(data, 0, data.Length);
                    //            //}
                    //            #endregion
                    //        }
                    //        catch (Exception)
                    //        {
                    //            throw new SecurityException("License file is not valid.");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        throw new SecurityException(string.Format("License file not found: '{0}'.", localPath));
                    //    }

                    //    #endregion
                    //}
                    {
                        string appIdValue = ApplicationHelper.ApplicationId;
                        if (debug)
                        {
                            APPLICATION_ID = DateTime.Now.Millisecond.ToString();
                        }
                        else
                        {
                            APPLICATION_ID = appIdValue.Trim();
                        }

                        bool isMutexNew = false;
                        mMutex = new Mutex(true, string.Format("TerraGraf_{0}", APPLICATION_ID), out isMutexNew);
                        if (!isMutexNew)
                        {
                            throw new InitializationException(string.Format("An other application with this id is running: '{0}'", appIdValue));
                        }
                    }

                    {
                        int workerThreads = 0;
                        int completionPortThreads = 0;
                        ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
                        if (workerThreads < Environment.ProcessorCount * 25)
                        {
                            workerThreads = Environment.ProcessorCount * 25;
                            completionPortThreads = workerThreads;
                            ThreadPool.SetMaxThreads(workerThreads, completionPortThreads);
                        }

                        ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
                        if (workerThreads < Environment.ProcessorCount * 25)
                        {
                            workerThreads = Environment.ProcessorCount * 25;
                            completionPortThreads = workerThreads;
                            ThreadPool.SetMinThreads(workerThreads, completionPortThreads);
                        }
                    }

                    this.mConfiguration = new ConfigContainer(); // ez jól elhasalhat, ha szar a config.
                    this.mConfiguration.Initialize();

                    if (this.mConfiguration.Settings.AddWindowsFirewallException)
                    {
                        try
                        {
                            string appFile = string.Empty;
                            if (Assembly.GetEntryAssembly() == null)
                            {
                                appFile = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, AppDomain.CurrentDomain.SetupInformation.ApplicationName);
                            }
                            else
                            {
                                appFile = new Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath;
                            }
                            appFile = appFile.Replace('/', '\\');
                            if (appFile.ToLower().EndsWith(".exe"))
                            {
                                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, add exception to Windows Firewall: {0}", appFile));
                                using (WindowsFirewallManager manager = new WindowsFirewallManager())
                                {
                                    manager.Initialize();
                                    FileInfo fi = new FileInfo(appFile);
                                    if (!manager.IsApplicationEnabled(appFile.ToLower()))
                                    {
                                        manager.AddApplication(appFile.ToLower(), AppDomain.CurrentDomain.SetupInformation.ApplicationName);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (LOGGER.IsErrorEnabled) LOGGER.Error("NETWORK, failed to add exception to the Windows Firewall.", ex);
                        }
                    }

                    this.mKnownNetworkContexts = new List<NetworkContext>();
                    this.mNetworkContextRuleManager = new NetworkContextRuleManager();

                    // localhost and network context init
                    this.mLocalHost = new NetworkPeerLocal();
                    this.mLocalHost.Id = string.Format("{0}_{1}_{2}", APPLICATION_ID, mConfiguration.NetworkContext.Name, System.Net.Dns.GetHostName().Trim());
                    this.mLocalHost.HostName = System.Net.Dns.GetHostName();
                    this.mLocalHost.BlackHoleContainer.IsBlackHole = mConfiguration.Settings.BlackHole;
                    this.mLocalHost.BlackHoleContainer.StateId = DateTime.UtcNow.Ticks;
                    this.mLocalHost.NATGatewayCollection.StateId = DateTime.UtcNow.Ticks;
                    foreach (AddressEndPoint ep in mConfiguration.NetworkPeering.NATGateways.EndPoints)
                    {
                        this.mLocalHost.NATGatewayCollection.NATGateways.Add(new NATGateway(ep));
                    }
                    this.mLocalHost.NetworkContext = NetworkContext.CreateNetworkContext(mConfiguration.NetworkContext.Name);
                    //this.mLocalHost.NetworkContext.InternalNetworkPeers.Add(this.mLocalHost); // nem kell lockolni a listát
                    this.mLocalHost.PeerType = PeerTypeEnum.Local;
                    this.mLocalHost.TCPServerCollection.StateId = DateTime.UtcNow.Ticks;
                    this.mLocalHost.Initialize();
                    this.mLocalHost.Version = typeof(NetworkManager).Assembly.GetName().Version;
                    this.mLocalHost.Session = new NetworkPeerSession(this.mLocalHost);

                    // ezt ki kell tölteni, amikor indítom a szervereket!
                    //this.mLocalHost.TCPServers.Servers;

                    this.mNetworkManager = new Synapse.NetworkManager();
                    this.mNetworkManager.NetworkPeerConnected += new EventHandler<ConnectionEventArgs>(NetworkManager_NetworkPeerConnectedOnServer);
                    this.mNetworkManager.SocketKeepAliveTime = mConfiguration.Settings.DefaultLowLevelSocketKeepAliveTime;
                    this.mNetworkManager.SocketKeepAliveTimeInterval = mConfiguration.Settings.DefaultLowLevelSocketKeepAliveTimeInterval;

                    this.mConnectionManager = new ConnectionManager();

                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, localhost id: [{0}], network context name: [{1}].", mLocalHost.Id, mLocalHost.NetworkContext.Name));

                    this.ManagerState = ManagerStateEnum.Started;

                    //ezután szabad indítani a hálózatot
                    this.mConnectionManager.InitializeTCPServers();
                    this.mConnectionManager.InitializeNATUPnPService();
                    this.mConnectionManager.InitializeTCPConnections();
                    this.mConnectionManager.InitializeUDPDetector();
                }
                catch (Exception)
                {
                    this.ManagerState = ManagerStateEnum.Fault;
                    throw;
                }
                finally
                {
                    OnStart(ManagerEventStateEnum.After);
                }
            }
        }

        private Socket CreateSocket(ProtocolType protocolType)
        {
            return new Socket(protocolType == ProtocolType.Tcp ? SocketType.Stream : SocketType.Dgram, protocolType);
        }

        private void NetworkManager_NetworkPeerConnectedOnServer(object sender, ConnectionEventArgs e)
        {
            // ide csak a TCP szerveren bejövő kapcsolatok kerülnek
            AddressEndPoint remoteEp = e.NetworkStream.RemoteEndPoint;
            AddressEndPoint localEp = e.NetworkStream.LocalEndPoint;
            e.NetworkStream.SendBufferSize = mConfiguration.Settings.DefaultLowLevelSocketSendBufferSize;
            e.NetworkStream.ReceiveBufferSize = mConfiguration.Settings.DefaultLowLevelSocketReceiveBufferSize;
            e.NetworkStream.NoDelay = NetworkManager.Instance.InternalConfiguration.Settings.DefaultLowLevelNoDelay;
            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, a new TCP connection established on serverId: {0}, streamId: {1}, RemoteIp: {2}, LocalIp: {3}, Local port: {4}", e.ServerId, e.NetworkStream.Id, remoteEp.Host, localEp.Host, localEp.Port));
            ProcessConnection(e.NetworkStream, InternalConfiguration.Settings.DefaultConnectionTimeoutInMS, false, !NetworkManager.Instance.InternalConfiguration.Settings.EnableMultipleConnectionWithNetworkPeers);
        }

        private void NetworkConnection_MessageSendBefore(object sender, MessageSendEventArgs e)
        {
            // üzenet küldés előtt vagyunk
            // csak a saját üzeneteimre várunk és csak a tcp-kre
            if (mLocalHost.Id.Equals(e.MessageTask.Message.SenderId) &&
                e.MessageTask.Message.MessageType == MessageTypeEnum.Tcp &&
                e.MessageTask.Message.MessageCode != MessageCodeEnum.Acknowledge &&
                e.MessageTask.Message.MessageCode != MessageCodeEnum.SocketOpenRequest &&
                e.MessageTask.Message.MessageCode != MessageCodeEnum.SocketOpenResponse &&
                !e.MessageTask.MessageSubscribedToAcknowledge)
            {
                lock (mMessagesToAcknowledge)
                {
                    if (!mMessagesToAcknowledge.ContainsKey(e.MessageTask.Message.MessageId))
                    {
                        mMessagesToAcknowledge.Add(e.MessageTask.Message.MessageId, e.MessageTask);
                        e.MessageTask.MessageSubscribedToAcknowledge = true;
                    }
                }
            }
        }

        private void NetworkConnection_MessageSendAfter(object sender, MessageSendEventArgs e)
        {
            // az üzenet kiment a hálózaton
            if (mLocalHost.Id.Equals(e.MessageTask.Message.SenderId) && e.MessageTask.Message.MessageType == MessageTypeEnum.Udp)
            {
                // a saját UDP üzeneteimnél tudom jelezni, hogy kimentek-e a hálózaton fizikailag
                if (e.MessageTask.SentEvent != null)
                {
                    try
                    {
                        e.MessageTask.SentEvent.Set();
                    }
                    catch (Exception) { }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void NetworkConnection_MessageArrived(object sender, MessageArrivedEventArgs e)
        {
            NetworkConnection connection = sender as NetworkConnection;
            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, message arrived on connection Id: {0}. {1}", connection == null ? "<localhost>" : connection.NetworkStream.Id.ToString(), e.Message.ToString()));
            if (mLocalHost.Id.Equals(e.Message.SenderId) && e.Message.MessageCode != MessageCodeEnum.SocketRawData &&
                e.Message.MessageCode != MessageCodeEnum.SocketClose &&
                e.Message.MessageCode != MessageCodeEnum.SocketOpenRequest &&
                e.Message.MessageCode != MessageCodeEnum.SocketOpenResponse &&
                e.Message.MessageCode != MessageCodeEnum.Acknowledge)
            {
                // valaki idekülde a saját üzenetem...
            }
            else if (e.Message.MessageType == MessageTypeEnum.Tcp && string.IsNullOrEmpty(e.Message.TargetId))
            {
                // hibás üzenet. TCP, de nincs címzett
                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, invalid message content. No target definied in a TCP type message. MessageId: {0}", e.Message.MessageId));
            }
            else if (mLocalHost.Id.Equals(e.Message.TargetId) ||
                (string.IsNullOrEmpty(e.Message.TargetId) && e.Message.MessageType == MessageTypeEnum.Udp))
            {
                // nekem szól az üzenet (TCP v. UDP) vagy pedig UDP broadcast
                switch (e.Message.MessageCode)
                {
                    case MessageCodeEnum.Undefinied:
                    case MessageCodeEnum.UdpBroadcast:
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, Unexpected message code: {0}", e.Message.MessageCode));
                        break;

                    case MessageCodeEnum.Acknowledge:
                        {
                            #region Acknowledge

                            MessageAcknowledge ack = (MessageAcknowledge)e.Message;
                            lock (mMessagesToAcknowledge)
                            {
                                if (mMessagesToAcknowledge.ContainsKey(ack.MessageIdToAck))
                                {
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, acknowledge the original message with id: {0}. SenderId: [{1}]", ack.MessageIdToAck, ack.SenderId));
                                    MessageTask task = mMessagesToAcknowledge[ack.MessageIdToAck];
                                    task.IsSuccess = true;
                                    task.RaiseSentEventFinished();
                                    mMessagesToAcknowledge.Remove(ack.MessageIdToAck);
                                }
                                else
                                {
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, acknowledge message unable to find the original message with id: {0}. SenderId: [{1}]", ack.MessageIdToAck, ack.SenderId));
                                }
                            }

                            #endregion
                        }
                        break;

                    case MessageCodeEnum.SocketRawData:
                        {
                            #region SocketRawData

                            int counter = 0;
                            NetworkPeerRemote remoteNetworkPeer = mLocalHost.Id.Equals(e.Message.SenderId) ? mLocalHost : NetworkPeerContext.GetNetworkPeerById(e.Message.SenderId) as NetworkPeerRemote;
                            if (remoteNetworkPeer == null)
                            {
                                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, a message arrived from an unknown network peer. PeerId: {0}, MessageCode: {1}, MessageId: {2}", e.Message.SenderId, e.Message.MessageCode, e.Message.MessageId));
                            }
                            else
                            {
                                bool decision = false;
                                if (string.IsNullOrEmpty(e.Message.TargetId) && e.Message.MessageType == MessageTypeEnum.Udp)
                                {
                                    // szükséges lehet továbbküldeni és azt vizsgálom, hogy fel szabad-e dolgoznom
                                    if (mLocalHost.Id.Equals(e.Message.SenderId))
                                    {
                                        // saját üzenetemet nem küldöm tovább csak vizsgálom a feldolgozhatóságát
                                        decision = CheckMessageForDuplicationProcess(e.Message, connection, out counter);
                                    }
                                    else
                                    {
                                        // más üzenetét tovább lehet küldeni és vizsgálom a feldolgozhatóságát
                                        decision = CheckMessageForDuplicationProcessAndForward(e.Message, connection);
                                    }
                                }
                                else
                                {
                                    // konkrétan nekem szól, csak azt vizsgálom, hogy fel szabad-e dolgoznom
                                    decision = CheckMessageForDuplicationProcess(e.Message, connection, out counter);
                                }
                                if (decision)
                                {
                                    Socket socket = null;
                                    SocketRawDataMessage message = (SocketRawDataMessage)e.Message;
                                    SafeBindedSocketsAccess(() =>
                                        {
                                            if (message.MessageType == MessageTypeEnum.Udp)
                                            {
                                                // UDP-nél portra küldjük
                                                foreach (Socket s in mBindedSockets.Values)
                                                {
                                                    if (s.IsBound && ((AddressEndPoint)s.LocalEndPoint).Port == message.TargetPort)
                                                    {
                                                        socket = s;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // TCP-nél azonosítóra szűrünk
                                                if (mBindedSockets.ContainsKey(message.TargetSocketId))
                                                {
                                                    socket = mBindedSockets[message.TargetSocketId];
                                                }
                                            }
                                        });
                                    if (socket != null)
                                    {
                                        // küldjük a socket-nek
                                        socket.InternalProcessRawData(message);
                                    }
                                    else
                                    {
                                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, TerraGraf socket not found. PeerId: {0}, MessageCode: {1}, MessageId: {2}, Socket TargetPort: {3}", e.Message.SenderId, e.Message.MessageCode, e.Message.MessageId, message.TargetPort.ToString()));
                                    }
                                }
                            }

                            #endregion
                        }
                        break;

                    case MessageCodeEnum.SocketOpenRequest:
                        {
                            #region SocketOpenRequest

                            int counter = 0;
                            NetworkPeerRemote remoteNetworkPeer = mLocalHost.Id.Equals(e.Message.SenderId) ? mLocalHost : NetworkPeerContext.GetNetworkPeerById(e.Message.SenderId) as NetworkPeerRemote;
                            if (remoteNetworkPeer == null)
                            {
                                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, a message arrived from an unknown network peer. PeerId: {0}, MessageCode: {1}, MessageId: {2}", e.Message.SenderId, e.Message.MessageCode, e.Message.MessageId));
                            }
                            else
                            {
                                bool decision = false;
                                if (string.IsNullOrEmpty(e.Message.TargetId) && e.Message.MessageType == MessageTypeEnum.Udp)
                                {
                                    // szükséges lehet továbbküldeni és azt vizsgálom, hogy fel szabad-e dolgoznom
                                    if (mLocalHost.Id.Equals(e.Message.SenderId))
                                    {
                                        // saját üzenetemet nem küldöm tovább csak vizsgálom a feldolgozhatóságát
                                        decision = CheckMessageForDuplicationProcess(e.Message, connection, out counter);
                                    }
                                    else
                                    {
                                        // más üzenetét tovább lehet küldeni és vizsgálom a feldolgozhatóságát
                                        decision = CheckMessageForDuplicationProcessAndForward(e.Message, connection);
                                    }
                                }
                                else
                                {
                                    // konkrétan nekem szól, csak azt vizsgálom, hogy fel szabad-e dolgoznom
                                    decision = CheckMessageForDuplicationProcess(e.Message, connection, out counter);
                                }
                                if (decision)
                                {
                                    Socket socket = null;
                                    SocketOpenRequestMessage message = (SocketOpenRequestMessage)e.Message;
                                    SafeBindedSocketsAccess(() =>
                                        {
                                            foreach (Socket s in mBindedSockets.Values)
                                            {
                                                if (s.IsBound && ((AddressEndPoint)s.LocalEndPoint).Port == message.TargetPort && s.IsListening)
                                                {
                                                    socket = s;
                                                    break;
                                                }
                                            }
                                        });
                                    if (socket == null)
                                    {
                                        // nem figyel senki ilyen porton
                                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, no socket listening on target port '{0}', MessageId: {1}", message.TargetPort.ToString(), message.MessageId.ToString()));

                                        SocketOpenResponseMessage response = new SocketOpenResponseMessage(mLocalHost.Id, message.SenderId, -1, message.SenderPort, -1, message.SenderSocketId);
                                        if (mLocalHost.Id.Equals(message.SenderId) && mLocalHost.Id.Equals(message.TargetId))
                                        {
                                            // házon belül
                                            NetworkConnection_MessageArrived(null, new MessageArrivedEventArgs(response));
                                        }
                                        else
                                        {
                                            InternalSendMessage(response, null);
                                        }
                                    }
                                    else
                                    {
                                        // küldöm a szerver portnak
                                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, listening socket found for target port '{0}', MessageId: {1}", message.TargetPort.ToString(), message.MessageId.ToString()));

                                        socket.InternalProcessSocketOpenRequest(message);
                                    }
                                }
                            }

                            #endregion
                        }
                        break;

                    case MessageCodeEnum.SocketOpenResponse:
                        {
                            #region SocketOpenResponse

                            int counter = 0;
                            NetworkPeerRemote remoteNetworkPeer = mLocalHost.Id.Equals(e.Message.SenderId) ? mLocalHost : NetworkPeerContext.GetNetworkPeerById(e.Message.SenderId) as NetworkPeerRemote;
                            if (remoteNetworkPeer == null)
                            {
                                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, a message arrived from an unknown network peer. PeerId: {0}, MessageCode: {1}, MessageId: {2}", e.Message.SenderId, e.Message.MessageCode, e.Message.MessageId));
                            }
                            else
                            {
                                bool decision = false;
                                if (string.IsNullOrEmpty(e.Message.TargetId) && e.Message.MessageType == MessageTypeEnum.Udp)
                                {
                                    // szükséges lehet továbbküldeni és azt vizsgálom, hogy fel szabad-e dolgoznom
                                    if (mLocalHost.Id.Equals(e.Message.SenderId))
                                    {
                                        // saját üzenetemet nem küldöm tovább csak vizsgálom a feldolgozhatóságát
                                        decision = CheckMessageForDuplicationProcess(e.Message, connection, out counter);
                                    }
                                    else
                                    {
                                        // más üzenetét tovább lehet küldeni és vizsgálom a feldolgozhatóságát
                                        decision = CheckMessageForDuplicationProcessAndForward(e.Message, connection);
                                    }
                                }
                                else
                                {
                                    // konkrétan nekem szól, csak azt vizsgálom, hogy fel szabad-e dolgoznom
                                    decision = CheckMessageForDuplicationProcess(e.Message, connection, out counter);
                                }
                                if (decision)
                                {
                                    Socket socket = null;
                                    SocketOpenResponseMessage message = (SocketOpenResponseMessage)e.Message;
                                    SafeBindedSocketsAccess(() =>
                                        {
                                            if (mBindedSockets.ContainsKey(message.TargetSocketId))
                                            {
                                                socket = mBindedSockets[message.TargetSocketId];
                                            }
                                        });
                                    if (socket == null)
                                    {
                                        // nincs ilyen socket
                                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, no local socket waiting for this response. Target socketId '{0}', MessageId: {1}", message.TargetSocketId.ToString(), message.MessageId.ToString()));
                                        if (message.SenderSocketId > 0)
                                        {
                                            SocketCloseMessage close = new SocketCloseMessage(mLocalHost.Id, message.SenderId, -1, message.SenderPort, message.TargetSocketId, message.SenderSocketId, MessageTypeEnum.Udp);
                                            if (mLocalHost.Id.Equals(message.SenderId) && mLocalHost.Id.Equals(message.TargetId))
                                            {
                                                // házon belül
                                                NetworkConnection_MessageArrived(null, new MessageArrivedEventArgs(close));
                                            }
                                            else
                                            {
                                                InternalSendMessage(close, null);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, target socket found, id '{0}', MessageId: {1}", message.TargetSocketId.ToString(), message.MessageId.ToString()));

                                        // küldöm a választ a csatlakozást kérő socket-nek
                                        socket.InternalProcessSocketOpenResponse(message);
                                    }
                                }
                            }

                            #endregion
                        }
                        break;

                    case MessageCodeEnum.SocketClose:
                        {
                            #region SocketClose

                            int counter = 0;
                            NetworkPeerRemote remoteNetworkPeer = mLocalHost.Id.Equals(e.Message.SenderId) ? mLocalHost : NetworkPeerContext.GetNetworkPeerById(e.Message.SenderId) as NetworkPeerRemote;
                            if (remoteNetworkPeer == null)
                            {
                                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, a message arrived from an unknown network peer. PeerId: {0}, MessageCode: {1}, MessageId: {2}", e.Message.SenderId, e.Message.MessageCode, e.Message.MessageId));
                            }
                            else
                            {
                                bool decision = false;
                                if (string.IsNullOrEmpty(e.Message.TargetId) && e.Message.MessageType == MessageTypeEnum.Udp)
                                {
                                    // szükséges lehet továbbküldeni és azt vizsgálom, hogy fel szabad-e dolgoznom
                                    if (mLocalHost.Id.Equals(e.Message.SenderId))
                                    {
                                        // saját üzenetemet nem küldöm tovább csak vizsgálom a feldolgozhatóságát
                                        decision = CheckMessageForDuplicationProcess(e.Message, connection, out counter);
                                    }
                                    else
                                    {
                                        // más üzenetét tovább lehet küldeni és vizsgálom a feldolgozhatóságát
                                        decision = CheckMessageForDuplicationProcessAndForward(e.Message, connection);
                                    }
                                }
                                else
                                {
                                    // konkrétan nekem szól, csak azt vizsgálom, hogy fel szabad-e dolgoznom
                                    decision = CheckMessageForDuplicationProcess(e.Message, connection, out counter);
                                }
                                if (decision)
                                {
                                    Socket socket = null;
                                    SocketCloseMessage message = (SocketCloseMessage)e.Message;
                                    SafeBindedSocketsAccess(() =>
                                        {
                                            if (mBindedSockets.ContainsKey(message.TargetSocketId))
                                            {
                                                socket = mBindedSockets[message.TargetSocketId];
                                            }
                                        });
                                    if (socket != null)
                                    {
                                        // küldjük a socket-nek
                                        socket.InternalProcessSocketClose(message);
                                    }
                                }
                            }

                            #endregion
                        }
                        break;

                    case MessageCodeEnum.Negotiation:
                        {
                            #region Negotiation

                            // megjött a graf leíró üzenet
                            if (connection.ConnectionTask == null)
                            {
                                // nem találok hozzá connection task-ot... ez valami hekk vagy bug
                                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, connection task not found for negotiation message. SenderId: {0}, MessageId: {1}", e.Message.SenderId, e.Message.MessageId));
                                connection.NetworkStream.Close();
                            }
                            else if (!string.IsNullOrEmpty(connection.ConnectionTask.RemoteNetworkContextName))
                            {
                                // többszöri negotiation üzenet... hack vagy bug
                                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, duplicated negotiation message. SenderId: {0}, MessageId: {1}, RemoteNetworkContextName: {2}", e.Message.SenderId, e.Message.MessageId, connection.ConnectionTask.RemoteNetworkContextName));
                                connection.NetworkStream.Close();
                            }
                            else
                            {
                                NegotiationMessage message = (NegotiationMessage)e.Message;
                                connection.ConnectionTask.RemoteNetworkPeerId = message.SenderId;
                                connection.ConnectionTask.RemoteNetworkContextName = message.NetworkContextName;
                                bool disconnected = false;
                                NetworkPeerRemote remoteNetworkPeer = NetworkPeerContext.GetNetworkPeerById(e.Message.SenderId) as NetworkPeerRemote;
                                if (connection.ConnectionTask.LocalRequest)
                                {
                                    // kliens fogadja, ez már a lentebbi üzenet
                                    // küldöm neki a gráf üzenetet
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, server sent its negotiation message. SenderId: [{0}], NetworkContextName: [{1}]", message.SenderId, message.NetworkContextName));
                                    if (connection.ConnectionTask.DisconnectOnConnectionDuplication)
                                    {
                                        if (remoteNetworkPeer != null && remoteNetworkPeer.Session != null && remoteNetworkPeer.Session.ActiveConnections.Count > 0)
                                        {
                                            // tiltott duplikált csatlakozás
                                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, duplicated physical connection not allowed. Negotiation failed with message id: {0}, senderId: [{1}]", message.MessageId, message.SenderId));
                                            connection.NetworkStream.Close();
                                            disconnected = true;
                                        }
                                    }
                                    if (!disconnected && remoteNetworkPeer != null && remoteNetworkPeer.Session != null && remoteNetworkPeer.Session.ActiveConnections.Count >= mConfiguration.Settings.MaxConnectionsWithNetworkPeers)
                                    {
                                        // túl sok fizikai kapcsolat ezzel a klienssel
                                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, too many physical connection. Negotiation failed with message id: {0}, senderId: [{1}]", message.MessageId, message.SenderId));
                                        connection.NetworkStream.Close();
                                        disconnected = true;
                                    }
                                    if (!disconnected)
                                    {
                                        mLockMessageProcessor.Lock();
                                        try
                                        {
                                            SendCompleteGrafMessage(connection);
                                        }
                                        finally
                                        {
                                            mLockMessageProcessor.Unlock();
                                        }
                                    }
                                }
                                else
                                {
                                    // szerver fogadja, küldöm neki Én is a negotiation-t
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, client sent its negotiation message. SenderId: [{0}], NetworkContextName: [{1}]", message.SenderId, message.NetworkContextName));

                                    if (remoteNetworkPeer != null && remoteNetworkPeer.Session != null && remoteNetworkPeer.Session.ActiveConnections.Count > 0 && !mConfiguration.Settings.EnableMultipleConnectionWithNetworkPeers)
                                    {
                                        // tiltott duplikált csatlakozás
                                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, duplicated physical connection not allowed. Negotiation failed with message id: {0}, senderId: [{1}]", message.MessageId, message.SenderId));
                                        connection.NetworkStream.Close();
                                        disconnected = true;
                                    }
                                    if (!disconnected && remoteNetworkPeer != null && remoteNetworkPeer.Session != null && remoteNetworkPeer.Session.ActiveConnections.Count >= mConfiguration.Settings.MaxConnectionsWithNetworkPeers)
                                    {
                                        // túl sok fizikai kapcsolat ezzel a klienssel
                                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, too many physical connection. Negotiation failed with message id: {0}, senderId: [{1}]", message.MessageId, message.SenderId));
                                        connection.NetworkStream.Close();
                                        disconnected = true;
                                    }

                                    if (!disconnected)
                                    {
                                        message = new NegotiationMessage(mLocalHost.Id, mLocalHost.NetworkContext.Name, mLocalHost.Version);
                                        MessageTask messageTask = new MessageTask(message);
                                        connection.AddMessageTask(messageTask);
                                    }
                                }
                            }

                            #endregion
                        }
                        break;

                    case MessageCodeEnum.TerraGrafInformation:
                        {
                            #region TerraGrafInformationMessage
                            // megjött a graf leíró üzenet
                            TerraGrafInformationMessage message = (TerraGrafInformationMessage)e.Message;
                            if (message.NetworkInfo == null && message.TargetHostRelation != null)
                            {
                                #region Relation update

                                mLockMessageProcessor.Lock();
                                try
                                {
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, {0} with id '{1}' is a relation update message.", typeof(TerraGrafInformationMessage).Name, message.MessageId));
                                    if (CheckMessageForDuplicationProcessAndForward(e.Message, connection))
                                    {
                                        NetworkPeerRemote peer = (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(message.SenderId);
                                        if (peer == null)
                                        {
                                            if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, a message arrived from an unknown network peer. PeerId: {0}, MessageCode: {1}, MessageId: {2}", e.Message.SenderId, e.Message.MessageCode, e.Message.MessageId));
                                        }
                                        else
                                        {
                                            NetworkPeerRemote peerA = mLocalHost.Id.Equals(message.TargetHostRelation.PeerA) ? mLocalHost : (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(message.TargetHostRelation.PeerA);
                                            NetworkPeerRemote peerB = mLocalHost.Id.Equals(message.TargetHostRelation.PeerB) ? mLocalHost : (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(message.TargetHostRelation.PeerB);
                                            if (peerA == null)
                                            {
                                                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, unknown network peer for relation update change. PeerId: {0}, MessageId: {1}", message.TargetHostRelation.PeerA, e.Message.MessageId));
                                            }
                                            if (peerB == null)
                                            {
                                                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, unknown network peer for relation update change. PeerId: {0}, MessageId: {1}", message.TargetHostRelation.PeerB, e.Message.MessageId));
                                            }
                                            if (peerA != null && peerB != null)
                                            {
                                                OptimizeNetworkGraf(message, null);
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    mLockMessageProcessor.Unlock();
                                }

                                #endregion
                            }
                            else
                            {
                                #region Propagation message

                                if (message.BlackHoleContainer != null)
                                {
                                    #region Blackhole state update

                                    // ha nincs ConnectionTask, akkor egy új peer-t propagál valaki
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, {0} with id '{1}' is a blackhole state update message.", typeof(TerraGrafInformationMessage).Name, message.MessageId));
                                    mLockMessageProcessor.Lock();
                                    try
                                    {
                                        if (message.NetworkInfo != null)
                                        {
                                            // blackhole bekapcsolás
                                            CheckProcessAndForwardTerraGrafInfoMessage(message, connection);
                                        }
                                        else
                                        {
                                            // blackhole kikapcsolás
                                            if (CheckMessageForDuplicationProcessAndForward(message, connection))
                                            {
                                                OptimizeNetworkGraf(message, null);
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        mLockMessageProcessor.Unlock();
                                    }

                                    #endregion
                                }
                                else if (message.NetworkInfo != null && message.TargetHostRelation != null)
                                {
                                    #region Propagation forwarding

                                    // ha nincs ConnectionTask, akkor egy új peer-t propagál valaki
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, {0} with id '{1}' is a propagation update message.", typeof(TerraGrafInformationMessage).Name, message.MessageId));
                                    mLockMessageProcessor.Lock();
                                    try
                                    {
                                        CheckProcessAndForwardTerraGrafInfoMessage(message, connection);
                                    }
                                    finally
                                    {
                                        mLockMessageProcessor.Unlock();
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region Connection establishing

                                    // van hozzá task, becsatlakozás történik
                                    NetworkConnectionTask connectionTask = connection.ConnectionTask;
                                    if (connectionTask != null)
                                    {
                                        connectionTask.StopWatch();
                                        connection.ReplyTime = connectionTask.ReplyTime;
                                        connectionTask.InformationMessage = message;
                                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, {0} with id '{1}' is part of a connection establishment.", typeof(TerraGrafInformationMessage).Name, message.MessageId));
                                        mLockMessageProcessor.Lock();
                                        try
                                        {
                                            if (!connectionTask.LocalRequest)
                                            {
                                                // ha hozzám csatlakoztak be, akkor viszont küldöm a gráf infot és frissítem, amit kaptam
                                                SendCompleteGrafMessage(connection);
                                            }
                                            NetworkPeerRemote remoteNetworkPeer = NetworkPeerContext.GetNetworkPeerById(e.Message.SenderId) as NetworkPeerRemote;
                                            if (remoteNetworkPeer != null && remoteNetworkPeer.Distance == 1)
                                            {
                                                // csak még egy fizikai kapcsolat a meglévő mellé
                                                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, {0} with id '{1}' just an other physical network connection with [{2}].", typeof(TerraGrafInformationMessage).Name, message.MessageId, message.SenderId));
                                                remoteNetworkPeer.Session.AddNetworkConnection(connection);
                                                mActiveNetworkConnections.Remove(connection);

                                                bool disconnected = false;
                                                if (remoteNetworkPeer.Session.ActiveConnections.Count > 1 && !mConfiguration.Settings.EnableMultipleConnectionWithNetworkPeers)
                                                {
                                                    // tiltott duplikált csatlakozás
                                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, duplicated physical connection not allowed. Negotiation failed with message id: {0}, senderId: [{1}]", message.MessageId, message.SenderId));
                                                    connection.NetworkStream.Close();
                                                    disconnected = true;
                                                }
                                                if (!disconnected && remoteNetworkPeer.Session.ActiveConnections.Count > mConfiguration.Settings.MaxConnectionsWithNetworkPeers)
                                                {
                                                    // túl sok fizikai kapcsolat ezzel a klienssel
                                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, too many physical connection. Negotiation failed with message id: {0}, senderId: [{1}]", message.MessageId, message.SenderId));
                                                    connection.NetworkStream.Close();
                                                }
                                            }
                                            else
                                            {
                                                // új kapcsolat
                                                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, {0} with id '{1}' a new physical network connection with [{2}].", typeof(TerraGrafInformationMessage).Name, message.MessageId, message.SenderId));
                                                mActiveNetworkConnections.Add(connection);
                                                OptimizeNetworkGraf(message, null);
                                            }
                                        }
                                        finally
                                        {
                                            mLockMessageProcessor.Unlock();
                                        }
                                        bool isLocalRequest = connectionTask.LocalRequest;
                                        connectionTask.RaiseConnectionTaskFinished();
                                        if (!isLocalRequest)
                                        {
                                            connection.ConnectionTask = null;
                                        }
                                    }

                                    #endregion
                                }

                                #endregion
                            }
                            #endregion
                        }
                        break;

                    case MessageCodeEnum.TerraGrafPeerUpdate:
                        {
                            #region Network peer update

                            TerraGrafPeerUpdateMessage message = (TerraGrafPeerUpdateMessage)e.Message;
                            NetworkPeerRemote peer = null;
                            bool raiseContextEvent = false;
                            mLockMessageProcessor.Lock();
                            try
                            {
                                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, {0}", message.ToString()));
                                if (CheckMessageForDuplicationProcessAndForward(e.Message, connection))
                                {
                                    peer = (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(message.SenderId);
                                    if (peer == null)
                                    {
                                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, a message arrived from an unknown network peer. PeerId: {0}, MessageCode: {1}, MessageId: {2}", e.Message.SenderId, e.Message.MessageCode, e.Message.MessageId));
                                    }
                                    else
                                    {
                                        // data context
                                        if (message.PeerContextContainer != null)
                                        {
                                            if (peer.InternalPeerContext.StateId < message.PeerContextContainer.StateId)
                                            {
                                                peer.InternalPeerContext.StateId = message.PeerContextContainer.StateId;
                                                peer.InternalPeerContext.PeerContext = message.PeerContextContainer.PeerContext;
                                                raiseContextEvent = true;
                                            }
                                        }

                                        // tcp servers
                                        if (message.ServerContainer != null)
                                        {
                                            if (peer.TCPServerCollection.StateId < message.ServerContainer.StateId)
                                            {
                                                peer.TCPServerCollection.StateId = message.ServerContainer.StateId;
                                                List<TCPServer> servers = new List<TCPServer>();
                                                foreach (AddressEndPoint ep in message.ServerContainer.Servers)
                                                {
                                                    TCPServer tcpServer = new TCPServer(ep);
                                                    if (!peer.TCPServerCollection.TCPServers.Contains(tcpServer))
                                                    {
                                                        // még új
                                                        servers.Add(tcpServer);
                                                    }
                                                    else
                                                    {
                                                        // létező, megtartjuk az infoját
                                                        foreach (TCPServer server in peer.TCPServerCollection.TCPServers)
                                                        {
                                                            if (server.EndPoint.Equals(ep))
                                                            {
                                                                servers.Add(server);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                // frissítés
                                                peer.TCPServerCollection.TCPServers.Clear();
                                                peer.TCPServerCollection.TCPServers.AddRange(servers);
                                                servers.Clear();
                                            }
                                        }

                                        // NAT gateways
                                        if (message.NATGatewayContainer != null)
                                        {
                                            if (peer.NATGatewayCollection.StateId < message.NATGatewayContainer.StateId)
                                            {
                                                peer.NATGatewayCollection.StateId = message.NATGatewayContainer.StateId;
                                                List<NATGateway> gateways = new List<NATGateway>();
                                                foreach (AddressEndPoint ep in message.NATGatewayContainer.Gateways)
                                                {
                                                    NATGateway natGateway = new NATGateway(ep);
                                                    if (!peer.NATGatewayCollection.NATGateways.Contains(natGateway))
                                                    {
                                                        // még új
                                                        gateways.Add(natGateway);
                                                    }
                                                    else
                                                    {
                                                        // létező, megtartjuk az infoját
                                                        foreach (NATGateway gw in peer.NATGatewayCollection.NATGateways)
                                                        {
                                                            if (gw.EndPoint.Equals(ep))
                                                            {
                                                                gateways.Add(gw);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                // frissítés
                                                peer.NATGatewayCollection.NATGateways.Clear();
                                                peer.NATGatewayCollection.NATGateways.AddRange(gateways);
                                                gateways.Clear();
                                            }
                                        }

                                    }
                                }
                            }
                            finally
                            {
                                mLockMessageProcessor.Unlock();
                            }

                            if (raiseContextEvent)
                            {
                                OnNetworkPeerContextChanged(peer, false);
                            }

                            #endregion
                        }
                        break;

                    default:
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, unsupported message code: {0}", e.Message.MessageCode));
                        break;
                }
            }
            else
            {
                // nem nekem szól az üzenet, továbbítani kellhet
                CheckMessageForDuplicationProcessAndForward(e.Message, connection);
            }
        }

        private void OptimizeNetworkGraf(TerraGrafInformationMessage message, NetworkConnection offlineConnection)
        {
            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, begin optimize network graf. Reason: {0}.", message == null ? "physical connection aborted" : string.Format("{0} arrived", typeof(TerraGrafInformationMessage).Name)));

            Dictionary<NetworkPeerRemote, int> beforePeerState = new Dictionary<NetworkPeerRemote, int>();
            List<NetworkPeerRemote> peers = new List<NetworkPeerRemote>(NetworkPeerContext.InternalKnownNetworkPeers.Cast<NetworkPeerRemote>());
            Dictionary<NetworkPeerRemote, int> discoveredPeers = new Dictionary<NetworkPeerRemote, int>();
            List<NetworkPeerRemote> lostPeers = new List<NetworkPeerRemote>();
            Dictionary<NetworkPeerRemote, int> accessiblePeers = new Dictionary<NetworkPeerRemote, int>();
            Dictionary<INetworkPeerRemote, int> peerBeforeDistance = new Dictionary<INetworkPeerRemote, int>();
            Dictionary<INetworkPeerRemote, int> peerAfterDistance = new Dictionary<INetworkPeerRemote, int>();

            // elmentjük az eredeti távolságait a jelenlegi objektumoknak
            foreach (NetworkPeerRemote peer in peers)
            {
                beforePeerState.Add(peer, peer.Distance);
            }
            peers.Clear();

            // indítom az analizálót
            AnalyzeGraf(message, offlineConnection);

            // megnézzük az optimalizáció után mi változott
            peers = new List<NetworkPeerRemote>(NetworkPeerContext.InternalKnownNetworkPeers.Cast<NetworkPeerRemote>());
            foreach (NetworkPeerRemote peer in peers)
            {
                if (!beforePeerState.ContainsKey(peer))
                {
                    // új résztvevő
                    if (peer.Distance != 1 && mConfiguration.Settings.EnableAgressiveConnectionEstablishment)
                    {
                        // agresszív kapcsolatépítés engedélyezve. Új tagokra próbálunk kapcsolódni.
                        NetworkManager.Instance.InternalConnectAsync(peer, true, !NetworkManager.Instance.InternalConfiguration.Settings.EnableMultipleConnectionWithNetworkPeers);
                    }
                    discoveredPeers.Add(peer, peer.Distance);
                }
                else
                {
                    // ezzel a géppel előzőleg volt kapcsolat
                    if (peer.Distance == 0 && beforePeerState[peer] != 0)
                    {
                        // most viszont már nincsen vele kapcsolat
                        //computer.SetLastConnectionTime(DateTime.Now);
                        //computer.clearConnectionPairs(); // minden kapcsolódás státuszának false-ra állítása
                        peer.PeerRelationPairs.SetAllConnectionOffline(false);
                        lostPeers.Add(peer);
                    }
                    else if (peer.Distance != beforePeerState[peer])
                    {
                        // csak az útvonal távolsága változott
                        accessiblePeers.Add(peer, peer.Distance);
                        peerBeforeDistance.Add(peer, beforePeerState[peer]);
                        peerAfterDistance.Add(peer, peer.Distance);
                    }
                }
            }

            // elérhetettlen gépek lejelentése
            if (lostPeers.Count > 0)
            {
                Raiser.CallDelegatorByAsync(NetworkPeerUnaccessible, new object[] { this, new NetworkPeerChangedEventArgs(lostPeers.ToArray()) });
            }
            // új gépek lejelentése
            if (discoveredPeers.Count > 0)
            {
                Raiser.CallDelegatorByAsync(NetworkPeerDiscovered, new object[] { this, new NetworkPeerChangedEventArgs(discoveredPeers.Keys.ToArray<NetworkPeerRemote>()) });
            }
            // távolságok változásának lejelentése
            if (accessiblePeers.Count > 0)
            {
                Raiser.CallDelegatorByAsync(NetworkPeerDistanceChanged,
                    new object[] 
                    { 
                        this, 
                        new NetworkPeerDistanceChangedEventArgs(accessiblePeers.Keys.ToArray<NetworkPeerRemote>(), peerBeforeDistance, peerAfterDistance)
                    });
            }

            // referenciák törlése
            peerBeforeDistance.Clear();
            peerAfterDistance.Clear();
            beforePeerState.Clear();
            discoveredPeers.Clear();
            lostPeers.Clear();
            accessiblePeers.Clear();
            peers.Clear();

            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, end optimize network graf. Reason: {0}. --------------------------------------------", message == null ? "physical connection aborted" : string.Format("{0} arrived", typeof(TerraGrafInformationMessage).Name)));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void AnalyzeGraf(TerraGrafInformationMessage message, NetworkConnection offlineConnection)
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("NETWORK, analyzing graf structure...");
            bool needOptimize = false;
            if (offlineConnection != null)
            {
                // jelenleg ide akkor futunk, ha nincs már fizikai átjáró
                offlineConnection.OwnerSession.RemoveNetworkConnection(offlineConnection);
                offlineConnection.OwnerSession.NetworkConnection = null; // nincs aktív átjáró
                offlineConnection.OwnerSession.RemotePeer.Distance = 0; // elérhetettlen
                PeerRelationPair pair = mLocalHost.PeerRelationPairs.AddOrUpdatePeerRelation(mLocalHost, offlineConnection.OwnerSession.RemotePeer, false, out needOptimize); // itt nem kell az out értéke
                offlineConnection.OwnerSession.RemotePeer.PeerRelationPairs.SetConnectionOffline(offlineConnection.OwnerSession.RemotePeer, mLocalHost);
                InternalSendMessage(new TerraGrafInformationMessage(mLocalHost.Id, pair.BuildPeerRelation()), null);
                needOptimize = true;
            }
            if (message != null)
            {
                if (message.NetworkInfo != null && !mLocalHost.Id.Equals(message.NetworkInfo.Sender.Id))
                {
                    Dictionary<NetworkPeer, NetworkPeerRemote> dictForRelations = new Dictionary<NetworkPeer, NetworkPeerRemote>();

                    // először létrehozom/frissítem a peer-eket
                    dictForRelations[message.NetworkInfo.Sender] = ProcessNetworkPeerData(message.NetworkInfo.Sender, message);
                    if (message.NetworkInfo.KnownNetworkPeers != null)
                    {
                        foreach (NetworkPeer peer in message.NetworkInfo.KnownNetworkPeers)
                        {
                            // önmagammal nem foglakozunk
                            if (!peer.Id.Equals(mLocalHost.Id))
                            {
                                dictForRelations[peer] = ProcessNetworkPeerData(peer, null);
                            }
                        }
                    }

                    // aztán felveszem a kapcsolataikat
                    foreach (KeyValuePair<NetworkPeer, NetworkPeerRemote> kv in dictForRelations)
                    {
                        NetworkPeerContext.RefreshNetworkPeerOnlyRelations(kv.Key, kv.Value);
                    }
                    dictForRelations.Clear();
                }
                if (message.BlackHoleContainer != null)
                {
                    // Blackhole állapot frissítése
                    NetworkPeerRemote remoteNetworkPeer = NetworkPeerContext.GetNetworkPeerById(message.SenderId) as NetworkPeerRemote;
                    if (remoteNetworkPeer.BlackHoleContainer.StateId < message.BlackHoleContainer.StateId)
                    {
                        remoteNetworkPeer.BlackHoleContainer.StateId = message.BlackHoleContainer.StateId;
                        remoteNetworkPeer.BlackHoleContainer.IsBlackHole = message.BlackHoleContainer.IsBlackHole;
                        if (remoteNetworkPeer.BlackHoleContainer.IsBlackHole)
                        {
                            // ha egy peer BH-vá változik, az összes belső kapcsolatát törlöm.
                            // ha kikapcsolja a BH állapotát, propagálnia kell a kapcsolatait és a relation-öket is felveszem majd ismét.
                            remoteNetworkPeer.PeerRelationPairs.SetAllConnectionOffline(false);
                        }
                    }
                }
                if (message.TargetHostRelation != null)
                {
                    // ez egy továbbított üzenet, amiben már benne van a csatlakozási host relation bejegyzése is
                    NetworkPeerRemote peerA = NetworkPeerContext.GetNetworkPeerById(message.TargetHostRelation.PeerA) as NetworkPeerRemote; // ilyen üzenet nem jöhet, amiben rólam van szó
                    NetworkPeerRemote peerB = mLocalHost.Id.Equals(message.TargetHostRelation.PeerB) ? mLocalHost : NetworkPeerContext.GetNetworkPeerById(message.TargetHostRelation.PeerB) as NetworkPeerRemote;
                    if (peerA == null)
                    {
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, unknown network peer in graf information message. PeerId: {0}, MessageId: {1}", message.TargetHostRelation.PeerA, message.MessageId));
                    }
                    if (peerB == null)
                    {
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, unknown network peer in graf information message. PeerId: {0}, MessageId: {1}", message.TargetHostRelation.PeerB, message.MessageId));
                    }
                    if (peerA != null && peerB != null)
                    {
                        peerA.PeerRelationPairs.AddOrUpdatePeerRelationForce(peerA, peerB, message.TargetHostRelation.Connected, message.TargetHostRelation.StateId);
                    }
                }
                else if (mLocalHost.Id.Equals(message.TargetId))
                {
                    // nekem küldték az üzenetet
                    NetworkPeerRemote remoteNetworkPeer = NetworkPeerContext.GetNetworkPeerById(message.SenderId) as NetworkPeerRemote;
                    bool changed = false;
                    remoteNetworkPeer.PeerRelationPairs.AddOrUpdatePeerRelation(remoteNetworkPeer, mLocalHost, true, out changed); // kapcsolat felvétele
                    changed = false;
                    PeerRelationPair pair = mLocalHost.PeerRelationPairs.AddOrUpdatePeerRelation(mLocalHost, remoteNetworkPeer, true, out changed); // kapcsolat felvétele
                    if (changed && !mLocalHost.BlackHoleContainer.IsBlackHole)
                    {
                        // propagálom az új kapcsolatom (nem a peer-t)
                        TerraGrafInformationMessage updateMessageExceptOrigin = new TerraGrafInformationMessage(mLocalHost.Id, message.NetworkInfo, pair.BuildPeerRelation());
                        TerraGrafInformationMessage updateMessageIncludeOrigin = new TerraGrafInformationMessage(mLocalHost.Id, pair.BuildPeerRelation());
                        foreach (NetworkConnection c in mActiveNetworkConnections)
                        {
                            if (c.IsConnected)
                            {
                                if (c.OwnerSession == null || (c.OwnerSession != null && !c.OwnerSession.RemotePeer.Id.Equals(message.SenderId)))
                                {
                                    // idegen kapcsolatra a teljes eredeti gráfot küldöm, benne az én relation bejegyzésemmel
                                    string networkContext = c.ConnectionTask == null ? c.OwnerSession.RemotePeer.NetworkContext.Name : c.ConnectionTask.RemoteNetworkContextName;
                                    if (mNetworkContextRuleManager.CheckSeparation(networkContext, remoteNetworkPeer.NetworkContext.Name) &&
                                        mNetworkContextRuleManager.CheckSeparation(mLocalHost.NetworkContext.Name, remoteNetworkPeer.NetworkContext.Name))
                                    {
                                        c.AddMessageTask(new MessageTask(updateMessageExceptOrigin));
                                    }
                                    else
                                    {
                                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, sending message to network context [{0}] is not allowed.", networkContext));
                                    }
                                }
                                else
                                {
                                    // ez a küldő kapcsolata, ide csak a kapcsolati relation update megy
                                    c.AddMessageTask(new MessageTask(updateMessageIncludeOrigin));
                                }
                            }
                        }
                    }
                }
            }
            if (this.mActiveNetworkConnections.Count > 0)
            {
                HashSet<NetworkPeerRemote> neighbors = new HashSet<NetworkPeerRemote>();
                foreach (NetworkConnection c in mActiveNetworkConnections)
                {
                    if (c.OwnerSession != null)
                    {
                        neighbors.Add(c.OwnerSession.RemotePeer);
                    }
                }
                if (neighbors.Count > 0)
                {
                    // megkeresünk minden olyan gépet, akinek van kapcsolata másik géppel
                    // és nem a szomszédom. Kitörlöm az optimális átjárót mindenkitől,
                    // hogy az optimalizáló beállíthassa azt
                    // akinél végül nem lesz beállítva semmi, azt elveszettnek nyilvánítjuk
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, number of my neighbors: {0}", neighbors.Count));

                    foreach (INetworkPeerRemote p in NetworkPeerContext.InternalKnownNetworkPeers)
                    {
                        NetworkPeerRemote peer = (NetworkPeerRemote)p;
                        if (!neighbors.Contains<NetworkPeerRemote>(peer))
                        {
                            peer.Session.NetworkConnection = null;
                            peer.Distance = 0;
                        }
                    }
                    NetworkGrafBuilder(neighbors);
                    neighbors.Clear();
                }
                else
                {
                    // nincsennek szomszédaim, egy sem
                    SetAllConnectionOffline();
                }
            }
            else
            {
                // nincs aktív kapcsolatom, egy sem
                SetAllConnectionOffline();
            }

        }

        private void NetworkGrafBuilder(HashSet<NetworkPeerRemote> neighbors)
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("NETWORK, rebuild network graf structure...");
            List<HashSet<NetworkPeerRemote>> levels = new List<HashSet<NetworkPeerRemote>>();
            levels.Add(neighbors);
            OptimizeNextLevel(levels);
            levels.Clear();
        }

        private void OptimizeNextLevel(List<HashSet<NetworkPeerRemote>> levels)
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, begin optimizing network hierarchy level {0}.", levels.Count));

            HashSet<NetworkPeerRemote> currentLevel = levels[levels.Count - 1];
            HashSet<NetworkPeerRemote> nextLevel = new HashSet<NetworkPeerRemote>();

            foreach (NetworkPeerRemote peer in currentLevel)
            {
                if (!peer.BlackHoleContainer.IsBlackHole)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, collecting neighborhood for peer '{0}'.", peer.Id));
                    ICollection<NetworkPeerRemote> neighborhood = peer.PeerRelationPairs.Neighborhood;
                    if (neighborhood.Count > 0)
                    {
                        foreach (NetworkPeerRemote neighbor in neighborhood)
                        {
                            // önmagamat nem vizsgálom
                            if (!neighbor.Id.Equals(mLocalHost.Id))
                            {
                                if (neighbor.Distance == 0)
                                {
                                    // még nem volt optimalizálva, most kerül átjáró bele
                                    neighbor.Session.NetworkConnection = peer.Session.NetworkConnection;
                                    neighbor.Distance = levels.Count + 1;
                                    neighbor.Session.ReplyTime = peer.Session.ReplyTime;
                                    // felvesszük a következő szinthez a szomszéd számítógépet
                                    nextLevel.Add(neighbor);
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, set new parent '{0}' for peer '{1}' on level {2}.", peer.Id, neighbor.Id, levels.Count));
                                }
                                else if (neighbor.Distance == levels.Count + 1)
                                {
                                    // már valaki bejegyezte ezen a szinten az átjárót
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, peer '{0}' already has gateway. Distance: {1}", neighbor.Id, neighbor.Distance));
                                    if (neighbor.Session.ReplyTime > peer.Session.ReplyTime)
                                    {
                                        // ezen a szülőn várhatóan rövidebb ideig fog utazni az üzenet
                                        neighbor.Session.NetworkConnection = peer.Session.NetworkConnection;
                                        neighbor.Session.ReplyTime = peer.Session.ReplyTime;
                                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, update to new parent '{0}' for peer '{1}' on level {2}.", peer.Id, neighbor.Id, levels.Count));
                                    }
                                }
                            }
                        }
                        neighborhood.Clear();
                    }
                    else
                    {
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, neighborhood not found for peer '{0}'.", peer.Id));
                    }
                }
                else
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, network peer '{0}' is a black hole. Skip it from optimization.", peer.Id));
                }
            }

            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, end of optimizing network hierarchy level {0}.", levels.Count));

            currentLevel.Clear();
            if (nextLevel.Count > 0)
            {
                // van következő szint
                levels.Add(nextLevel);
                OptimizeNextLevel(levels);
            }
        }

        private NetworkPeerRemote ProcessNetworkPeerData(NetworkPeer networkPeer, TerraGrafInformationMessage message)
        {
            NetworkPeerRemote remoteNetworkPeer = NetworkPeerContext.GetNetworkPeerById(networkPeer.Id) as NetworkPeerRemote;
            if (remoteNetworkPeer == null)
            {
                // új network peer létrehozása
                remoteNetworkPeer = NetworkPeerContext.CreateNetworkPeer(networkPeer);
                remoteNetworkPeer.Session = new NetworkPeerSession(remoteNetworkPeer);
                if (mUnknownSenderMessageCounter.ContainsKey(remoteNetworkPeer.Id))
                {
                    Dictionary<long, int> dict = mUnknownSenderMessageCounter[remoteNetworkPeer.Id];
                    remoteNetworkPeer.Session.MergeMessageIDCounters(dict);
                    mUnknownSenderMessageCounter.Remove(remoteNetworkPeer.Id);
                }
            }
            else
            {
                // már létező peer frissítése
                NetworkPeerContext.RefreshNetworkPeerWithoutRelations(networkPeer, remoteNetworkPeer);
            }
            if (message != null)
            {
                // itt csatolom a session-höz a kapcsolatot ezzel a message-es trükkel
                foreach (NetworkConnection c in mActiveNetworkConnections)
                {
                    if (c.ConnectionTask != null && message.Equals(c.ConnectionTask.InformationMessage))
                    {
                        // ezaz
                        remoteNetworkPeer.Session.AddNetworkConnection(c);
                        remoteNetworkPeer.Session.NetworkConnection = c;
                        remoteNetworkPeer.Distance = 1;
                    }
                }
            }
            return remoteNetworkPeer;
        }

        private void SetAllConnectionOffline()
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("NETWORK, no neighborhood found. All connections lost.");
            mLocalHost.PeerRelationPairs.SetAllConnectionOffline(true);
            foreach (INetworkPeerRemote peer in NetworkPeerContext.InternalKnownNetworkPeers)
            {
                NetworkPeerRemote p = (NetworkPeerRemote)peer;
                p.Distance = 0;
                p.PeerRelationPairs.SetAllConnectionOffline(false);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void CheckProcessAndForwardTerraGrafInfoMessage(TerraGrafInformationMessage message, NetworkConnection exceptConnection)
        {
            if (message == null)
            {
                ThrowHelper.ThrowArgumentNullException("message");
            }
            if (exceptConnection == null)
            {
                ThrowHelper.ThrowArgumentNullException("exceptConnection");
            }

            int counter = 0;
            if (CheckMessageForDuplicationProcess(message, exceptConnection, out counter))
            {
                OptimizeNetworkGraf(message, null); // útvonal optimalizálás
                if (!mConfiguration.Settings.BlackHole)
                {
                    string arrivedNetworkContext = exceptConnection.ConnectionTask == null ? exceptConnection.OwnerSession.RemotePeer.NetworkContext.Name : exceptConnection.ConnectionTask.RemoteNetworkContextName;
                    Dictionary<string, TerraGrafInformationMessage> filteredMessageForNetworkContexts = new Dictionary<string, TerraGrafInformationMessage>();
                    foreach (NetworkConnection c in mActiveNetworkConnections)
                    {
                        // 1. élő kapcsolat
                        // 2. kivétel kapcsolatra ne (jellemzően, ahonnan jött)
                        // 3. a feladónak szintén ne küldjük vissza
                        try
                        {
                            if (c.IsConnected &&
                                !c.Equals(exceptConnection) &&
                                (c.OwnerSession == null || !c.OwnerSession.RemotePeer.Id.Equals(message.SenderId)))
                            {
                                string targetNetworkContext = c.ConnectionTask == null ? c.OwnerSession.RemotePeer.NetworkContext.Name : c.ConnectionTask.RemoteNetworkContextName;
                                NetworkPeerRemote sourceNetworkPeer = message.SenderId.Equals(mLocalHost.Id) ? mLocalHost : (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(message.SenderId);
                                if (mNetworkContextRuleManager.CheckSeparation(mLocalHost.NetworkContext.Name, targetNetworkContext))
                                {
                                    if (mNetworkContextRuleManager.CheckSeparation(sourceNetworkPeer.NetworkContext.Name, targetNetworkContext))
                                    {
                                        if (sourceNetworkPeer == null || string.IsNullOrEmpty(arrivedNetworkContext) || (mNetworkContextRuleManager.CheckSeparation(arrivedNetworkContext, targetNetworkContext)))
                                        {
                                            // a belső network info sender-e alapján is vizsgálunk, ami nem biztos hogy egyezik a message sender-el
                                            if (message.NetworkInfo.Sender == null || mNetworkContextRuleManager.CheckSeparation(targetNetworkContext, message.NetworkInfo.Sender.NetworkContext))
                                            {
                                                NetworkPeerRemote connectionPeer = c.ConnectionTask == null ? c.OwnerSession.RemotePeer : null;
                                                if (ForwardSpecialDecision(message, sourceNetworkPeer, connectionPeer))
                                                {
                                                    // belső tartalom szűrése, a networkinfo nem minden tartalmát szabad továbbítani
                                                    if (filteredMessageForNetworkContexts.ContainsKey(targetNetworkContext))
                                                    {
                                                        // már létezik a szűrt és testreszabott üzenet ennek a network-nek
                                                        c.AddMessageTask(new MessageTask(filteredMessageForNetworkContexts[targetNetworkContext]));
                                                    }
                                                    else
                                                    {
                                                        #region Create filtered message

                                                        // még nem létezik
                                                        List<NetworkPeer> allowedPeers = new List<NetworkPeer>();
                                                        HashSet<string> filteredPeers = new HashSet<string>();
                                                        PeerRelation targetHostRelation = message.TargetHostRelation;
                                                        if (message.NetworkInfo.KnownNetworkPeers != null)
                                                        {
                                                            foreach (NetworkPeer peer in message.NetworkInfo.KnownNetworkPeers)
                                                            {
                                                                if (mNetworkContextRuleManager.CheckSeparation(targetNetworkContext, peer.NetworkContext))
                                                                {
                                                                    // engedélyezett
                                                                    bool decision = mLocalHost.Id.Equals(peer.Id);
                                                                    if (!decision)
                                                                    {
                                                                        NetworkPeerRemote remotePeerToForward = (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(peer.Id);
                                                                        if (remotePeerToForward.Session.NetworkConnection != null)
                                                                        {
                                                                            // megnézem a hálózati kapcsolatot, hogy amerre el lehet jutni, az engedélyezett-e
                                                                            if (mNetworkContextRuleManager.CheckSeparation(targetNetworkContext, remotePeerToForward.Session.NetworkConnection.OwnerSession.RemotePeer.NetworkContext.Name))
                                                                            {
                                                                                decision = true;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            decision = true;
                                                                        }
                                                                    }
                                                                    if (decision)
                                                                    {
                                                                        allowedPeers.Add(peer);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    // tiltott
                                                                    filteredPeers.Add(peer.Id);
                                                                }
                                                            }
                                                            if (filteredPeers.Count > 0)
                                                            {
                                                                if (allowedPeers.Count > 0)
                                                                {
                                                                    // kiszedem a tiltott peer-ek kapcsolatait is
                                                                    foreach (NetworkPeer peer in allowedPeers)
                                                                    {
                                                                        if (peer.PeerRelations.PeerRelations != null)
                                                                        {
                                                                            List<PeerRelation> filteredRelations = new List<PeerRelation>();
                                                                            foreach (PeerRelation r in peer.PeerRelations.PeerRelations)
                                                                            {
                                                                                if (!filteredPeers.Contains(r.PeerB))
                                                                                {
                                                                                    filteredRelations.Add(r);
                                                                                }
                                                                            }
                                                                            peer.PeerRelations.PeerRelations = filteredRelations.ToArray();
                                                                        }
                                                                    }
                                                                }
                                                                if (targetHostRelation != null && filteredPeers.Contains(targetHostRelation.PeerB))
                                                                {
                                                                    targetHostRelation = null;
                                                                }
                                                            }
                                                        }

                                                        TerraGrafNetworkInformation info = new TerraGrafNetworkInformation();
                                                        info.Sender = message.NetworkInfo.Sender;
                                                        info.KnownNetworkPeers = allowedPeers.ToArray();

                                                        TerraGrafInformationMessage filteredMessage = new TerraGrafInformationMessage(message.SenderId, message.MessageId, info, targetHostRelation, message.BlackHoleContainer);
                                                        filteredMessageForNetworkContexts[targetNetworkContext] = filteredMessage;
                                                        c.AddMessageTask(new MessageTask(filteredMessage));

                                                        #endregion
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, sending message between network contexts [{0}] and [{1}] is not allowed. Sender id: [{2}], message id: {3}.", targetNetworkContext, arrivedNetworkContext, message.SenderId, message.MessageId));
                                        }
                                    }
                                    else
                                    {
                                        if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, sending message between network contexts [{0}] and [{1}] is not allowed. Sender id: [{2}], message id: {3}.", targetNetworkContext, sourceNetworkPeer.NetworkContext.Name, message.SenderId, message.MessageId));
                                    }
                                }
                                else
                                {
                                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("NETWORK, sending message between network contexts [{0}] and [{1}] is not allowed. Sender id: [{2}], message id: {3}.", targetNetworkContext, mLocalHost.NetworkContext.Name, message.SenderId, message.MessageId));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug("NETWORK, failed to forward a broadcast message.", ex);
                        }
                    }
                }
            }
        }

        private bool ForwardSpecialDecision(TerraGrafMessageBase message, NetworkPeerRemote messageSenderPeer, NetworkPeerRemote connectionPeer)
        {
            bool result = true;

            if (messageSenderPeer != null && connectionPeer != null)
            {
                if (!mLocalHost.Id.Equals(messageSenderPeer.Id))
                {
                    // saját üzenetemet nem vizsgálom
                    if (message.MessageCode == MessageCodeEnum.SocketClose ||
                        message.MessageCode == MessageCodeEnum.SocketOpenRequest ||
                        message.MessageCode == MessageCodeEnum.SocketOpenResponse ||
                        message.MessageCode == MessageCodeEnum.SocketRawData ||
                        message.MessageCode == MessageCodeEnum.Acknowledge)
                    {
                        // ha az üzenet küldőjének közvetlen szomszédja az, akinek Én most küldeni akarom, akkor nem küldöm az üzenetet
                        result = !messageSenderPeer.PeerRelationPairs.Neighborhood.Contains(connectionPeer);
                    }
                }
                if (result)
                {
                    // az üzenet eredeti feladójának nem küldöm vissza az üzenetet
                    result = !messageSenderPeer.Id.Equals(connectionPeer.Id);
                }
            }

            return result;
        }

        /// <summary>
        /// Checks the message for duplication process and forward.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exceptConnection">The except connection.</param>
        /// <returns></returns>
        private bool CheckMessageForDuplicationProcessAndForward(TerraGrafMessageBase message, NetworkConnection exceptConnection)
        {
            int counter = 0;
            bool result = CheckMessageForDuplicationProcess(message, exceptConnection, out counter);

            if (message.MessageType == MessageTypeEnum.Udp)
            {
                // broadcast és UDP üzenetet csak 1x küldünk tovább és 1x dolgozunk fel
                if (counter == 0)
                {
                    if (string.IsNullOrEmpty(message.TargetId))
                    {
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, message with id '{0}' is a broadcast type and allowed to forwarding.", message.MessageId));
                    }
                    else
                    {
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, message with id '{0}' is a {1} type and allowed to forwarding.", message.MessageId, message.MessageType));
                    }
                    InternalSendMessage(message, exceptConnection); // továbbküldés
                }
                else
                {
                    if (string.IsNullOrEmpty(message.TargetId))
                    {
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, message with id '{0}' is a broadcast type and NOT allowed to forwarding.", message.MessageId));
                    }
                    else
                    {
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, message with id '{0}' is a {1} type and NOT allowed to forwarding.", message.MessageId, message.MessageType));
                    }
                }
            }
            else
            {
                // a TCP típusú üzenetnél a számláló dönt
                if (counter < InternalConfiguration.Settings.MaxMessagePassageNumber && !mLocalHost.IsBlackHole)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, message with id '{0}' is a {1} type and allowed to forwarding.", message.MessageId, message.MessageType));
                    InternalSendMessage(message, exceptConnection); // továbbküldés
                }
                else
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, message with id '{0}' is a {1} type and NOT allowed to forwarding. Maximum passage number reached.", message.MessageId, message.MessageType));
                }
            }

            return result;
        }

        private bool CheckMessageForDuplicationProcess(TerraGrafMessageBase message, NetworkConnection exceptConnection, out int counter)
        {
            bool result = false;

            NetworkPeerRemote peer = mLocalHost.Id.Equals(message.SenderId) ? mLocalHost : (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(message.SenderId);
            if (peer == null)
            {
                // ismeretlen küldő
                if (exceptConnection == null)
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, message arrived from an unknown network peer [{0}].", message.SenderId));
                }
                else
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NETWORK, message arrived from an unknown network peer [{0}]. Received from [{1}].", message.SenderId, exceptConnection.OwnerSession == null ? "<unknown>" : exceptConnection.OwnerSession.RemotePeer.Id));
                }

                Dictionary<long, int> dictCounter = null;
                if (mUnknownSenderMessageCounter.ContainsKey(message.SenderId))
                {
                    dictCounter = mUnknownSenderMessageCounter[message.SenderId];
                }
                else
                {
                    dictCounter = new Dictionary<long, int>();
                    mUnknownSenderMessageCounter[message.SenderId] = dictCounter;
                }
                if (dictCounter.ContainsKey(message.MessageId))
                {
                    counter = dictCounter[message.MessageId] + 1;
                }
                else
                {
                    counter = 1;
                }
                dictCounter[message.MessageId] = counter;
            }
            else
            {
                // ismert küldő
                counter = peer.Session.GetMessageIDCounter(message.MessageId);
                if (counter == 0)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, message with id '{0}' allowed to process.", message.MessageId));
                    result = true; // feldolgozható az üzenet, mert még sosem járt itt
                }
                else
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, message with id '{0}' not allowed to process. Counter: {1}", message.MessageId, counter));
                }
                peer.Session.IncMessageIDCounter(message.MessageId); // növeljük a számlálót
            }

            return result;
        }

        private void NetworkConnection_Disconnect(object sender, EventArgs e)
        {
            // egy hálózati kapcsolat megszűnt
            NetworkConnection connection = (NetworkConnection)sender;
            NetworkConnectionTask connectionTask = connection.ConnectionTask;
            connection.Disconnect -= new EventHandler<EventArgs>(NetworkConnection_Disconnect);
            connection.MessageSendBefore -= new EventHandler<MessageSendEventArgs>(NetworkConnection_MessageSendBefore);
            connection.MessageSendAfter -= new EventHandler<MessageSendEventArgs>(NetworkConnection_MessageSendAfter);
            connection.MessageArrived -= new EventHandler<MessageArrivedEventArgs>(NetworkConnection_MessageArrived);

            if (connectionTask == null)
            {
                //TODO: a benne lévő üzeneteket meg kell próbálni másik útvonalon eljuttatni (prio 1 és 3)
                //TODO: ha van még fizikai kapcsolat, akkor gyorsan másikat kijelölni (replytime) és abba bemásolni az üzeneteket
                //TODO: ha nincs több kapcsolat, akkor prio 2 üzik elvesznek és indul a gráf opti
                mLockMessageProcessor.Lock();
                try
                {
                    // keresünk másik átjárót
                    if (connection.OwnerSession != null)
                    {
                        connection.OwnerSession.RemoveNetworkConnection(connection);
                        if (connection.OwnerSession.NetworkConnection != null && connection.OwnerSession.NetworkConnection.Equals(connection))
                        {
                            // az aktuális kijelölt átjáró szakadt le
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, a network connection aborted ({0}). This was the gateway between me and [{1}].", connection.NetworkStream.Id.ToString(), connection.OwnerSession.RemotePeer.Id));
                            mActiveNetworkConnections.Remove(connection);
                            connection.OwnerSession.NetworkConnection = null;
                            // választok másikat, ha van miből...
                            foreach (NetworkConnection c in connection.OwnerSession.ActiveConnections)
                            {
                                if (c.IsConnected)
                                {
                                    // ide bemásolok minden üzenetet továbbküldésre
                                    connection.OwnerSession.NetworkConnection = c;
                                    connection.CopyOtherNetworkConnectionContent(c);
                                    mActiveNetworkConnections.Add(c);
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, a backup network connection selected between me and [{0}].", connection.OwnerSession.RemotePeer.Id));
                                    break;
                                }
                            }
                            if (connection.OwnerSession.NetworkConnection == null)
                            {
                                // nem sikerült másik közvetlen átjárót választani, mert nincs több aktív fizikai kapcsolata
                                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, no backup network connection found between me and [{0}].", connection.OwnerSession.RemotePeer.Id));
                                OptimizeNetworkGraf(null, connection);
                                if (connection.OwnerSession.NetworkConnection == null)
                                {
                                    // kapcsolat végleg megszakad, kipucolom a várakozó üzeneteket és meglököm a szálukat
                                    connection.SetFinishedToTaskFailed(mMessagesToAcknowledge); // minden connection a sajátját takarítja
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, [{0}] is unaccessible.", connection.OwnerSession.RemotePeer.Id));
                                }
                                else
                                {
                                    // optimalizálás után a kijelölt átjáróba másolás
                                    connection.CopyOtherNetworkConnectionContent(connection.OwnerSession.NetworkConnection);
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, [{0}] is accessible across [{1}].", connection.OwnerSession.RemotePeer.Id, connection.OwnerSession.NetworkConnection.OwnerSession.RemotePeer.Id));
                                }
                            }
                        }
                        else
                        {
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, a network connection aborted ({0}). This was a backup connection between me and [{1}].", connection.NetworkStream.Id.ToString(), connection.OwnerSession.RemotePeer.Id));
                        }
                    }
                }
                finally
                {
                    mLockMessageProcessor.Unlock();
                }
                // értesítem a csatlakozási manager-t, hogy leszakadt egy hálózati kapcsolat
                this.mConnectionManager.NetworkConnection_Disconnect(connection.NetworkStream);
            }
            else
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK, a network connection aborted ({0}) while establishing connection to a remote host.", connection.NetworkStream.Id.ToString()));
                connectionTask.RaiseConnectionTaskFinished(); // csatlakozási feladatot értesítjük, hogy sikertelen a kapcsolatfelvétel
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void SendCompleteGrafMessage(NetworkConnection connection)
        {
            mActiveNetworkConnections.Add(connection);
            TerraGrafNetworkInformation grafInfo = BuildCompleteGrafSnapshot(connection.ConnectionTask.RemoteNetworkContextName);
            TerraGrafInformationMessage message = new TerraGrafInformationMessage(mLocalHost.Id, connection.ConnectionTask.RemoteNetworkPeerId, grafInfo);
            MessageTask messageTask = new MessageTask(message);
            connection.AddMessageTask(messageTask);
        }

        private TerraGrafNetworkInformation BuildCompleteGrafSnapshot(string targetContextName)
        {
            TerraGrafNetworkInformation result = new TerraGrafNetworkInformation();

            // 1, Köztem és a cél context között van tiltás?
            // 2, Van tiltás a két network context-en?
            // 3, Van legalább egy közvetlen és nem BlackHole kapcsolatom a context bármely tagjára?
            result.Sender = mLocalHost.BuildNetworkPeer(InternalConfiguration.Settings.BlackHole, targetContextName);
            if (!InternalConfiguration.Settings.BlackHole)
            {
                // 1. szabály (lásd 5. példa)
                if (mNetworkContextRuleManager.CheckSeparation(mLocalHost.NetworkContext.Name, targetContextName))
                {
                    ICollection<INetworkPeerRemote> peers = NetworkPeerContext.InternalKnownNetworkPeers;
                    List<NetworkPeerRemote> allowedNetworkPeers = new List<NetworkPeerRemote>();
                    HashSet<NetworkContext> enabledContexts = new HashSet<NetworkContext>();
                    HashSet<NetworkContext> disabledContexts = new HashSet<NetworkContext>();
                    foreach (INetworkPeerRemote p in peers)
                    {
                        NetworkPeerRemote peer = (NetworkPeerRemote)p;
                        // 2. szabály (lásd 1. példa)
                        if (mNetworkContextRuleManager.CheckSeparation(peer.NetworkContext.Name, targetContextName))
                        {
                            // nincs tiltás, de...
                            if (peer.Session.NetworkConnection != null)
                            {
                                // a peer átjárójának tulaja engedélyezett-e
                                if (mNetworkContextRuleManager.CheckSeparation(targetContextName, peer.Session.NetworkConnection.OwnerSession.RemotePeer.NetworkContext.Name))
                                {
                                    allowedNetworkPeers.Add(peer);
                                }
                            }
                            else
                            {
                                allowedNetworkPeers.Add(peer);
                            }
                            //bool decision = enabledContexts.Contains<NetworkContext>(peer.NetworkContext);
                            //if (targetContextName.Equals(peer.NetworkContext.Name))
                            //{
                            //    // ugyanarról a context-ről van szó, nem vizsgáljuk a 3. szabályt
                            //    decision = true;
                            //    enabledContexts.Add(peer.NetworkContext);
                            //}
                            //else if (!disabledContexts.Contains<NetworkContext>(peer.NetworkContext))
                            //{
                            //    // 3. szabály (lásd 2. példa)
                            //    ICollection<INetworkPeerRemote> peersInContext = peer.NetworkContext.KnownNetworkPeers;
                            //    foreach (INetworkPeerRemote _peer in peersInContext)
                            //    {
                            //        NetworkPeerRemote remote = (NetworkPeerRemote)_peer;
                            //        // BlackHole?
                            //        if (!remote.BlackHoleContainer.IsBlackHole)
                            //        {
                            //            // nem BlackHole
                            //            if (remote.Id.Equals(remote.Session.NetworkConnection.OwnerSession.RemotePeer.Id))
                            //            {
                            //                // közvetlen kapcsolatunk van
                            //                decision = true;
                            //                enabledContexts.Add(peer.NetworkContext);
                            //                break;
                            //            }
                            //        }
                            //    }
                            //    if (!decision)
                            //    {
                            //        disabledContexts.Add(peer.NetworkContext);
                            //    }
                            //}
                            //if (decision)
                            //{
                            //// van kapcsolatom a context legalább egy tagjára, aki nem blackhole
                            //allowedNetworkPeers.Add(peer);
                            //}
                        }
                    }

                    enabledContexts.Clear();
                    disabledContexts.Clear();

                    if (allowedNetworkPeers.Count > 0)
                    {
                        result.KnownNetworkPeers = new NetworkPeer[allowedNetworkPeers.Count];
                        int index = 0;
                        foreach (NetworkPeerRemote p in allowedNetworkPeers)
                        {
                            result.KnownNetworkPeers[index] = p.BuildNetworkPeer(false, targetContextName);
                            index++;
                        }
                    }
                }
            }

            return result;
        }

        private void CloseAsyncActiveConnectEvent(int asyncActiveCount)
        {
            if ((this.mAsyncActiveConnectEvent != null) && (asyncActiveCount == 0))
            {
                this.mAsyncActiveConnectEvent.Close();
                this.mAsyncActiveConnectEvent = null;
            }
        }

        private void InternalConnectAsyncEnd(IAsyncResult result)
        {
            NetworkManagerInternalConnectDelegate d = (NetworkManagerInternalConnectDelegate)result.AsyncState;
            d.EndInvoke(result);
        }

        private void SafeBindedSocketsAccess(Action action)
        {
            SafeBindedSocketsAccess(false, action);
        }

        private void SafeBindedSocketsAccess(bool upgradeLock, Action action)
        {
            if (upgradeLock)
            {
                mBindedSocketsLock.EnterUpgradeableReadLock();
            }
            else
            {
                mBindedSocketsLock.EnterReadLock();
            }
            try
            {
                action();
            }
            finally
            {
                if (upgradeLock)
                {
                    mBindedSocketsLock.ExitUpgradeableReadLock();
                }
                else
                {
                    mBindedSocketsLock.ExitReadLock();
                }
            }
        }

        #endregion

    }

}
