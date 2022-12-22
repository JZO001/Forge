/* *********************************************************************
 * Date: 24 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
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

    /// <summary>
    /// Represents a TCP client
    /// </summary>
    public sealed class TcpClient : MBRBase, ITcpClient, IDisposable
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Socket mClient = null;

        private Synapse.NetworkStream mStream = null;

        private System.Action<string, int> mConnectDelegate = null;
        private int mAsyncActiveConnectCount = 0;
        private AutoResetEvent mAsyncActiveConnectEvent = null;
        private readonly object LOCK_CONNECT = new object();

        private bool mDisposed = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClient"/> class.
        /// </summary>
        public TcpClient()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClient"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public TcpClient(int port)
        {
            if (!AddressEndPoint.ValidateTcpPort(port))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("port");
            }

            CreateClientSocket();
            mClient.Bind(new AddressEndPoint(AddressEndPoint.Any, port));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClient"/> class.
        /// </summary>
        /// <param name="acceptedSocket">The accepted socket.</param>
        internal TcpClient(Socket acceptedSocket)
        {
            if (acceptedSocket == null)
            {
                ThrowHelper.ThrowArgumentNullException("acceptedSocket");
            }
            mClient = acceptedSocket;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the available.
        /// </summary>
        /// <value>
        /// The available bytes.
        /// </value>
        public int Available
        {
            get { return mClient.Available; }
        }

        /// <summary>
        /// Gets the client.
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
        /// Gets a value indicating whether this <see cref="TcpClient"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected
        {
            get { return mClient.Connected; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [exclusive address use].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclusive address use]; otherwise, <c>false</c>.
        /// </value>
        public bool ExclusiveAddressUse
        {
            get
            {
                return mClient.ExclusiveAddressUse;
            }
            set
            {
                mClient.ExclusiveAddressUse = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [no delay].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no delay]; otherwise, <c>false</c>.
        /// </value>
        public bool NoDelay
        {
            get { return mClient.NoDelay; }
            set { mClient.NoDelay = value; }
        }

        /// <summary>
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        /// <value>
        /// The size of the receive buffer.
        /// </value>
        public int ReceiveBufferSize
        {
            get { return mClient.ReceiveBufferSize; }
            set { mClient.ReceiveBufferSize = value; }
        }

        /// <summary>
        /// Gets or sets the receive timeout.
        /// </summary>
        /// <value>
        /// The receive timeout.
        /// </value>
        public int ReceiveTimeout
        {
            get { return mClient.ReceiveTimeout; }
            set { mClient.ReceiveTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the size of the send buffer.
        /// </summary>
        /// <value>
        /// The size of the send buffer.
        /// </value>
        public int SendBufferSize
        {
            get { return mClient.SendBufferSize; }
            set { mClient.SendBufferSize = value; }
        }

        /// <summary>
        /// Gets or sets the send timeout.
        /// </summary>
        /// <value>
        /// The send timeout.
        /// </value>
        public int SendTimeout
        {
            get { return mClient.SendTimeout; }
            set { mClient.SendTimeout = value; }
        }

        #endregion

        #region Public method(s)

#if NET40

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginConnect(string host, int port, AsyncCallback callback, object state)
        {
            DoDisposeCheck();
            return mClient.BeginConnect(host, port, callback, state);
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        public IAsyncResult BeginConnect(AddressEndPoint localEp, AsyncCallback callback, object state)
        {
            DoDisposeCheck();
            return mClient.BeginConnect(localEp, callback, state);
        }

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        public void EndConnect(IAsyncResult asyncResult)
        {
            DoDisposeCheck();
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            mClient.EndConnect(asyncResult);
        }

#endif

        /// <summary>Begins the connect.</summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginConnect(string host, int port, ReturnCallback callback, object state)
        {
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
            mConnectDelegate = d;
            return d.BeginInvoke(host, port, callback, state);
        }

        /// <summary>Begins the connect.</summary>
        /// <param name="localEp">The local ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginConnect(AddressEndPoint localEp, ReturnCallback callback, object state)
        {
            if (localEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("localEp");
            }
            return BeginConnect(localEp.Host, localEp.Port, callback, state);
        }

        /// <summary>Ends the connect.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        public void EndConnect(ITaskResult asyncResult)
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
                mConnectDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mConnectDelegate = null;
                mAsyncActiveConnectEvent.Set();
                CloseAsyncActiveConnectEvent(Interlocked.Decrement(ref mAsyncActiveConnectCount));
            }
        }

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public async Task ConnectAsync(string host, int port)
        {
            await Task.Run(() => Connect(new AddressEndPoint(host, port)));
        }

        /// <summary>
        /// Connects the specified local ep.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        public async Task ConnectAsync(AddressEndPoint localEp)
        {
            if (localEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("localEp");
            }
            await Task.Run(() => Connect(localEp));
        }

#endif

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public void Connect(string host, int port)
        {
            Connect(new AddressEndPoint(host, port));
        }

        /// <summary>
        /// Connects the specified local ep.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        public void Connect(AddressEndPoint localEp)
        {
            DoDisposeCheck();
            mClient.Connect(localEp);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <returns>
        /// Network Stream instance
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Socket not connected.</exception>
        public Synapse.NetworkStream GetStream()
        {
            DoDisposeCheck();
            if (!mClient.Connected)
            {
                throw new InvalidOperationException("Socket not connected.");
            }
            if (mStream == null)
            {
                mStream = new Synapse.NetworkStream(mClient);
            }
            return mStream;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            mDisposed = true;
            if (mStream != null)
            {
                mStream.Dispose();
            }
            mClient.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private method(s)

        private void DoDisposeCheck()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private void CreateClientSocket()
        {
            mClient = new TerraGraf.Socket(SocketType.Stream, ProtocolType.Tcp);
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

    }

}
