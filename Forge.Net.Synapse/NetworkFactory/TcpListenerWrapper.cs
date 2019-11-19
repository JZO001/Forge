/* *********************************************************************
 * Date: 08 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using Forge.Net.Synapse.NetworkServices;

namespace Forge.Net.Synapse.NetworkFactory
{

    internal delegate ISocket TcpListenerAcceptSocketDelegate();
    internal delegate ITcpClient TcpListenerAcceptTcpClientDelegate();

    /// <summary>
    /// Wrapper for TcpListener
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TcpListenerWrapper : MBRBase, ITcpListener
    {

        #region Field(s)

        private TcpListenerAcceptSocketDelegate mAcceptSocketDelegate = null;
        private int mAsyncActiveAcceptSocketCount = 0;
        private AutoResetEvent mAsyncActiveAcceptSocketEvent = null;

        private TcpListenerAcceptTcpClientDelegate mAcceptTcpClientDelegate = null;
        private int mAsyncActiveAcceptTcpClientCount = 0;
        private AutoResetEvent mAsyncActiveAcceptTcpClientEvent = null;

        private TcpListener mListener = null;
        private SocketWrapper mServerSocketWrapper = null;

        private readonly object LOCK_ACCEPTSOCKET = new object();

        private readonly object LOCK_ACCEPTCLIENT = new object();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpListenerWrapper"/> class.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public TcpListenerWrapper(TcpListener listener)
        {
            if (listener == null)
            {
                ThrowHelper.ThrowArgumentNullException("listener");
            }
            this.mListener = listener;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Accepts the socket.
        /// </summary>
        /// <returns>Socket instance</returns>
        public ISocket AcceptSocket()
        {
            return new SocketWrapper(mListener.AcceptSocket());
        }

        /// <summary>
        /// Accepts the TCP client.
        /// </summary>
        /// <returns>TcpClient implementation</returns>
        public ITcpClient AcceptTcpClient()
        {
            return new TcpClientWrapper(mListener.AcceptTcpClient());
        }

        /// <summary>
        /// Begins the accept socket.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginAcceptSocket(AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveAcceptSocketCount);
            TcpListenerAcceptSocketDelegate d = new TcpListenerAcceptSocketDelegate(this.AcceptSocket);
            if (this.mAsyncActiveAcceptSocketEvent == null)
            {
                lock (LOCK_ACCEPTSOCKET)
                {
                    if (this.mAsyncActiveAcceptSocketEvent == null)
                    {
                        this.mAsyncActiveAcceptSocketEvent = new AutoResetEvent(true);
                    }
                }
            }
            this.mAsyncActiveAcceptSocketEvent.WaitOne();
            this.mAcceptSocketDelegate = d;
            return d.BeginInvoke(callback, state);
        }

        /// <summary>
        /// Begins the accept TCP client.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginAcceptTcpClient(AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveAcceptTcpClientCount);
            TcpListenerAcceptTcpClientDelegate d = new TcpListenerAcceptTcpClientDelegate(this.AcceptTcpClient);
            if (this.mAsyncActiveAcceptTcpClientEvent == null)
            {
                lock (LOCK_ACCEPTCLIENT)
                {
                    if (this.mAsyncActiveAcceptTcpClientEvent == null)
                    {
                        this.mAsyncActiveAcceptTcpClientEvent = new AutoResetEvent(true);
                    }
                }
            }
            this.mAsyncActiveAcceptTcpClientEvent.WaitOne();
            this.mAcceptTcpClientDelegate = d;
            return d.BeginInvoke(callback, state);
        }

        /// <summary>
        /// Ends the accept socket.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Socket implementation</returns>
        public ISocket EndAcceptSocket(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (this.mAcceptSocketDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndAcceptSocket called multiple times.", "asyncResult");
            }
            try
            {
                return this.mAcceptSocketDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                this.mAcceptSocketDelegate = null;
                this.mAsyncActiveAcceptSocketEvent.Set();
                CloseAsyncActiveAcceptSocketEvent(Interlocked.Decrement(ref mAsyncActiveAcceptSocketCount));
            }
        }

        /// <summary>
        /// Ends the accept TCP client.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>TcpClient implementation</returns>
        public ITcpClient EndAcceptTcpClient(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (this.mAcceptTcpClientDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndAcceptTcpClient called multiple times.", "asyncResult");
            }
            try
            {
                return this.mAcceptTcpClientDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                this.mAcceptTcpClientDelegate = null;
                this.mAsyncActiveAcceptTcpClientEvent.Set();
                CloseAsyncActiveAcceptTcpClientEvent(Interlocked.Decrement(ref mAsyncActiveAcceptTcpClientCount));
            }
        }

        /// <summary>
        /// Pendings this instance.
        /// </summary>
        /// <returns>True, if a connection waiting to receive, otherwise False.</returns>
        public bool Pending()
        {
            return mListener.Pending();
        }

        /// <summary>
        /// Starts listening.
        /// </summary>
        public void Start()
        {
            mListener.Start();
        }

        /// <summary>
        /// Starts the listening.
        /// </summary>
        /// <param name="backlog">The backlog value means, how many incoming connection queued.</param>
        public void Start(int backlog)
        {
            mListener.Start(backlog);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            mListener.Stop();
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
            get { return mListener.ExclusiveAddressUse; }
            set { mListener.ExclusiveAddressUse = value; }
        }

        /// <summary>
        /// Gets the local endpoint.
        /// </summary>
        public AddressEndPoint LocalEndpoint
        {
            get { return new AddressEndPoint(((IPEndPoint)mListener.LocalEndpoint).Address.ToString(), ((IPEndPoint)mListener.LocalEndpoint).Port, ((IPEndPoint)mListener.LocalEndpoint).AddressFamily); }
        }

        /// <summary>
        /// Gets the server socket.
        /// </summary>
        public ISocket Server
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (mServerSocketWrapper == null)
                {
                    mServerSocketWrapper = new SocketWrapper(mListener.Server);
                }
                return mServerSocketWrapper;
            }
        }

        #endregion

        #region Private method(s)

        private void CloseAsyncActiveAcceptSocketEvent(int asyncActiveCount)
        {
            if ((this.mAsyncActiveAcceptSocketEvent != null) && (asyncActiveCount == 0))
            {
                this.mAsyncActiveAcceptSocketEvent.Dispose();
                this.mAsyncActiveAcceptSocketEvent = null;
            }
        }

        private void CloseAsyncActiveAcceptTcpClientEvent(int asyncActiveCount)
        {
            if ((this.mAsyncActiveAcceptTcpClientEvent != null) && (asyncActiveCount == 0))
            {
                this.mAsyncActiveAcceptTcpClientEvent.Dispose();
                this.mAsyncActiveAcceptTcpClientEvent = null;
            }
        }

        #endregion

    }

}
