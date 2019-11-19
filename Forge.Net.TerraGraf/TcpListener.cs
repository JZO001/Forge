/* *********************************************************************
 * Date: 24 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Net.Sockets;
using Forge.Net.Synapse;
using Forge.Net.Synapse.NetworkServices;

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
            if (!this.mActive)
            {
                throw new InvalidOperationException("TCP Listener is not active.");
            }
            return mSocket.BeginAccept(callback, state);
        }

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
            if (!this.mActive)
            {
                throw new InvalidOperationException("TCP Listener is not active.");
            }
            return mSocket.BeginAccept(callback, state);
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

    }

}
