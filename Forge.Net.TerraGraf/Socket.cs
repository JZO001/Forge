/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Forge.Collections;
using Forge.Legacy;
using Forge.Logging.Abstraction;
using Forge.Net.Synapse;
using Forge.Net.Synapse.NetworkServices;
using Forge.Net.TerraGraf.Contexts;
using Forge.Net.TerraGraf.Messaging;
using Forge.Net.TerraGraf.NetworkPeers;
using Forge.Shared;
using Forge.Threading;
using Forge.Threading.Tasking;

namespace Forge.Net.TerraGraf
{

#if NET40
    internal delegate ISocket SocketAcceptDelegate();
    internal delegate void SocketConnectDelegate(EndPoint endPoint);
    internal delegate int SocketReceiveDelegate(byte[] buffer, int offset, int size);
    internal delegate int SocketReceiveFromDelegate(byte[] buffer, int offset, int size, ref EndPoint remoteEp);
    internal delegate int SocketSendDelegate(byte[] buffer, int offset, int size);
    internal delegate int SocketSendToDelegate(byte[] buffer, int offset, int size, EndPoint remoteEp);
#endif

    /// <summary>
    /// Socket implementation for TerraGraf
    /// </summary>
    internal sealed class Socket : MBRBase, ISocket
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<Socket>();

        private static Semaphore mSemaphoreConnection = new Semaphore(NetworkManager.Instance.InternalConfiguration.Settings.DefaultConcurrentSocketConnectionAttempts,
            NetworkManager.Instance.InternalConfiguration.Settings.DefaultConcurrentSocketConnectionAttempts);

        private static Thread mThreadServerSocketAcceptTimeoutListener = null;

        private static HashSet<Socket> mServerSockets = null;

        private static Semaphore mSemaphoreServerSockets = null;

        private static HashSet<int> mUsedPorts = new HashSet<int>();

        private static long mGlobalSocketId = 0;
        private long mLocalSocketId = Interlocked.Increment(ref mGlobalSocketId);

        private long mRemoteSocketId = -1; // at TCP messages

        private ulong mRemotePacketOrderNumber = 0;

        private ulong mLocalPacketOrderNumber = 0;

        private int mReceiveBufferSize = 0; // set in constructor

        private int mSendBufferSize = 0; // set in constructor

        private int mReceiveTimeout = 0; // set in constructor

        private int mSendTimeout = 0; // set in constructor

#if NET40
        private SocketAcceptDelegate mAcceptDelegateOld = null;
        private SocketConnectDelegate mConnectDelegateOld = null;
        private SocketReceiveDelegate mReceiveDelegateOld = null;
        private SocketReceiveFromDelegate mReceiveFromDelegateOld = null;
        private SocketSendDelegate mSendDelegateOld = null;
        private SocketSendToDelegate mSendToDelegateOld = null;
#endif

        private System.Func<ISocket> mAcceptDelegate = null;
        private int mAsyncActiveAcceptCount = 0;
        private AutoResetEvent mAsyncActiveAcceptEvent = null;
        private readonly object LOCK_ACCEPT = new object();

        private System.Action<EndPoint> mEndpointConnectDelegate = null;
        private System.Action<string, int> mHostIpConnectDelegate = null;
        private int mAsyncActiveConnectCount = 0;
        private AutoResetEvent mAsyncActiveConnectEvent = null;
        private readonly object LOCK_CONNECT = new object();

        private System.Func<byte[], int, int, int> mReceiveDelegate = null;
        private int mAsyncActiveReceiveCount = 0;
        private AutoResetEvent mAsyncActiveReceiveEvent = null;
        private readonly object LOCK_RECEIVE = new object();

        private System.Func<byte[], int, int, EndPoint, int> mReceiveFromDelegate = null;
        private int mAsyncActiveReceiveFromCount = 0;
        private AutoResetEvent mAsyncActiveReceiveFromEvent = null;
        private readonly object LOCK_RECEIVE_FROM = new object();
        private EndPoint mReceiveFromEndpoint = null;

        private System.Func<byte[], int, int, int> mSendDelegate = null;
        private int mAsyncActiveSendCount = 0;
        private AutoResetEvent mAsyncActiveSendEvent = null;
        private readonly object LOCK_SEND = new object();

        private System.Func<byte[], int, int, EndPoint, int> mSendToDelegate = null;
        private int mAsyncActiveSendToCount = 0;
        private AutoResetEvent mAsyncActiveSendToEvent = null;
        private readonly object LOCK_SENDTO = new object();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ProtocolType mProtocolType = ProtocolType.Tcp;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SocketType mSocketType = SocketType.Stream;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AddressEndPoint mLocalEndPoint = null;

        private AddressEndPoint mLocalEndPointInternal = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AddressEndPoint mRemoteEndPoint = null;

        private AddressEndPoint mRemoteEndPointInternal = null;

        private AutoResetEvent mConnectEvent = new AutoResetEvent(false);

        private NetworkPeerRemote mRemoteNetworkPeer = null;

        private Queue<IncomingConnection> mListenerQueue = null;

        private int mBacklog = NetworkManager.Instance.InternalConfiguration.Settings.DefaultSocketBacklogSize;

        private Semaphore mSemaphoreListener = null;

        private bool mListening = false;

        private readonly ListSpecialized<ReceivedMessage> mReceivedMessages = new ListSpecialized<ReceivedMessage>();

        private AutoResetEvent mReceiveEvent = new AutoResetEvent(false);

        private AutoResetEvent mSendEvent = new AutoResetEvent(true);

        private ManualResetEvent mSentEventForMessages = new ManualResetEvent(false);

        private int mCloseTimeout = 0;

        private int mClosedLevel = 0;

        private bool mShutdownReceive = false;

        private bool mShutdownSend = false;

        private readonly object mConnectLockObject = new object();

        private readonly object mSendCloseLockObject = new object();

        private readonly object mDisposeLock = new object();

        private readonly object mCloseLock = new object();

        private readonly object mListenLock = new object();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="Socket"/> class.
        /// </summary>
        /// <param name="socketType">Type of the socket.</param>
        /// <param name="protocolType">Type of the protocol.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)")]
        internal Socket(SocketType socketType, ProtocolType protocolType)
        {
            if (!(socketType == SocketType.Dgram && protocolType == ProtocolType.Udp ||
                socketType == SocketType.Stream && protocolType == ProtocolType.Tcp))
            {
                ThrowHelper.ThrowArgumentException("Unsupported protocol and socket type.", "protocolType");
            }
            mProtocolType = protocolType;
            mSocketType = socketType;
            mReceiveBufferSize = NetworkManager.Instance.InternalConfiguration.Settings.DefaultReceiveBufferSize;
            mSendBufferSize = NetworkManager.Instance.InternalConfiguration.Settings.DefaultSendBufferSize;
            mReceiveTimeout = NetworkManager.Instance.InternalConfiguration.Settings.DefaultReceiveTimeoutInMS;
            mSendTimeout = NetworkManager.Instance.InternalConfiguration.Settings.DefaultSendTimeoutInMS;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Socket"/> is reclaimed by garbage collection.
        /// </summary>
        ~Socket()
        {
            Dispose(false);
        }

        #endregion

        #region Public method(s)

#if NET40

        /// <summary>
        /// Begins the accept.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            DoDisposeCheck();
            DoListenNotCheck(); // must listen on

            Interlocked.Increment(ref mAsyncActiveAcceptCount);
            SocketAcceptDelegate d = new SocketAcceptDelegate(Accept);
            if (mAsyncActiveAcceptEvent == null)
            {
                lock (LOCK_ACCEPT)
                {
                    if (mAsyncActiveAcceptEvent == null)
                    {
                        mAsyncActiveAcceptEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveAcceptEvent.WaitOne();
            mAcceptDelegateOld = d;
            return d.BeginInvoke(callback, state);
        }

        /// <summary>
        /// Ends the accept.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>
        /// Socket implementation
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)")]
        public ISocket EndAccept(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mAcceptDelegateOld == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndAccept called multiple times.", "asyncResult");
            }
            try
            {
                return mAcceptDelegateOld.EndInvoke(asyncResult);
            }
            finally
            {
                mAcceptDelegateOld = null;
                mAsyncActiveAcceptEvent.Set();
                CloseAsyncActiveAcceptEvent(Interlocked.Decrement(ref mAsyncActiveAcceptCount));
            }
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginConnect(EndPoint endPoint, AsyncCallback callBack, object state)
        {
            DoDisposeCheck();
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }

            Interlocked.Increment(ref mAsyncActiveConnectCount);
            SocketConnectDelegate d = new SocketConnectDelegate(Connect);
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
            mConnectDelegateOld = d;
            return d.BeginInvoke(endPoint, callBack, state);
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginConnect(string host, int port, AsyncCallback callBack, object state)
        {
            return BeginConnect(new AddressEndPoint(host, port), callBack, state);
        }

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)")]
        public void EndConnect(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mConnectDelegateOld == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndConnect called multiple times.", "asyncResult");
            }
            try
            {
                mConnectDelegateOld.EndInvoke(asyncResult);
            }
            finally
            {
                mConnectDelegateOld = null;
                mAsyncActiveConnectEvent.Set();
                CloseAsyncActiveConnectEvent(Interlocked.Decrement(ref mAsyncActiveConnectCount));
            }
        }

