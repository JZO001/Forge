/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Forge.Legacy;
using Forge.Net.Synapse.NetworkServices;
using Forge.Shared;
using Forge.Threading.Tasking;
using Forge.Threading;

namespace Forge.Net.Synapse.NetworkFactory
{

    /// <summary>
    /// Wrapper class for Socket.NET
    /// </summary>
    public sealed class SocketWrapper : MBRBase, ISocket
    {

        #region Field(s)

        private Socket mSocket = null;

        private System.Func<ISocket> mAcceptDelegate = null;
        private int mAsyncActiveAcceptCount = 0;
        private AutoResetEvent mAsyncActiveAcceptEvent = null;
        private readonly object LOCK_ACCEPT = new object();

        private System.Action<EndPoint> mEndpointConnectDelegate = null;
        private System.Action<string, int> mHostIpConnectDelegate = null;
        private AutoResetEvent mAsyncActiveConnectEvent = null;
        private int mAsyncActiveConnectCount = 0;
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

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketWrapper"/> class.
        /// </summary>
        /// <param name="socket">The socket.</param>
        public SocketWrapper(Socket socket)
        {
            if (socket == null)
            {
                ThrowHelper.ThrowArgumentNullException("socket");
            }
            this.mSocket = socket;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SocketWrapper"/> is reclaimed by garbage collection.
        /// </summary>
        ~SocketWrapper()
        {
            Dispose(false);
        }

        #endregion

        #region Public member(s)

        /// <summary>
        /// Gets the address family.
        /// </summary>
        public AddressFamily AddressFamily
        {
            get { return mSocket.AddressFamily; }
        }

#if NET40

        /// <summary>
        /// Begins the accept.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            return mSocket.BeginAccept(callback, state);
        }

