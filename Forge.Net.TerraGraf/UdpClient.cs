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
using Forge.Net.Synapse;
using Forge.Net.Synapse.NetworkServices;

namespace Forge.Net.TerraGraf
{

    internal delegate int UdpClientSendDelegate(byte[] buffer, int offset, int size);
    internal delegate int UdpClientSendToDelegate(byte[] buffer, int offset, int size, AddressEndPoint remoteEp);

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

        private UdpClientSendDelegate mUdpClientSendDelegate = null;
        private int mAsyncActiveUdpClientSendCount = 0;
        private AutoResetEvent mAsyncActiveUdpClientSendEvent = null;

        private UdpClientSendToDelegate mUdpClientSendToDelegate = null;
        private int mAsyncActiveUdpClientSendToCount = 0;
        private AutoResetEvent mAsyncActiveUdpClientSendToEvent = null;

        private readonly object LOCK_SEND = new object();

        private readonly object LOCK_SENDTO = new object();

        private bool mDisposed = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClient"/> class.
        /// </summary>
        public UdpClient()
        {
            this.CreateClientSocket();
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

            this.CreateClientSocket();
            this.Connect(new AddressEndPoint(AddressEndPoint.Any, port));
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
            return this.mClient.BeginReceiveFrom(mBuffer, 0, MAX_UDP_SIZE, ref any, callback, state);
        }

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
                Buffer.BlockCopy(this.mBuffer, 0, dst, 0, count);
                return dst;
            }
            return this.mBuffer;
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
                Buffer.BlockCopy(this.mBuffer, 0, dst, 0, count);
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
            UdpClientSendDelegate d = new UdpClientSendDelegate(this.Send);
            if (this.mAsyncActiveUdpClientSendEvent == null)
            {
                lock (LOCK_SEND)
                {
                    if (this.mAsyncActiveUdpClientSendEvent == null)
                    {
                        this.mAsyncActiveUdpClientSendEvent = new AutoResetEvent(true);
                    }
                }
            }
            this.mAsyncActiveUdpClientSendEvent.WaitOne();
            this.mUdpClientSendDelegate = d;
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
            UdpClientSendToDelegate d = new UdpClientSendToDelegate(this.Send);
            if (this.mAsyncActiveUdpClientSendToEvent == null)
            {
                lock (LOCK_SENDTO)
                {
                    if (this.mAsyncActiveUdpClientSendToEvent == null)
                    {
                        this.mAsyncActiveUdpClientSendToEvent = new AutoResetEvent(true);
                    }
                }
            }
            this.mAsyncActiveUdpClientSendToEvent.WaitOne();
            this.mUdpClientSendToDelegate = d;
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
            if (this.mUdpClientSendDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndSend called multiple times.", "asyncResult");
            }

            try
            {
                return this.mUdpClientSendDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                this.mUdpClientSendDelegate = null;
                this.mAsyncActiveUdpClientSendEvent.Set();
                CloseAsyncActiveUdpClientSendEvent(Interlocked.Decrement(ref mAsyncActiveUdpClientSendCount));
            }
        }

        /// <summary>
        /// Ends the send.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>
        /// Number of sent bytes
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)")]
        public int EndSend(IAsyncResult asyncResult, AddressEndPoint remoteEp)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (this.mUdpClientSendToDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndSend called multiple times.", "asyncResult");
            }

            try
            {
                return this.mUdpClientSendToDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                this.mUdpClientSendToDelegate = null;
                this.mAsyncActiveUdpClientSendToEvent.Set();
                CloseAsyncActiveUdpClientSendToEvent(Interlocked.Decrement(ref mAsyncActiveUdpClientSendToCount));
            }
        }

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
                this.Connect(new AddressEndPoint(AddressEndPoint.Any, 0));
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
            this.mClient.Bind(localEp);
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
            this.mClient = new TerraGraf.Socket(SocketType.Dgram, ProtocolType.Udp);
        }

        private void CloseAsyncActiveUdpClientSendEvent(int asyncActiveCount)
        {
            if ((this.mAsyncActiveUdpClientSendEvent != null) && (asyncActiveCount == 0))
            {
                this.mAsyncActiveUdpClientSendEvent.Dispose();
                this.mAsyncActiveUdpClientSendEvent = null;
            }
        }

        private void CloseAsyncActiveUdpClientSendToEvent(int asyncActiveCount)
        {
            if ((this.mAsyncActiveUdpClientSendToEvent != null) && (asyncActiveCount == 0))
            {
                this.mAsyncActiveUdpClientSendToEvent.Dispose();
                this.mAsyncActiveUdpClientSendToEvent = null;
            }
        }

        private void DoDisposeCheck()
        {
            if (this.mDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        #endregion

    }

}
