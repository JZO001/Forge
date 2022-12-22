/* *********************************************************************
 * Date: 08 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Forge.Legacy;
using Forge.Net.Synapse.NetworkServices;
using Forge.Shared;
using Forge.Threading.Tasking;
using Forge.Threading;
using System.Threading;
using System.Drawing;

namespace Forge.Net.Synapse.NetworkFactory
{

    /// <summary>
    /// Wrapper for .NET UdpClient
    /// </summary>
    public class UdpClientWrapper : MBRBase, IUdpClient
    {

        #region Field(s)

        private UdpClient mUdpClient = null;
        private SocketWrapper mSocketWrapper = null;

        private System.Func<byte[]> mReceiveDelegate = null;
        private int mAsyncActiveReceiveCount = 0;
        private AutoResetEvent mAsyncActiveReceiveEvent = null;
        private readonly object LOCK_RECEIVE = new object();
        private IPEndPoint mReceiveEndpoint = null;

        private System.Func<byte[], int, int, AddressEndPoint, int> mSendDelegate = null;
        private int mAsyncActiveSendCount = 0;
        private AutoResetEvent mAsyncActiveSendEvent = null;
        private readonly object LOCK_SEND = new object();

        private bool mIsBroadcast = false;

        private bool mActive = false;

        private bool mDisposed = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClientWrapper"/> class.
        /// </summary>
        /// <param name="udpClient">The UDP client.</param>
        public UdpClientWrapper(UdpClient udpClient)
        {
            if (udpClient == null)
            {
                ThrowHelper.ThrowArgumentNullException("udpClient");
            }
            mUdpClient = udpClient;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="UdpClientWrapper"/> is reclaimed by garbage collection.
        /// </summary>
        ~UdpClientWrapper()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the available.
        /// </summary>
        public int Available
        {
            get { return mUdpClient.Available; }
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        public ISocket Client
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (mSocketWrapper == null)
                {
                    mSocketWrapper = new SocketWrapper(mUdpClient.Client);
                }
                return mSocketWrapper;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable broadcast].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable broadcast]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableBroadcast
        {
            get { return mUdpClient.EnableBroadcast; }
            set { mUdpClient.EnableBroadcast = value; }
        }

        /// <summary>
        /// Gets or sets the TTL.
        /// </summary>
        /// <value>
        /// The TTL.
        /// </value>
        public short Ttl
        {
            get { return mUdpClient.Ttl; }
            set { mUdpClient.Ttl = value; }
        }

        #endregion

        #region Public method(s)

#if NET40

        /// <summary>
        /// Begins the receive.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return mUdpClient.BeginReceive(callback, state);
        }

        /// <summary>
        /// Ends the receive.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Received data</returns>
        public byte[] EndReceive(IAsyncResult asyncResult, ref AddressEndPoint remoteEp)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] result = mUdpClient.EndReceive(asyncResult, ref endPoint);
            remoteEp = new AddressEndPoint(endPoint.Address.ToString(), endPoint.Port);
            return result;
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginSend(byte[] buffer, AsyncCallback callback, object state)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, buffer.Length, null, callback, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return BeginSend(buffer, offset, size, null, callback, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginSend(byte[] buffer, int size, AsyncCallback callback, object state)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, size, null, callback, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginSend(byte[] buffer, AddressEndPoint remoteEp, AsyncCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, buffer.Length, callback, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginSend(byte[] buffer, int size, AddressEndPoint remoteEp, AsyncCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            IPEndPoint ep = new IPEndPoint(Dns.GetHostAddresses(remoteEp.Host)[0], remoteEp.Port);
            return mUdpClient.BeginSend(buffer, size, ep, callback, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginSend(byte[] buffer, string hostName, int port, AsyncCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, buffer.Length, new AddressEndPoint(hostName, port), callback, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginSend(byte[] buffer, int size, string hostName, int port, AsyncCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, size, new AddressEndPoint(hostName, port), callback, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, string hostName, int port, AsyncCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, offset, size, new AddressEndPoint(hostName, port), callback, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, AddressEndPoint remoteEp, AsyncCallback callback, object state)
        {
            DoDisposeCheck();
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (size > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("bytes");
            }
            if (mActive && (remoteEp != null))
            {
                throw new InvalidOperationException("UdpClient is already connected.");
            }
            if (remoteEp == null)
            {
                return mUdpClient.Client.BeginSend(buffer, offset, size, SocketFlags.None, callback, state);
            }
            IPEndPoint ep = new IPEndPoint(Dns.GetHostAddresses(remoteEp.Host)[0], remoteEp.Port);
            CheckForBroadcast(ep.Address);
            return mUdpClient.Client.BeginSendTo(buffer, offset, size, SocketFlags.None, ep, callback, state);
        }

        /// <summary>
        /// Ends the receive.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of sent bytes</returns>
        public int EndSend(IAsyncResult asyncResult)
        {
            return mUdpClient.EndSend(asyncResult);
        }

#endif

        /// <summary>Begins the receive.</summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginReceive(ReturnCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveReceiveCount);
            System.Func<byte[]> d = new System.Func<byte[]>(InternalReceive);
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
            return d.BeginInvoke(callback, state);
        }

        /// <summary>Ends the receive.</summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        public byte[] EndReceive(ITaskResult asyncResult, ref AddressEndPoint remoteEp)
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
                byte[] result = mReceiveDelegate.EndInvoke(asyncResult);
                remoteEp = new AddressEndPoint(mReceiveEndpoint.Address.ToString(), mReceiveEndpoint.Port, mReceiveEndpoint.AddressFamily);
                return result;
            }
            finally
            {
                mReceiveDelegate = null;
                mAsyncActiveReceiveEvent.Set();
                CloseAsyncActiveReceiveEvent(Interlocked.Decrement(ref mAsyncActiveReceiveCount));
            }
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        public ITaskResult BeginSend(byte[] buffer, ReturnCallback callback, object state)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, buffer.Length, null, callback, state);
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        public ITaskResult BeginSend(byte[] buffer, int offset, int size, ReturnCallback callback, object state)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return BeginSend(buffer, offset, size, null, callback, state);
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        public ITaskResult BeginSend(byte[] buffer, int size, ReturnCallback callback, object state)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, size, null, callback, state);
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginSend(byte[] buffer, AddressEndPoint remoteEp, ReturnCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, buffer.Length, callback, state);
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginSend(byte[] buffer, int size, AddressEndPoint remoteEp, ReturnCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, size, remoteEp, callback, state);
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginSend(byte[] buffer, string hostName, int port, ReturnCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, buffer.Length, new AddressEndPoint(hostName, port), callback, state);
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginSend(byte[] buffer, int size, string hostName, int port, ReturnCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, size, new AddressEndPoint(hostName, port), callback, state);
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginSend(byte[] buffer, int offset, int size, string hostName, int port, ReturnCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, offset, size, new AddressEndPoint(hostName, port), callback, state);
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">bytes</exception>
        /// <exception cref="System.InvalidOperationException">UdpClient is already connected.</exception>
        public ITaskResult BeginSend(byte[] buffer, int offset, int size, AddressEndPoint remoteEp, ReturnCallback callback, object state)
        {
            DoDisposeCheck();
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (size > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("bytes");
            }
            if (mActive && (remoteEp != null))
            {
                throw new InvalidOperationException("UdpClient is already connected.");
            }

            IPEndPoint ep = null;
            if (remoteEp != null)
            {
                ep = new IPEndPoint(Dns.GetHostAddresses(remoteEp.Host)[0], remoteEp.Port);
                CheckForBroadcast(ep.Address);
            }

            Interlocked.Increment(ref mAsyncActiveSendCount);
            System.Func<byte[], int, int, AddressEndPoint, int> d = new System.Func<byte[], int, int, AddressEndPoint, int>(Send);
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
            return d.BeginInvoke(buffer, offset, size, remoteEp, callback, state);
        }

        /// <summary>Ends the receive.</summary>
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

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Receives the specified remote ep.
        /// </summary>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        public async Task<(byte[], AddressEndPoint)> ReceiveAsync(AddressEndPoint remoteEp)
        {
            return (await Task.Run(() => Receive(ref remoteEp)), remoteEp);
        }

#endif

        /// <summary>
        /// Receives the specified remote ep.
        /// </summary>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Received data</returns>
        public byte[] Receive(ref AddressEndPoint remoteEp)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] result = mUdpClient.Receive(ref endPoint);
            remoteEp = new AddressEndPoint(endPoint.Address.ToString(), endPoint.Port);
            return result;
        }


#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendAsync(byte[] buffer)
        {
            return await Task.Run(() => Send(buffer));
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

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendAsync(byte[] buffer, int size)
        {
            return await Task.Run(() => Send(buffer, size));
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendAsync(byte[] buffer, AddressEndPoint remoteEp)
        {
            return await Task.Run(() => Send(buffer, remoteEp));
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendAsync(byte[] buffer, int size, AddressEndPoint remoteEp)
        {
            return await Task.Run(() => Send(buffer, size, remoteEp));
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendAsync(byte[] buffer, int offset, int size, AddressEndPoint remoteEp)
        {
            return await Task.Run(() => Send(buffer, offset, size, remoteEp));
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendAsync(byte[] buffer, string hostName, int port)
        {
            return await Task.Run(() => Send(buffer, hostName, port));
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendAsync(byte[] buffer, int size, string hostName, int port)
        {
            return await Task.Run(() => Send(buffer, size, hostName, port));
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <returns>Number of sent bytes</returns>
        public async Task<int> SendAsync(byte[] buffer, int offset, int size, string hostName, int port)
        {
            return await Task.Run(() => Send(buffer, offset, size, hostName, port));
        }

#endif

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Number of sent bytes</returns>
        public int Send(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return Send(buffer, 0, buffer.Length);
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
            DoDisposeCheck();
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (!mActive)
            {
                throw new InvalidOperationException("UdpClient not connected.");
            }
            return mUdpClient.Client.Send(buffer, offset, size, SocketFlags.None);
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <returns>Number of sent bytes</returns>
        public int Send(byte[] buffer, int size)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return Send(buffer, 0, size);
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        public int Send(byte[] buffer, AddressEndPoint remoteEp)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return Send(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        public int Send(byte[] buffer, int size, AddressEndPoint remoteEp)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return Send(buffer, 0, size);
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">bytes</exception>
        /// <exception cref="System.InvalidOperationException">UdpClient is already connected.</exception>
        public int Send(byte[] buffer, int offset, int size, AddressEndPoint remoteEp)
        {
            DoDisposeCheck();
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (size > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("bytes");
            }
            if (mActive && (remoteEp != null))
            {
                throw new InvalidOperationException("UdpClient is already connected.");
            }
            if (remoteEp == null)
            {
                return mUdpClient.Client.Send(buffer, offset, size, SocketFlags.None);
            }
            IPEndPoint ep = new IPEndPoint(Dns.GetHostAddresses(remoteEp.Host)[0], remoteEp.Port);
            CheckForBroadcast(ep.Address);
            return mUdpClient.Client.SendTo(buffer, offset, size, SocketFlags.None, ep);
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        public int Send(byte[] buffer, string hostName, int port)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return Send(buffer, 0, buffer.Length, new AddressEndPoint(hostName, port));
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        public int Send(byte[] buffer, int size, string hostName, int port)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return Send(buffer, 0, size, new AddressEndPoint(hostName, port));
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <returns>Number of sent bytes</returns>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        public int Send(byte[] buffer, int offset, int size, string hostName, int port)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            return Send(buffer, offset, size, new AddressEndPoint(hostName, port));
        }

        /// <summary>
        /// Connects the specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        public void Connect(int port)
        {
            Connect(new AddressEndPoint(IPAddress.Any.ToString(), port));
        }

        /// <summary>
        /// Connects the specified local ep.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        public void Connect(AddressEndPoint localEp)
        {
            IPAddress a = null;
            if (localEp.Host.Equals(IPAddress.Any.ToString()))
            {
                a = IPAddress.Any;
            }
            else if (localEp.Host.Equals(IPAddress.IPv6Any.ToString()))
            {
                a = IPAddress.IPv6Any;
            }
            else
            {
                a = Dns.GetHostAddresses(localEp.Host)[0];
            }
            mUdpClient.Connect(new IPEndPoint(a, localEp.Port));
            mActive = true;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            mUdpClient.Close();
            mActive = false;
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

        #region Protected method(s)

        /// <summary>
        /// Does the dispose check.
        /// </summary>
        protected void DoDisposeCheck()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mSocketWrapper")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                mDisposed = true;
                mActive = false;
                mUdpClient.Close();
            }
        }

        #endregion

        #region Private method(s)

        private byte[] InternalReceive()
        {
            mReceiveEndpoint = new IPEndPoint(IPAddress.Any, 0);
            return mUdpClient.Receive(ref mReceiveEndpoint);
        }

        private void CheckForBroadcast(IPAddress ipAddress)
        {
            if (((Client != null) && !mIsBroadcast) && IPAddress.Broadcast.Equals(ipAddress))
            {
                mIsBroadcast = true;
                mUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
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

        private void CloseAsyncActiveSendEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveSendEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveSendEvent.Dispose();
                mAsyncActiveSendEvent = null;
            }
        }

        #endregion

    }

}
