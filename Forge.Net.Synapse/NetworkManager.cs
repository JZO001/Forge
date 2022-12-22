/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Forge.Invoker;
using Forge.Legacy;
using Forge.Net.Synapse.NetworkFactory;
using Forge.Net.Synapse.NetworkServices;
using Forge.Net.Synapse.NetworkServices.Defaults;
using Forge.Net.Synapse.Options;
using Forge.Shared;
using Forge.Threading.Tasking;
using Forge.Threading;
using Forge.Logging.Abstraction;

namespace Forge.Net.Synapse
{

#if NET40

    internal delegate NetworkStream NetworkManagerConnectDelegate(AddressEndPoint endPoint, int bufferSize, IClientStreamFactory clientStreamFactory);

#endif

    /// <summary>
    /// Network manager
    /// </summary>
    public sealed class NetworkManager : MBRBase, IDefaultNetworkManager, IDisposable
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<NetworkManager>();

        private static IServerStreamFactory mDefaultServerStreamFactory = new DefaultServerStreamFactory();

        private static IClientStreamFactory mDefaultClientStreamFactory = new DefaultClientStreamFactory();

        private static readonly int mDefaultSocketReceiveBufferSize = 8192;

        private static readonly int mDefaultSocketSendBufferSize = 8192;

        private static readonly int mDefaultSocketKeepAliveTime = 60000;

        private static readonly int mDefaultSocketKeepAliveTimeInterval = 1000;

        private static bool mDefaultUseSocketKeepAlive = true;

        private static long mServerGlobalId = 0;

        private INetworkFactoryBase mNetworkFactory = null;

        private IServerStreamFactory mServerStreamFactory = DefaultServerStreamFactory;

        private IClientStreamFactory mClientStreamFactory = DefaultClientStreamFactory;

        private int mSocketKeepAliveTime = mDefaultSocketKeepAliveTime;

        private int mSocketKeepAliveTimeInterval = mDefaultSocketKeepAliveTimeInterval;

        private bool mUseSocketKeepAlive = mDefaultUseSocketKeepAlive;

        private int mSocketReceiveBufferSize = DefaultSocketReceiveBufferSize;

        private int mSocketSendBufferSize = DefaultSocketSendBufferSize;

        private readonly Dictionary<long, ServerContainer> mServers = new Dictionary<long, ServerContainer>();

        private readonly ReaderWriterLockSlim mLockForServers = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private int mAsyncActiveConnectCount = 0;
        private AutoResetEvent mAsyncActiveConnectEvent = null;
#if NET40
        private NetworkManagerConnectDelegate mConnectDelegate = null;
#endif
        private System.Func<AddressEndPoint, int, IClientStreamFactory, NetworkStream> mConnectFuncDelegate = null;

        private readonly object LOCK_CONNECT = new object();

        private bool mDisposed = false;