        /// <summary>
        /// Begins the receive.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, AsyncCallback callBack, object state)
        {
            DoDisposeCheck();
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if ((offset < 0) || (offset > buffer.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("offset");
            }
            if ((size < 0) || (size > (buffer.Length - offset)))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("size");
            }

            Interlocked.Increment(ref mAsyncActiveReceiveCount);
            SocketReceiveDelegate d = new SocketReceiveDelegate(Receive);
            if (mAsyncActiveReceiveEvent == null)
            {
                lock (LOCK_RECEIVE)
                {
                    if (mAsyncActiveReceiveEvent == null)
                    {
                        mAsyncActiveReceiveEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveReceiveEvent.WaitOne();
            mReceiveDelegateOld = d;
            return d.BeginInvoke(buffer, offset, size, callBack, state);
        }

        /// <summary>
        /// Ends the receive.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>
        /// Number of received bytes
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)")]
        public int EndReceive(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mReceiveDelegateOld == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndReceive called multiple times.", "asyncResult");
            }

            try
            {
                return mReceiveDelegateOld.EndInvoke(asyncResult);
            }
            finally
            {
                mReceiveDelegateOld = null;
                mAsyncActiveReceiveEvent.Set();
                CloseAsyncActiveReceiveEvent(Interlocked.Decrement(ref mAsyncActiveReceiveCount));
            }
        }

        /// <summary>
        /// Receives from.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, ref EndPoint remoteEp, AsyncCallback callBack, object state)
        {
            DoDisposeCheck();
            if (remoteEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("remoteEp");
            }
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if ((offset < 0) || (offset > buffer.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("offset");
            }
            if ((size < 0) || (size > (buffer.Length - offset)))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("size");
            }

            Interlocked.Increment(ref mAsyncActiveReceiveFromCount);
            SocketReceiveFromDelegate d = new SocketReceiveFromDelegate(ReceiveFrom);
            if (mAsyncActiveReceiveFromEvent == null)
            {
                lock (LOCK_RECEIVE_FROM)
                {
                    if (mAsyncActiveReceiveFromEvent == null)
                    {
                        mAsyncActiveReceiveFromEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveReceiveFromEvent.WaitOne();
            mReceiveFromDelegateOld = d;
            return d.BeginInvoke(buffer, offset, size, ref remoteEp, callBack, state);
        }

        /// <summary>
        /// Ends the receive from.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>
        /// Number of received bytes
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)")]
        public int EndReceiveFrom(IAsyncResult asyncResult, ref EndPoint remoteEp)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mReceiveFromDelegateOld == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndReceiveFrom called multiple times.", "asyncResult");
            }

            try
            {
                return mReceiveFromDelegateOld.EndInvoke(ref remoteEp, asyncResult);
            }
            finally
            {
                mReceiveFromDelegateOld = null;
                mAsyncActiveReceiveFromEvent.Set();
                CloseAsyncActiveReceiveFromEvent(Interlocked.Decrement(ref mAsyncActiveReceiveFromCount));
            }
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, AsyncCallback callBack, object state)
        {
            DoDisposeCheck();
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if ((offset < 0) || (offset > buffer.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("offset");
            }
            if ((size < 0) || (size > (buffer.Length - offset)))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("size");
            }

            Interlocked.Increment(ref mAsyncActiveSendCount);
            SocketSendDelegate d = new SocketSendDelegate(Send);
            if (mAsyncActiveSendEvent == null)
            {
                lock (LOCK_SEND)
                {
                    if (mAsyncActiveSendEvent == null)
                    {
                        mAsyncActiveSendEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveSendEvent.WaitOne();
            mSendDelegateOld = d;
            return d.BeginInvoke(buffer, offset, size, callBack, state);
        }

        /// <summary>
        /// Ends the send.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)")]
        public int EndSend(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mSendDelegateOld == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndSend called multiple times.", "asyncResult");
            }

            try
            {
                return mSendDelegateOld.EndInvoke(asyncResult);
            }
            finally
            {
                mSendDelegateOld = null;
                mAsyncActiveSendEvent.Set();
                CloseAsyncActiveSendEvent(Interlocked.Decrement(ref mAsyncActiveSendCount));
            }
        }

        /// <summary>
        /// Begins the send to.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, EndPoint remoteEp, AsyncCallback callBack, object state)
        {
            DoDisposeCheck();
            if (remoteEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("remoteEp");
            }
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if ((offset < 0) || (offset > buffer.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("offset");
            }
            if ((size < 0) || (size > (buffer.Length - offset)))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("size");
            }

            Interlocked.Increment(ref mAsyncActiveSendToCount);
            SocketSendToDelegate d = new SocketSendToDelegate(SendTo);
            if (mAsyncActiveSendToEvent == null)
            {
                lock (LOCK_SENDTO)
                {
                    if (mAsyncActiveSendToEvent == null)
                    {
                        mAsyncActiveSendToEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveSendToEvent.WaitOne();
            mSendToDelegateOld = d;
            return d.BeginInvoke(buffer, offset, size, remoteEp, callBack, state);
        }

        /// <summary>
        /// Ends the send to.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)")]
        public int EndSendTo(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mSendToDelegateOld == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndSendTo called multiple times.", "asyncResult");
            }

            try
            {
                return mSendToDelegateOld.EndInvoke(asyncResult);
            }
            finally
            {
                mSendToDelegateOld = null;
                mAsyncActiveSendToEvent.Set();
                CloseAsyncActiveSendToEvent(Interlocked.Decrement(ref mAsyncActiveSendToCount));
            }
        }

#endif

        /// <summary>Begins the accept.</summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginAccept(ReturnCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveAcceptCount);
            System.Func<ISocket> d = new System.Func<ISocket>(Accept);
            if (mAsyncActiveAcceptEvent == null)
            {
                lock (LOCK_ACCEPT)
                {
                    if (mAsyncActiveAcceptEvent == null)
                    {
                        mAsyncActiveAcceptEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveAcceptEvent.WaitOne();
            mAcceptDelegate = d;
            return d.BeginInvoke(callback, state);
        }

        /// <summary>Ends the accept.</summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Socket implementation</returns>
        public ISocket EndAccept(ITaskResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mAcceptDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndAccept called multiple times.", "asyncResult");
            }
            try
            {
                return mAcceptDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mAcceptDelegate = null;
                mAsyncActiveAcceptEvent.Set();
                CloseAsyncActiveAcceptEvent(Interlocked.Decrement(ref mAsyncActiveAcceptCount));
            }
        }

        /// <summary>Begins the connect.</summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="callback">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        /// <exception cref="System.ArgumentNullException">endPoint</exception>
        public ITaskResult BeginConnect(EndPoint endPoint, ReturnCallback callback, object state)
        {
            if (endPoint == null) throw new ArgumentNullException(nameof(endPoint));

            Interlocked.Increment(ref mAsyncActiveConnectCount);
            System.Action<EndPoint> d = new System.Action<EndPoint>(Connect);
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
            mEndpointConnectDelegate = d;
            return d.BeginInvoke(endPoint, callback, state);
        }

        /// <summary>Begins the connect.</summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        /// <exception cref="System.ArgumentNullException">host</exception>
        public ITaskResult BeginConnect(string host, int port, ReturnCallback callback, object state)
        {
            if (string.IsNullOrWhiteSpace(host)) throw new ArgumentNullException(nameof(host));

            Interlocked.Increment(ref mAsyncActiveConnectCount);
            System.Action<string, int> d = new System.Action<string, int>(Connect);
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
            mHostIpConnectDelegate = d;
            return d.BeginInvoke(host, port, callback, state);
        }

        /// <summary>Ends the connect.</summary>
        /// <param name="asyncResult">The async result.</param>
        public void EndConnect(ITaskResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mEndpointConnectDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndConnect called multiple times.", "asyncResult");
            }
            try
            {
                mEndpointConnectDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mEndpointConnectDelegate = null;
                mAsyncActiveConnectEvent.Set();
                CloseAsyncActiveConnectEvent(Interlocked.Decrement(ref mAsyncActiveConnectCount));
            }
        }

        /// <summary>Begins the receive.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callback">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        public ITaskResult BeginReceive(byte[] buffer, int offset, int size, ReturnCallback callback, object state)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            Interlocked.Increment(ref mAsyncActiveReceiveCount);
            System.Func<byte[], int, int, int> d = new System.Func<byte[], int, int, int>(Receive);
            if (mAsyncActiveReceiveEvent == null)
            {
                lock (LOCK_RECEIVE)
                {
                    if (mAsyncActiveReceiveEvent == null)
                    {
                        mAsyncActiveReceiveEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveReceiveEvent.WaitOne();
            mReceiveDelegate = d;
            return d.BeginInvoke(buffer, offset, size, callback, state);
        }

        /// <summary>Ends the receive.</summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of received bytes</returns>
        public int EndReceive(ITaskResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mReceiveDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndReceive called multiple times.", "asyncResult");
            }
            try
            {
                return mReceiveDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mReceiveDelegate = null;
                mAsyncActiveReceiveEvent.Set();
                CloseAsyncActiveReceiveEvent(Interlocked.Decrement(ref mAsyncActiveReceiveCount));
            }
        }

        /// <summary>Receives from.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        /// <exception cref="System.ArgumentNullException">buffer
        /// or
        /// remoteEp</exception>
        public ITaskResult BeginReceiveFrom(byte[] buffer, int offset, int size, ref EndPoint remoteEp, ReturnCallback callback, object state)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (remoteEp == null) throw new ArgumentNullException(nameof(remoteEp));

            mReceiveFromEndpoint = remoteEp;

            Interlocked.Increment(ref mAsyncActiveReceiveFromCount);
            System.Func<byte[], int, int, EndPoint, int> d = new System.Func<byte[], int, int, EndPoint, int>(InternalReceiveFrom);
            if (mAsyncActiveReceiveFromEvent == null)
            {
                lock (LOCK_RECEIVE_FROM)
                {
                    if (mAsyncActiveReceiveFromEvent == null)
                    {
                        mAsyncActiveReceiveFromEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveReceiveFromEvent.WaitOne();
            mReceiveFromDelegate = d;
            return d.BeginInvoke(buffer, offset, size, remoteEp, callback, state);
        }

        /// <summary>Ends the receive from.</summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        public int EndReceiveFrom(ITaskResult asyncResult, ref EndPoint remoteEp)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mReceiveFromDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndReceiveFrom called multiple times.", "asyncResult");
            }
            try
            {
                int result = mReceiveFromDelegate.EndInvoke(asyncResult);
                remoteEp = mReceiveFromEndpoint;
                return result;
            }
            finally
            {
                mReceiveFromDelegate = null;
                mAsyncActiveReceiveFromEvent.Set();
                CloseAsyncActiveReceiveFromEvent(Interlocked.Decrement(ref mAsyncActiveReceiveFromCount));
            }
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callback">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        public ITaskResult BeginSend(byte[] buffer, int offset, int size, ReturnCallback callback, object state)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            Interlocked.Increment(ref mAsyncActiveSendCount);
            System.Func<byte[], int, int, int> d = new System.Func<byte[], int, int, int>(Send);
            if (mAsyncActiveSendEvent == null)
            {
                lock (LOCK_SEND)
                {
                    if (mAsyncActiveSendEvent == null)
                    {
                        mAsyncActiveSendEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveSendEvent.WaitOne();
            mSendDelegate = d;
            return d.BeginInvoke(buffer, offset, size, callback, state);
        }

        /// <summary>Ends the send.</summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of sent bytes</returns>
        public int EndSend(ITaskResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mSendDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndSend called multiple times.", "asyncResult");
            }
            try
            {
                return mSendDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mSendDelegate = null;
                mAsyncActiveSendEvent.Set();
                CloseAsyncActiveSendEvent(Interlocked.Decrement(ref mAsyncActiveSendCount));
            }
        }

        /// <summary>Begins the send to.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        /// <exception cref="System.ArgumentNullException">buffer
        /// or
        /// remoteEp</exception>
        public ITaskResult BeginSendTo(byte[] buffer, int offset, int size, EndPoint remoteEp, ReturnCallback callback, object state)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (remoteEp == null) throw new ArgumentNullException(nameof(remoteEp));

            Interlocked.Increment(ref mAsyncActiveSendToCount);
            System.Func<byte[], int, int, EndPoint, int> d = new System.Func<byte[], int, int, EndPoint, int>(SendTo);
            if (mAsyncActiveSendToEvent == null)
            {
                lock (LOCK_SENDTO)
                {
                    if (mAsyncActiveSendToEvent == null)
                    {
                        mAsyncActiveSendToEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveSendToEvent.WaitOne();
            mSendToDelegate = d;
            return d.BeginInvoke(buffer, offset, size, remoteEp, callback, state);
        }

        /// <summary>Ends the send to.</summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of sent bytes</returns>
        public int EndSendTo(ITaskResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mSendToDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndSendTo called multiple times.", "asyncResult");
            }
            try
            {
                return mSendToDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mSendToDelegate = null;
                mAsyncActiveSendToEvent.Set();
                CloseAsyncActiveSendToEvent(Interlocked.Decrement(ref mAsyncActiveSendToCount));
            }
        }

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Accepts a new incoming connection.
        /// </summary>
        /// <returns>Socket implementation</returns>
        public async Task<ISocket> AcceptAsync()
        {
            return await Task.Run(() => Accept());
        }

#endif

        /// <summary>
        /// Accepts a new incoming connection.
        /// </summary>
        /// <returns>
        /// Socket implementation
        /// </returns>
        /// <exception cref="System.Net.Sockets.SocketException"></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public ISocket Accept()
        {
            DoDisposeCheck();
            DoBindCheck(); // need to be binded
            DoListenNotCheck(); // must listen to
            DoShutdownReceiveCheck();
            DoShutdownSendCheck();

            Socket result = null;

            while (result == null)
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), start accept, waiting for an incoming connection.", mLocalSocketId.ToString()));
                mSemaphoreListener.WaitOne(); // only one thread is allowed at the same time
                DoShutdownReceiveCheck();
                if (mClosedLevel > 0)
                {
                    throw new SocketException((int)SocketError.Shutdown);
                }
                lock (mDisposeLock)
                {
                    DoShutdownReceiveCheck();
                    if (mClosedLevel > 0)
                    {
                        throw new SocketException((int)SocketError.Shutdown);
                    }
                    lock (mListenerQueue)
                    {
                        IncomingConnection c = mListenerQueue.Peek();
                        if (!c.Timedout)
                        {
                            c.Accepted = true;
                            mListenerQueue.Dequeue();
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), accepting incoming connection. {1}", mLocalSocketId.ToString(), c.Message.ToString()));
                            if (ExclusiveAddressUse)
                            {
                                // using existing port
                                mListening = false;
                                SocketOpenResponseMessage response = null;
                                while (mListenerQueue.Count > 0)
                                {
                                    // drop out, whose are landed into the queue in the meantime
                                    IncomingConnection trash = mListenerQueue.Dequeue();
                                    response = new SocketOpenResponseMessage(NetworkManager.Instance.InternalLocalhost.Id,
                                        trash.Message.SenderId, -1, trash.Message.SenderPort, -1, trash.Message.SenderSocketId);
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), socket connection declined, socket port used exclusivelly, sending response to the client. MessageId: {1}", mLocalSocketId.ToString(), trash.Message.MessageId.ToString()));
                                    NetworkManager.Instance.InternalSendMessage(response, null);
                                }
                                mRemoteSocketId = c.Message.SenderSocketId;
                                mRemoteEndPointInternal = new AddressEndPoint(c.Message.SenderId, c.Message.SenderPort);
                                mRemoteEndPoint = mRemoteEndPointInternal;
                                mRemoteNetworkPeer = (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(c.Message.SenderId);
                                //mReceivedMessages = new ListSpecialized<ReceivedMessage>();
                                //mReceiveEvent = new AutoResetEvent(false);

                                NetworkManager.Instance.NetworkPeerUnaccessible += new EventHandler<NetworkPeerChangedEventArgs>(NetworkPeerUnaccessibleHandler);
                                lock (mServerSockets)
                                {
                                    mServerSockets.Remove(this); // I am not a server socket anymore
                                }

                                // válaszüzenet küldése
                                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), socket connection established, sending response to the client. MessageId: {1}", mLocalSocketId.ToString(), c.Message.MessageId.ToString()));
                                response = new SocketOpenResponseMessage(NetworkManager.Instance.InternalLocalhost.Id,
                                    c.Message.SenderId, mLocalEndPointInternal.Port, c.Message.SenderPort, mLocalSocketId, c.Message.SenderSocketId);
                                NetworkManager.Instance.InternalSendMessage(response, null);
                                result = this;
                            }
                            else
                            {
                                // redirecting to an other port
                                result = new Socket(System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                                try
                                {
                                    result.Bind(new AddressEndPoint(NetworkManager.Instance.InternalLocalhost.Id, 0));
                                    result.LocalEndPoint = mLocalEndPointInternal;
                                }
                                catch (Exception)
                                {
                                    // if this throws an exception, that means there are no free port numbers
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), no internal port available for incoming connections. MessageId: {1}", mLocalSocketId.ToString(), c.Message.MessageId.ToString()));
                                    result = null;
                                }

                                if (result == null)
                                {
                                    // sending the response message, there is no free socket connection port
                                    SocketOpenResponseMessage response = new SocketOpenResponseMessage(NetworkManager.Instance.InternalLocalhost.Id,
                                        c.Message.SenderId, -1, c.Message.SenderPort, -1, c.Message.SenderSocketId);
                                    NetworkManager.Instance.InternalSendMessage(response, null);
                                }
                                else
                                {
                                    // connection successful
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), socket connection established, sending response to the client. MessageId: {1}", mLocalSocketId.ToString(), c.Message.MessageId.ToString()));
                                    result.mRemoteSocketId = c.Message.SenderSocketId;
                                    result.mRemoteEndPointInternal = new AddressEndPoint(c.Message.SenderId, c.Message.SenderPort);
                                    result.mRemoteEndPoint = result.mRemoteEndPointInternal;
                                    result.mRemoteNetworkPeer = NetworkManager.Instance.InternalLocalhost.Id.Equals(c.Message.SenderId) ? NetworkManager.Instance.InternalLocalhost : (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(c.Message.SenderId);
                                    //result.mReceivedMessages = new ListSpecialized<ReceivedMessage>();
                                    //result.mReceiveEvent = new AutoResetEvent(false);
                                    NetworkManager.Instance.NetworkPeerUnaccessible += new EventHandler<NetworkPeerChangedEventArgs>(result.NetworkPeerUnaccessibleHandler);
                                    SocketOpenResponseMessage response = new SocketOpenResponseMessage(NetworkManager.Instance.InternalLocalhost.Id,
                                        c.Message.SenderId, result.mLocalEndPointInternal.Port, c.Message.SenderPort, result.mLocalSocketId, c.Message.SenderSocketId);
                                    NetworkManager.Instance.InternalSendMessage(response, null);
                                }
                            }
                        }
                        else
                        {
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), this connection request has timed out. MessageId: {1}", mLocalSocketId.ToString(), c.Message.MessageId.ToString()));
                        }
                    }
                }
            }

#pragma warning disable CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception
            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), accept finished. Result socket id: {1}", mLocalSocketId.ToString(), result.mLocalSocketId.ToString()));
#pragma warning restore CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception

            return result;
        }

        /// <summary>
        /// Binds the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)"), MethodImpl(MethodImplOptions.Synchronized)]
        public void Bind(EndPoint endPoint)
        {
            DoDisposeCheck();
            DoBindNotCheck(); // binded not allowed
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }

            int port = 0;
            if (endPoint is AddressEndPoint)
            {
                AddressEndPoint ep = (AddressEndPoint)endPoint;
                port = ep.Port;
            }
            else if (endPoint is DnsEndPoint)
            {
                DnsEndPoint ep = (DnsEndPoint)endPoint;
                port = ep.Port;
            }
            else
            {
                ThrowHelper.ThrowArgumentException("Unsupported endpoint type.", "endPoint");
            }

            lock (mUsedPorts)
            {
                if (port == 0)
                {
                    // find a free port, if no one, throws a SocketException
                    for (int i = 1; i < 65536; i++)
                    {
                        if (!mUsedPorts.Contains(i))
                        {
                            port = i;
                            mUsedPorts.Add(i);
                            break;
                        }
                    }
                    if (port == 0)
                    {
                        throw new SocketException((int)SocketError.TooManyOpenSockets);
                    }
                }
                else
                {
                    // trying to bind to the requested port. It it is unsuccessful, throws a SocketException.
                    if (mUsedPorts.Contains(port))
                    {
                        throw new SocketException((int)SocketError.AddressAlreadyInUse);
                    }
                    else
                    {
                        mUsedPorts.Add(port);
                    }
                }
            }

            // listening on that port. It is good for both TCP and UDP.
            mLocalEndPointInternal = new AddressEndPoint(NetworkManager.Instance.InternalLocalhost.Id, port);
            mLocalEndPoint = mLocalEndPointInternal;
            NetworkManager.Instance.InternalSocketRegister(this);
        }

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        public async Task ConnectAsync(EndPoint endPoint)
        {
            await Task.Run(() => Connect(endPoint));
        }

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public async Task ConnectAsync(string host, int port)
        {
            await Task.Run(() => Connect(host, port));
        }

#endif

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <exception cref="System.InvalidOperationException">Connect method used for connection-oriented sockets, not for datagrams.</exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)")]
        public void Connect(EndPoint endPoint)
        {
            DoDisposeCheck();
            DoListenCheck(); // no binding allowed
            DoConnectedRemoteCheck(); // connected state is not allowed
            DoShutdownReceiveCheck();
            DoShutdownSendCheck();

            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            if (!((endPoint is AddressEndPoint) || (endPoint is DnsEndPoint)))
            {
                ThrowHelper.ThrowArgumentException(string.Format("Supported EndPoint types: {0} and {1}", typeof(AddressEndPoint).FullName, typeof(DnsEndPoint).FullName), "endPoint");
            }
            // only TCP
            if (mSocketType == SocketType.Dgram)
            {
                throw new InvalidOperationException("Connect method used for connection-oriented sockets, not for datagrams.");
            }

            AddressEndPoint ep = endPoint as AddressEndPoint;
            if (ep == null)
            {
                DnsEndPoint dnsEp = (DnsEndPoint)endPoint;
                ep = new AddressEndPoint(dnsEp.Host, dnsEp.Port);
            }

            if (ep.Host.ToLower().Equals("localhost") || ep.Host.ToLower().Equals("127.0.0.1"))
            {
                ep = new AddressEndPoint(NetworkManager.Instance.InternalLocalhost.Id, ep.Port);
            }

            mSemaphoreConnection.WaitOne();
            try
            {
                lock (mConnectLockObject)
                {
                    string netContext = string.Empty;
                    string peerStr = string.Empty;
                    string[] tags = ep.Host.Split(new string[] { @"\" }, StringSplitOptions.None);
                    if (tags.Length > 2)
                    {
                        ThrowHelper.ThrowArgumentException("Invalid host name.", "endPoint.Host");
                    }
                    if (tags.Length == 1)
                    {
                        peerStr = tags[0];
                    }
                    else
                    {
                        netContext = tags[0];
                        peerStr = tags[1];
                    }

                    if (string.IsNullOrEmpty(netContext))
                    {
                        // first I try to resolve the client by ID
                        mRemoteNetworkPeer = NetworkManager.Instance.InternalLocalhost.Id.Equals(peerStr) ? NetworkManager.Instance.InternalLocalhost : (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(peerStr);
                        if (mRemoteNetworkPeer == null)
                        {
                            // if it is not found, than trying based on host name
                            mRemoteNetworkPeer = (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerByName(peerStr);
                        }
                    }
                    else
                    {
                        // it is context definied
                        NetworkContext c = NetworkContext.GetNetworkContextByName(netContext);
                        if (c == null)
                        {
                            throw new SocketException((int)SocketError.NetworkUnreachable);
                        }
                        if (NetworkManager.Instance.InternalLocalhost.NetworkContext.Name.Equals(c.Name))
                        {
                            mRemoteNetworkPeer = NetworkManager.Instance.InternalLocalhost.Id.Equals(peerStr) ? NetworkManager.Instance.InternalLocalhost : c.GetNetworkPeerById(peerStr) as NetworkPeerRemote;
                        }
                        else
                        {
                            mRemoteNetworkPeer = c.GetNetworkPeerById(peerStr) as NetworkPeerRemote;
                        }
                    }
                    if (mRemoteNetworkPeer == null)
                    {
                        throw new SocketException((int)SocketError.HostNotFound);
                    }

                    if (!NetworkManager.Instance.InternalLocalhost.Id.Equals(mRemoteNetworkPeer.Id))
                    {
                        // ezeket csak akkor vizsgálom, ha nem a localhost-ra csatlakozom
                        if (mRemoteNetworkPeer.Distance != 1)
                        {
                            // megpróbálunk közvetlen kapcsolatot felépíteni
                            if (mRemoteNetworkPeer.Distance == 0)
                            {
                                // nincs kapcsolat, várunk a kapcsolódásra
                                NetworkManager.Instance.InternalConnect(mRemoteNetworkPeer, true, !NetworkManager.Instance.InternalConfiguration.Settings.EnableMultipleConnectionWithNetworkPeers);
                            }
                            else
                            {
                                // agresszív kapcsolatépítés engedélyezve?
                                if (NetworkManager.Instance.InternalConfiguration.Settings.EnableAgressiveConnectionEstablishment)
                                {
                                    // van kapcsolat, háttérben próbálunk közvetlen átjárót nyitni
                                    NetworkManager.Instance.InternalConnectAsync(mRemoteNetworkPeer, true, !NetworkManager.Instance.InternalConfiguration.Settings.EnableMultipleConnectionWithNetworkPeers);
                                }
                            }
                        }
                        if (mRemoteNetworkPeer.Distance == 0)
                        {
                            // teljesen elérhetetlen a távoli kliens
                            mRemoteNetworkPeer = null;
                            throw new SocketException((int)SocketError.HostUnreachable);
                        }
                    }

                    // meg van a peer, küldeni kell neki a csatorna felépítési üzenetet. A port szám fentebb adott.
                    // ehhez allokálni kell egy helyi portot és elküldeni neki. Az üzeneteket ide kell majd címeznie.
                    // A válaszüzenetben vagy elutasítás lesz (ConnectionRefused) vagy a port szám, amire pedig majd nekem kell
                    // az üzeneteket küldenem.

                    try
                    {
                        if (!IsBound)
                        {
                            Bind(new AddressEndPoint(AddressEndPoint.Any, 0)); // helyi port kérése
                        }
                    }
                    catch (Exception)
                    {
                        mRemoteNetworkPeer = null;
                        throw;
                    }

                    NetworkManager.Instance.NetworkPeerUnaccessible += new EventHandler<NetworkPeerChangedEventArgs>(NetworkPeerUnaccessibleHandler);
                    SocketOpenRequestMessage message = new SocketOpenRequestMessage(NetworkManager.Instance.InternalLocalhost.Id,
                        mRemoteNetworkPeer.Id, mLocalEndPointInternal.Port, ep.Port, mLocalSocketId);
                    NetworkManager.Instance.InternalSendMessage(message, null);
                    if (mConnectEvent.WaitOne(NetworkManager.Instance.InternalConfiguration.Settings.DefaultConnectionTimeoutInMS))
                    {
                        // kaptam visszajelzést
                        if (mClosedLevel > 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        if (mRemoteEndPointInternal == null)
                        {
                            // nem sikerült a csatlakozás
                            mRemoteNetworkPeer = null;
                            NetworkManager.Instance.InternalSocketUnregister(this);
                            NetworkManager.Instance.NetworkPeerUnaccessible -= new EventHandler<NetworkPeerChangedEventArgs>(NetworkPeerUnaccessibleHandler);
                            throw new SocketException((int)SocketError.ConnectionRefused);
                        }
                        mRemoteEndPoint = ep;
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), socket connected with remote peer '{1}'.", mLocalSocketId.ToString(), mRemoteNetworkPeer.Id));
                    }
                    else
                    {
                        // nem kaptam visszajelzést (timeout)
                        if (mClosedLevel > 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        NetworkManager.Instance.InternalSocketUnregister(this);
                        mRemoteEndPointInternal = null;
                        mRemoteSocketId = -1;
                        NetworkManager.Instance.NetworkPeerUnaccessible -= new EventHandler<NetworkPeerChangedEventArgs>(NetworkPeerUnaccessibleHandler);
                        throw new SocketException((int)SocketError.TimedOut);
                    }
                }
            }
            finally
            {
                mSemaphoreConnection.Release();
            }
        }

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public void Connect(string host, int port)
        {
            Connect(new AddressEndPoint(host, port));
        }

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Receives the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Number of received bytes</returns>
        public async Task<int> ReceiveAsync(byte[] buffer)
        {
            return await Task.Run(() => Receive(buffer, 0, buffer.Length));
        }

        /// <summary>
        /// Receives the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <returns>Number of received bytes</returns>
        public async Task<int> ReceiveAsync(byte[] buffer, int offset, int size)
        {
            return await Task.Run(() => Receive(buffer, offset, size));
        }

#endif

        /// <summary>
        /// Receives the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>
        /// Number of received bytes
        /// </returns>
        public int Receive(byte[] buffer)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return Receive(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Receives the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        /// Number of received bytes
        /// </returns>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// </exception>
        /// <exception cref="System.InvalidOperationException">Socket not connected.</exception>
        /// <exception cref="System.TimeoutException">Receive timed out.</exception>
        public int Receive(byte[] buffer, int offset, int size)
        {
            if (mClosedLevel == 2)
            {
                throw new SocketException((int)SocketError.Shutdown);
            }
            if (mRemoteNetworkPeer == null)
            {
                throw new InvalidOperationException("Socket not connected.");
            }
            DoBindCheck(); // legyen bindolva
            DoListenCheck(); // ne figyeljen
            DoShutdownReceiveCheck();
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if ((offset < 0) || (offset > buffer.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("offset");
            }
            if ((size < 0) || (size > (buffer.Length - offset)))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("size");
            }
            if (size == 0)
            {
                return 0;
            }

            int receivedBytes = 0;
            bool exit = false;

            while (!exit)
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), receive data. Buffer size: {1}, offset: {2}, size: {3}. Need: {4}, received: {5}", mLocalSocketId.ToString(), buffer.Length.ToString(), offset.ToString(), size.ToString(), size.ToString(), receivedBytes.ToString()));
                if (mClosedLevel > 0 || mReceiveEvent.WaitOne(mReceiveTimeout))
                {
                    // ide akkor futunk be:
                    // 1, ha a sokcet-et lezárták
                    // 2, ha jelzést kaptunk, hogy van adat
                    DoShutdownReceiveCheck();
                    if (mClosedLevel == 2)
                    {
                        throw new SocketException((int)SocketError.Interrupted);
                    }
                    else
                    {
                        // ide csak a nyitott vagy a túloldal által zárt socket-eken lehet bejutni és kiolvasni a buffert
                        lock (mDisposeLock)
                        {
                            if (mClosedLevel == 2)
                            {
                                throw new SocketException((int)SocketError.Interrupted);
                            }
                            DoShutdownReceiveCheck();
                            if (mClosedLevel > 0)
                            {
                                throw new SocketException((int)SocketError.Interrupted);
                            }
                            IEnumeratorSpecialized<ReceivedMessage> specEnum = mReceivedMessages.GetEnumerator();
                            while (specEnum.MoveNext())
                            {
                                ReceivedMessage m = specEnum.Current;
                                if (mRemotePacketOrderNumber == m.Message.PacketOrderNumber)
                                {
                                    // ez a sorszámú üzenet jön
                                    int availableBytesInMessage = m.Message.Data.Length - m.SentBytes; // ennyi tudok még kiolvasni ebből az üzenetből
                                    int freeSpaceBytes = size - receivedBytes; // összességében még ennyi adat fér el a buffer-ben
                                    if (availableBytesInMessage <= freeSpaceBytes)
                                    {
                                        // a, több helyem van, mint amit a message adni tud (a teljes message tartalmát viszem)
                                        // b, az üzenet pont kitölti a szabad helyet
                                        Array.Copy(m.Message.Data, m.SentBytes, buffer, offset + receivedBytes, m.Message.Data.Length - m.SentBytes);
                                        receivedBytes += (m.Message.Data.Length - m.SentBytes);
                                        specEnum.Remove();
                                        mRemotePacketOrderNumber++; // következő várt azonosítójú csomag beállítása
                                        if (!m.Acknowledged)
                                        {
                                            // megerősítő üzenet küldése
                                            //LOGGER.Debug(string.Format("ACK ({0})--------------------------------------- {1}", GetHashCode().ToString(), m.Message.MessageId));
                                            SendAcknowledgeMessage(m.Message.SenderId, m.Message.MessageId);
                                        }
                                        if (receivedBytes == size)
                                        {
                                            if (Available > 0)
                                            {
                                                mReceiveEvent.Set(); // de van még mit olvasni
                                            }
                                            //LOGGER.Debug(string.Format("BUFFER IS FULL ({0})--------------------------------------- {1}", GetHashCode().ToString(), m.Message.MessageId));
                                            exit = true;
                                            break; // megtelt a buffer, véget ért az olvasás
                                        }
                                        else if (Available == 0)
                                        {
                                            exit = true;
                                            break; // nincs már mit tovább olvasni
                                        }
                                    }
                                    else
                                    {
                                        // az üzenetben több adat van, mint amennyi helyem a buffer-ben
                                        Array.Copy(m.Message.Data, m.SentBytes, buffer, offset + receivedBytes, size - receivedBytes);
                                        m.SentBytes += (size - receivedBytes);
                                        receivedBytes = size; // megtelt a buffer
                                        mReceiveEvent.Set();
                                        //LOGGER.Debug(string.Format("BUFFER IS FULL, DATA STILL AVB ({0})--------------------------------------- {1}", GetHashCode().ToString(), m.Message.MessageId));
                                        exit = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    // nem ennek a sorszámú üzenetnek kellene lennie a buffer-ben, véget ért az olvasás
                                    //LOGGER.Debug(string.Format("BAD REMOTE PACKET NUMBER ({0})--------------------------------------- {1}", GetHashCode().ToString(), m.Message.MessageId));
                                    if (receivedBytes > 0)
                                    {
                                        exit = true;
                                    }
                                    break;
                                }
                            }
                            if (receivedBytes == 0)
                            {
                                // ez akkor van, ha lezáródik a socket. Ilyenkor engedünk olvasni.
                                exit = true;
                            }
                        }
                    }
                }
                else
                {
                    throw new TimeoutException("Receive timed out.");
                }
            }

            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), end receive data. Buffer size: {1}, offset: {2}, size: {3}. Need: {4}, received: {5}", mLocalSocketId.ToString(), buffer.Length.ToString(), offset.ToString(), size.ToString(), size.ToString(), receivedBytes.ToString()));

            return receivedBytes;
        }

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Receives from.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        public async Task<(int, System.Net.EndPoint)> ReceiveFromAsync(byte[] buffer, System.Net.EndPoint remoteEp)
        {
            return (await Task.Run(() => ReceiveFrom(buffer, ref remoteEp)), remoteEp);
        }

        /// <summary>
        /// Receives from.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        public async Task<(int, System.Net.EndPoint)> ReceiveFromAsync(byte[] buffer, int offset, int size, System.Net.EndPoint remoteEp)
        {
            return (await Task.Run(() => ReceiveFrom(buffer, offset, size, ref remoteEp)), remoteEp);
        }

#endif

        /// <summary>
        /// Receives from.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>
        /// Number of received bytes
        /// </returns>
        public int ReceiveFrom(byte[] buffer, ref EndPoint remoteEp)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return ReceiveFrom(buffer, 0, buffer.Length, ref remoteEp);
        }

        /// <summary>
        /// Receives from.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>
        /// Number of received bytes
        /// </returns>
        /// <exception cref="System.InvalidOperationException">This socket is not connection-less oriented type (UDP).</exception>
        /// <exception cref="System.Net.Sockets.SocketException"></exception>
        /// <exception cref="System.TimeoutException">Receive timed out.</exception>
        public int ReceiveFrom(byte[] buffer, int offset, int size, ref EndPoint remoteEp)
        {
            DoDisposeCheck();
            DoListenCheck(); // ne figyeljen
            DoShutdownReceiveCheck();
            if (remoteEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("remoteEp");
            }
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if (size == 0)
            {
                return 0;
            }
            if ((offset < 0) || (offset > buffer.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("offset");
            }
            if ((size < 0) || (size > (buffer.Length - offset)))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("size");
            }
            if (mProtocolType != System.Net.Sockets.ProtocolType.Udp)
            {
                throw new InvalidOperationException("This socket is not connection-less oriented type (UDP).");
            }

            int receivedBytes = 0;

            if (mClosedLevel > 0 || mReceiveEvent.WaitOne(mReceiveTimeout))
            {
                // ide akkor jövök be:
                // 1, ha a socketet lezárták (dispose vagy closed)
                // 2, ha jelzést kaptam, hogy van adat
                DoShutdownReceiveCheck();
                if (mClosedLevel == 2)
                {
                    // le van dispose-olve a socket
                    throw new SocketException((int)SocketError.Interrupted);
                }
                else
                {
                    // ide csak a nyitott vagy a túloldal által zárt socket-eken lehet bejutni és kiolvasni a buffert (mClosedLevel == 1)
                    lock (mDisposeLock)
                    {
                        if (mClosedLevel == 2)
                        {
                            // le van dispose-olve a socket
                            throw new SocketException((int)SocketError.Interrupted);
                        }

                        IEnumeratorSpecialized<ReceivedMessage> specEnum = mReceivedMessages.GetEnumerator();
                        while (specEnum.MoveNext())
                        {
                            ReceivedMessage m = specEnum.Current;
                            int availableBytesInMessage = m.Message.Data.Length - m.SentBytes; // ennyi tudok még kiolvasni ebből az üzenetből
                            int freeSpaceBytes = size - receivedBytes; // összességében még ennyi adat fér el a buffer-ben
                            if (availableBytesInMessage <= freeSpaceBytes)
                            {
                                // a, több helyem van, mint amit a message adni tud (a teljes message tartalmát viszem)
                                // b, az üzenet pont kitölti a szabad helyet
                                Array.Copy(m.Message.Data, m.SentBytes, buffer, offset + receivedBytes, m.Message.Data.Length - m.SentBytes);
                                receivedBytes += (m.Message.Data.Length - m.SentBytes);
                                specEnum.Remove();
                                remoteEp = new AddressEndPoint(m.Message.SenderId, m.Message.SenderPort); // beállítom kitől jött az üzenet
                                break;
                            }
                            else
                            {
                                // az üzenetben több adat van, mint amennyi helyem a buffer-ben (a maradék adatot kidobom)
                                Array.Copy(m.Message.Data, m.SentBytes, buffer, offset + receivedBytes, size - receivedBytes);
                                m.SentBytes += (size - receivedBytes);
                                receivedBytes = size; // megtelt a buffer
                                specEnum.Remove();
                                remoteEp = new AddressEndPoint(m.Message.SenderId, m.Message.SenderPort); // beállítom kitől jött az üzenet
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                throw new TimeoutException("Receive timed out.");
            }

            return receivedBytes;
        }

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendAsync(byte[] buffer)
        {
            return await Task.Run(() => Send(buffer, 0, buffer.Length));
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendAsync(byte[] buffer, int offset, int size)
        {
            return await Task.Run(() => Send(buffer, offset, size));
        }

#endif

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        public int Send(byte[] buffer)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return Send(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// </exception>
        /// <exception cref="System.TimeoutException">
        /// Send timed out.
        /// or
        /// Data sending timed out.
        /// or
        /// Send timed out.
        /// or
        /// Send timed out.
        /// </exception>
        public int Send(byte[] buffer, int offset, int size)
        {
            DoDisposeCheck();
            DoBindCheck(); // legyen bindolva
            DoShutdownSendCheck();

            if (EnableBroadcast && mProtocolType == System.Net.Sockets.ProtocolType.Udp)
            {
                // kivételes eset, .NET-el való kompatibilitás tartására, UDP broadcast üzenet küldhető
                return SendTo(buffer, offset, size, new AddressEndPoint(AddressEndPoint.Broadcast, mLocalEndPointInternal.Port));
            }

            DoConnectedRemoteNotCheck(); // csatlakoztatva kell lennie
            DoListenCheck(); // ne figyeljen
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if (size == 0)
            {
                return 0;
            }
            if ((offset < 0) || (offset > buffer.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("offset");
            }
            if ((size < 0) || (size > (buffer.Length - offset)))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("size");
            }

            int confirmedBytes = 0;

            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), send data. Buffer size: {1}, offset: {2}, size: {3}. Need: {4}", mLocalSocketId.ToString(), buffer.Length.ToString(), offset.ToString(), size.ToString(), size.ToString()));

                if (mSendEvent.WaitOne(mSendTimeout))
                {
                    lock (mSendCloseLockObject)
                    {
                        DoShutdownSendCheck();
                        DoConnectedRemoteNotCheck(); // ellenőrzöm, meg van-e még a kapcsolatom
                        if (mClosedLevel == 1)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        DoDisposeCheck();
                        mSentEventForMessages.Reset();
                    }

                    List<MessageTask> slots = new List<MessageTask>();
                    try
                    {
                        if ((watch.ElapsedMilliseconds > mSendTimeout) && (mSendTimeout > 0))
                        {
                            throw new TimeoutException("Send timed out.");
                        }
                        {
                            // kezdő üzenet. Itt tudom lemérni a válaszidőből, hogy egyszerre hány üzenetet szabad a hálózatba küldeni.
                            byte[] data = buffer;
                            int bufferSize = SendBufferSize;
                            if (size > bufferSize || offset > 0)
                            {
                                data = new byte[bufferSize];
                                Array.Copy(buffer, offset, data, 0, bufferSize);
                            }
                            else if (size < data.Length)
                            {
                                data = new byte[size];
                                Array.Copy(buffer, offset, data, 0, size);
                            }
                            SocketRawDataMessage initMessage = new SocketRawDataMessage(NetworkManager.Instance.InternalLocalhost.Id,
                                mRemoteEndPointInternal.Host, mLocalEndPointInternal.Port, mRemoteEndPointInternal.Port,
                                mLocalSocketId, mRemoteSocketId, mLocalPacketOrderNumber, data);
                            slots.Add(NetworkManager.Instance.InternalSendMessage(initMessage, null, mSentEventForMessages));
                            mLocalPacketOrderNumber++;
                        }
                        // kiszámolom mennyi időm van még
                        int availableTime = mSendTimeout - Convert.ToInt32(watch.ElapsedMilliseconds);
                        if (availableTime < 0)
                        {
                            availableTime = 0;
                        }
                        bool decision = false;
                        lock (mSendCloseLockObject)
                        {
                            DoShutdownSendCheck();
                            DoConnectedRemoteNotCheck(); // ellenőrzöm, meg van-e még a kapcsolatom
                            if (mClosedLevel == 1)
                            {
                                throw new SocketException((int)SocketError.Shutdown);
                            }
                            DoDisposeCheck();
                            decision = mSendTimeout <= 0 || (mClosedLevel == 0 && mSentEventForMessages.WaitOne(availableTime));
                        }
                        if (decision)
                        {
                            const int unitTime = 1000;
                            int sentBytes = ((SocketRawDataMessage)slots[0].Message).Data.Length;
                            while (Connected && confirmedBytes < size)
                            {
                                // ide csak akkor futunk, ha csatlakoztatva vagyunk és még nem küldünk el mindent
                                // vagy pedig a függőben lévő csomagok közül még nincs mindegyik visszaigazolva
                                availableTime = (mSendTimeout == 0 || mSendTimeout.Equals(Timeout.Infinite)) ? Timeout.Infinite : mSendTimeout - Convert.ToInt32(watch.ElapsedMilliseconds);
                                if (availableTime < 0)
                                {
                                    availableTime = 0;
                                }
                                if (mSendTimeout == 0 || mSendTimeout.Equals(Timeout.Infinite))
                                {
                                    // végtelen timeout-ra várunk
                                    mSentEventForMessages.WaitOne();
                                }
                                else if (availableTime > 0)
                                {
                                    // a hátra évő ideig várunk max
                                    if (!mSentEventForMessages.WaitOne(availableTime))
                                    {
                                        availableTime = 0;
                                    }
                                }
                                if (!(mSendTimeout == 0 || mSendTimeout.Equals(Timeout.Infinite)) && availableTime <= 0)
                                {
                                    DoShutdownSendCheck();
                                    DoConnectedRemoteNotCheck(); // ellenőrzöm, meg van-e még a kapcsolatom
                                    if (mClosedLevel == 1)
                                    {
                                        throw new SocketException((int)SocketError.Shutdown);
                                    }
                                    DoDisposeCheck();
                                    // nincs már több idő vagy timeout lett a vége
                                    throw new TimeoutException("Data sending timed out.");
                                }
                                else
                                {
                                    lock (mSendCloseLockObject)
                                    {
                                        DoShutdownSendCheck();
                                        DoConnectedRemoteNotCheck(); // ellenőrzöm, meg van-e még a kapcsolatom
                                        if (mClosedLevel == 1)
                                        {
                                            throw new SocketException((int)SocketError.Shutdown);
                                        }
                                        DoDisposeCheck();
                                        mSentEventForMessages.Reset();
                                    }

                                    long replyTime = 0;
                                    bool allConfirmed = true;

                                    for (int index = 0; index < slots.Count; index++)
                                    {
                                        MessageTask task = slots[index];
                                        if (task != null)
                                        {
                                            if (task.IsSuccess)
                                            {
                                                // csomag visszaigazolva
                                                if (task.ReplyTime > replyTime)
                                                {
                                                    replyTime = task.ReplyTime;
                                                }
                                                confirmedBytes += ((SocketRawDataMessage)task.Message).Data.Length; // ennyit igazoltak vissza
                                                // új adatcsomag indítása
                                                if (Connected && sentBytes < size)
                                                {
                                                    int bufferSize = SendBufferSize;
                                                    int leftBytes = size - sentBytes; // ennyit kell még elküldenem
                                                    byte[] data = new byte[leftBytes > bufferSize ? bufferSize : leftBytes];
                                                    Array.Copy(buffer, offset + sentBytes, data, 0, data.Length);
                                                    SocketRawDataMessage initMessage = new SocketRawDataMessage(NetworkManager.Instance.InternalLocalhost.Id,
                                                        mRemoteEndPointInternal.Host, mLocalEndPointInternal.Port, mRemoteEndPointInternal.Port,
                                                        mLocalSocketId, mRemoteSocketId, mLocalPacketOrderNumber, data);
                                                    slots[index] = NetworkManager.Instance.InternalSendMessage(initMessage, null, mSentEventForMessages);
                                                    mLocalPacketOrderNumber++;
                                                    sentBytes += data.Length; // ennyit küldtem el
                                                }
                                                else
                                                {
                                                    slots[index] = null;
                                                }
                                            }
                                            else
                                            {
                                                allConfirmed = false;
                                            }
                                        }
                                    }

                                    if (allConfirmed && Connected && (sentBytes < size) && (replyTime < unitTime))
                                    {
                                        // lehet még további slotokat indítani
                                        if (replyTime <= 0)
                                        {
                                            replyTime = 1;
                                        }
                                        int count = 1 + Convert.ToInt32(Math.Round(Convert.ToDouble(unitTime / replyTime)));
                                        if (count > 5)
                                        {
                                            count = 5;
                                        }
                                        for (int i = 0; i < count; i++)
                                        {
                                            if (Connected && sentBytes < size)
                                            {
                                                int bufferSize = SendBufferSize;
                                                int leftBytes = size - sentBytes; // ennyit kell még elküldenem
                                                byte[] data = new byte[leftBytes > bufferSize ? bufferSize : leftBytes];
                                                Array.Copy(buffer, offset + sentBytes, data, 0, data.Length);
                                                SocketRawDataMessage initMessage = new SocketRawDataMessage(NetworkManager.Instance.InternalLocalhost.Id,
                                                    mRemoteEndPointInternal.Host, mLocalEndPointInternal.Port, mRemoteEndPointInternal.Port,
                                                    mLocalSocketId, mRemoteSocketId, mLocalPacketOrderNumber, data);
                                                slots.Add(NetworkManager.Instance.InternalSendMessage(initMessage, null, mSentEventForMessages));
                                                mLocalPacketOrderNumber++;
                                                sentBytes += data.Length; // ennyit küldtem el
                                            }
                                            else
                                            {
                                                // megszűnt a kapcsolat vagy nincs már mit küldenem
                                                break;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            // timeout
                            throw new TimeoutException("Send timed out.");
                        }
                    }
                    finally
                    {
                        slots.ForEach(task => { if (task != null && task.TimeWatch != null && task.TimeWatch.IsRunning) task.TimeWatch.Stop(); });
                        slots.Clear();
                        try
                        {
                            mSendEvent.Set();
                        }
                        catch (Exception) { }
                    }
                }
                else
                {
                    throw new TimeoutException("Send timed out.");
                }
            }
            finally
            {
                watch.Stop();
            }

            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), send data. Buffer size: {1}, offset: {2}, size: {3}. Need: {4}, confirmed: {5}.", mLocalSocketId.ToString(), buffer.Length.ToString(), offset.ToString(), size.ToString(), size.ToString(), confirmedBytes.ToString()));

            return confirmedBytes;
        }

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Sends to.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendToAsync(byte[] buffer, System.Net.EndPoint remoteEp)
        {
            return await Task.Run(() => SendTo(buffer, remoteEp));
        }

        /// <summary>
        /// Sends to.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendToAsync(byte[] buffer, int offset, int size, System.Net.EndPoint remoteEp)
        {
            return await Task.Run(() => SendTo(buffer, offset, size, remoteEp));
        }

#endif

        /// <summary>
        /// Sends to.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        public int SendTo(byte[] buffer, EndPoint remoteEp)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return SendTo(buffer, 0, buffer.Length, remoteEp);
        }

        /// <summary>
        /// Sends to.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// This socket is not connection-less oriented type (UDP).
        /// or
        /// This socket instance cannot send or receive broadcast packets.
        /// or
        /// This socket instance sends or receives broadcast packets.
        /// </exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.TimeoutException">
        /// Send timed out.
        /// or
        /// Data sending timed out.
        /// or
        /// Send timed out.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// </exception>
        public int SendTo(byte[] buffer, int offset, int size, EndPoint remoteEp)
        {
            DoDisposeCheck(); // ne legyen kidobva
            DoListenCheck(); // ne figyeljen
            DoBindCheck(); // legyen bindolva
            DoShutdownSendCheck();
            if (remoteEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("remoteEp");
            }
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if (size == 0)
            {
                return 0;
            }
            if ((offset < 0) || (offset > buffer.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("offset");
            }
            if ((size < 0) || (size > (buffer.Length - offset)))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("size");
            }
            if (mProtocolType != System.Net.Sockets.ProtocolType.Udp)
            {
                throw new InvalidOperationException("This socket is not connection-less oriented type (UDP).");
            }

            AddressEndPoint ep = null;
            if (remoteEp is AddressEndPoint)
            {
                ep = (AddressEndPoint)remoteEp;
            }
            else if (remoteEp is DnsEndPoint)
            {
                DnsEndPoint dnsEp = (DnsEndPoint)remoteEp;
                ep = new AddressEndPoint(dnsEp.Host, dnsEp.Port);
            }
            else
            {
                throw new NotSupportedException(string.Format("Endpoint type '{0}' not supported.", ep.GetType().FullName));
            }

            if (ep.Port < 1)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("port");
            }

            if (!EnableBroadcast && ep.Host.Equals(AddressEndPoint.Broadcast))
            {
                throw new InvalidOperationException("This socket instance cannot send or receive broadcast packets.");
            }
            if (EnableBroadcast && !ep.Host.Equals(AddressEndPoint.Broadcast))
            {
                throw new InvalidOperationException("This socket instance sends or receives broadcast packets.");
            }

            int sentBytes = 0;

            using (AutoResetEvent sentEvent = new AutoResetEvent(false))
            {
                if (ep.Host.Equals(AddressEndPoint.Broadcast))
                {
                    // UDP Broadcast üzenet
                    SocketRawDataMessage broadCastMsg = new SocketRawDataMessage(NetworkManager.Instance.InternalLocalhost.Id, mLocalEndPointInternal.Port, ep.Port, buffer);
                    NetworkManager.Instance.InternalSendMessage(broadCastMsg, null, sentEvent);
                    if (!sentEvent.WaitOne(SendTimeout))
                    {
                        throw new TimeoutException("Send timed out.");
                    }
                    sentBytes = buffer.Length;
                }
                else
                {
                    // UDP üzenet
                    INetworkPeerRemote networkPeer = NetworkManager.Instance.InternalLocalhost.Id.Equals(ep.Host) ? NetworkManager.Instance.InternalLocalhost : NetworkPeerContext.GetNetworkPeerById(ep.Host);
                    if (networkPeer == null)
                    {
                        throw new SocketException((int)SocketError.HostNotFound);
                    }

                    Stopwatch watch = Stopwatch.StartNew();
                    try
                    {
                        if (mSendEvent.WaitOne(mSendTimeout))
                        {
                            DoShutdownSendCheck();
                            if (mClosedLevel > 0)
                            {
                                throw new SocketException((int)SocketError.Shutdown);
                            }
                            DoBindCheck(); // legyen bindolva
                            try
                            {
                                while (sentBytes < size)
                                {
                                    DoShutdownSendCheck();
                                    if (mClosedLevel > 0)
                                    {
                                        throw new SocketException((int)SocketError.Shutdown);
                                    }

                                    int availableTime = (mSendTimeout == 0 || mSendTimeout.Equals(Timeout.Infinite)) ? Timeout.Infinite : mSendTimeout - Convert.ToInt32(watch.ElapsedMilliseconds);
                                    if (availableTime < 0)
                                    {
                                        availableTime = 0;
                                    }
                                    if ((!(mSendTimeout == 0 || mSendTimeout.Equals(Timeout.Infinite)) && availableTime <= 0))
                                    {
                                        // nincs már több idő vagy timeout lett a vége
                                        throw new TimeoutException("Data sending timed out.");
                                    }

                                    int bufferSize = SendBufferSize;
                                    int leftBytes = size - sentBytes; // ennyit kell még elküldenem
                                    byte[] data = new byte[leftBytes > bufferSize ? bufferSize : leftBytes];
                                    Array.Copy(buffer, offset + sentBytes, data, 0, data.Length);

                                    SocketRawDataMessage broadCastMsg = new SocketRawDataMessage(NetworkManager.Instance.InternalLocalhost.Id, networkPeer.Id, mLocalEndPointInternal.Port, ep.Port, data);
                                    NetworkManager.Instance.InternalSendMessage(broadCastMsg, null, sentEvent);
                                    sentBytes += data.Length; // hozzáadom, amit elküldtem
                                }
                            }
                            finally
                            {
                                try
                                {
                                    mSendEvent.Set();
                                }
                                catch (Exception) { }
                            }
                        }
                        else
                        {
                            throw new TimeoutException("Send timed out.");
                        }
                    }
                    finally
                    {
                        watch.Stop();
                    }
                }
            }

            return sentBytes;
        }

        /// <summary>
        /// Listens the specified backlog.
        /// </summary>
        /// <param name="backlog">The maximum size of the connection request queue.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2002:DoNotLockOnObjectsWithWeakIdentity")]
        public void Listen(int backlog)
        {
            DoDisposeCheck(); // nem lehet kidobva
            lock (mDisposeLock)
            {
                DoDisposeCheck(); // nem lehet kidobva
                DoListenCheck(); // ne figyeljen
                DoConnectedRemoteCheck(); // ne legyen csatlakoztatva
                DoShutdownReceiveCheck();
                DoShutdownSendCheck();

                if (!mListening)
                {
                    lock (mListenLock)
                    {
                        if (!mListening)
                        {
                            mListenerQueue = new Queue<IncomingConnection>(backlog < 1 ? NetworkManager.Instance.InternalConfiguration.Settings.DefaultSocketBacklogSize : backlog);
                            mSemaphoreListener = new Semaphore(0, int.MaxValue);

                            if (backlog > 0)
                            {
                                mBacklog = backlog;
                            }

                            // ez a lock az ismételt behívás elkerülésére szolgál
                            if (mThreadServerSocketAcceptTimeoutListener == null)
                            {
                                lock (typeof(Socket))
                                {
                                    // statikus változók, egy darab timeout figyelő thread van
                                    if (mThreadServerSocketAcceptTimeoutListener == null)
                                    {
                                        mServerSockets = new HashSet<Socket>();
                                        mSemaphoreServerSockets = new Semaphore(0, int.MaxValue);

                                        mThreadServerSocketAcceptTimeoutListener = new Thread(new ThreadStart(
                                            delegate()
                                            {

                                                #region Timeout listener

                                                while (true)
                                                {
                                                    mSemaphoreServerSockets.WaitOne();
                                                    Socket selectedSocket = null;
                                                    int smallestValue = Int32.MaxValue;
                                                    IncomingConnection connection = null;

                                                    lock (mServerSockets)
                                                    {
                                                        foreach (Socket socket in mServerSockets)
                                                        {
                                                            int value = 0;
                                                            IncomingConnection c = null;
                                                            if (socket.CheckForSocketOpenRequestTimeout(out value, out c))
                                                            {
                                                                if (value < smallestValue)
                                                                {
                                                                    selectedSocket = socket;
                                                                    smallestValue = value;
                                                                    connection = c;
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (selectedSocket != null)
                                                    {
                                                        // megtaláltuk a leghamarabb lejáró csatlakozási kérést, várunk
                                                        connection.WaitForAcceptOrTimeout(smallestValue);
                                                    }

                                                    Thread.Sleep(0);
                                                }

                                                #endregion

                                            }));
                                        mThreadServerSocketAcceptTimeoutListener.IsBackground = true;
                                        mThreadServerSocketAcceptTimeoutListener.Name = "TerraGraf_ServerSocketAcceptTimeoutListenerThread";
                                        mThreadServerSocketAcceptTimeoutListener.Start();
                                    }
                                }
                            }

                            lock (mServerSockets)
                            {
                                mServerSockets.Add(this);
                            }

                            mListening = true;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Detect the status of the socket
        /// </summary>
        /// <param name="microSeconds">The micro seconds.</param>
        /// <param name="selectMode">The select mode.</param>
        /// <returns>
        /// True, if select mode state is valid
        /// </returns>
        public bool Pool(int microSeconds, SelectMode selectMode)
        {
            bool result = false;

            switch (selectMode)
            {
                case SelectMode.SelectError:
                    break;

                case SelectMode.SelectRead:
                    {
                        if (mListening && mListenerQueue.Count > 0)
                        {
                            result = true;
                        }
                        else if (Available > 0)
                        {
                            result = true;
                        }
                        else if (mClosedLevel > 0)
                        {
                            result = true;
                        }
                    }
                    break;

                case SelectMode.SelectWrite:
                    {
                        if (Connected)
                        {
                            result = true;
                        }
                        else if (mLocalEndPointInternal != null)
                        {
                            result = true;
                        }
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Sets the keep alive values.
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <param name="keepAliveTime">The keep alive time.</param>
        /// <param name="keepAliveInterval">The keep alive interval.</param>
        /// <returns>
        /// value
        /// </returns>
        public int SetKeepAliveValues(bool state, int keepAliveTime, int keepAliveInterval)
        {
            return 0;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Close the socket and wait for the specified time.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void Close(int timeout)
        {
            if (timeout > 1 && timeout < 500)
            {
                mCloseTimeout = 500;
            }
            else if (timeout >= 500)
            {
                mCloseTimeout = timeout;
            }
            if (Connected)
            {
                if (mCloseTimeout > 0)
                {
                    // üzenet küldés megerősítéssel
                    SocketCloseMessage close = new SocketCloseMessage(NetworkManager.Instance.InternalLocalhost.Id,
                        mRemoteEndPointInternal.Host, mLocalEndPointInternal.Port, mRemoteEndPointInternal.Port, mLocalSocketId, mRemoteSocketId, MessageTypeEnum.Tcp);
                    ProcessClose(); // lezárás
                    NetworkManager.Instance.InternalSendMessage(close, null).WaitForSentEvent(mCloseTimeout);
                }
                else
                {
                    // üzenet küldés várakozás nélkül
                    SocketCloseMessage close = new SocketCloseMessage(NetworkManager.Instance.InternalLocalhost.Id,
                        mRemoteEndPointInternal.Host, mLocalEndPointInternal.Port, mRemoteEndPointInternal.Port, mLocalSocketId, mRemoteSocketId, MessageTypeEnum.Udp);
                    ProcessClose(); // lezárás
                    NetworkManager.Instance.InternalSendMessage(close, null);
                }
            }
            Dispose();
        }

        /// <summary>
        /// Shutdowns the socket.
        /// </summary>
        /// <param name="how">The how.</param>
        public void Shutdown(SocketShutdown how)
        {
            if (mClosedLevel == 0)
            {
                switch (how)
                {
                    case SocketShutdown.Both:
                        {
                            Dispose();
                        }
                        break;

                    case SocketShutdown.Receive:
                        {
                            if (mListening)
                            {
                                Dispose();
                            }
                            else
                            {
                                mShutdownReceive = true;
                                if (mReceiveEvent != null)
                                {
                                    mReceiveEvent.Set();
                                }
                            }
                        }
                        break;

                    case SocketShutdown.Send:
                        {
                            if (mListening)
                            {
                                Dispose();
                            }
                            else
                            {
                                mShutdownSend = true;

                                try
                                {
                                    mSendEvent.Set();
                                    mSendEvent.Close();
                                }
                                catch (Exception)
                                {
                                }

                                try
                                {
                                    mSentEventForMessages.Set();
                                    mSentEventForMessages.Close();
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        break;
                }
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

        #region Public properties

        /// <summary>
        /// Gets the available.
        /// </summary>
        public int Available
        {
            get
            {
                int result = 0;

                lock (mDisposeLock)
                {
                    if (mProtocolType == System.Net.Sockets.ProtocolType.Udp)
                    {
                        foreach (ReceivedMessage m in mReceivedMessages)
                        {
                            result += m.Message.Data.Length;
                        }
                    }
                    else
                    {
                        ReceivedMessage beforeMessage = null;
                        foreach (ReceivedMessage m in mReceivedMessages)
                        {
                            if (beforeMessage == null)
                            {
                                if (mRemotePacketOrderNumber == m.Message.PacketOrderNumber)
                                {
                                    // várt és régebbi még ki nem olvasott üzenetek méretei is számítanak
                                    result += (m.Message.Data.Length - m.SentBytes);
                                    beforeMessage = m;
                                }
                                else
                                {
                                    // nem ennek a sorszámú üzenetnek kellene lennie a buffer-ben
                                    break;
                                }
                            }
                            else
                            {
                                if (beforeMessage.Message.PacketOrderNumber + 1 == m.Message.PacketOrderNumber)
                                {
                                    result += (m.Message.Data.Length - m.SentBytes);
                                    beforeMessage = m;
                                }
                                else
                                {
                                    // nem folytonos az üzenetsor a buffer-ben, hiányzik egy elem
                                    break;
                                }
                            }
                        }
                    }
                }

                return result > ReceiveBufferSize ? ReceiveBufferSize : result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ISocket"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected
        {
            get { return mRemoteEndPointInternal != null; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable broadcast].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable broadcast]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool EnableBroadcast
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [exclusive address use].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclusive address use]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool ExclusiveAddressUse
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is bound.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bound; otherwise, <c>false</c>.
        /// </value>
        public bool IsBound
        {
            get { return mLocalEndPointInternal != null; }
        }

        /// <summary>
        /// Gets the local end point.
        /// </summary>
        public AddressEndPoint LocalEndPoint
        {
            get
            {
                DoDisposeCheck();
                return mLocalEndPoint;
            }
            internal set
            {
                mLocalEndPoint = value;
            }
        }

        /// <summary>
        /// Gets the address family.
        /// </summary>
        public AddressFamily AddressFamily
        {
            get { return NetworkManager.Instance.InternalConfiguration.Settings.EnableIPV6 ? System.Net.Sockets.AddressFamily.InterNetworkV6 : System.Net.Sockets.AddressFamily.InterNetwork; }
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
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        /// <value>
        /// The size of the receive buffer.
        /// </value>
        public int ReceiveBufferSize
        {
            get { return mReceiveBufferSize; }
            set
            {
                if (value >= 1024)
                {
                    mReceiveBufferSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the receive timeout.
        /// </summary>
        /// <value>
        /// The receive timeout.
        /// </value>
        public int ReceiveTimeout
        {
            get { return mReceiveTimeout; }
            set
            {
                if (value >= 500)
                {
                    mReceiveTimeout = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the remote end point.
        /// </summary>
        /// <value>
        /// The remote end point.
        /// </value>
        public AddressEndPoint RemoteEndPoint
        {
            get
            {
                DoDisposeCheck();
                return mRemoteEndPoint;
            }
        }

        /// <summary>
        /// Gets or sets the size of the send buffer.
        /// </summary>
        /// <value>
        /// The size of the send buffer.
        /// </value>
        public int SendBufferSize
        {
            get { return mSendBufferSize; }
            set
            {
                if (value >= 1024)
                {
                    mSendBufferSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the send timeout.
        /// </summary>
        /// <value>
        /// The send timeout.
        /// </value>
        public int SendTimeout
        {
            get { return mSendTimeout; }
            set
            {
                if (value >= 500)
                {
                    mSendTimeout = value;
                }
            }
        }

        /// <summary>
        /// Gets the type of the protocol.
        /// </summary>
        /// <value>
        /// The type of the protocol.
        /// </value>
        [DebuggerHidden]
        public ProtocolType ProtocolType
        {
            get { return mProtocolType; }
        }

        /// <summary>
        /// Gets the type of the socket.
        /// </summary>
        /// <value>
        /// The type of the socket.
        /// </value>
        [DebuggerHidden]
        public SocketType SocketType
        {
            get { return mSocketType; }
        }

        /// <summary>
        /// Gets or sets the Time-to-live value.
        /// </summary>
        /// <value>
        /// The TTL.
        /// </value>
        [DebuggerHidden]
        public short Ttl
        {
            get;
            set;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the local socket id.
        /// </summary>
        internal long LocalSocketId
        {
            get { return mLocalSocketId; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is listening.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is listening; otherwise, <c>false</c>.
        /// </value>
        internal bool IsListening
        {
            get { return mListening; }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Process socket open request.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void InternalProcessSocketOpenRequest(SocketOpenRequestMessage message)
        {
            bool sendResponse = false;
            if (mListening && mClosedLevel == 0)
            {
                // lockoknál ügyelni a helyes hívási sorrendre
                lock (mDisposeLock)
                {
                    lock (mListenerQueue)
                    {
                        if (mListening && mClosedLevel == 0)
                        {
                            if (mListenerQueue.Count >= mBacklog)
                            {
                                // nem fér be több kérés a queue-ba
                                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), listening queue is full. Backlog size: {1}, MessageId: {2}", mLocalSocketId.ToString(), mBacklog.ToString(), message.ToString()));
                                sendResponse = true;
                            }
                            else
                            {
                                // elhelyezem a timeout queue-ban
                                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), add to listening queue. Backlog size: {1}, MessageId: {2}", mLocalSocketId.ToString(), mBacklog.ToString(), message.ToString()));
                                mListenerQueue.Enqueue(new IncomingConnection(message, this));
                            }
                        }
                        else
                        {
                            // időközben megállhatott exkluzív portnál a figyelés
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), socket not listening anymore(2). MessageId: {1}", mLocalSocketId.ToString(), message.ToString()));
                            sendResponse = true;
                        }
                        if (!sendResponse)
                        {
                            try
                            {
                                mSemaphoreListener.Release(); // meglököm az accept szálat
                            }
                            catch (Exception) { }
                            mSemaphoreServerSockets.Release(); // meglököm a timeout figyelőt
                        }
                    }
                }
            }
            else
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), socket not listening anymore(1). MessageId: {1}", mLocalSocketId.ToString(), message.ToString()));
                sendResponse = true;
            }
            if (sendResponse)
            {
                // ez nem szerver socket vagy leállítottuk a figyelést
                SocketOpenResponseMessage response = new SocketOpenResponseMessage(NetworkManager.Instance.InternalLocalhost.Id,
                    message.SenderId, -1, message.SenderPort, -1, message.SenderSocketId);
                NetworkManager.Instance.InternalSendMessage(response, null);
            }
        }

        /// <summary>
        /// Process socket open response.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void InternalProcessSocketOpenResponse(SocketOpenResponseMessage message)
        {
            if (mClosedLevel == 0)
            {
                lock (mDisposeLock)
                {
                    if (mClosedLevel == 0)
                    {
                        try
                        {
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("NETWORK-SOCKET({0}), socket open response has arrived. IsBound: {1}, local port: {2} MessageId: {3}", mLocalSocketId.ToString(), IsBound.ToString(), mLocalEndPointInternal == null ? "-1" : mLocalEndPointInternal.Port.ToString(), message.ToString()));
                            if (IsBound && message.TargetPort == mLocalEndPointInternal.Port)
                            {
                                if (message.SenderPort > -1 && message.SenderSocketId > -1)
                                {
                                    mRemoteEndPointInternal = new AddressEndPoint(message.SenderId, message.SenderPort);
                                    mRemoteSocketId = message.SenderSocketId;
                                    //mReceivedMessages = new ListSpecialized<ReceivedMessage>();
                                    //mReceiveEvent = new AutoResetEvent(false);
                                }
                                mConnectEvent.Set();
                            }
                        }
                        catch (Exception)
                        {
                            mRemoteEndPointInternal = null;
                            mRemoteSocketId = -1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Process socket close.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void InternalProcessSocketClose(SocketCloseMessage message)
        {
            lock (mDisposeLock)
            {
                if (IsBound && Connected && message.TargetPort == mLocalEndPointInternal.Port && message.SenderPort == mRemoteEndPointInternal.Port)
                {
                    if (message.MessageType == MessageTypeEnum.Tcp)
                    {
                        SendAcknowledgeMessage(message.SenderId, message.MessageId);
                    }
                    if (mClosedLevel == 0)
                    {
                        // lecsatlakozási kérés a túloldalról
                        ProcessClose();
                    }
                }
            }
        }

        /// <summary>
        /// Internals the process raw data.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void InternalProcessRawData(SocketRawDataMessage message)
        {
            if (mClosedLevel == 0)
            {
                if (ProtocolType == System.Net.Sockets.ProtocolType.Tcp && message.MessageType == MessageTypeEnum.Tcp)
                {
                    // TCP üzenet
                    lock (mDisposeLock)
                    {
                        if (IsBound && Connected && message.TargetPort == mLocalEndPointInternal.Port && message.SenderPort == mRemoteEndPointInternal.Port)
                        {
                            // jó helyen jár...
                            ReceivedMessage m = new ReceivedMessage() { Message = message };
                            if (Available + message.Data.Length <= mReceiveBufferSize)
                            {
                                m.Acknowledged = true;
                                SendAcknowledgeMessage(message.SenderId, message.MessageId);
                            }
                            if (mClosedLevel == 0)
                            {
                                if (!mReceivedMessages.Contains(m))
                                {
                                    mReceivedMessages.Add(m);
                                    mReceivedMessages.Sort(); // rendezés packet order number alapján (sorrend tartás)
                                    if (Available > 0)
                                    {
                                        // ha ezáltal teremtettem kiolvasható adatot, akkor meglököm az olvasó szálat
                                        //LOGGER.Debug(string.Format("RELEASE({0}), message {1}", GetHashCode().ToString(), message.MessageId));
                                        mReceiveEvent.Set();
                                    }
                                }
                            }
                        }
                    }
                }
                else if (ProtocolType == System.Net.Sockets.ProtocolType.Udp && message.MessageType == MessageTypeEnum.Udp)
                {
                    // UDP üzenet
                    if ((EnableBroadcast && string.IsNullOrEmpty(message.TargetId)) ||
                        (!EnableBroadcast && NetworkManager.Instance.InternalLocalhost.Id.Equals(message.TargetId)))
                    {
                        // 1, broadcast üzenet fogható és az üzenet maga az
                        // 2, sima nekem szóló UDP üzenet
                        if (mClosedLevel == 0)
                        {
                            lock (mDisposeLock)
                            {
                                if (mClosedLevel == 0)
                                {
                                    if (mReceivedMessages.Count > 0)
                                    {
                                        if (Available + message.Data.Length <= ReceiveBufferSize)
                                        {
                                            mReceivedMessages.Add(new ReceivedMessage() { Message = message });
                                            mReceiveEvent.Set();
                                        }
                                    }
                                    else
                                    {
                                        mReceivedMessages.Add(new ReceivedMessage() { Message = message });
                                        mReceiveEvent.Set();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Private method(s)

        private void DoBindCheck()
        {
            if (!IsBound)
            {
                throw new InvalidOperationException("Socket not bind to a local port.");
            }
        }

        private void DoBindNotCheck()
        {
            if (IsBound)
            {
                throw new InvalidOperationException("Socket already connected.");
            }
        }

        private void DoConnectedRemoteNotCheck()
        {
            if (!Connected)
            {
                throw new SocketException((int)SocketError.ConnectionAborted);
            }
        }

        private void DoConnectedRemoteCheck()
        {
            if (Connected)
            {
                throw new SocketException((int)SocketError.IsConnected);
            }
        }

        private void DoDisposeCheck()
        {
            if (mClosedLevel == 2)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private void DoListenCheck()
        {
            if (mListening)
            {
                throw new InvalidOperationException("Socket is in listening mode.");
            }
        }

        private void DoListenNotCheck()
        {
            if (!mListening)
            {
                throw new InvalidOperationException("Socket is not in listening mode.");
            }
        }

        private void DoShutdownReceiveCheck()
        {
            if (mShutdownReceive)
            {
                throw new SocketException((int)SocketError.Shutdown);
            }
        }

        private void DoShutdownSendCheck()
        {
            if (mShutdownSend)
            {
                throw new SocketException((int)SocketError.Shutdown);
            }
        }

        private void CloseAsyncActiveAcceptEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveAcceptEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveAcceptEvent.Dispose();
                mAsyncActiveAcceptEvent = null;
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

        private void CloseAsyncActiveReceiveEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveReceiveEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveReceiveEvent.Dispose();
                mAsyncActiveReceiveEvent = null;
            }
        }

        private void CloseAsyncActiveReceiveFromEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveReceiveFromEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveReceiveFromEvent.Dispose();
                mAsyncActiveReceiveFromEvent = null;
            }
        }

        private void CloseAsyncActiveSendEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveSendEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveSendEvent.Dispose();
                mAsyncActiveSendEvent = null;
            }
        }

        private void CloseAsyncActiveSendToEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveSendToEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveSendToEvent.Dispose();
                mAsyncActiveSendToEvent = null;
            }
        }

        private bool CheckForSocketOpenRequestTimeout(out int value, out IncomingConnection connection)
        {
            // timeout szál vizsgálja hogy ki timeoutol el leghamarabb
            bool result = false;
            value = 0;
            connection = null;

            if (mListening)
            {
                lock (mListenerQueue)
                {
                    if (mListenerQueue.Count > 0)
                    {
                        IncomingConnection c = mListenerQueue.Peek();
                        if (c.Stopwatch.IsRunning)
                        {
                            value = NetworkManager.Instance.InternalConfiguration.Settings.DefaultSocketAcceptTimeWaitInMS - Convert.ToInt32(c.Stopwatch.ElapsedMilliseconds);
                            connection = c;
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        private void NetworkPeerUnaccessibleHandler(object sender, NetworkPeerChangedEventArgs e)
        {
            if (Connected && IsBound && e.NetworkPeers.Contains(mRemoteNetworkPeer))
            {
                // ez server socket-nél nem hívódik, így a deadlock lehetősége nem áll fent a mListenerQueue-val
                ProcessClose(); // lezárás, mivel a távoli számítógép elérhetetlenné vált
            }
        }

        private void SendAcknowledgeMessage(string targetId, long messageIdToAck)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                ThrowHelper.ThrowArgumentNullException("targetId");
            }
            MessageAcknowledge message = new MessageAcknowledge(NetworkManager.Instance.InternalLocalhost.Id, targetId, messageIdToAck);
            NetworkManager.Instance.InternalSendMessage(message, null);
        }

        private void ProcessClose()
        {
            if (mClosedLevel == 0)
            {
                // ez a lock csak az ismételt behívást elkerülendő van itt
                lock (mCloseLock)
                {
                    if (mClosedLevel == 0)
                    {
                        mClosedLevel = 1;

                        mRemoteEndPointInternal = null;
                        mRemoteSocketId = -1;

                        if (mRemoteNetworkPeer != null)
                        {
                            NetworkManager.Instance.NetworkPeerUnaccessible -= new EventHandler<NetworkPeerChangedEventArgs>(NetworkPeerUnaccessibleHandler);
                        }

                        mReceiveEvent.Set();

                        if (mListening)
                        {
                            mListening = false;

                            lock (mServerSockets)
                            {
                                mServerSockets.Remove(this);
                            }

                            // bárki, aki hívja a ProcessClose-t és nem a Dispose-ból jön, az deadlockot okozhat az mListenerQueue-n
                            lock (mListenerQueue)
                            {
                                while (mListenerQueue.Count > 0)
                                {
                                    // akik időközben a queue-ba kerültek, azokat "lerázom"
                                    IncomingConnection trash = mListenerQueue.Dequeue();
                                    try
                                    {
                                        // ha a timeout figyelő bent van, meglökjük
                                        trash.TimeoutEvent.Set();
                                    }
                                    catch (Exception) { }
                                    SocketOpenResponseMessage response = new SocketOpenResponseMessage(NetworkManager.Instance.InternalLocalhost.Id,
                                        trash.Message.SenderId, -1, trash.Message.SenderPort, -1, trash.Message.SenderSocketId);
                                    NetworkManager.Instance.InternalSendMessage(response, null);
                                }
                            }

                            if (mSemaphoreListener != null)
                            {
                                mSemaphoreListener.Release();
                                mSemaphoreListener.Close();
                            }
                            mSemaphoreServerSockets.Release(); // meglököm a timeout szálat, hogy keressen másik feladatot
                        }

                        try
                        {
                            mSendEvent.Set();
                            mSendEvent.Close();
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            lock (mSendCloseLockObject)
                            {
                                mSentEventForMessages.Set();
                                mSentEventForMessages.Close();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        private int InternalReceiveFrom(byte[] buffer, int offset, int size, EndPoint remoteEp)
        {
            return ReceiveFrom(buffer, offset, size, ref mReceiveFromEndpoint);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mAsyncActiveSendEvent"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mAsyncActiveSendToEvent"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mAsyncActiveReceiveFromEvent"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mAsyncActiveReceiveEvent"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mAsyncActiveConnectEvent"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mAsyncActiveAcceptEvent")]
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (mClosedLevel < 2)
                {
                    lock (mDisposeLock)
                    {
                        if (mClosedLevel < 2)
                        {
                            if (Connected)
                            {
                                // üzenet küldés várakozás nélkül
                                SocketCloseMessage close = new SocketCloseMessage(NetworkManager.Instance.InternalLocalhost.Id,
                                    mRemoteEndPointInternal.Host, mLocalEndPointInternal.Port, mRemoteEndPointInternal.Port, mLocalSocketId, mRemoteSocketId, MessageTypeEnum.Udp);
                                ProcessClose(); // lezárás
                                NetworkManager.Instance.InternalSendMessage(close, null);
                            }
                            else
                            {
                                ProcessClose(); // lezárás
                            }

                            mShutdownReceive = true;
                            mShutdownSend = true;
                            mClosedLevel = 2;

                            if (IsBound)
                            {
                                // helyi port zárása
                                NetworkManager.Instance.InternalSocketUnregister(this);
                                lock (mUsedPorts)
                                {
                                    mUsedPorts.Remove(mLocalEndPointInternal.Port); // port törlése
                                }
                                mLocalEndPointInternal = null;
                            }

                            // bárki, aki hozzányúl a received messages-hez, az disposelock scope-ban van
                            mReceivedMessages.Clear();

                            try
                            {
                                mConnectEvent.Set();
                                mConnectEvent.Close();
                            }
                            catch (Exception) { }

                            // ezt azért tettem ide, mert a warn jelzett. Itt elvileg nem okozhat gondot a kidobása.
                            mReceiveEvent.Dispose();

                            //if (mSemaphoreListener != null)
                            //{
                            //    mSemaphoreListener.Release();
                            //    mSemaphoreListener.Close();
                            //}
                        }
                    }
                }
            }
        }

        #endregion

        #region Nested classes

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
        private class IncomingConnection
        {

            #region Field(s)

            private ManualResetEvent mTimeoutEvent = new ManualResetEvent(false);

            private Stopwatch mStopwatch = Stopwatch.StartNew();

            private SocketOpenRequestMessage mMessage = null;

            private Socket mServerSocket = null;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="IncomingConnection" /> class.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="serverSocket">The server socket.</param>
            internal IncomingConnection(SocketOpenRequestMessage message, Socket serverSocket)
            {
                mMessage = message;
                mServerSocket = serverSocket;
            }

            #endregion

            #region Internal properties

            /// <summary>
            /// Gets the timeout event.
            /// </summary>
            internal ManualResetEvent TimeoutEvent
            {
                get { return mTimeoutEvent; }
            }

            /// <summary>
            /// Gets the stopwatch.
            /// </summary>
            internal Stopwatch Stopwatch
            {
                get { return mStopwatch; }
            }

            /// <summary>
            /// Gets the message.
            /// </summary>
            internal SocketOpenRequestMessage Message
            {
                get { return mMessage; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="IncomingConnection"/> is accepted.
            /// </summary>
            /// <value>
            ///   <c>true</c> if accepted; otherwise, <c>false</c>.
            /// </value>
            internal bool Accepted
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="IncomingConnection"/> is timedout.
            /// </summary>
            /// <value>
            ///   <c>true</c> if timedout; otherwise, <c>false</c>.
            /// </value>
            internal bool Timedout
            {
                get;
                private set;
            }

            #endregion

            #region Internal method(s)

            /// <summary>
            /// Waits for accept or timeout.
            /// </summary>
            /// <param name="timeout">The timeout.</param>
            internal void WaitForAcceptOrTimeout(int timeout)
            {
                mStopwatch.Stop();
                if (timeout > 0)
                {
                    mTimeoutEvent.WaitOne(timeout);
                }
                mTimeoutEvent.Dispose();

                // lezárástól megóvás, ügyelni a lock hívási sorrendre
                lock (mServerSocket.mCloseLock)
                {
                    lock (mServerSocket.mListenerQueue)
                    {
                        if (!Accepted && mServerSocket.mListening)
                        {
                            // ha nem fogadtuk és aktív a szerver socket...
                            Timedout = true;
                            mServerSocket.mListenerQueue.Dequeue();
                            SocketOpenResponseMessage response = new SocketOpenResponseMessage(NetworkManager.Instance.InternalLocalhost.Id,
                                mMessage.SenderId, -1, mMessage.SenderPort, -1, mMessage.SenderSocketId);
                            NetworkManager.Instance.InternalSendMessage(response, null);
                        }
                    }
                }
            }

            #endregion

        }

        private class ReceivedMessage : IComparable<ReceivedMessage>, IComparable, IEquatable<ReceivedMessage>
        {

            #region Field(s)

            internal SocketRawDataMessage Message = null;

            internal int SentBytes = 0;

            internal bool Acknowledged = false;

            #endregion

            #region Public method(s)

            /// <summary>
            /// Compares to.
            /// </summary>
            /// <param name="other">The other.</param>
            /// <returns></returns>
            public int CompareTo(ReceivedMessage other)
            {
                return Message.PacketOrderNumber.CompareTo(other.Message.PacketOrderNumber);
            }

            /// <summary>
            /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
            /// </summary>
            /// <param name="obj">An object to compare with this instance.</param>
            /// <returns>
            /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
            /// </returns>
            /// <exception cref="T:System.ArgumentException">
            ///   <paramref name="obj"/> is not the same type as this instance. </exception>
            public int CompareTo(object obj)
            {
                return CompareTo((ReceivedMessage)obj);
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
                return Equals((ReceivedMessage)obj);
            }

            /// <summary>
            /// Equalses the specified other.
            /// </summary>
            /// <param name="other">The other.</param>
            /// <returns></returns>
            public bool Equals(ReceivedMessage other)
            {
                return other.Message.PacketOrderNumber == Message.PacketOrderNumber;
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

        }

        #endregion

    }

}
