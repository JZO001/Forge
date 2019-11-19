/* *********************************************************************
 * Date: 14 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Forge.Collections;
using Forge.Configuration.Shared;
using Forge.IO;
using Forge.Net.Remoting.Messaging;
using Forge.Net.Remoting.Sinks;
using Forge.Net.Synapse;
using Forge.Net.Synapse.NetworkFactory;
using Forge.Net.Synapse.NetworkServices;
using Forge.Reflection;
using Forge.Threading;
using log4net;

namespace Forge.Net.Remoting.Channels
{

    internal delegate void SendTaskSendDelegate();

    /// <summary>
    /// TCP Channel implementation
    /// </summary>
    public sealed class TCPChannel : Channel
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TCPChannel));

        private static readonly Protocol mNetworkProtocol = new Protocol();

        private NetworkManager mNetworkManager = null;

        private readonly Dictionary<long, SessionMap> mConnectionId_and_SessionMap = new Dictionary<long, SessionMap>();

        private readonly Dictionary<String, SessionMap> mSessionId_and_SessionMap = new Dictionary<String, SessionMap>();

        private DeadlockSafeLock mSessionMapLock = null;

        private bool mListening = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private String mTempStreamStorageFolder = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mMaxSendMessageSize = 32768; // 32KiB

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mMaxSendStreamSize = 268435456; // 256 MiB

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mMaxReceiveMessageSize = 32768; // 32KiB

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mMaxReceiveStreamSize = 268435456; // 256 MiB

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<AddressEndPoint> mServerActiveEndpoints = new List<AddressEndPoint>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPChannel"/> class.
        /// </summary>
        public TCPChannel()
            : base()
        {
            this.mStreamsSupported = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPChannel"/> class.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="sendMessageSinks">The send message sinks.</param>
        /// <param name="receiveMessageSinks">The receive message sinks.</param>
        public TCPChannel(String channelId, ICollection<IMessageSink> sendMessageSinks, ICollection<IMessageSink> receiveMessageSinks)
            : this(channelId, sendMessageSinks, receiveMessageSinks, new List<AddressEndPoint>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPChannel"/> class.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="sendMessageSinks">The send message sinks.</param>
        /// <param name="receiveMessageSinks">The receive message sinks.</param>
        /// <param name="serverData">The server data.</param>
        public TCPChannel(String channelId, ICollection<IMessageSink> sendMessageSinks, ICollection<IMessageSink> receiveMessageSinks,
            IEnumerable<AddressEndPoint> serverData)
            : this(channelId, sendMessageSinks, receiveMessageSinks, new List<AddressEndPoint>(), new DefaultNetworkFactory(),
            NetworkManager.DefaultServerStreamFactory, NetworkManager.DefaultClientStreamFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPChannel"/> class.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="sendMessageSinks">The send message sinks.</param>
        /// <param name="receiveMessageSinks">The receive message sinks.</param>
        /// <param name="networkFactory">The network factory.</param>
        /// <param name="serverStreamFactory">The server stream factory.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        public TCPChannel(String channelId, ICollection<IMessageSink> sendMessageSinks, ICollection<IMessageSink> receiveMessageSinks,
            INetworkFactory networkFactory, IServerStreamFactory serverStreamFactory, IClientStreamFactory clientStreamFactory)
            : this(channelId, sendMessageSinks, receiveMessageSinks, new List<AddressEndPoint>(), networkFactory, serverStreamFactory, clientStreamFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPChannel"/> class.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="sendMessageSinks">The send message sinks.</param>
        /// <param name="receiveMessageSinks">The receive message sinks.</param>
        /// <param name="serverData">The server data.</param>
        /// <param name="networkFactory">The network factory.</param>
        public TCPChannel(String channelId, ICollection<IMessageSink> sendMessageSinks, ICollection<IMessageSink> receiveMessageSinks,
            IEnumerable<AddressEndPoint> serverData, INetworkFactory networkFactory)
            : this(channelId, sendMessageSinks, receiveMessageSinks, serverData, networkFactory,
            NetworkManager.DefaultServerStreamFactory, NetworkManager.DefaultClientStreamFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPChannel"/> class.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="sendMessageSinks">The send message sinks.</param>
        /// <param name="receiveMessageSinks">The receive message sinks.</param>
        /// <param name="serverData">The server data.</param>
        /// <param name="networkFactory">The network factory.</param>
        /// <param name="serverStreamFactory">The server stream factory.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        public TCPChannel(String channelId, ICollection<IMessageSink> sendMessageSinks, ICollection<IMessageSink> receiveMessageSinks,
            IEnumerable<AddressEndPoint> serverData,
            INetworkFactory networkFactory, IServerStreamFactory serverStreamFactory, IClientStreamFactory clientStreamFactory)
            : base(channelId, sendMessageSinks, receiveMessageSinks)
        {
            if (serverData == null)
            {
                ThrowHelper.ThrowArgumentNullException("serverData");
            }
            if (networkFactory == null)
            {
                ThrowHelper.ThrowArgumentNullException("networkFactory");
            }
            if (serverStreamFactory == null)
            {
                ThrowHelper.ThrowArgumentNullException("serverStreamFactory");
            }
            if (clientStreamFactory == null)
            {
                ThrowHelper.ThrowArgumentNullException("clientStreamFactory");
            }

            this.mNetworkManager = new NetworkManager(networkFactory);
            this.mNetworkManager.ServerStreamFactory = serverStreamFactory;
            this.mNetworkManager.ClientStreamFactory = clientStreamFactory;
            this.mServerEndpoints.AddRange(serverData);
            this.mNetworkManager.NetworkPeerConnected += new EventHandler<ConnectionEventArgs>(NetworkManager_NetworkPeerConnected);
            this.mStreamsSupported = true;
            this.mSessionMapLock = new DeadlockSafeLock(String.Format("TCPChannel_SessionMaps_{0}", mChannelId));
            this.mInitialized = true;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the temp stream storage folder.
        /// </summary>
        /// <value>
        /// The temp stream storage folder.
        /// </value>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationException"></exception>
        public String TempStreamStorageFolder
        {
            get { return mTempStreamStorageFolder; }
            set
            {
                if (mTempStreamStorageFolder == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                if (!PathHelper.PerformFolderSecurityTest(value))
                {
                    throw new InvalidConfigurationException(String.Format("Security test failed on the temp stream folder: {0}. Please add read, write and delete rights to this folder.", this.mTempStreamStorageFolder));
                }
                mTempStreamStorageFolder = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the max send message.
        /// </summary>
        /// <value>
        /// The size of the max send message.
        /// </value>
        [DebuggerHidden]
        public int MaxSendMessageSize
        {
            get { return mMaxSendMessageSize; }
            set
            {
                if (value < 1)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mMaxSendMessageSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the max send stream.
        /// </summary>
        /// <value>
        /// The size of the max send stream.
        /// </value>
        [DebuggerHidden]
        public long MaxSendStreamSize
        {
            get { return mMaxSendStreamSize; }
            set
            {
                if (value < 1)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mMaxSendStreamSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the max receive message.
        /// </summary>
        /// <value>
        /// The size of the max receive message.
        /// </value>
        [DebuggerHidden]
        public int MaxReceiveMessageSize
        {
            get { return mMaxReceiveMessageSize; }
            set
            {
                if (value < 1)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mMaxReceiveMessageSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the max receive stream.
        /// </summary>
        /// <value>
        /// The size of the max receive stream.
        /// </value>
        [DebuggerHidden]
        public long MaxReceiveStreamSize
        {
            get { return mMaxReceiveStreamSize; }
            set
            {
                if (value < 1)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mMaxReceiveStreamSize = value;
            }
        }

        /// <summary>
        /// Gets the server endpoints.
        /// </summary>
        /// <value>
        /// The server endpoints.
        /// </value>
        [DebuggerHidden]
        public override ICollection<AddressEndPoint> ServerEndpoints
        {
            get
            {
                return mServerActiveEndpoints.Count > 0 ? new List<AddressEndPoint>(mServerActiveEndpoints) : base.ServerEndpoints;
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the channel.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationException">Channel id has not been definied.
        /// or
        /// or
        /// No send message sink definied.
        /// or
        /// No receive message sink definied.
        /// or
        /// or
        /// or</exception>
        /// <exception cref="System.IO.IOException">Temp stream folder failed on security test.</exception>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationValueException">No server stream factory type definied.
        /// or
        /// No client stream factory type definied.</exception>
        public override void Initialize(CategoryPropertyItem pi)
        {
            if (!mInitialized)
            {
                base.Initialize(pi);
                try
                {
                    if (string.IsNullOrEmpty(pi.Id))
                    {
                        throw new InvalidConfigurationException("Channel id has not been definied.");
                    }
                    this.mChannelId = pi.Id;
                    {
                        CategoryPropertyItem tempStreamFolder = ConfigurationAccessHelper.GetCategoryPropertyByPath(pi.PropertyItems, "TempStreamStorageFolder");
                        if (tempStreamFolder != null && tempStreamFolder.EntryValue != null)
                        {
                            this.mTempStreamStorageFolder = tempStreamFolder.EntryValue;
                        }
                        try
                        {
                            if (!PathHelper.PerformFolderSecurityTest(this.mTempStreamStorageFolder))
                            {
                                throw new IOException("Temp stream folder failed on security test.");
                            }
                        }
                        catch (IOException ex)
                        {
                            throw new InvalidConfigurationException(String.Format("Security test failed on the temp stream folder: {0}. Please add read, write and delete rights to this folder.", this.mTempStreamStorageFolder), ex);
                        }
                    }
                    {
                        this.mMaxSendMessageSize = 32768;
                        int value = 32768;
                        if (ConfigurationAccessHelper.ParseIntValue(pi.PropertyItems, "MaxSendMessageSize", 1, Int32.MaxValue, ref value))
                        {
                            this.mMaxSendMessageSize = value;
                        }
                    }
                    {
                        this.mMaxReceiveMessageSize = 32768;
                        int value = 32768;
                        if (ConfigurationAccessHelper.ParseIntValue(pi.PropertyItems, "MaxReceiveMessageSize", 1, Int32.MaxValue, ref value))
                        {
                            this.mMaxReceiveMessageSize = value;
                        }
                    }
                    {
                        this.mMaxSendStreamSize = 268435456;
                        long value = 268435456;
                        if (ConfigurationAccessHelper.ParseLongValue(pi.PropertyItems, "MaxSendStreamSize", 1, Int64.MaxValue, ref value))
                        {
                            this.mMaxSendStreamSize = value;
                        }
                    }
                    {
                        this.mMaxReceiveStreamSize = 268435456;
                        long value = 268435456;
                        if (ConfigurationAccessHelper.ParseLongValue(pi.PropertyItems, "MaxReceiveStreamSize", 1, Int64.MaxValue, ref value))
                        {
                            this.mMaxReceiveStreamSize = value;
                        }
                    }
                    {
                        this.mSendMessageSinks.Clear();
                        this.mReceiveMessageSinks.Clear();
                        CategoryPropertyItem itemRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(pi.PropertyItems, "SendSinks");
                        if (itemRoot != null)
                        {
                            IEnumerator<CategoryPropertyItem> iterator = itemRoot.GetEnumerator();
                            while (iterator.MoveNext())
                            {
                                this.mSendMessageSinks.Add(CreateMessageSink(iterator.Current));
                            }
                        }
                        itemRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(pi.PropertyItems, "ReceiveSinks");
                        if (itemRoot != null)
                        {
                            IEnumerator<CategoryPropertyItem> iterator = itemRoot.GetEnumerator();
                            while (iterator.MoveNext())
                            {
                                this.mReceiveMessageSinks.Add(CreateMessageSink(iterator.Current));
                            }
                        }
                        if (this.mSendMessageSinks.Count == 0)
                        {
                            throw new InvalidConfigurationException("No send message sink definied.");
                        }
                        if (this.mReceiveMessageSinks.Count == 0)
                        {
                            throw new InvalidConfigurationException("No receive message sink definied.");
                        }
                    }
                    {
                        CategoryPropertyItem itemRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(pi.PropertyItems, "NetworkFactoryType");
                        if (itemRoot != null)
                        {
                            if (string.IsNullOrEmpty(itemRoot.EntryValue))
                            {
                                mNetworkManager = new NetworkManager();
                            }
                            else
                            {
                                try
                                {
                                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("TCPChannel, create network factory instance from type '{0}'. ChannelId: '{1}'.", itemRoot.EntryValue, this.ChannelId));
                                    Type networkFactoryType = TypeHelper.GetTypeFromString(itemRoot.EntryValue);
                                    INetworkFactory factory = (INetworkFactory)networkFactoryType.GetConstructor(new Type[] { }).Invoke(null);
                                    mNetworkManager = new NetworkManager(factory);
                                }
                                catch (Exception ex)
                                {
                                    throw new InvalidConfigurationException(string.Format("Failed to instantiate network factory, type: {0}", itemRoot.EntryValue), ex);
                                }
                            }
                        }
                        else
                        {
                            mNetworkManager = new NetworkManager();
                        }
                    }
                    {
                        CategoryPropertyItem itemRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(pi.PropertyItems, "ServerStreamFactoryType");
                        if (itemRoot != null)
                        {
                            if (string.IsNullOrEmpty(itemRoot.EntryValue))
                            {
                                throw new InvalidConfigurationValueException("No server stream factory type definied.");
                            }
                            IServerStreamFactory factory = null;
                            try
                            {
                                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("TCPChannel, create server stream factory instance from type '{0}'. ChannelId: '{1}'.", itemRoot.EntryValue, this.ChannelId));
                                Type serverStreamFactoryType = TypeHelper.GetTypeFromString(itemRoot.EntryValue);
                                factory = (IServerStreamFactory)serverStreamFactoryType.GetConstructor(new Type[] { }).Invoke(null);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidConfigurationException(string.Format("Failed to instantiate server stream factory, type: {0}", itemRoot.EntryValue), ex);
                            }
                            factory.Initialize(itemRoot);
                            mNetworkManager.ServerStreamFactory = factory;
                        }
                    }
                    {
                        CategoryPropertyItem itemRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(pi.PropertyItems, "ClientStreamFactoryType");
                        if (itemRoot != null)
                        {
                            if (string.IsNullOrEmpty(itemRoot.EntryValue))
                            {
                                throw new InvalidConfigurationValueException("No client stream factory type definied.");
                            }
                            IClientStreamFactory factory = null;
                            try
                            {
                                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("TCPChannel, create client stream factory instance from type '{0}'. ChannelId: '{1}'.", itemRoot.EntryValue, this.ChannelId));
                                Type clientStreamFactoryType = TypeHelper.GetTypeFromString(itemRoot.EntryValue);
                                factory = (IClientStreamFactory)clientStreamFactoryType.GetConstructor(new Type[] { }).Invoke(null);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidConfigurationException(string.Format("Failed to instantiate client stream factory, type: {0}", itemRoot.EntryValue), ex);
                            }
                            factory.Initialize(itemRoot);
                            mNetworkManager.ClientStreamFactory = factory;
                        }
                    }
                    this.mSessionMapLock = new DeadlockSafeLock(String.Format("TCPChannel_SessionMaps_{0}", mChannelId));
                    this.mNetworkManager.NetworkPeerConnected += new EventHandler<ConnectionEventArgs>(NetworkManager_NetworkPeerConnected);
                    this.mInitialized = true;
                }
                catch (InvalidConfigurationException ex)
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error("Failed to initialize TCPChannel.", ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns>SessionId</returns>
        /// <exception cref="System.IO.IOException">Connection information has not been specified.</exception>
        public override string Connect()
        {
            DoDisposeCheck();
            if (mConnectionData == null)
            {
                throw new IOException("Connection information has not been specified.");
            }
            return Connect(mConnectionData);
        }

        /// <summary>
        /// Connects the specified remote ep.
        /// </summary>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>SessionId</returns>
        public override string Connect(AddressEndPoint remoteEp)
        {
            DoDisposeCheck();
            if (remoteEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("remoteEp");
            }

            String result = null;

            this.mSessionMapLock.Lock();
            try
            {
                foreach (SessionMap map in mConnectionId_and_SessionMap.Values)
                {
                    if (map.Reconnectable)
                    {
                        if (map.RemoteEndPoint.Host.Equals(remoteEp.Host) && map.RemoteEndPoint.Port.Equals(remoteEp.Port) && map.Stream.Connected)
                        {
                            result = map.SessionId;
                        }
                        break;
                    }
                }
            }
            finally
            {
                this.mSessionMapLock.Unlock();
            }

            if (result == null)
            {
                NetworkStream networkStream = mNetworkManager.Connect(remoteEp); // csatlakozás
                this.mSessionMapLock.Lock();
                try
                {
                    ConnectionEstablishedEventHandler(networkStream);
                    SessionMap map = mConnectionId_and_SessionMap[networkStream.Id]; // map kinyerése és adminja
                    map.Reconnectable = true; // általam kezdeményezett kapcsolat session-je újraindítható
                    result = map.SessionId;
                }
                finally
                {
                    this.mSessionMapLock.Unlock();
                }
            }

            return result;
        }

        /// <summary>
        /// Disconnects the specified session id.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns>
        /// True, if the connection found and closed, otherwise False.
        /// </returns>
        public override bool Disconnect(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                ThrowHelper.ThrowArgumentNullException("sessionId");
            }

            bool result = false;

            this.mSessionMapLock.Lock();
            try
            {
                foreach (SessionMap map in mConnectionId_and_SessionMap.Values)
                {
                    if (map.SessionId.Equals(sessionId))
                    {
                        map.Reconnectable = false; // a SessionMap eldobásra kerül, a session azonosító megszűnik
                        map.Stream.Dispose();
                        ConnectionEndEventHandler(map.Stream);
                        result = true;
                        break;
                    }
                }
            }
            finally
            {
                this.mSessionMapLock.Unlock();
            }

            return result;
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// Response message or null
        /// </returns>
        /// <exception cref="ConnectionNotFoundException">
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// Unable to connect remote host.
        /// or
        /// Unable to connect remote host.
        /// </exception>
        public override IMessage SendMessage(string sessionId, IMessage message, long timeout)
        {
            DoDisposeCheck();

            if (string.IsNullOrEmpty(sessionId))
            {
                ThrowHelper.ThrowArgumentNullException("sessionId");
            }

            if (message == null)
            {
                ThrowHelper.ThrowArgumentNullException("message");
            }

            SessionMap map = null;
            bool connectIt = false;
            this.mSessionMapLock.Lock();
            try
            {
                if (mSessionId_and_SessionMap.ContainsKey(sessionId))
                {
                    // ismert sessionId
                    map = mSessionId_and_SessionMap[sessionId];
                    if (!mConnectionId_and_SessionMap.ContainsKey(map.Stream.Id))
                    {
                        // a session-hez tartozó network kapcsolat halott
                        if (map.Reconnectable)
                        {
                            connectIt = true; // általam előzőleg kezdeményezett kapcsolat volt, ami újraindítható
                        }
                        else
                        {
                            throw new ConnectionNotFoundException(String.Format("Unable to reopen connection. SessionId: '{0}'.", sessionId));
                        }
                    }
                }
                else
                {
                    throw new ConnectionNotFoundException(String.Format("Session not found. SessionId: '{0}'.", sessionId));
                }

                //LOGGER.info("SessionId: " + sessionId + ", Connect: " + connectIt);

                if (connectIt)
                {
                    // a kapcsolatot fel kell építeni újra
                    try
                    {
                        // újracsatlakozás a sessionId megtartásával
                        NetworkStream networkStream = mNetworkManager.Connect(map.RemoteEndPoint);
                        ConnectionEstablishedEventHandler(networkStream);
                        map = mConnectionId_and_SessionMap[networkStream.Id];

                        //LOGGER.info("SEND, ConId: " + conId);

                        if (map == null || (!map.SessionId.Equals(sessionId) && !string.IsNullOrEmpty(sessionId)))
                        {
                            throw new IOException(String.Format("Unable to reopen connection with the same sessionId. New sessionId: {0}, old sessionId: {1}", map == null ? "" : map.SessionId, sessionId));
                        }
                        if (!map.Stream.Connected)
                        {
                            throw new IOException("Unable to connect remote host.");
                        }
                    }
                    catch (IOException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new IOException("Unable to connect remote host.", ex);
                    }
                }
            }
            finally
            {
                this.mSessionMapLock.Unlock();
            }

            // üzenet küldése
            return map.Send(message, timeout);
        }

        /// <summary>
        /// Starts the listening.
        /// </summary>
        public override void StartListening()
        {
            DoDisposeCheck();
            if (!mListening)
            {
                if (mServerEndpoints.Count > 0)
                {
                    bool foundUndefiniedPort = false;
                    List<AddressEndPoint> tmpList = new List<AddressEndPoint>();
                    foreach (AddressEndPoint cData in mServerEndpoints)
                    {
                        if (cData.Port == 0)
                        {
                            mNetworkManager.StartServer(cData);
                            tmpList.Add(cData);
                        }
                        else
                        {
                            long serverId = mNetworkManager.StartServer(cData);
                            tmpList.Add(mNetworkManager.GetServerEndPoint(serverId));
                            foundUndefiniedPort = true;
                        }
                    }
                    mServerActiveEndpoints.Clear();
                    if (foundUndefiniedPort)
                    {
                        mServerActiveEndpoints.AddRange(tmpList);
                    }
                }
                this.mListening = true;
            }
        }

        /// <summary>
        /// Stops the listening.
        /// </summary>
        public override void StopListening()
        {
            DoDisposeCheck();
            if (this.mNetworkManager != null)
            {
                this.mNetworkManager.StopServers();
            }
            this.mListening = false;
        }

        /// <summary>
        /// Gets the session info.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        public override ISessionInfo GetSessionInfo(string sessionId)
        {
            SessionMap result = null;

            this.mSessionMapLock.Lock();
            try
            {
                mSessionId_and_SessionMap.TryGetValue(sessionId, out result);
            }
            finally
            {
                this.mSessionMapLock.Unlock();
            }

            return result;
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                if (this.mNetworkManager != null)
                {
                    this.mNetworkManager.StopServers();
                    this.mNetworkManager.NetworkPeerConnected -= new EventHandler<ConnectionEventArgs>(NetworkManager_NetworkPeerConnected);

                    if (this.mSessionMapLock != null)
                    {
                        this.mSessionMapLock.Lock();
                        try
                        {
                            foreach (KeyValuePair<string, SessionMap> entry in this.mSessionId_and_SessionMap)
                            {
                                entry.Value.Stream.Dispose();
                            }
                            this.mSessionId_and_SessionMap.Clear();
                        }
                        finally
                        {
                            this.mSessionMapLock.Unlock();
                        }
                    }

                    this.mNetworkManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Private method(s)

        private void NetworkManager_NetworkPeerConnected(object sender, ConnectionEventArgs e)
        {
            ConnectionEstablishedEventHandler(e.NetworkStream);
        }

        private void ConnectionEstablishedEventHandler(NetworkStream stream)
        {
            this.mSessionMapLock.Lock();
            try
            {
                SessionMap sessionMap = null;
                bool isExist = false;
                foreach (KeyValuePair<String, SessionMap> entry in this.mSessionId_and_SessionMap)
                {
                    SessionMap innerSessionMap = entry.Value;
                    if (innerSessionMap.Reconnectable && !innerSessionMap.Stream.Connected
                        && innerSessionMap.RemoteEndPoint.Equals(stream.RemoteEndPoint) && innerSessionMap.LocalEndPoint.Host.Equals(stream.LocalEndPoint.Host))
                    {
                        // revive session
                        // csak akkor éleszthető újra a session, ha eredetileg Én kezdeményeztem a kapcsolódást (azaz a kliens vagyok)
                        isExist = true;
                        innerSessionMap.Stream = stream;
                        sessionMap = innerSessionMap;
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(String.Format("TCPChannel, Session revived: {0}, StreamId: {1}", innerSessionMap.SessionId, stream.Id));
                        break;
                    }
                }
                if (!isExist)
                {
                    sessionMap = new SessionMap(this, stream.RemoteEndPoint, stream.LocalEndPoint, stream);
                    this.mSessionId_and_SessionMap.Add(sessionMap.SessionId, sessionMap);
                }

                if (stream.Connected)
                {
                    this.mConnectionId_and_SessionMap.Add(stream.Id, sessionMap);
                    sessionMap.StartReceive();
                    sessionMap.StartSend();
                    OnSessionStateChange(new SessionStateEventArgs(sessionMap.SessionId, true));
                }
                else
                {
                    stream.Dispose();
                }
            }
            catch (Exception ex)
            {
                String message = "Unexpected exception occured while processing new connection.";
                if (LOGGER.IsErrorEnabled) LOGGER.Error(message, ex);
            }
            finally
            {
                this.mSessionMapLock.Unlock();
            }
        }

        private void ConnectionEndEventHandler(NetworkStream stream)
        {
            String sessionId = null;

            if (LOGGER.IsDebugEnabled) LOGGER.Debug(String.Format("TCPChannel, an underlying connection lost. Id: {0}", stream.Id));

            this.mSessionMapLock.Lock();
            try
            {
                long id = stream.Id;
                if (this.mConnectionId_and_SessionMap.ContainsKey(id))
                {
                    SessionMap map = this.mConnectionId_and_SessionMap[id];
                    sessionId = map.SessionId;
                    this.mConnectionId_and_SessionMap.Remove(id);
                    if (!map.Reconnectable)
                    {
                        // ha nem újracsatlakoztatható, a session-t eldobjuk azonnal
                        this.mSessionId_and_SessionMap.Remove(sessionId);
                    }
                }
            }
            finally
            {
                this.mSessionMapLock.Unlock();
            }

            if (sessionId != null)
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(String.Format("TCPChannel, a session has been deactivated. SessionId: {0}", sessionId));
                OnSessionStateChange(new SessionStateEventArgs(sessionId, false));
            }
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// Connection session handler
        /// </summary>
        private sealed class SessionMap : ISessionInfo
        {

            #region Field(s)

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly String mSessionId = Guid.NewGuid().ToString();

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private TCPChannel mChannel = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly AddressEndPoint mRemoteEndPoint = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly AddressEndPoint mLocalEndPoint = null;

            private NetworkStream mStream = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool mReconnectable = false;

            private readonly Dictionary<String, SendTask> mWaitsForAcknowledge = new Dictionary<String, SendTask>();

            private readonly Dictionary<String, SendTask> mWaitsForResponse = new Dictionary<String, SendTask>();

            private Thread mReceiveThread = null;

            private bool mReceiveStopped = true;

            private readonly Queue<SendTask> mSendTasks = new Queue<SendTask>();

            private readonly Semaphore mSendTaskEvent = new Semaphore(0, int.MaxValue);

            private Thread mSendThread = null;

            private bool mSendStopped = true;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="SessionMap"/> class.
            /// </summary>
            /// <param name="channel">The channel.</param>
            /// <param name="remoteEndPoint">The remote end point.</param>
            /// <param name="localEndPoint">The local end point.</param>
            /// <param name="stream">The stream.</param>
            internal SessionMap(TCPChannel channel, AddressEndPoint remoteEndPoint, AddressEndPoint localEndPoint, NetworkStream stream)
            {
                this.mChannel = channel;
                this.mRemoteEndPoint = remoteEndPoint;
                this.mLocalEndPoint = localEndPoint;
                this.mStream = stream;
            }

            #endregion

            #region Public properties

            /// <summary>
            /// Gets the session id.
            /// </summary>
            [DebuggerHidden]
            public String SessionId
            {
                get { return mSessionId; }
            }

            /// <summary>
            /// Gets the remote end point.
            /// </summary>
            [DebuggerHidden]
            public AddressEndPoint RemoteEndPoint
            {
                get { return mRemoteEndPoint; }
            }

            /// <summary>
            /// Gets the local end point.
            /// </summary>
            [DebuggerHidden]
            public AddressEndPoint LocalEndPoint
            {
                get { return mLocalEndPoint; }
            }

            #endregion

            #region Internal properties

            /// <summary>
            /// Gets or sets the stream.
            /// </summary>
            /// <value>
            /// The stream.
            /// </value>
            [DebuggerHidden]
            internal NetworkStream Stream
            {
                get { return mStream; }
                set { mStream = value; }
            }

            /// <summary>
            /// Gets a value indicating whether this <see cref="SessionMap"/> is reconnectable.
            /// </summary>
            /// <value>
            ///   <c>true</c> if reconnectable; otherwise, <c>false</c>.
            /// </value>
            [DebuggerHidden]
            public bool Reconnectable
            {
                get { return mReconnectable; }
                internal set { mReconnectable = value; }
            }

            #endregion

            #region Internal method(s)

            /// <summary>
            /// Begins the receive.
            /// </summary>
            [MethodImpl(MethodImplOptions.Synchronized)]
            internal void StartReceive()
            {
                if (mReceiveStopped)
                {
                    mReceiveStopped = false;
                    mReceiveThread = new Thread(new ThreadStart(ReceiveMain));
                    mReceiveThread.Name = string.Format("SessionMapReceive_{0}", mStream.Id);
                    mReceiveThread.IsBackground = true;
                    mReceiveThread.Start();
                }
            }

            /// <summary>
            /// Starts the send.
            /// </summary>
            internal void StartSend()
            {
                if (mSendStopped)
                {
                    mSendStopped = false;
                    mSendThread = new Thread(new ThreadStart(SendMain));
                    mSendThread.Name = string.Format("SessionMapSend_{0}", mStream.Id);
                    mSendThread.IsBackground = true;
                    mSendThread.Start();
                }
            }

            /// <summary>
            /// Sends the specified message.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="timeout">The timeout.</param>
            /// <returns></returns>
            internal IMessage Send(IMessage message, long timeout)
            {
                return Send(message, timeout, false);
            }

            #endregion

            #region Private method(s)

            private IMessage Send(IMessage message, long timeout, bool isAcknowledgeMessageEnabled)
            {
                if (message.MessageType == MessageTypeEnum.Acknowledge && !isAcknowledgeMessageEnabled)
                {
                    throw new InvalidMessageException(String.Format("Invalid message type: {0}", MessageTypeEnum.Acknowledge.ToString()));
                }

                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("TCPChannel-Send, sending {0}", message.ToString()));

                // decision: isStreamed ?
                List<Stream> streamTasks = new List<Stream>();
                if (message is ResponseMessage)
                {
                    ResponseMessage rm = (ResponseMessage)message;
                    if (rm.ReturnValue.Value != null && rm.ReturnValue.Value is Stream)
                    {
                        Stream s = (Stream)rm.ReturnValue.Value;
                        if (mChannel.MaxSendStreamSize > 0 && s.Length - s.Position > mChannel.MaxSendStreamSize)
                        {
                            throw new MessageSecurityException("Stream size is larger than the maximum allowed.");
                        }
                        streamTasks.Add(s);
                        rm.SetReturnValueToNull();
                    }
                }
                else if (message is RequestMessage)
                {
                    RequestMessage rm = (RequestMessage)message;
                    if (rm.MethodParameters != null)
                    {
                        foreach (MethodParameter mp in rm.MethodParameters)
                        {
                            if (mp.Value != null && mp.Value is Stream)
                            {
                                Stream s = (Stream)mp.Value;
                                if (mChannel.MaxSendStreamSize > 0 && s.Length - s.Position > mChannel.MaxSendStreamSize)
                                {
                                    streamTasks.Clear();
                                    throw new MessageSecurityException("Stream size is larger than the maximum allowed.");
                                }
                                streamTasks.Add(s);
                                mp.SetValueToNull();
                            }
                        }
                    }
                }

                NetworkStream ns = this.mStream;
                if (ns == null)
                {
                    throw new IOException("Connection closed while sending message.");
                }

                SendTask sendTask = new SendTask(message, streamTasks, ns, mChannel, TCPChannel.mNetworkProtocol);

                // adminisztráció
                if (message.MessageType == MessageTypeEnum.Request)
                {
                    lock (mWaitsForResponse)
                    {
                        mWaitsForResponse.Add(message.CorrelationId, sendTask); // elhelyezem egy közös pontra a task-ot, hogy a fogadó szál el tudja érni, ha jön a Response
                    }
                }
                else if (message.MessageType != MessageTypeEnum.DatagramOneway && message.MessageType != MessageTypeEnum.Acknowledge)
                {
                    lock (mWaitsForAcknowledge)
                    {
                        mWaitsForAcknowledge.Add(message.CorrelationId, sendTask); // elhelyezem egy közös pontra a task-ot, hogy a fogadó szál el tudja érni, ha jön az Acknowledge
                    }
                }

                bool timedOut = false;
                if (message.MessageType == MessageTypeEnum.Acknowledge)
                {
                    // ez azt jelenti, hogy ack üzenetnek mindig végtelen a timeout-ja
                    // ez nem biztos, hogy annyira jó, de ezt fentebbi szinteken kell kezelni
                    sendTask.ExecutingSendOnNetwork();
                }
                else if (message.MessageType == MessageTypeEnum.DatagramOneway)
                {
                    // nem blokkol
                    EnqueueSendTask(sendTask);
                }
                else
                {
                    // küldés végrehajtása
                    EnqueueSendTask(sendTask);

                    // várakozás timeout-ra
                    if (timeout < 1)
                    {
                        timedOut = !sendTask.WaitHandleForTimeoutEvent.WaitOne();
                    }
                    else
                    {
                        timedOut = !sendTask.WaitHandleForTimeoutEvent.WaitOne(TimeSpan.FromMilliseconds(timeout));
                    }
                }

                // adminisztráció
                if (message.MessageType == MessageTypeEnum.Request)
                {
                    lock (mWaitsForResponse)
                    {
                        mWaitsForResponse.Remove(message.CorrelationId);
                    }
                }
                else if (message.MessageType != MessageTypeEnum.DatagramOneway && message.MessageType != MessageTypeEnum.Acknowledge)
                {
                    lock (mWaitsForAcknowledge)
                    {
                        mWaitsForAcknowledge.Remove(message.CorrelationId);
                    }
                }

                // válaszüzenet feldolgozása
                IMessage result = null;
                if (message.MessageType != MessageTypeEnum.DatagramOneway)
                {
                    // oneway datagram üzenetnél nincs timeout figyelés, sem válaszüzenet
                    if (timedOut && !sendTask.Finished)
                    {
                        // timed out
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("TCPChannel-SessionMap, send timed out. {0}", message.ToString()));
                        sendTask.Dispose();
                        ns.Dispose();
                        throw new TimeoutException("Sending timed out.");
                    }
                    else if (!sendTask.Finished && !ns.Connected && sendTask.Exception == null)
                    {
                        // kapcsolat lebomlott.
                        // ez akkor van, amikor a küldés folyamatban van és a receive disposeolja le a sendTask-ot
                        // ekkor még nem áll be a kivétel (IOException) a send végén a finally blokkban és nincs kivétel sem.
                        sendTask.Dispose();
                        throw new IOException("Network stream closed and disposed while sending message content.");
                    }
                    else
                    {
                        result = sendTask.ResponseMessage;
                        sendTask.Dispose();
                        if (sendTask.Exception != null)
                        {
                            throw sendTask.Exception;
                        }
                    }
                }

                return result;
            }

            private void EnqueueSendTask(SendTask task)
            {
                lock (mSendTasks)
                {
                    mSendTasks.Enqueue(task);
                    mSendTaskEvent.Release();
                }
            }

            private void SendMain()
            {
                NetworkStream networkStream = this.mStream;
                if (networkStream != null)
                {
                    while (networkStream.Connected)
                    {
                        mSendTaskEvent.WaitOne();

                        SendTask task = null;
                        lock (mSendTasks)
                        {
                            if (mSendTasks.Count > 0)
                            {
                                task = mSendTasks.Dequeue();
                            }
                        }

                        if (task != null)
                        {
                            task.ExecutingSendOnNetwork();
                        }
                    }
                }
                lock (mSendTasks)
                {
                    mSendTasks.Clear();
                }
                mSendStopped = true;
            }

            private void ReceiveMain()
            {
                NetworkStream networkStream = this.mStream;
                if (networkStream != null)
                {
                    // ezzel a lock-al megakadályozom, hogy session újraélesztéskor véletlenül egynél több szál ügyködjön a run metóduson.
                    // protokol implementáció, amíg van kapcsolat, innen nem ugrik ki
                    while (networkStream.Connected)
                    {
                        List<ReadOnlySelfRemoverFileStream> readOnlyStreams = new List<ReadOnlySelfRemoverFileStream>();
                        bool isError = true;
                        try
                        {
                            MessageHeaderWithBody headerWithBody = mNetworkProtocol.Read(networkStream, mChannel.MaxReceiveMessageSize);
                            MessageSinkParameters parameters = new MessageSinkParameters(headerWithBody.MessageSinkConfiguration, headerWithBody.Data);
                            IMessage message = null;

                            // megkeresem, hogy melyik sink tudja deserializálni a kapott üzenetet a nyers adatsorból
                            foreach (IMessageSink sink in mChannel.ReceiveMessageSinks)
                            {
                                if (sink.MessageSinkId.Equals(headerWithBody.MessageSinkId))
                                {
                                    message = sink.Deserialize(parameters);
                                    break;
                                }
                            }

                            if (message == null)
                            {
                                // nem található sink a megadott ID-val, ezért nincs mivel visszafordítani az üzenetet
                                if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format("TCPChannel-SessionMap, unable to find {0} with id '{1}'. Connection will be dropped.", typeof(IMessageSink).Name, headerWithBody.MessageSinkId));
                                networkStream.Close(); // mivel nem értjük a másik oldal nyelvét, lezárjuk a kapcsolatot
                                break;
                            }
                            else
                            {
                                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("TCPChannel-SessionMap, message extracted from MessageHeaderwithBody ({0}). {1}", headerWithBody.GetHashCode().ToString(), message.ToString()));
                                if (message.MessageType == MessageTypeEnum.Acknowledge)
                                {
                                    // acknowledge üzenet fogadása és végrehajtása. Jelzés a várakozó szálnak, ha még nem timeout-olt el.
                                    lock (mWaitsForAcknowledge)
                                    {
                                        if (mWaitsForAcknowledge.ContainsKey(message.CorrelationId))
                                        {
                                            SendTask task = mWaitsForAcknowledge[message.CorrelationId];
                                            task.AcknowledgeEvent.Set();
                                        }
                                        else
                                        {
                                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(String.Format("TCPChannel-SessionMap, unable to find send task for acknowledge message, correlation id: {0}, size of the map: {1}", message.CorrelationId, mWaitsForAcknowledge.Count));
                                        }
                                    }
                                }
                                else
                                {
                                    // megvizsgálom, hogy vannak-e stream-ek
                                    if (message is RequestMessage)
                                    {
                                        RequestMessage m = (RequestMessage)message;
                                        if (m.MethodParameters != null)
                                        {
                                            foreach (MethodParameter mp in m.MethodParameters)
                                            {
                                                if (mp.Size > -1)
                                                {
                                                    StreamReceiveTask task = new StreamReceiveTask(networkStream, mChannel.TempStreamStorageFolder, mp.Size);
                                                    task.Receive(mChannel.MaxReceiveStreamSize);
                                                    ReadOnlySelfRemoverFileStream s = task.GetTempStream();
                                                    readOnlyStreams.Add(s);
                                                    mp.SetValueToStream(s); // elhelyezem a temp stream-et, a feldolgozó felelőssége, hogy ledisposolja
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ResponseMessage m = (ResponseMessage)message;
                                        if (m.ReturnValue.Size > -1)
                                        {
                                            StreamReceiveTask task = new StreamReceiveTask(networkStream, mChannel.TempStreamStorageFolder, m.ReturnValue.Size);
                                            task.Receive(mChannel.MaxReceiveStreamSize);
                                            ReadOnlySelfRemoverFileStream s = task.GetTempStream();
                                            readOnlyStreams.Add(s);
                                            m.ReturnValue.SetValueToStream(s); // elhelyezem a temp stream-et, a feldolgozó felelőssége, hogy ledisposolja
                                        }
                                    }

                                    if (message.MessageType == MessageTypeEnum.Response || message.MessageType == MessageTypeEnum.Datagram)
                                    {
                                        // send acknowledge message. Csak Response és Datagramüzenetnek van ack-ja.
                                        Send(new AcknowledgeMessage(message.CorrelationId), DEFAULT_TIMEOUT, true);
                                    }

                                    if (message is ResponseMessage)
                                    {
                                        // response üzenetet nem propagálunk ki eseményként, arra mindig várakoznia kell egy request szálnak, ha még
                                        // nem timeoutolt el.
                                        lock (mWaitsForResponse)
                                        {
                                            if (mWaitsForResponse.ContainsKey(message.CorrelationId))
                                            {
                                                SendTask task = mWaitsForResponse[message.CorrelationId];
                                                task.ResponseMessage = message;
                                                //LOGGER.Debug(String.Format("TCPChannel-SessionMap, signaling waiting sender thread. MessageHeaderWithBody hash: {0}.", headerWithBody.GetHashCode().ToString()));
                                                task.AcknowledgeEvent.Set();
                                            }
                                            else
                                            {
                                                if (LOGGER.IsDebugEnabled) LOGGER.Debug(String.Format("TCPChannel-SessionMap, unable to find send task for response message, correlation id: {0}, size of the map: {1}", message.CorrelationId, mWaitsForResponse.Count));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // request, datagram és oneway datagram típusú üzenetek eseményként való továbbítása
                                        mChannel.OnReceiveRequestMessage(mSessionId, message);
                                    }
                                }
                            }
                            isError = false;
                        }
                        catch (FormatException ex)
                        {
                            // akkor dobódik, ha a message nem állítható vissza a kapott adatokból. Olyan formátumban van, amit nem értünk.
                            String message = String.Format("TCPChannel-SessionMap, unable to deserialize data from the network stream. NetworkStreamId: {0}, Reason: {1}", networkStream.Id, ex.Message);
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(message, ex);
                            break;
                        }
                        catch (ObjectDisposedException)
                        {
                            // tipikusan akkor történik, ha a kapcsolat megszakadt és a stream-et már ki is dobtuk.
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(String.Format("TCPChannel-SessionMap, connection lost and the network stream already disposed while receving data. NetworkStreamId: {0}", networkStream.Id));
                            break;
                        }
                        catch (IOException ex)
                        {
                            // tipikusan akkor történik, ha a kapcsolat megszakadt vagy probléma van az adatátvitellel. Okozhatja még stream feltöltési hiba is.
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(String.Format("TCPChannel-SessionMap, a connection or a network stream has been closed while listening. NetworkStreamId: {0}, Reason: {1}", networkStream.Id, ex.Message));
                            break;
                        }
                        catch (MessageSecurityException ex)
                        {
                            // protocol handler vagy a StreamReceiveTask dobja, ha az üzenet mérete maximalizált és az túl nagy
                            String message = String.Format("TCPChannel-SessionMap, oversized message detected. NetworkStreamId: {0}", networkStream.Id);
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(message, ex);
                            break;
                        }
                        catch (ProtocolViolationException ex)
                        {
                            // protocol handler dobja, ha nem tudja a header-t visszaolvasni.
                            String message = String.Format("TCPChannel-SessionMap, protocol violation detected. NetworkStreamId: {0}, Reason: {1}", networkStream.Id, ex.Message);
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(message);
                            break;
                        }
                        catch (Exception ex)
                        {
                            // ismeretlen eredetű hiba keletkezett
                            String message = string.Format("TCPChannel-SessionMap, unexpected exception occured. NetworkStreamId: {0}", networkStream.Id);
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(message, ex);
                            break;
                        }
                        finally
                        {
                            // csak akkor takarítunk, ha valami baj volt és ott maradt egy vagy több fogadott stream.
                            if (isError && readOnlyStreams.Count > 0)
                            {
                                foreach (ReadOnlySelfRemoverFileStream s in readOnlyStreams)
                                {
                                    s.Dispose();
                                }
                            }
                            readOnlyStreams.Clear();
                        }
                    }
                    mReceiveStopped = true;
                    networkStream.Dispose();
                    ConnectionEndEventHandler();
                }
            }

            private void ConnectionEndEventHandler()
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(String.Format("TCPChannel-SessionMap, underlying network stream closed. Id: {0}. Removing waiting process(es).", mStream.Id));
                mSendTaskEvent.Release();
                lock (mWaitsForAcknowledge)
                {
                    // felszámolom a várakozó szálakat
                    if (mWaitsForAcknowledge.Count > 0)
                    {
                        foreach (SendTask task in mWaitsForAcknowledge.Values)
                        {
                            task.Connected = false;
                            task.Dispose();
                        }
                        mWaitsForAcknowledge.Clear();
                    }
                }
                lock (mWaitsForResponse)
                {
                    // felszámolom a várakozó szálakat
                    if (mWaitsForResponse.Count > 0)
                    {
                        foreach (SendTask task in mWaitsForResponse.Values)
                        {
                            task.Connected = false;
                            task.Dispose();
                        }
                        mWaitsForResponse.Clear();
                    }
                }
                mChannel.ConnectionEndEventHandler(mStream);
            }

            #endregion

        }

        /// <summary>
        /// Receive the content of a remote stream and store data in a temporary local stream
        /// </summary>
        private sealed class StreamReceiveTask
        {

            #region Field(s)

            private NetworkStream mNetworkStream = null;
            private String mTempStorageFolder = string.Empty;
            private int mStreamSize = 0;
            private FileInfo mTempFile = null;
            private FileStream mFileStream = null;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="StreamReceiveTask"/> class.
            /// </summary>
            /// <param name="networkStream">The network stream.</param>
            /// <param name="tempStorageFolder">The temp storage folder.</param>
            /// <param name="streamSize">Size of the stream.</param>
            internal StreamReceiveTask(NetworkStream networkStream, String tempStorageFolder, int streamSize)
            {
                this.mNetworkStream = networkStream;
                this.mTempStorageFolder = tempStorageFolder;
                this.mStreamSize = streamSize;
            }

            #endregion

            #region Internal method(s)

            /// <summary>
            /// Receives the specified max stream size.
            /// </summary>
            /// <param name="maxStreamSize">Size of the max stream.</param>
            internal void Receive(long maxStreamSize)
            {
                // létrehozom a stream-et a háttértáron
                if (maxStreamSize > 0 && mStreamSize > maxStreamSize)
                {
                    throw new MessageSecurityException("Stream size is larger than the maximum allowed stream size.");
                }
                mTempFile = new FileInfo(Path.Combine(mTempStorageFolder, Guid.NewGuid().ToString()));
                try
                {
                    using (mFileStream = new FileStream(mTempFile.FullName, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        // olvasom a networkStream-et
                        byte[] buffer = new byte[mNetworkStream.ReceiveBufferSize];
                        int receivedData = 0;
                        while (receivedData < mStreamSize)
                        {
                            int readBytes = mNetworkStream.Read(buffer, 0, (receivedData + buffer.Length) < mStreamSize ? buffer.Length : (mStreamSize - receivedData));
                            receivedData += readBytes;
                            mFileStream.Write(buffer, 0, readBytes); // a kapott adatmennyiséget beleírom a filestream-be
                        }
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        // ez akkor kell, ha fájlfogadás közben megszakad a kapcsolat
                        if (mTempFile.Exists)
                        {
                            mTempFile.Delete();
                        }
                    }
                    catch (Exception) { }
                    throw;
                }
            }

            /// <summary>
            /// Gets the temp stream.
            /// </summary>
            /// <returns></returns>
            internal ReadOnlySelfRemoverFileStream GetTempStream()
            {
                return new ReadOnlySelfRemoverFileStream(mTempFile, FileMode.Open); // az előzőleg lementett file stream-re rányitok egy ideiglenes olvasó stream-et
            }

            #endregion

        }

        /// <summary>
        /// Send data across the network
        /// </summary>
        private sealed class SendTask : IDisposable
        {

            #region Field(s)

            private IMessage mMessage = null;

            private List<Stream> mStreams = null;

            private NetworkStream mStreamLocal = null;

            private TCPChannel mChannelLocal = null;

            private Protocol mProtocol = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private IOException mException = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool mFinished = false;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private ManualResetEvent mWaitHandleForTimeoutEvent = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private ManualResetEvent mAcknowledgeEvent = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool mConnected = true;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private IMessage mResponseMessage = null;

            private bool mDisposed = false;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="SendTask"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="streams">The stream tasks.</param>
            /// <param name="streamLocal">The stream local.</param>
            /// <param name="channelLocal">The channel local.</param>
            /// <param name="protocol">The protocol.</param>
            internal SendTask(IMessage message, List<Stream> streams, NetworkStream streamLocal, TCPChannel channelLocal, Protocol protocol)
            {
                this.mMessage = message;
                this.mStreams = streams;
                this.mStreamLocal = streamLocal;
                this.mChannelLocal = channelLocal;
                this.mProtocol = protocol;
                if (message.MessageType != MessageTypeEnum.DatagramOneway)
                {
                    // oneway datagram üzeentnél nincs timeout, sem ack
                    mWaitHandleForTimeoutEvent = new ManualResetEvent(false);
                    mAcknowledgeEvent = new ManualResetEvent(false);
                }
            }

            /// <summary>
            /// Releases unmanaged resources and performs other cleanup operations before the
            /// <see cref="SendTask"/> is reclaimed by garbage collection.
            /// </summary>
            ~SendTask()
            {
                Dispose(false);
            }

            #endregion

            #region Internal properties

            /// <summary>
            /// Gets the exception.
            /// </summary>
            [DebuggerHidden]
            internal IOException Exception
            {
                get { return mException; }
            }

            /// <summary>
            /// Gets a value indicating whether this <see cref="SendTask"/> is finished.
            /// </summary>
            /// <value>
            ///   <c>true</c> if finished; otherwise, <c>false</c>.
            /// </value>
            [DebuggerHidden]
            internal bool Finished
            {
                get { return mFinished; }
            }

            /// <summary>
            /// Gets a value indicating whether this <see cref="SendTask"/> is connected.
            /// </summary>
            /// <value>
            ///   <c>true</c> if connected; otherwise, <c>false</c>.
            /// </value>
            [DebuggerHidden]
            internal bool Connected
            {
                get { return mConnected; }
                set { mConnected = value; }
            }

            /// <summary>
            /// Gets the wait handle for timeout event.
            /// </summary>
            [DebuggerHidden]
            internal ManualResetEvent WaitHandleForTimeoutEvent
            {
                get { return mWaitHandleForTimeoutEvent; }
            }

            /// <summary>
            /// Gets the acknowledge event.
            /// </summary>
            [DebuggerHidden]
            internal ManualResetEvent AcknowledgeEvent
            {
                get { return mAcknowledgeEvent; }
            }

            /// <summary>
            /// Gets or sets the response message.
            /// </summary>
            /// <value>
            /// The response message.
            /// </value>
            [DebuggerHidden]
            internal IMessage ResponseMessage
            {
                get { return mResponseMessage; }
                set { mResponseMessage = value; }
            }

            #endregion

            #region Public method(s)

            /// <summary>
            /// Sends this instance.
            /// </summary>
            internal void ExecutingSendOnNetwork()
            {
                try
                {
                    // a sync context azért kell, hogy ugyanazon a kapcsolaton egyszerre ne lehessen több szálnak adatot küldenie, mert összeakadnak
                    NetworkStream ns = this.mStreamLocal;
                    lock (ns)
                    {
                        // küldöm az üzenetet
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("TCPChannel-ExecutingSend, sending {0}", mMessage.ToString()));

                        IEnumeratorSpecialized<IMessageSink> sendSinks = mChannelLocal.SendMessageSinks.GetEnumerator();
                        while (sendSinks.MoveNext())
                        {
                            IMessageSink sink = sendSinks.Current;
                            try
                            {
                                MessageSinkParameters parameters = sink.Serialize(mMessage);
                                mProtocol.Write(ns, parameters.SerializedData, sink.MessageSinkId, parameters.ConfigurationToDeserialize, mChannelLocal.MaxSendMessageSize);
                                break;
                            }
                            catch (FormatException ex)
                            {
                                if (!sendSinks.HasNext())
                                {
                                    throw new IOException("Unable to serialize message.", ex);
                                }
                            }
                        }

                        // ha van stream, küldöm a tartalmukat
                        if (mStreams.Count > 0)
                        {
                            byte[] buffer = new byte[ns.ReceiveBufferSize];
                            foreach (Stream stream in mStreams)
                            {
                                try
                                {
                                    while (stream.Position < stream.Length)
                                    {
                                        int readBytes = stream.Read(buffer, 0, (stream.Length - stream.Position > buffer.Length ? buffer.Length : Convert.ToInt32(stream.Length - stream.Position)));
                                        ns.Write(buffer, 0, readBytes);
                                    }
                                }
                                catch (IOException)
                                {
                                    ns.Dispose(); // súlyos hiba, kapcsolat lezárásra kerül
                                    throw;
                                }
                            }
                        }
                    } // lock vége

                    if (mMessage.MessageType == MessageTypeEnum.Request || mMessage.MessageType == MessageTypeEnum.Response || mMessage.MessageType == MessageTypeEnum.Datagram)
                    {
                        this.mAcknowledgeEvent.WaitOne(); // várakozás a Response-ra vagy az Acknowledge üzenetre
                        if (!mStreamLocal.Connected)
                        {
                            throw new IOException("Network stream closed and disposed while sending message content.");
                        }
                        if (mDisposed)
                        {
                            throw new ObjectDisposedException(typeof(SendTask).Name);
                        }
                    }

                }
                catch (IOException ex)
                {
                    this.mConnected = false;
                    this.mException = ex;
                }
                catch (ObjectDisposedException ex)
                {
                    this.mConnected = false;
                    this.mException = new IOException("Network stream closed and disposed while sending message content.", ex);
                }
                catch (Exception ex)
                {
                    this.mConnected = false;
                    this.mException = new IOException("Unexpected exception occured while sending Message.", ex);
                    this.mStreamLocal.Dispose();
                }
                finally
                {
                    this.mFinished = true;
                    if (this.mWaitHandleForTimeoutEvent != null)
                    {
                        try
                        {
                            mWaitHandleForTimeoutEvent.Set();
                        }
                        catch (Exception) { }
                    }
                    mStreams.Clear();
                    mMessage = null;
                    mStreamLocal = null;
                    mChannelLocal = null;
                    mProtocol = null;
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region Private method(s)

            [MethodImpl(MethodImplOptions.Synchronized)]
            private void Dispose(bool disposing)
            {
                if (disposing && !mDisposed)
                {
                    this.mDisposed = true;
                    if (this.mWaitHandleForTimeoutEvent != null)
                    {
                        this.mWaitHandleForTimeoutEvent.Set();
                        this.mWaitHandleForTimeoutEvent.Dispose();
                    }
                    if (this.mAcknowledgeEvent != null)
                    {
                        this.mAcknowledgeEvent.Set();
                        this.mAcknowledgeEvent.Dispose();
                    }
                }
            }

            #endregion

        }

        #endregion

    }

}
