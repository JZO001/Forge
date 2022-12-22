/* *********************************************************************
 * Date: 24 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Forge.Legacy;
using Forge.Net.Synapse;
using Forge.Net.Synapse.NetworkServices;
using Forge.Shared;
using Forge.Threading.Tasking;
using Forge.Threading;

namespace Forge.Net.TerraGraf
{

#if NET40
    internal delegate int UdpClientSendDelegate(byte[] buffer, int offset, int size);
    internal delegate int UdpClientSendToDelegate(byte[] buffer, int offset, int size, AddressEndPoint remoteEp);
#endif

    /// <summary>
    /// Represents an UDP client
    /// </summary>
    public sealed class UdpClient : MBRBase, IUdpClient, IDisposable
    {

        #region Field(s)

        private static readonly int MAX_UDP_SIZE = 65536;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Socket mClient = null;

        private byte[] mBuffer = new byte[MAX_UDP_SIZE];

#if NET40
        private UdpClientSendDelegate mSendDelegateOld = null;
        private UdpClientSendToDelegate mSendToDelegateOld = null;
#endif

        private System.Func<byte[], int, int, int> mSendDelegate = null;
        private int mAsyncActiveUdpClientSendCount = 0;
        private AutoResetEvent mAsyncActiveUdpClientSendEvent = null;
        private readonly object LOCK_SEND = new object();

        private System.Func<byte[], int, int, AddressEndPoint, int> mSendToDelegate = null;
        private int mAsyncActiveUdpClientSendToCount = 0;
        private AutoResetEvent mAsyncActiveUdpClientSendToEvent = null;
        private readonly object LOCK_SENDTO = new object();

        private bool mDisposed = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClient"/> class.
        /// </summary>
        public UdpClient()
        {
            CreateClientSocket();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClient"/> class.
        /// </summary>
        /// <param name="port">The port where this instance will listen.</param>
        public UdpClient(int port)
        {
            if (!AddressEndPoint.ValidateTcpPort(port))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("port");
            }

            CreateClientSocket();
            Connect(new AddressEndPoint(AddressEndPoint.Any, port));
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the available.
        /// </summary>
        /// <value>
        /// The available.
        /// </value>
        public int Available
        {
            get { return mClient.Available; }
        }

        /// <summary>
        /// Gets the underlying socket.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        [DebuggerHidden]
        public ISocket Client
        {
            get { return mClient; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable broadcast].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable broadcast]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableBroadcast
        {
            get { return mClient.EnableBroadcast; }
            set { mClient.EnableBroadcast = value; }
        }

        /// <summary>
        /// Gets or sets the TTL.
        /// </summary>
        /// <value>
        /// The TTL.
        /// </value>
        public short Ttl
        {
            get { return mClient.Ttl; }
            set { mClient.Ttl = value; }
        }

        #endregion

        #region Public method(s)

#if NET40

        /// <summary>
        /// Begins the receive.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            DoDisposeCheck();
            EndPoint any = new AddressEndPoint(AddressEndPoint.Any, 0);
            return mClient.BeginReceiveFrom(mBuffer, 0, MAX_UDP_SIZE, ref any, callback, state);
        }

        /// <summary>
        /// Ends the receive.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>
        /// Number of received bytes
        /// </returns>
        public byte[] EndReceive(IAsyncResult asyncResult, ref AddressEndPoint remoteEp)
        {
            DoDisposeCheck();
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }

            EndPoint refEp = null;
            int count = mClient.EndReceiveFrom(asyncResult, ref refEp);
            remoteEp = (AddressEndPoint)refEp;
            if (count < MAX_UDP_SIZE)
            {
                byte[] dst = new byte[count];
                Buffer.BlockCopy(mBuffer, 0, dst, 0, count);
                return dst;
            }
            return mBuffer;
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginSend(byte[] buffer, AsyncCallback callback, object state)
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
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginSend(byte[] buffer, int size, AsyncCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, size, callback, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
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

            Interlocked.Increment(ref mAsyncActiveUdpClientSendCount);
            UdpClientSendDelegate d = new UdpClientSendDelegate(Send);
            if (mAsyncActiveUdpClientSendEvent == null)
            {
                lock (LOCK_SEND)
                {
                    if (mAsyncActiveUdpClientSendEvent == null)
                    {
                        mAsyncActiveUdpClientSendEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveUdpClientSendEvent.WaitOne();
            mSendDelegateOld = d;
            return d.BeginInvoke(buffer, offset, size, callback, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginSend(byte[] buffer, AddressEndPoint remoteEp, AsyncCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, buffer.Length, remoteEp, callback, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginSend(byte[] buffer, int size, AddressEndPoint remoteEp, AsyncCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, size, remoteEp, callback, state);
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
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, AddressEndPoint remoteEp, AsyncCallback callback, object state)
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

            Interlocked.Increment(ref mAsyncActiveUdpClientSendToCount);
            UdpClientSendToDelegate d = new UdpClientSendToDelegate(Send);
            if (mAsyncActiveUdpClientSendToEvent == null)
            {
                lock (LOCK_SENDTO)
                {
                    if (mAsyncActiveUdpClientSendToEvent == null)
                    {
                        mAsyncActiveUdpClientSendToEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveUdpClientSendToEvent.WaitOne();
            mSendToDelegateOld = d;
            return d.BeginInvoke(buffer, offset, size, remoteEp, callback, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
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
        /// <returns>
        /// Async property
        /// </returns>
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
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, string hostName, int port, AsyncCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, offset, size, new AddressEndPoint(hostName, port), callback, state);
        }

        /// <summary>
        /// Ends the receive.
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
                mAsyncActiveUdpClientSendEvent.Set();
                CloseAsyncActiveUdpClientSendEvent(Interlocked.Decrement(ref mAsyncActiveUdpClientSendCount));
            }
        }

#endif

        /// <summary>Begins the receive.</summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginReceive(ReturnCallback callback, object state)
        {
            DoDisposeCheck();
            EndPoint any = new AddressEndPoint(AddressEndPoint.Any, 0);
            return mClient.BeginReceiveFrom(mBuffer, 0, MAX_UDP_SIZE, ref any, callback, state);
        }

        /// <summary>Ends the receive.</summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        public byte[] EndReceive(ITaskResult asyncResult, ref AddressEndPoint remoteEp)
        {
            DoDisposeCheck();
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }

            EndPoint refEp = null;
            int count = mClient.EndReceiveFrom(asyncResult, ref refEp);
            remoteEp = (AddressEndPoint)refEp;
            if (count < MAX_UDP_SIZE)
            {
                byte[] dst = new byte[count];
                Buffer.BlockCopy(mBuffer, 0, dst, 0, count);
                return dst;
            }
            return mBuffer;
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginSend(byte[] buffer, ReturnCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, buffer.Length, callback, state);
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginSend(byte[] buffer, int offset, int size, ReturnCallback callback, object state)
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

            Interlocked.Increment(ref mAsyncActiveUdpClientSendCount);
            System.Func<byte[], int, int, int> d = new System.Func<byte[], int, int, int>(Send);
            if (mAsyncActiveUdpClientSendEvent == null)
            {
                lock (LOCK_SEND)
                {
                    if (mAsyncActiveUdpClientSendEvent == null)
                    {
                        mAsyncActiveUdpClientSendEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveUdpClientSendEvent.WaitOne();
            mSendDelegate = d;
            return d.BeginInvoke(buffer, offset, size, callback, state);
        }

        /// <summary>Begins the send.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginSend(byte[] buffer, int size, ReturnCallback callback, object state)
        {
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return BeginSend(buffer, 0, size, callback, state);
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
            return BeginSend(buffer, 0, buffer.Length, remoteEp, callback, state);
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
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginSend(byte[] buffer, int offset, int size, AddressEndPoint remoteEp, ReturnCallback callback, object state)
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

            Interlocked.Increment(ref mAsyncActiveUdpClientSendToCount);
            System.Func<byte[], int, int, AddressEndPoint, int> d = new System.Func<byte[], int, int, AddressEndPoint, int>(Send);
            if (mAsyncActiveUdpClientSendToEvent == null)
            {
                lock (LOCK_SENDTO)
                {
                    if (mAsyncActiveUdpClientSendToEvent == null)
                    {
                        mAsyncActiveUdpClientSendToEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveUdpClientSendToEvent.WaitOne();
            mSendToDelegate = d;
            return d.BeginInvoke(buffer, offset, size, remoteEp, callback, state);
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
                mAsyncActiveUdpClientSendEvent.Set();
                CloseAsyncActiveUdpClientSendEvent(Interlocked.Decrement(ref mAsyncActiveUdpClientSendCount));
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
        /// <returns>
        /// Number of received bytes
        /// </returns>
        public byte[] Receive(ref AddressEndPoint remoteEp)
        {
            DoDisposeCheck();
            if (remoteEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("remoteEp");
            }
            EndPoint refEp = new AddressEndPoint(AddressEndPoint.Any, 0);
            int count = mClient.ReceiveFrom(mBuffer, ref refEp);
            remoteEp = (AddressEndPoint)refEp;
            if (count < MAX_UDP_SIZE)
            {
                byte[] dst = new byte[count];
                Buffer.BlockCopy(mBuffer, 0, dst, 0, count);
                return dst;
            }
            return mBuffer;
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
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        public int Send(byte[] buffer)
        {
            DoDisposeCheck();
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return mClient.Send(buffer);
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        public int Send(byte[] buffer, int size)
        {
            DoDisposeCheck();
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            return Send(buffer, 0, size);
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
        /// <exception cref="System.InvalidOperationException">Socket not connected.</exception>
        public int Send(byte[] buffer, int offset, int size)
        {
            DoDisposeCheck();
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if (!mClient.Connected)
            {
                throw new InvalidOperationException("Socket not connected.");
            }
            return mClient.Send(buffer, offset, size);
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        public int Send(byte[] buffer, AddressEndPoint remoteEp)
        {
            DoDisposeCheck();
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if (remoteEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("remoteEp");
            }

            if (!mClient.IsBound)
            {
                Connect(new AddressEndPoint(AddressEndPoint.Any, 0));
            }

            return mClient.SendTo(buffer, remoteEp);
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        public int Send(byte[] buffer, int size, AddressEndPoint remoteEp)
        {
            return Send(buffer, 0, size, remoteEp);
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
        public int Send(byte[] buffer, int offset, int size, AddressEndPoint remoteEp)
        {
            DoDisposeCheck();
            if (buffer == null)
            {
                ThrowHelper.ThrowArgumentNullException("buffer");
            }
            if (remoteEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("remoteEp");
            }

            if (!mClient.IsBound)
            {
                Connect(new AddressEndPoint(AddressEndPoint.Any, 0));
            }

            return mClient.SendTo(buffer, offset, size, remoteEp);
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
        public int Send(byte[] buffer, string hostName, int port)
        {
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
        public int Send(byte[] buffer, int size, string hostName, int port)
        {
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
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        public int Send(byte[] buffer, int offset, int size, string hostName, int port)
        {
            return Send(buffer, offset, size, new AddressEndPoint(hostName, port));
        }

        /// <summary>
        /// Bind the socket to the specified port and receives UDP datagrams.
        /// </summary>
        /// <param name="port">The port.</param>
        public void Connect(int port)
        {
            Connect(new AddressEndPoint(AddressEndPoint.Any, port));
        }

        /// <summary>
        /// Bind the socket to the specified port and receives UDP datagrams from the specified network peer.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        public void Connect(AddressEndPoint localEp)
        {
            DoDisposeCheck();
            mClient.Bind(localEp);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mAsyncActiveUdpClientSendToEvent"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mAsyncActiveUdpClientSendEvent")]
        public void Dispose()
        {
            if (!mDisposed)
            {
                mDisposed = true;
                mClient.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private method(s)

        private void CreateClientSocket()
        {
            mClient = new TerraGraf.Socket(SocketType.Dgram, ProtocolType.Udp);
        }

        private void CloseAsyncActiveUdpClientSendEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveUdpClientSendEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveUdpClientSendEvent.Dispose();
                mAsyncActiveUdpClientSendEvent = null;
            }
        }

        private void CloseAsyncActiveUdpClientSendToEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveUdpClientSendToEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveUdpClientSendToEvent.Dispose();
                mAsyncActiveUdpClientSendToEvent = null;
            }
        }

        private void DoDisposeCheck()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        #endregion

    }

}
