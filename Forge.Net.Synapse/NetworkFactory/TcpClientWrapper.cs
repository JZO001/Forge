/* *********************************************************************
 * Date: 08 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Forge.Net.Synapse.NetworkServices;

namespace Forge.Net.Synapse.NetworkFactory
{

    /// <summary>
    /// Wrapper for Tcp Client
    /// </summary>
    public class TcpClientWrapper : MBRBase, ITcpClient
    {

        #region Field(s)

        private TcpClient mTcpClient = null;
        private NetworkStream mNetworkStream = null;
        private SocketWrapper mSocketWrapper = null;
        private bool mDisposed = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClientWrapper"/> class.
        /// </summary>
        /// <param name="tcpClient">The TCP client.</param>
        public TcpClientWrapper(TcpClient tcpClient)
        {
            if (tcpClient == null)
            {
                ThrowHelper.ThrowArgumentNullException("tcpClient");
            }
            this.mTcpClient = tcpClient;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="TcpClientWrapper"/> is reclaimed by garbage collection.
        /// </summary>
        ~TcpClientWrapper()
        {
            Dispose(false);
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(string host, int port, AsyncCallback callback, object state)
        {
            return mTcpClient.BeginConnect(host, port, callback, state);
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(AddressEndPoint localEp, AsyncCallback callback, object state)
        {
            if (localEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("localEp");
            }
            return BeginConnect(localEp.Host, localEp.Port, callback, state);
        }

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public void Connect(string host, int port)
        {
            mTcpClient.Connect(host, port);
        }

        /// <summary>
        /// Connects the specified local ep.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        public void Connect(AddressEndPoint localEp)
        {
            if (localEp == null)
            {
                ThrowHelper.ThrowArgumentNullException("localEp");
            }
            Connect(localEp.Host, localEp.Port);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            mTcpClient.Close();
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <returns>Network Stream instance</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public NetworkStream GetStream()
        {
            DoDisposeCheck();
            if (mNetworkStream == null)
            {
                mNetworkStream = new NetworkStream(this.Client);
            }
            return mNetworkStream;
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
            get { return mTcpClient.Available; }
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
                    mSocketWrapper = new SocketWrapper(mTcpClient.Client);
                }
                return mSocketWrapper;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ITcpClient"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected
        {
            get { return mTcpClient.Connected; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [exclusive address use].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclusive address use]; otherwise, <c>false</c>.
        /// </value>
        public bool ExclusiveAddressUse
        {
            get { return mTcpClient.ExclusiveAddressUse; }
            set { mTcpClient.ExclusiveAddressUse = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [no delay].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no delay]; otherwise, <c>false</c>.
        /// </value>
        public bool NoDelay
        {
            get { return mTcpClient.NoDelay; }
            set { mTcpClient.NoDelay = value; }
        }

        /// <summary>
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        /// <value>
        /// The size of the receive buffer.
        /// </value>
        public int ReceiveBufferSize
        {
            get { return mTcpClient.ReceiveBufferSize; }
            set { mTcpClient.ReceiveBufferSize = value; }
        }

        /// <summary>
        /// Gets or sets the receive timeout.
        /// </summary>
        /// <value>
        /// The receive timeout.
        /// </value>
        public int ReceiveTimeout
        {
            get { return mTcpClient.ReceiveTimeout; }
            set { mTcpClient.ReceiveTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the size of the send buffer.
        /// </summary>
        /// <value>
        /// The size of the send buffer.
        /// </value>
        public int SendBufferSize
        {
            get { return mTcpClient.SendBufferSize; }
            set { mTcpClient.SendBufferSize = value; }
        }

        /// <summary>
        /// Gets or sets the send timeout.
        /// </summary>
        /// <value>
        /// The send timeout.
        /// </value>
        public int SendTimeout
        {
            get { return mTcpClient.SendTimeout; }
            set { mTcpClient.SendTimeout = value; }
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
                throw new ObjectDisposedException(this.GetType().FullName);
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
                if (mNetworkStream != null)
                {
                    mNetworkStream.Dispose();
                }
                mTcpClient.Close();
            }
        }

        #endregion

    }

}
