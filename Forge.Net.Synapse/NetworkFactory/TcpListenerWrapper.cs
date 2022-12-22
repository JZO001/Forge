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
using System.Threading.Tasks;
using Forge.Legacy;
using Forge.Net.Synapse.NetworkServices;
using Forge.Shared;
using Forge.Threading.Tasking;
using Forge.Threading;

namespace Forge.Net.Synapse.NetworkFactory
{

#if NET40
    internal delegate ISocket TcpListenerAcceptSocketDelegate();
    internal delegate ITcpClient TcpListenerAcceptTcpClientDelegate();
#endif

    /// <summary>
    /// Wrapper for TcpListener
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TcpListenerWrapper : MBRBase, ITcpListener
    {

        #region Field(s)

#if NET40
        private TcpListenerAcceptSocketDelegate mAcceptSocketDelegate = null;
#endif
        private System.Func<ISocket> mAcceptSocketFuncDelegate = null;
        private int mAsyncActiveAcceptSocketCount = 0;
        private AutoResetEvent mAsyncActiveAcceptSocketEvent = null;

#if NET40
        private TcpListenerAcceptTcpClientDelegate mAcceptTcpClientDelegate = null;
#endif
        private System.Func<ITcpClient> mAcceptTcpClientFuncDelegate = null;
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
            mListener = listener;
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
            return new SocketWrapper(await mListener.AcceptSocketAsync());
        }

        /// <summary>
        /// Accepts the TCP client.
        /// </summary>
        /// <returns>TcpClient implementation</returns>
        public async Task<ITcpClient> AcceptTcpClientAsync()
        {
            return new TcpClientWrapper(await mListener.AcceptTcpClientAsync());
        }
#endif

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

#if NET40

        /// <summary>
        /// Begins the accept socket.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginAcceptSocket(AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveAcceptSocketCount);
            TcpListenerAcceptSocketDelegate d = new TcpListenerAcceptSocketDelegate(AcceptSocket);
            if (mAsyncActiveAcceptSocketEvent == null)
            {
                lock (LOCK_ACCEPTSOCKET)
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

        /// <summary>
        /// Begins the accept TCP client.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginAcceptTcpClient(AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveAcceptTcpClientCount);
            TcpListenerAcceptTcpClientDelegate d = new TcpListenerAcceptTcpClientDelegate(AcceptTcpClient);
            if (mAsyncActiveAcceptTcpClientEvent == null)
            {
                lock (LOCK_ACCEPTCLIENT)
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
                lock (LOCK_ACCEPTSOCKET)
                {
                    if (mAsyncActiveAcceptSocketEvent == null)
                    {
                        mAsyncActiveAcceptSocketEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveAcceptSocketEvent.WaitOne();
            mAcceptSocketFuncDelegate = d;
            return d.BeginInvoke(callback, state);
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
                lock (LOCK_ACCEPTCLIENT)
                {
                    if (mAsyncActiveAcceptTcpClientEvent == null)
                    {
                        mAsyncActiveAcceptTcpClientEvent = new AutoResetEvent(true);
                    }
                }
            }
            mAsyncActiveAcceptTcpClientEvent.WaitOne();
            mAcceptTcpClientFuncDelegate = d;
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
            if (mAcceptSocketFuncDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndAcceptSocket called multiple times.", "asyncResult");
            }
            try
            {
                return mAcceptSocketFuncDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mAcceptSocketFuncDelegate = null;
                mAsyncActiveAcceptSocketEvent.Set();
                CloseAsyncActiveAcceptSocketEvent(Interlocked.Decrement(ref mAsyncActiveAcceptSocketCount));
            }
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
            if (mAcceptTcpClientFuncDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndAcceptTcpClient called multiple times.", "asyncResult");
            }
            try
            {
                return mAcceptTcpClientFuncDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                mAcceptTcpClientFuncDelegate = null;
                mAsyncActiveAcceptTcpClientEvent.Set();
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
