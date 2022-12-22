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
    /// Tcp listener implementation
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public sealed class TcpListener : MBRBase, ITcpListener
    {

        #region Field(s)

        private AddressEndPoint mLocalEndpoint = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Socket mSocket = null;

        private bool mActive = false;

        private System.Func<ISocket> mAcceptSocketDelegate = null;
        private int mAsyncActiveAcceptSocketCount = 0;
        private AutoResetEvent mAsyncActiveAcceptSocketEvent = null;
        private readonly object LOCK_ACCEPT_SOCKET = new object();

        private System.Func<ITcpClient> mAcceptTcpClientDelegate = null;
        private int mAsyncActiveAcceptTcpClientCount = 0;
        private AutoResetEvent mAsyncActiveAcceptTcpClientEvent = null;
        private readonly object LOCK_ACCEPT_TCPCLIENT = new object();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpListener"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public TcpListener(int port)
        {
            AddressEndPoint.ValidateTcpPort(port);
            mSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            mLocalEndpoint = new AddressEndPoint(AddressEndPoint.Any, port);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets a value indicating whether [exclusive address use].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclusive address use]; otherwise, <c>false</c>.
        /// </value>
        public bool ExclusiveAddressUse
        {
            get { return mSocket.ExclusiveAddressUse; }
            set
            {
                if (mActive)
                {
                    throw new InvalidOperationException("TCP Listener must be stopped.");
                }
                mSocket.ExclusiveAddressUse = value;
            }
        }

        /// <summary>
        /// Gets the server socket.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        public ISocket Server
        {
            get { return mSocket; }
        }

        /// <summary>
        /// Gets the local endpoint.
        /// </summary>
        /// <value>
        /// The local endpoint.
        /// </value>
        public AddressEndPoint LocalEndpoint
        {
            get { return mActive ? mSocket.LocalEndPoint : mLocalEndpoint; }
        }

        #endregion

        #region Public method(s)

#if NETCOREAPP3_1_OR_GREATER
        /// <summary>
        /// Accepts the socket.
        /// </summary>
        /// <returns>Socket instance</returns>
        public async Task<ISocket> AcceptSocketAsync()
        {
            return await mSocket.AcceptAsync();
        }

        /// <summary>
        /// Accepts the TCP client.
        /// </summary>
        /// <returns>TcpClient implementation</returns>
        public async Task<ITcpClient> AcceptTcpClientAsync()
        {
            return await Task.Run(() => AcceptTcpClient());
        }
#endif

        /// <summary>
        /// Accepts the socket.
        /// </summary>
        /// <returns>
        /// Socket implementation
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Listener not active.</exception>
        public ISocket AcceptSocket()
        {
            if (!mActive)
            {
                throw new InvalidOperationException("Listener not active.");
            }
            return mSocket.Accept();
        }

        /// <summary>
        /// Accepts the TCP client.
        /// </summary>
        /// <returns>
        /// TcpClient implementation
        /// </returns>
        public ITcpClient AcceptTcpClient()
        {
            return new TcpClient((Socket)AcceptSocket());
        }

#if NET40

        /// <summary>
        /// Begins the accept socket.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        /// <exception cref="System.InvalidOperationException">TCP Listener is not active.</exception>
        public IAsyncResult BeginAcceptSocket(AsyncCallback callback, object state)
        {
            if (!mActive)
            {
                throw new InvalidOperationException("TCP Listener is not active.");
            }
            return mSocket.BeginAccept(callback, state);
        }

        /// <summary>
        /// Ends the accept socket.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>
        /// Socket implementation
        /// </returns>
        public ISocket EndAcceptSocket(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }

            return mSocket.EndAccept(asyncResult);
        }

        /// <summary>
        /// Begins the accept TCP client.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        /// <exception cref="System.InvalidOperationException">TCP Listener is not active.</exception>
        public IAsyncResult BeginAcceptTcpClient(AsyncCallback callback, object state)
        {
            if (!mActive)
            {
                throw new InvalidOperationException("TCP Listener is not active.");
            }
            return mSocket.BeginAccept(callback, state);
        }

        /// <summary>
        /// Ends the accept TCP client.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>
        /// TcpClient implementation
        /// </returns>
        public ITcpClient EndAcceptTcpClient(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            return new TcpClient((Socket)EndAcceptSocket(asyncResult));
        }