        /// <summary>
        /// Ends the accept.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Socket implementation</returns>
        public ISocket EndAccept(IAsyncResult asyncResult)
        {
            return new SocketWrapper(mSocket.EndAccept(asyncResult));
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(System.Net.EndPoint endPoint, AsyncCallback callBack, object state)
        {
            return mSocket.BeginConnect(endPoint, callBack, state);
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(string host, int port, AsyncCallback callBack, object state)
        {
            return mSocket.BeginConnect(host, port, callBack, state);
        }

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        public void EndConnect(IAsyncResult asyncResult)
        {
            mSocket.EndConnect(asyncResult);
        }

        /// <summary>
        /// Begins the receive.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, AsyncCallback callBack, object state)
        {
            return mSocket.BeginReceive(buffer, offset, size, SocketFlags.None, callBack, state);
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
        /// <returns>Async property</returns>
        public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, ref System.Net.EndPoint remoteEp, AsyncCallback callBack, object state)
        {
            return mSocket.BeginReceiveFrom(buffer, offset, size, SocketFlags.None, ref remoteEp, callBack, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, AsyncCallback callBack, object state)
        {
            return mSocket.BeginSend(buffer, offset, size, SocketFlags.None, callBack, state);
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
        /// <returns>Async property</returns>
        public IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, System.Net.EndPoint remoteEp, AsyncCallback callBack, object state)
        {
            return mSocket.BeginSendTo(buffer, offset, size, SocketFlags.None, remoteEp, callBack, state);
        }

        /// <summary>
        /// Ends the receive.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of received bytes</returns>
        public int EndReceive(IAsyncResult asyncResult)
        {
            return mSocket.EndReceive(asyncResult);
        }

        /// <summary>
        /// Ends the receive from.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        public int EndReceiveFrom(IAsyncResult asyncResult, ref System.Net.EndPoint remoteEp)
        {
            return mSocket.EndReceiveFrom(asyncResult, ref remoteEp);
        }

        /// <summary>
        /// Ends the send.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of received bytes</returns>
        public int EndSend(IAsyncResult asyncResult)
        {
            return mSocket.EndSend(asyncResult);
        }

        /// <summary>
        /// Ends the send to.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of received bytes</returns>
        public int EndSendTo(IAsyncResult asyncResult)
        {
            return mSocket.EndSendTo(asyncResult);
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
            return new SocketWrapper(await mSocket.AcceptAsync());
        }

#endif

        /// <summary>
        /// Accepts a new incoming connection.
        /// </summary>
        /// <returns>Socket implementation</returns>
        public ISocket Accept()
        {
            return new SocketWrapper(mSocket.Accept());
        }

        /// <summary>
        /// Binds the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        public void Bind(System.Net.EndPoint endPoint)
        {
            mSocket.Bind(endPoint);
        }

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        public async Task ConnectAsync(EndPoint endPoint)
        {
            await mSocket.ConnectAsync(endPoint);
        }

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public async Task ConnectAsync(string host, int port)
        {
            await mSocket.ConnectAsync(host, port);
        }

#endif

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        public void Connect(System.Net.EndPoint endPoint)
        {
            mSocket.Connect(endPoint);
        }

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public void Connect(string host, int port)
        {
            mSocket.Connect(host, port);
        }

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Receives the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Number of received bytes</returns>
        public async Task<int> ReceiveAsync(byte[] buffer)
        {
            return await Task.Run(() => mSocket.Receive(buffer));
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
            return await Task.Run(() => mSocket.Receive(buffer, offset, size, SocketFlags.None));
        }

#endif

        /// <summary>
        /// Receives the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Number of received bytes</returns>
        public int Receive(byte[] buffer)
        {
            return mSocket.Receive(buffer);
        }

        /// <summary>
        /// Receives the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <returns>Number of received bytes</returns>
        public int Receive(byte[] buffer, int offset, int size)
        {
            return mSocket.Receive(buffer, offset, size, SocketFlags.None);
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
            return (await Task.Run(() => mSocket.ReceiveFrom(buffer, ref remoteEp)), remoteEp);
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
            return (await Task.Run(() => mSocket.ReceiveFrom(buffer, offset, size, SocketFlags.None, ref remoteEp)), remoteEp);
        }

#endif

        /// <summary>
        /// Receives from.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        public int ReceiveFrom(byte[] buffer, ref System.Net.EndPoint remoteEp)
        {
            return mSocket.ReceiveFrom(buffer, ref remoteEp);
        }

        /// <summary>
        /// Receives from.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        public int ReceiveFrom(byte[] buffer, int offset, int size, ref System.Net.EndPoint remoteEp)
        {
            return mSocket.ReceiveFrom(buffer, offset, size, SocketFlags.None, ref remoteEp);
        }

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendAsync(byte[] buffer)
        {
            return await Task.Run(() => mSocket.Send(buffer));
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
            return await Task.Run(() => mSocket.Send(buffer, offset, size, SocketFlags.None));
        }

#endif

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Number of sent bytes</returns>
        public int Send(byte[] buffer)
        {
            return mSocket.Send(buffer);
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <returns>Number of sent bytes</returns>
        public int Send(byte[] buffer, int offset, int size)
        {
            return mSocket.Send(buffer, offset, size, SocketFlags.None);
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
            return await Task.Run(() => mSocket.SendTo(buffer, remoteEp));
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
            return await Task.Run(() => mSocket.SendTo(buffer, offset, size, SocketFlags.None, remoteEp));
        }

#endif

        /// <summary>
        /// Sends to.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        public int SendTo(byte[] buffer, System.Net.EndPoint remoteEp)
        {
            return mSocket.SendTo(buffer, remoteEp);
        }

        /// <summary>
        /// Sends to.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        public int SendTo(byte[] buffer, int offset, int size, System.Net.EndPoint remoteEp)
        {
            return mSocket.SendTo(buffer, offset, size, SocketFlags.None, remoteEp);
        }

        /// <summary>
        /// Listens the specified backlog.
        /// </summary>
        /// <param name="backlog">The backlog.</param>
        public void Listen(int backlog)
        {
            mSocket.Listen(backlog);
        }

        /// <summary>
        /// Detect the status of the socket
        /// </summary>
        /// <param name="microSeconds">The micro seconds.</param>
        /// <param name="selectMode">The select mode.</param>
        /// <returns>True, if the socket is active, otherwise False.</returns>
        public bool Pool(int microSeconds, SelectMode selectMode)
        {
            return mSocket.Poll(microSeconds, selectMode);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            mSocket.Close();
        }

        /// <summary>
        /// Close the socket and wait for the specified time.
        /// </summary>
        /// <param name="timeout">The timeout value.</param>
        public void Close(int timeout)
        {
            mSocket.Close(timeout);
        }

        /// <summary>
        /// Gets the available bytes.
        /// </summary>
        public int Available
        {
            get { return mSocket.Available; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ISocket"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected
        {
            get { return mSocket.Connected; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable broadcast].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable broadcast]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableBroadcast
        {
            get { return mSocket.EnableBroadcast; }
            set { mSocket.EnableBroadcast = value; }
        }

        /// <summary>
        /// Gets the local end point.
        /// </summary>
        public AddressEndPoint LocalEndPoint
        {
            get { return new AddressEndPoint(((IPEndPoint)mSocket.LocalEndPoint).Address.ToString(), ((IPEndPoint)mSocket.LocalEndPoint).Port, ((IPEndPoint)mSocket.LocalEndPoint).AddressFamily); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [no delay].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no delay]; otherwise, <c>false</c>.
        /// </value>
        public bool NoDelay
        {
            get { return mSocket.NoDelay; }
            set { mSocket.NoDelay = value; }
        }

        /// <summary>
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        /// <value>
        /// The size of the receive buffer.
        /// </value>
        public int ReceiveBufferSize
        {
            get { return mSocket.ReceiveBufferSize; }
            set { mSocket.ReceiveBufferSize = value; }
        }

        /// <summary>
        /// Gets or sets the receive timeout.
        /// </summary>
        /// <value>
        /// The receive timeout.
        /// </value>
        public int ReceiveTimeout
        {
            get { return mSocket.ReceiveTimeout; }
            set { mSocket.ReceiveTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the remote end point.
        /// </summary>
        /// <value>
        /// The remote end point.
        /// </value>
        public AddressEndPoint RemoteEndPoint
        {
            get { return new AddressEndPoint(((IPEndPoint)mSocket.RemoteEndPoint).Address.ToString(), ((IPEndPoint)mSocket.RemoteEndPoint).Port, ((IPEndPoint)mSocket.RemoteEndPoint).AddressFamily); }
        }

        /// <summary>
        /// Gets or sets the size of the send buffer.
        /// </summary>
        /// <value>
        /// The size of the send buffer.
        /// </value>
        public int SendBufferSize
        {
            get { return mSocket.SendBufferSize; }
            set { mSocket.SendBufferSize = value; }
        }

        /// <summary>
        /// Gets or sets the send timeout.
        /// </summary>
        /// <value>
        /// The send timeout.
        /// </value>
        public int SendTimeout
        {
            get { return mSocket.SendTimeout; }
            set { mSocket.SendTimeout = value; }
        }

        /// <summary>
        /// Gets the type of the protocol.
        /// </summary>
        /// <value>
        /// The type of the protocol.
        /// </value>
        public ProtocolType ProtocolType
        {
            get { return mSocket.ProtocolType; }
        }

        /// <summary>
        /// Gets the type of the socket.
        /// </summary>
        /// <value>
        /// The type of the socket.
        /// </value>
        public SocketType SocketType
        {
            get { return mSocket.SocketType; }
        }

        /// <summary>
        /// Gets or sets the TTL.
        /// </summary>
        /// <value>
        /// The TTL.
        /// </value>
        public short Ttl
        {
            get { return mSocket.Ttl; }
            set { mSocket.Ttl = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [exclusive address use].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclusive address use]; otherwise, <c>false</c>.
        /// </value>
        public bool ExclusiveAddressUse
        {
            get { return mSocket.ExclusiveAddressUse; }
            set { mSocket.ExclusiveAddressUse = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is bound.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bound; otherwise, <c>false</c>.
        /// </value>
        public bool IsBound
        {
            get { return mSocket.IsBound; }
        }

        /// <summary>
        /// Shutdowns the socket.
        /// </summary>
        /// <param name="how">The how.</param>
        public void Shutdown(SocketShutdown how)
        {
            mSocket.Shutdown(how);
        }

#if IS_WINDOWS

        /// <summary>
        /// Sets the keep alive values.
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <param name="keepAliveTime">The keep alive time.</param>
        /// <param name="keepAliveInterval">The keep alive interval.</param>
        /// <returns></returns>
        public int SetKeepAliveValues(bool state, int keepAliveTime, int keepAliveInterval)
        {
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

            return mSocket.IOControl(IOControlCode.KeepAliveValues, keepAlive.ToArray(), null);
        }

#endif

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

        private int InternalReceiveFrom(byte[] buffer, int offset, int size, EndPoint remoteEp)
        {
            return mSocket.ReceiveFrom(buffer, offset, size, SocketFlags.None, ref mReceiveFromEndpoint);
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

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                mSocket.Dispose();
            }
        }

        #endregion

    }

}
