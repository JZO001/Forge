/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Forge.Collections;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.EventRaiser;
using Forge.Net.Remoting.Messaging;
using Forge.Net.Remoting.Sinks;
using Forge.Net.Synapse;
using Forge.Reflection;
using log4net;

namespace Forge.Net.Remoting.Channels
{

    /// <summary>
    /// Represents channel connection restoration
    /// </summary>
    /// <returns></returns>
    public delegate string ChannelConnectAgainDelegate();

    /// <summary>
    /// Represents channel establish a connection to a remote host
    /// </summary>
    /// <param name="remoteEp">The remote ep.</param>
    /// <returns></returns>
    public delegate string ChannelConnectDelegate(AddressEndPoint remoteEp);

    internal delegate void InternalReceiveRequestMessageDelegate(Channel channel, ReceiveMessageEventArgs e);

    /// <summary>
    /// Channel base implementation
    /// </summary>
    public abstract class Channel : MBRBase, IInitializable, IDisposable
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(Channel));

        /// <summary>
        /// Default method call timeout
        /// </summary>
        protected static readonly long DEFAULT_TIMEOUT = Timeout.Infinite;

        /// <summary>
        /// Channel identifier
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected String mChannelId = Guid.NewGuid().ToString();

        /// <summary>
        /// Send message sinks
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly List<IMessageSink> mSendMessageSinks = new List<IMessageSink>();

        /// <summary>
        /// Receive message sinks
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly List<IMessageSink> mReceiveMessageSinks = new List<IMessageSink>();

        /// <summary>
        /// Indicates the channel initialized.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected bool mInitialized = false;

        /// <summary>
        /// Indicated the channel supports the streams
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected bool mStreamsSupported = false;

        /// <summary>
        /// Indicates the channel supports reusable sessions
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected bool mSessionReusable = true;

        /// <summary>
        /// Stores the default connection data
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected AddressEndPoint mConnectionData = null;

        /// <summary>
        /// List of the server endpoints
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly List<AddressEndPoint> mServerEndpoints = new List<AddressEndPoint>();

        private readonly SerializedEventForSession mSerializedEvents = null;

        private int mAsyncActiveConnectCount = 0;
        private AutoResetEvent mAsyncActiveConnectEvent = null;
        private ChannelConnectAgainDelegate mConnectAgainDelegate = null;
        private ChannelConnectDelegate mConnectDelegate = null;

        private long mDefaultErrorResponseTimeout = 60000;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mDisposed = false;

        /// <summary>
        /// Occurs when [session state change].
        /// </summary>
        public event EventHandler<SessionStateEventArgs> SessionStateChange;

        /// <summary>
        /// Occurs when [receive message].
        /// </summary>
        public event EventHandler<ReceiveMessageEventArgs> ReceiveMessage;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="Channel"/> class.
        /// </summary>
        protected Channel()
        {
            mSerializedEvents = new SerializedEventForSession(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Channel"/> class.
        /// </summary>
        /// <param name="channelId">The channel unique id.</param>
        /// <param name="sendMessageSinks">The send message sinks.</param>
        /// <param name="receiveMessageSinks">The receive message sinks.</param>
        protected Channel(String channelId, ICollection<IMessageSink> sendMessageSinks, ICollection<IMessageSink> receiveMessageSinks)
            : this()
        {
            if (string.IsNullOrEmpty(channelId))
            {
                ThrowHelper.ThrowArgumentNullException("channelId");
            }
            if (sendMessageSinks == null)
            {
                ThrowHelper.ThrowArgumentNullException("sendMessageSinks");
            }
            if (receiveMessageSinks == null)
            {
                ThrowHelper.ThrowArgumentNullException("receiveMessageSinks");
            }
            if (sendMessageSinks.Count == 0)
            {
                ThrowHelper.ThrowArgumentException("No send message sink definied.");
            }
            if (receiveMessageSinks.Count == 0)
            {
                ThrowHelper.ThrowArgumentException("No receive message sink definied.");
            }
            if (ChannelServices.GetChannelById(channelId) != null)
            {
                ThrowHelper.ThrowArgumentException(string.Format("An other channel instance with this id '{0}' has already registered.", channelId));
            }
            this.mChannelId = channelId;
            this.mSendMessageSinks.AddRange(sendMessageSinks);
            this.mReceiveMessageSinks.AddRange(receiveMessageSinks);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Channel"/> is reclaimed by garbage collection.
        /// </summary>
        ~Channel()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the channel id.
        /// </summary>
        /// <value>
        /// The channel id.
        /// </value>
        [DebuggerHidden]
        public String ChannelId
        {
            get { return mChannelId; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsInitialized
        {
            get { return mInitialized; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is stream supported.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is stream supported; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsStreamSupported
        {
            get { return mStreamsSupported; }
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

        /// <summary>
        /// Gets the default connection data.
        /// </summary>
        /// <value>
        /// The default connection data.
        /// </value>
        [DebuggerHidden]
        public virtual AddressEndPoint DefaultConnectionData
        {
            get { return mConnectionData; }
        }

        /// <summary>
        /// Gets or sets the default error response timeout.
        /// </summary>
        /// <value>
        /// The default error response timeout.
        /// </value>
        [DebuggerHidden]
        public virtual long DefaultErrorResponseTimeout
        {
            get { return mDefaultErrorResponseTimeout; }
            set
            {
                if (value < Timeout.Infinite)
                {
                    mDefaultErrorResponseTimeout = Timeout.Infinite;
                }
                else
                {
                    mDefaultErrorResponseTimeout = value;
                }
            }
        }

        /// <summary>
        /// Gets the server endpoints.
        /// </summary>
        /// <value>
        /// The server endpoints.
        /// </value>
        [DebuggerHidden]
        public virtual ICollection<AddressEndPoint> ServerEndpoints
        {
            get { return new List<AddressEndPoint>(mServerEndpoints); }
        }

        /// <summary>
        /// Gets a value indicating whether [session reusable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [session reusable]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsSessionReusable
        {
            get { return mSessionReusable; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the channel.
        /// </summary>
        /// <param name="pi">The pi.</param>
        public virtual void Initialize(CategoryPropertyItem pi)
        {
            DoDisposeCheck();
            if (pi == null)
            {
                ThrowHelper.ThrowArgumentNullException("pi");
            }

            if (!mInitialized)
            {
                {
                    this.mConnectionData = null;
                    CategoryPropertyItem item = ConfigurationAccessHelper.GetCategoryPropertyByPath(pi.PropertyItems, "RemoteAddress");
                    if (item != null)
                    {
                        this.mConnectionData = AddressEndPoint.Parse(item.EntryValue);
                    }
                    if (item != null)
                    {
                        ConfigurationAccessHelper.ParseLongValue(pi.PropertyItems, "DefaultErrorResponseTimeout", Timeout.Infinite, long.MaxValue, ref mDefaultErrorResponseTimeout);
                    }
                }
                {
                    mServerEndpoints.Clear();
                    CategoryPropertyItem baseAddressesItems = ConfigurationAccessHelper.GetCategoryPropertyByPath(pi.PropertyItems, "BaseAddresses");
                    if (baseAddressesItems != null)
                    {
                        IEnumerator<CategoryPropertyItem> iterator = baseAddressesItems.GetEnumerator();
                        while (iterator.MoveNext())
                        {
                            mServerEndpoints.Add(AddressEndPoint.Parse(iterator.Current.EntryValue));
                        }
                    }
                }
                {
                    mSessionReusable = true;
                    ConfigurationAccessHelper.ParseBooleanValue(pi.PropertyItems, "SessionReusable", ref mSessionReusable);
                }
            }
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public virtual IAsyncResult BeginConnect(AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveConnectCount);
            ChannelConnectAgainDelegate d = new ChannelConnectAgainDelegate(this.Connect);
            if (this.mAsyncActiveConnectEvent == null)
            {
                lock (this)
                {
                    if (this.mAsyncActiveConnectEvent == null)
                    {
                        this.mAsyncActiveConnectEvent = new AutoResetEvent(true);
                    }
                }
            }
            this.mAsyncActiveConnectEvent.WaitOne();
            this.mConnectAgainDelegate = d;
            return d.BeginInvoke(callback, state);
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        public abstract string Connect();

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public virtual IAsyncResult BeginConnect(AddressEndPoint remoteEp, AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveConnectCount);
            ChannelConnectDelegate d = new ChannelConnectDelegate(this.Connect);
            if (this.mAsyncActiveConnectEvent == null)
            {
                lock (this)
                {
                    if (this.mAsyncActiveConnectEvent == null)
                    {
                        this.mAsyncActiveConnectEvent = new AutoResetEvent(true);
                    }
                }
            }
            this.mAsyncActiveConnectEvent.WaitOne();
            this.mConnectDelegate = d;
            return d.BeginInvoke(remoteEp, callback, state);
        }

        /// <summary>
        /// Connects the specified remote ep.
        /// </summary>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns></returns>
        public abstract string Connect(AddressEndPoint remoteEp);

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>SessionId</returns>
        public virtual string EndConnect(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (this.mConnectAgainDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndConnect called multiple times.", "asyncResult");
            }
            try
            {
                return this.mConnectAgainDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                this.mConnectAgainDelegate = null;
                this.mAsyncActiveConnectEvent.Set();
                CloseAsyncActiveConnectEvent(Interlocked.Decrement(ref mAsyncActiveConnectCount));
            }
        }

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>SessionId</returns>
        public virtual string EndConnect(IAsyncResult asyncResult, AddressEndPoint remoteEp)
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
        /// Disconnects the specified session id.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns>True, if the connection found and closed, otherwise False.</returns>
        public abstract bool Disconnect(String sessionId);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="message">The message.</param>
        /// <returns>Response message or null</returns>
        public IMessage SendMessage(string sessionId, IMessage message)
        {
            return SendMessage(sessionId, message, DEFAULT_TIMEOUT);
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>Response message or null</returns>
        public abstract IMessage SendMessage(string sessionId, IMessage message, long timeout);

        /// <summary>
        /// Starts the listening.
        /// </summary>
        public abstract void StartListening();

        /// <summary>
        /// Stops the listening.
        /// </summary>
        public abstract void StopListening();

        /// <summary>
        /// Gets the send message sinks.
        /// </summary>
        /// <value>
        /// The send message sinks.
        /// </value>
        public IEnumerableSpecialized<IMessageSink> SendMessageSinks
        {
            get
            {
                DoDisposeCheck();
                return new ListSpecialized<IMessageSink>(mSendMessageSinks);
            }
        }

        /// <summary>
        /// Gets the receive message sink.
        /// </summary>
        /// <value>
        /// The receive message sinks.
        /// </value>
        public IEnumerableSpecialized<IMessageSink> ReceiveMessageSinks
        {
            get
            {
                DoDisposeCheck();
                return new ListSpecialized<IMessageSink>(mReceiveMessageSinks);
            }
        }

        /// <summary>
        /// Gets the session info.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        public abstract ISessionInfo GetSessionInfo(string sessionId);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            Channel other = (Channel)obj;
            return other.mChannelId == mChannelId && other.mInitialized == mInitialized &&
                other.mDisposed == mDisposed && Arrays.DeepEquals(other.mSendMessageSinks.ToArray(), mSendMessageSinks.ToArray()) &&
                Arrays.DeepEquals(other.mReceiveMessageSinks.ToArray(), mReceiveMessageSinks.ToArray());
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Raises the <see cref="E:SessionStateChange"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Forge.Net.Remoting.Channels.SessionStateEventArgs"/> instance containing the event data.</param>
        protected void OnSessionStateChange(SessionStateEventArgs e)
        {
            Raiser.CallDelegatorBySync(SessionStateChange, new object[] { this, e });
        }

        /// <summary>
        /// Called when [receive request message].
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="message">The message.</param>
        protected void OnReceiveRequestMessage(String sessionId, IMessage message)
        {
            mSerializedEvents.AddMessage(sessionId, message);
        }

        /// <summary>
        /// Internals the on receive request message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="e">The <see cref="Forge.Net.Remoting.Channels.ReceiveMessageEventArgs"/> instance containing the event data.</param>
        protected void InternalOnReceiveRequestMessage(Channel channel, ReceiveMessageEventArgs e)
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("{0}-Event, raise event ReceiveMessage (BEGIN). SessionId: '{1}', {2}", this.GetType().Name, e.SessionId, e.Message.ToString()));
            Raiser.CallDelegatorBySync(ReceiveMessage, new object[] { channel, e });
            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("{0}-Event, raise event ReceiveMessage (END). SessionId: '{1}', {2}", this.GetType().Name, e.SessionId, e.Message.ToString()));
        }

        /// <summary>
        /// Creates the message sink.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns></returns>
        protected IMessageSink CreateMessageSink(CategoryPropertyItem pi)
        {
            if (string.IsNullOrEmpty(pi.EntryValue))
            {
                throw new InvalidConfigurationException("No message sink class definied in a message sink configuration entry.");
            }

            IMessageSink result = null;
            try
            {
                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("Channel, create message sink from type '{0}'. ChannelId: '{1}'.", pi.EntryValue, this.ChannelId));
                Type type = TypeHelper.GetTypeFromString(pi.EntryValue);
                result = (IMessageSink)type.GetConstructor(new Type[] { }).Invoke(null);
                result.Initialize(pi);
            }
            catch (Exception ex)
            {
                throw new InvalidConfigurationException(String.Format("Unable to instantiate message sink '{0}' specified in configuration.", pi.EntryValue), ex);
            }
            return result;
        }

        /// <summary>
        /// Does the dispose check.
        /// </summary>
        protected void DoDisposeCheck()
        {
            if (this.mDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.mSendMessageSinks.ForEach(i => i.Dispose());
                this.mReceiveMessageSinks.ForEach(i => i.Dispose());
                this.mSendMessageSinks.Clear();
                this.mReceiveMessageSinks.Clear();
            }
            this.mDisposed = true;
        }

        #endregion

        #region Private method(s)

        private void CloseAsyncActiveConnectEvent(int asyncActiveCount)
        {
            if ((this.mAsyncActiveConnectEvent != null) && (asyncActiveCount == 0))
            {
                this.mAsyncActiveConnectEvent.Close();
                this.mAsyncActiveConnectEvent = null;
            }
        }

        #endregion

        #region Nested classes

        private sealed class SerializedEventForSession
        {

            #region Field(s)

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Channel mChannel = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Dictionary<string, EventContainer> mContainers = new Dictionary<string, EventContainer>();

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="SerializedEventForSession"/> class.
            /// </summary>
            /// <param name="channel">The channel.</param>
            internal SerializedEventForSession(Channel channel)
            {
                this.mChannel = channel;
            }

            #endregion

            #region Internal properties

            /// <summary>
            /// Gets the channel.
            /// </summary>
            [DebuggerHidden]
            internal Channel Channel
            {
                get { return mChannel; }
            }

            /// <summary>
            /// Gets the containers.
            /// </summary>
            [DebuggerHidden]
            internal Dictionary<string, EventContainer> Containers
            {
                get { return mContainers; }
            }

            #endregion

            #region Internal method(s)

            /// <summary>
            /// Adds the message.
            /// </summary>
            /// <param name="sessionId">The session id.</param>
            /// <param name="message">The message.</param>
            internal void AddMessage(string sessionId, IMessage message)
            {
                lock (mContainers)
                {
                    EventContainer container = null;
                    if (mContainers.ContainsKey(sessionId))
                    {
                        container = mContainers[sessionId];
                    }
                    else
                    {
                        container = new EventContainer(this, sessionId);
                        mContainers.Add(sessionId, container);
                    }
                    container.AddMessage(message);
                }
            }

            #endregion

        }

        private sealed class EventContainer
        {

            #region Field(s)

            private readonly SerializedEventForSession mContainer = null;

            private String mSessionId = string.Empty;

            private readonly ManualResetEvent mResetEvent = new ManualResetEvent(false);

            private readonly Queue<IMessage> mQueue = new Queue<IMessage>();

            private readonly Thread mThread = null;

            private bool mDisposed = false;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="EventContainer"/> class.
            /// </summary>
            /// <param name="container">The container.</param>
            /// <param name="sessionId">The session id.</param>
            internal EventContainer(SerializedEventForSession container, String sessionId)
            {
                this.mContainer = container;
                this.mSessionId = sessionId;
                this.mThread = new Thread(new ThreadStart(ThreadMain));
                this.mThread.IsBackground = true;
                this.mThread.Name = String.Format("ChannelEvent_{0}", sessionId);
                this.mThread.Start();
            }

            #endregion

            #region Internal method(s)

            /// <summary>
            /// Adds the message.
            /// </summary>
            /// <param name="message">The message.</param>
            internal void AddMessage(IMessage message)
            {
                if (message.AllowParallelExecution)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("TCPChannel-Receive, forwarding message in parallel mode. {0}", message.ToString()));
                    InternalReceiveRequestMessageDelegate d = new InternalReceiveRequestMessageDelegate(mContainer.Channel.InternalOnReceiveRequestMessage);
                    d.BeginInvoke(mContainer.Channel, new ReceiveMessageEventArgs(mSessionId, message), new AsyncCallback(CallBack), d);
                }
                else
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("TCPChannel-Receive, forwarding message in sequential mode. {0}", message.ToString()));
                    mQueue.Enqueue(message);
                    mResetEvent.Set();
                }
            }

            #endregion

            #region Private method(s)

            private void CallBack(IAsyncResult asyncResult)
            {
                InternalReceiveRequestMessageDelegate d = asyncResult.AsyncState as InternalReceiveRequestMessageDelegate;
                d.EndInvoke(asyncResult);
            }

            private void ThreadMain()
            {
                IMessage message = null;
                while (!mDisposed)
                {
                    mResetEvent.WaitOne(10000); // 10mp után leáll és felszabadul
                    lock (mContainer.Containers)
                    {
                        if (mQueue.Count > 0)
                        {
                            message = mQueue.Dequeue();
                        }
                        else
                        {
                            mDisposed = true;
                            mContainer.Containers.Remove(this.mSessionId);
                            mResetEvent.Dispose();
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(String.Format("EVENT_CONTAINER, auto-shutdown for connection event handler, sessionId: '{0}'.", this.mSessionId));
                        }
                    }
                    if (message != null)
                    {
                        mContainer.Channel.InternalOnReceiveRequestMessage(mContainer.Channel, new ReceiveMessageEventArgs(mSessionId, message));
                        message = null;
                        lock (mContainer.Containers)
                        {
                            if (mQueue.Count == 0)
                            {
                                mResetEvent.Reset();
                            }
                        }
                    }
                }
            }

            #endregion

        }

        #endregion

    }

}