#endif

        /// <summary>Begins the accept socket.</summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginAcceptSocket(ReturnCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveAcceptSocketCount);
            System.Func<ISocket> d = new System.Func<ISocket>(AcceptSocket);
            if (mAsyncActiveAcceptSocketEvent == null)
            {
                lock (LOCK_ACCEPT_SOCKET)
                {
                    if (mAsyncActiveAcceptSocketEvent == null)
                    {
                        mAsyncActiveAcceptSocketEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveAcceptSocketEvent.WaitOne();
            mAcceptSocketDelegate = d;
            return d.BeginInvoke(callback, state);
        }

        /// <summary>Ends the accept socket.</summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Socket implementation</returns>
        public ISocket EndAcceptSocket(ITaskResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mAcceptSocketDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndAcceptSocket called multiple times.", "asyncResult");
            }
            try
            {
                return mAcceptSocketDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mAcceptSocketDelegate = null;
                mAsyncActiveAcceptSocketEvent.Set();
                CloseAsyncActiveAcceptSocketEvent(Interlocked.Decrement(ref mAsyncActiveAcceptSocketCount));
            }
        }

        /// <summary>Begins the accept TCP client.</summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public ITaskResult BeginAcceptTcpClient(ReturnCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveAcceptTcpClientCount);
            System.Func<ITcpClient> d = new System.Func<ITcpClient>(AcceptTcpClient);
            if (mAsyncActiveAcceptTcpClientEvent == null)
            {
                lock (LOCK_ACCEPT_TCPCLIENT)
                {
                    if (mAsyncActiveAcceptTcpClientEvent == null)
                    {
                        mAsyncActiveAcceptTcpClientEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveAcceptTcpClientEvent.WaitOne();
            mAcceptTcpClientDelegate = d;
            return d.BeginInvoke(callback, state);
        }

        /// <summary>Ends the accept TCP client.</summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>TcpClient implementation</returns>
        public ITcpClient EndAcceptTcpClient(ITaskResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (mAcceptTcpClientDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndAcceptTcpClient called multiple times.", "asyncResult");
            }
            try
            {
                return mAcceptTcpClientDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mAcceptTcpClientDelegate = null;
                mAsyncActiveAcceptTcpClientEvent.Set();
                CloseAsyncActiveAcceptTcpClientEvent(Interlocked.Decrement(ref mAsyncActiveAcceptTcpClientCount));
            }
        }

        /// <summary>
        /// Pendings this instance.
        /// </summary>
        /// <returns>
        /// True, if an incoming connection is waiting
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Listener not active.</exception>
        public bool Pending()
        {
            if (!mActive)
            {
                throw new InvalidOperationException("Listener not active.");
            }
            return mSocket.Pool(0, SelectMode.SelectRead);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            Start(0);
        }

        /// <summary>
        /// Starts the specified backlog.
        /// </summary>
        /// <param name="backlog">The backlog.</param>
        public void Start(int backlog)
        {
            if (backlog < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("backlog");
            }
            if (!mActive)
            {
                mSocket.Bind(mLocalEndpoint);
                mSocket.Listen(backlog);
                mActive = true;
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (mActive)
            {
                mSocket.Dispose();
                mSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                mActive = false;
            }
        }

        #endregion

        #region Private method(s)

        private void CloseAsyncActiveAcceptSocketEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveAcceptSocketEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveAcceptSocketEvent.Dispose();
                mAsyncActiveAcceptSocketEvent = null;
            }
        }

        private void CloseAsyncActiveAcceptTcpClientEvent(int asyncActiveCount)
        {
            if ((mAsyncActiveAcceptTcpClientEvent != null) && (asyncActiveCount == 0))
            {
                mAsyncActiveAcceptTcpClientEvent.Dispose();
                mAsyncActiveAcceptTcpClientEvent = null;
            }
        }

        #endregion

    }

}