        /// <summary>
        /// Occurs when [network peer connected].
        /// </summary>
        public event EventHandler<ConnectionEventArgs> NetworkPeerConnected;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkManager"/> class.
        /// </summary>
        public NetworkManager()
            : this(new DefaultNetworkFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkManager"/> class.
        /// </summary>
        /// <param name="networkFactory">The network factory.</param>
        public NetworkManager(INetworkFactoryBase networkFactory)
        {
            if (networkFactory == null)
            {
                ThrowHelper.ThrowArgumentNullException("networkFactory");
            }
            mNetworkFactory = networkFactory;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the default server stream factory.
        /// </summary>
        /// <value>
        /// The default server stream factory.
        /// </value>
        public static IServerStreamFactory DefaultServerStreamFactory
        {
            get { return mDefaultServerStreamFactory; }
            internal set 
            {
                if (value == null) ThrowHelper.ThrowArgumentNullException("value");
                mDefaultServerStreamFactory = value; 
            }
        }

        /// <summary>
        /// Gets the default client stream factory.
        /// </summary>
        /// <value>
        /// The default client stream factory.
        /// </value>
        public static IClientStreamFactory DefaultClientStreamFactory
        {
            get { return mDefaultClientStreamFactory; }
            internal set
            {
                if (value == null) ThrowHelper.ThrowArgumentNullException("value");
                mDefaultClientStreamFactory = value;
            }
        }

        /// <summary>
        /// Gets the default size of the socket buffer.
        /// </summary>
        /// <value>
        /// The default size of the socket buffer.
        /// </value>
        [DebuggerHidden]
        public static int DefaultSocketReceiveBufferSize
        {
            get { return mDefaultSocketReceiveBufferSize; }
        }

        /// <summary>
        /// Gets the default size of the socket send buffer.
        /// </summary>
        /// <value>
        /// The default size of the socket send buffer.
        /// </value>
        [DebuggerHidden]
        public static int DefaultSocketSendBufferSize
        {
            get { return mDefaultSocketSendBufferSize; }
        }

        /// <summary>
        /// Gets or sets the default socket keep alive time.
        /// </summary>
        /// <value>
        /// The default socket keep alive time.
        /// </value>
        [DebuggerHidden]
        public static int DefaultSocketKeepAliveTime
        {
            get { return mDefaultSocketKeepAliveTime; }
        }

        /// <summary>
        /// Gets or sets the default socket keep alive time interval.
        /// </summary>
        /// <value>
        /// The default socket keep alive time interval.
        /// </value>
        [DebuggerHidden]
        public static int DefaultSocketKeepAliveTimeInterval
        {
            get { return mDefaultSocketKeepAliveTimeInterval; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [default use socket keep alive].
        /// </summary>
        /// <value>
        /// <c>true</c> if [default use socket keep alive]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public static bool DefaultUseSocketKeepAlive
        {
            get { return mDefaultUseSocketKeepAlive; }
            set { mDefaultUseSocketKeepAlive = value; }
        }

        /// <summary>
        /// Gets the network factory.
        /// </summary>
        /// <value>
        /// The network factory.
        /// </value>
        [DebuggerHidden]
        public INetworkFactoryBase NetworkFactory
        {
            get { return mNetworkFactory; }
        }

        /// <summary>
        /// Gets or sets the server stream factory.
        /// </summary>
        /// <value>
        /// The server stream factory.
        /// </value>
        [DebuggerHidden]
        public IServerStreamFactory ServerStreamFactory
        {
            get { return mServerStreamFactory; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                mServerStreamFactory = value;
            }
        }

        /// <summary>
        /// Gets or sets the client stream factory.
        /// </summary>
        /// <value>
        /// The client stream factory.
        /// </value>
        [DebuggerHidden]
        public IClientStreamFactory ClientStreamFactory
        {
            get { return mClientStreamFactory; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                mClientStreamFactory = value;
            }
        }

        /// <summary>
        /// Gets or sets the socket keep alive time.
        /// </summary>
        /// <value>
        /// The socket keep alive time.
        /// </value>
        [DebuggerHidden]
        public int SocketKeepAliveTime
        {
            get { return mSocketKeepAliveTime; }
            set
            {
                if (value < 1000)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mSocketKeepAliveTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the socket keep alive time interval.
        /// </summary>
        /// <value>
        /// The socket keep alive time interval.
        /// </value>
        [DebuggerHidden]
        public int SocketKeepAliveTimeInterval
        {
            get { return mSocketKeepAliveTimeInterval; }
            set
            {
                if (value < 1000)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mSocketKeepAliveTimeInterval = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use socket keep alive].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use socket keep alive]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool UseSocketKeepAlive
        {
            get { return mUseSocketKeepAlive; }
            set { mUseSocketKeepAlive = value; }
        }

        /// <summary>
        /// Gets or sets the default size of the buffer.
        /// </summary>
        /// <value>
        /// The default size of the buffer.
        /// </value>
        [DebuggerHidden]
        public int SocketReceiveBufferSize
        {
            get { return mSocketReceiveBufferSize; }
            set
            {
                if (value < 1024)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mSocketReceiveBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the socket send buffer.
        /// </summary>
        /// <value>
        /// The size of the socket send buffer.
        /// </value>
        [DebuggerHidden]
        public int SocketSendBufferSize
        {
            get { return mSocketSendBufferSize; }
            set
            {
                if (value < 1024)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mSocketSendBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [no delay].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no delay]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool NoDelay
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsDisposed
        {
            get { return mDisposed; }
        }

        #endregion

        #region Public method(s)

#if IS_WINDOWS

        /// <summary>
        /// Sets the keep alive values.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <param name="keepAliveTime">The keep alive time. Specifies the connection idle time in milliseconds before TCP will begin sending keepalives, if keepalives are enabled on a connection.</param>
        /// <param name="keepAliveInterval">The keep alive interval. Specifies the time in milliseconds between retransmissions of keepalives, once the KeepAliveTime has expired. Once KeepAliveTime has expired, keepalives are sent every KeepAliveInterval milliseconds until a response is received, up to a maximum of MaxDataRetries before the connection is terminated.</param>
        /// <returns>value</returns>
        public static int SetKeepAliveValues(Socket socket, bool state, int keepAliveTime, int keepAliveInterval)
        {
            if (socket == null)
            {
                ThrowHelper.ThrowArgumentNullException("socket");
            }
            if (keepAliveTime < 1000)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("keepAliveTime");
            }
            if (keepAliveInterval < 1000)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("keepAliveInterval");
            }

            TcpKeepAlive keepAlive = new TcpKeepAlive();
            keepAlive.State = Convert.ToUInt32(state);
            keepAlive.KeepAliveTime = Convert.ToUInt32(keepAliveTime);
            keepAlive.KeepAliveInterval = Convert.ToUInt32(keepAliveInterval);

            return socket.IOControl(IOControlCode.KeepAliveValues, keepAlive.ToArray(), null);
        }

#endif

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="localPort">The local port.</param>
        /// <returns>Identifier of the listener</returns>
        public long StartServer(int localPort)
        {
            DoDisposeCheck();
            return StartServer(new AddressEndPoint(AddressEndPoint.Any.ToString(), localPort), int.MaxValue, mServerStreamFactory);
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>Identifier of the listener</returns>
        public long StartServer(AddressEndPoint endPoint)
        {
            DoDisposeCheck();
            return StartServer(endPoint, int.MaxValue, mServerStreamFactory);
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="serverStreamFactory">The server stream factory.</param>
        /// <returns>Identifier of the listener</returns>
        public long StartServer(AddressEndPoint endPoint, IServerStreamFactory serverStreamFactory)
        {
            DoDisposeCheck();
            return StartServer(endPoint, int.MaxValue, serverStreamFactory);
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="backlog">The maximum number of pending connections allowed in the waiting queue.</param>
        /// <param name="serverStreamFactory">The server stream factory.</param>
        /// <returns>Identifier of the listener</returns>
        public long StartServer(AddressEndPoint endPoint, int backlog, IServerStreamFactory serverStreamFactory)
        {
            DoDisposeCheck();

            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }

            if (backlog < 1)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("backlog");
            }

            if (serverStreamFactory == null)
            {
                ThrowHelper.ThrowArgumentNullException("serverStreamFactory");
            }

            ITcpListener listener = mNetworkFactory.CreateTcpListener(endPoint);
            if (int.MaxValue == backlog)
            {
                listener.Start(); // it can throw exception
            }
            else
            {
                listener.Start(backlog); // it can throw exception
            }

            ServerContainer container = new ServerContainer(Interlocked.Increment(ref mServerGlobalId), listener, this, serverStreamFactory);

            mLockForServers.EnterWriteLock();
            try
            {
                mServers.Add(container.ServerId, container);
                container.Initialize();
            }
            catch (Exception ex)
            {
                if (LOGGER.IsFatalEnabled) LOGGER.Fatal(ex.Message, ex);
                listener.Stop();
                throw;
            }
            finally
            {
                mLockForServers.ExitWriteLock();
            }

            return container.ServerId;
        }

        /// <summary>Starts a set of TCP servers from options</summary>
        /// <param name="serverOptions">The server options.</param>
        /// <returns>
        ///   Dictionary with the original ServerOption and server id
        /// </returns>
        public Dictionary<ServerOptions, long> StartTCPServers(IEnumerable<ServerOptions> serverOptions)
        {
            Dictionary<ServerOptions, long> result = new Dictionary<ServerOptions, long>();
            if (serverOptions != null && serverOptions.Any())
            {
                foreach (ServerOptions option in serverOptions)
                {
                    result.Add(option, StartServer(
                        new AddressEndPoint(string.IsNullOrWhiteSpace(option.HostIP) ? AddressEndPoint.Any.ToString() : option.HostIP, option.Port), 
                        option.Backlog == null || option.Backlog <= 0 ? int.MaxValue : option.Backlog.Value, 
                        mServerStreamFactory));
                }
            }
            return result;
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <param name="serverId">The server id.</param>
        /// <returns>True, if the server stopped, otherwise False.</returns>
        public bool StopServer(long serverId)
        {
            DoDisposeCheck();

            bool result = false;

            mLockForServers.EnterUpgradeableReadLock();
            try
            {
                if (mServers.ContainsKey(serverId))
                {
                    mLockForServers.EnterWriteLock();
                    try
                    {
                        if (mServers.ContainsKey(serverId))
                        {
                            result = true;
                            ServerContainer sc = mServers[serverId];
                            sc.Listener.Stop();
                            mServers.Remove(serverId);
                        }
                    }
                    finally
                    {
                        mLockForServers.ExitWriteLock();
                    }
                }
            }
            finally
            {
                mLockForServers.ExitUpgradeableReadLock();
            }

            return result;
        }

        /// <summary>
        /// Stops the servers.
        /// </summary>
        public void StopServers()
        {
            DoDisposeCheck();
            if (mServers.Count > 0)
            {
                mLockForServers.EnterWriteLock();
                try
                {
                    foreach (KeyValuePair<long, ServerContainer> kv in mServers)
                    {
                        kv.Value.Listener.Stop();
                    }
                    mServers.Clear();
                }
                finally
                {
                    mLockForServers.ExitWriteLock();
                }
            }
        }

#if NET40

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(AddressEndPoint endPoint, AsyncCallback callback, object state)
        {
            return BeginConnect(endPoint, mSocketReceiveBufferSize, mClientStreamFactory, callback, state);
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
            return BeginConnect(endPoint, mSocketReceiveBufferSize, clientStreamFactory, callback, state);
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(AddressEndPoint endPoint, int bufferSize, AsyncCallback callback, object state)
        {
            return BeginConnect(endPoint, bufferSize, mClientStreamFactory, callback, state);
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(AddressEndPoint endPoint, int bufferSize, IClientStreamFactory clientStreamFactory, AsyncCallback callback, object state)
        {
            DoDisposeCheck();
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            if (bufferSize < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("bufferSize");
            }
            if (clientStreamFactory == null)
            {
                ThrowHelper.ThrowArgumentNullException("clientStreamFactory");
            }

            Interlocked.Increment(ref mAsyncActiveConnectCount);
            NetworkManagerConnectDelegate d = new NetworkManagerConnectDelegate(Connect);
            if (mAsyncActiveConnectEvent == null)
            {
                lock (LOCK_CONNECT)
                {
                    if (mAsyncActiveConnectEvent == null)
                    {
                        mAsyncActiveConnectEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveConnectEvent.WaitOne();
            mConnectDelegate = d;
            return d.BeginInvoke(endPoint, bufferSize, clientStreamFactory, callback, state);
        }

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>
        /// Network Stream instance
        /// </returns>
        public NetworkStream EndConnect(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mConnectDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndConnect called multiple times.", "asyncResult");
            }
            try
            {
                return mConnectDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mConnectDelegate = null;
                mAsyncActiveConnectEvent.Set();
                CloseAsyncActiveConnectEvent(Interlocked.Decrement(ref mAsyncActiveConnectCount));
            }
        }

#endif

        /// <summary>Begins the connect.</summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginConnect(AddressEndPoint endPoint, ReturnCallback callback, object state)
        {
            return BeginConnect(endPoint, mSocketReceiveBufferSize, mClientStreamFactory, callback, state);
        }

        /// <summary>Begins the connect.</summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginConnect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory, ReturnCallback callback, object state)
        {
            return BeginConnect(endPoint, mSocketReceiveBufferSize, clientStreamFactory, callback, state);
        }

        /// <summary>Begins the connect.</summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginConnect(AddressEndPoint endPoint, int bufferSize, ReturnCallback callback, object state)
        {
            return BeginConnect(endPoint, bufferSize, mClientStreamFactory, callback, state);
        }

        /// <summary>Begins the connect.</summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginConnect(AddressEndPoint endPoint, int bufferSize, IClientStreamFactory clientStreamFactory, ReturnCallback callback, object state)
        {
            DoDisposeCheck();
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            if (bufferSize < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("bufferSize");
            }
            if (clientStreamFactory == null)
            {
                ThrowHelper.ThrowArgumentNullException("clientStreamFactory");
            }

            Interlocked.Increment(ref mAsyncActiveConnectCount);
            System.Func<AddressEndPoint, int, IClientStreamFactory, NetworkStream> d = new System.Func<AddressEndPoint, int, IClientStreamFactory, NetworkStream>(Connect);
            if (mAsyncActiveConnectEvent == null)
            {
                lock (LOCK_CONNECT)
                {
                    if (mAsyncActiveConnectEvent == null)
                    {
                        mAsyncActiveConnectEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveConnectEvent.WaitOne();
            mConnectFuncDelegate = d;
            return d.BeginInvoke(endPoint, bufferSize, clientStreamFactory, callback, state);
        }

        /// <summary>Ends the connect.</summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Network Stream instance</returns>
        public NetworkStream EndConnect(ITaskResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mConnectFuncDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndConnect called multiple times.", "asyncResult");
            }
            try
            {
                return mConnectFuncDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mConnectFuncDelegate = null;
                mAsyncActiveConnectEvent.Set();
                CloseAsyncActiveConnectEvent(Interlocked.Decrement(ref mAsyncActiveConnectCount));
            }
        }

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>Network Stream instance</returns>
        public async Task<NetworkStream> ConnectAsync(AddressEndPoint endPoint)
        {
            DoDisposeCheck();
            return await ConnectAsync(endPoint, mSocketReceiveBufferSize, mClientStreamFactory);
        }

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <returns>Network Stream instance</returns>
        public async Task<NetworkStream> ConnectAsync(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory)
        {
            DoDisposeCheck();
            return await ConnectAsync(endPoint, mSocketReceiveBufferSize, clientStreamFactory);
        }

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns>Network Stream instance</returns>
        public async Task<NetworkStream> ConnectAsync(AddressEndPoint endPoint, int bufferSize)
        {
            DoDisposeCheck();
            return await ConnectAsync(endPoint, bufferSize, mClientStreamFactory);
        }

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <returns>
        /// Network Stream instance
        /// </returns>
        public async Task<NetworkStream> ConnectAsync(AddressEndPoint endPoint, int bufferSize, IClientStreamFactory clientStreamFactory)
        {
            DoDisposeCheck();
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            if (bufferSize < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("bufferSize");
            }
            if (clientStreamFactory == null)
            {
                ThrowHelper.ThrowArgumentNullException("clientStreamFactory");
            }

            ITcpClient client = mNetworkFactory.CreateTcpClient(); // add async methods to the interface and implement them
            await client.ConnectAsync(endPoint); // it can throw exception
            client.Client.SendBufferSize = bufferSize;
            client.Client.ReceiveBufferSize = bufferSize;
            client.Client.SendTimeout = Timeout.Infinite;
            client.Client.ReceiveTimeout = Timeout.Infinite;
#if IS_WINDOWS
            client.Client.SetKeepAliveValues(true, DefaultSocketKeepAliveTime, DefaultSocketKeepAliveTimeInterval);
#endif
            client.Client.NoDelay = NoDelay;

            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("SYNAPSE_NETWORK_MANAGER, create client network stream for connection. Factory type: '{0}'. Connection remote endpoint: '{1}'", clientStreamFactory.GetType().FullName, endPoint.ToString()));
            return await clientStreamFactory.CreateNetworkStreamAsync(client);
        }

#endif

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>Network Stream instance</returns>
        public NetworkStream Connect(AddressEndPoint endPoint)
        {
            DoDisposeCheck();
            return Connect(endPoint, mSocketReceiveBufferSize, mClientStreamFactory);
        }

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <returns>Network Stream instance</returns>
        public NetworkStream Connect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory)
        {
            DoDisposeCheck();
            return Connect(endPoint, mSocketReceiveBufferSize, clientStreamFactory);
        }

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns>Network Stream instance</returns>
        public NetworkStream Connect(AddressEndPoint endPoint, int bufferSize)
        {
            DoDisposeCheck();
            return Connect(endPoint, bufferSize, mClientStreamFactory);
        }

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <returns>
        /// Network Stream instance
        /// </returns>
        public NetworkStream Connect(AddressEndPoint endPoint, int bufferSize, IClientStreamFactory clientStreamFactory)
        {
            DoDisposeCheck();
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            if (bufferSize < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("bufferSize");
            }
            if (clientStreamFactory == null)
            {
                ThrowHelper.ThrowArgumentNullException("clientStreamFactory");
            }

            ITcpClient client = mNetworkFactory.CreateTcpClient(); // add async methods to the interface and implement them
            client.Connect(endPoint); // it can throw exception
            client.Client.SendBufferSize = bufferSize;
            client.Client.ReceiveBufferSize = bufferSize;
            client.Client.SendTimeout = Timeout.Infinite;
            client.Client.ReceiveTimeout = Timeout.Infinite;
#if IS_WINDOWS
            client.Client.SetKeepAliveValues(true, DefaultSocketKeepAliveTime, DefaultSocketKeepAliveTimeInterval);
#endif
            client.Client.NoDelay = NoDelay;

            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("SYNAPSE_NETWORK_MANAGER, create client network stream for connection. Factory type: '{0}'. Connection remote endpoint: '{1}'", clientStreamFactory.GetType().FullName, endPoint.ToString()));
            return clientStreamFactory.CreateNetworkStream(client);
        }

        /// <summary>
        /// Gets the server end point.
        /// </summary>
        /// <param name="serverId">The server id.</param>
        /// <returns>AddressEndPoint</returns>
        public AddressEndPoint GetServerEndPoint(long serverId)
        {
            AddressEndPoint result = null;

            mLockForServers.EnterReadLock();
            try
            {
                if (mServers.ContainsKey(serverId))
                {
                    result = mServers[serverId].Listener.LocalEndpoint;
                }
            }
            finally
            {
                mLockForServers.ExitReadLock();
            }

            return result;
        }

        /// <summary>
        /// Determines whether [is server end point exist] [the specified end point].
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>
        ///   <c>true</c> if [is server end point exist] [the specified end point]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsServerEndPointExist(AddressEndPoint endPoint)
        {
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }

            bool result = false;

            mLockForServers.EnterReadLock();
            try
            {
                foreach (KeyValuePair<long, ServerContainer> kv in mServers)
                {
                    if (kv.Value.Listener.LocalEndpoint.Equals(endPoint))
                    {
                        result = true;
                        break;
                    }
                }
            }
            finally
            {
                mLockForServers.ExitReadLock();
            }

            return result;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mLockForServers"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mAsyncActiveConnectEvent"), MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            if (!mDisposed)
            {
                StopServers();
                mLockForServers.Dispose();
                mDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Private method(s)

        internal void OnNetworkPeerConnected(ConnectionEventArgs e)
        {
            Executor.Invoke(NetworkPeerConnected, this, e);
        }

        private void DoDisposeCheck()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private void CloseAsyncActiveConnectEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveConnectEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveConnectEvent.Dispose();
                mAsyncActiveConnectEvent = null;
            }
        }

        #endregion

        #region Nested classes

        private class ServerContainer : MBRBase
        {

            #region Field(s)

            private readonly long mServerId = -1;

            private readonly ITcpListener mListener = null;

            private readonly NetworkManager mManager = null;

            private readonly IServerStreamFactory mServerStreamFactory = null;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="ServerContainer"/> class.
            /// </summary>
            /// <param name="serverId">The server id.</param>
            /// <param name="listener">The listener.</param>
            /// <param name="manager">The manager.</param>
            /// <param name="serverStreamFactory">The stream factory.</param>
            internal ServerContainer(long serverId, ITcpListener listener, NetworkManager manager, IServerStreamFactory serverStreamFactory)
            {
                mServerId = serverId;
                mListener = listener;
                mManager = manager;
                mServerStreamFactory = serverStreamFactory;
            }

            #endregion

            #region Public Properties

            internal long ServerId
            {
                get { return mServerId; }
            }

            internal ITcpListener Listener
            {
                get { return mListener; }
            }

            #endregion

            #region Public method(s)

            /// <summary>
            /// Initializes this instance.
            /// </summary>
            internal void Initialize()
            {
                BeginAccept();
            }

            #endregion

            #region Private method(s)

            private void DoAcceptSocketCallback(ITaskResult ar)
            {
                ITcpListener listener = (ITcpListener)ar.AsyncState;
                ITcpClient tcpClient = null;
                try
                {
                    tcpClient = listener.EndAcceptTcpClient(ar);

#if IS_WINDOWS

                    if (mManager.UseSocketKeepAlive)
                    {
                        tcpClient.Client.SetKeepAliveValues(true, mManager.SocketKeepAliveTime, mManager.SocketKeepAliveTimeInterval);
                    }

#endif

                    tcpClient.Client.SendBufferSize = mManager.SocketSendBufferSize;
                    tcpClient.Client.ReceiveBufferSize = mManager.SocketReceiveBufferSize;
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("New TcpClient connection accepted. Local endpoint: {0}, remote endpoint: {1}", tcpClient.Client.LocalEndPoint.ToString(), tcpClient.Client.RemoteEndPoint.ToString()));
                }
                catch (Exception ex)
                {
                    // this happened while server shut down
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("Failed to configure accepted TcpClient. Reason: {0}", ex.Message));
                    return;
                }

                NetworkStream networkStream = null;
                try
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("Create network stream around the socket connection. Server stream factory type is '{0}'.", mServerStreamFactory.GetType().FullName));
                    networkStream = mServerStreamFactory.CreateNetworkStream(tcpClient);
                }
                catch (Exception ex)
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("ServerStreamFactory implementation threw an exception ({0}).", mServerStreamFactory.GetType().AssemblyQualifiedName), ex);
                }

                if (networkStream == null)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("Failed to create network stream for a connection. Server stream factory '{0}' does not provide NetworkStream.", mServerStreamFactory.GetType().FullName));
                    tcpClient.Close();
                    BeginAccept();
                }
                else
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("Propagate event about the new connection. Listening endpoint: {0}, local endpoint: {1}, remote endpoint: {2}. Network stream id: {3}", listener.LocalEndpoint.ToString(), tcpClient.Client.LocalEndPoint.ToString(), tcpClient.Client.RemoteEndPoint.ToString(), networkStream.Id.ToString()));
                    BeginAccept();
                    mManager.OnNetworkPeerConnected(new ConnectionEventArgs(mServerId, listener.LocalEndpoint, networkStream));
                }
            }

            private void BeginAccept()
            {
                try
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("Begin accept TcpClient on a TcpListener. Local endpoint: {0}", mListener.LocalEndpoint.ToString()));
                    mListener.BeginAcceptTcpClient(new ReturnCallback(DoAcceptSocketCallback), mListener);
                }
                catch (Exception ex)
                {
                    // this happens, while I shut down the listener
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("Failed to begin accept TcpClient on a TcpListener. Reason: {0}", ex.Message));
                }
            }

            #endregion

        }

        #endregion

    }

}
