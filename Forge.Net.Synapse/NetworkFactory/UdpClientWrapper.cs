/* *********************************************************************
 * Date: 08 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Forge.Net.Synapse.NetworkServices;

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
            this.mUdpClient = udpClient;
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
            if (this.mActive && (remoteEp != null))
            {
                throw new InvalidOperationException("UdpClient is already connected.");
            }
            if (remoteEp == null)
            {
                return this.mUdpClient.Client.BeginSend(buffer, offset, size, SocketFlags.None, callback, state);
            }
            IPEndPoint ep = new IPEndPoint(Dns.GetHostAddresses(remoteEp.Host)[0], remoteEp.Port);
            this.CheckForBroadcast(ep.Address);
            return this.mUdpClient.Client.BeginSendTo(buffer, offset, size, SocketFlags.None, ep, callback, state);
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
            if (!this.mActive)
            {
                throw new InvalidOperationException("UdpClient not connected.");
            }
            return this.mUdpClient.Client.Send(buffer, offset, size, SocketFlags.None);
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
            if (this.mActive && (remoteEp != null))
            {
                throw new InvalidOperationException("UdpClient is already connected.");
            }
            if (remoteEp == null)
            {
                return this.mUdpClient.Client.Send(buffer, offset, size, SocketFlags.None);
            }
            IPEndPoint ep = new IPEndPoint(Dns.GetHostAddresses(remoteEp.Host)[0], remoteEp.Port);
            this.CheckForBroadcast(ep.Address);
            return this.mUdpClient.Client.SendTo(buffer, offset, size, SocketFlags.None, ep);
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
        /// Ends the receive.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of sent bytes</returns>
        public int EndSend(IAsyncResult asyncResult)
        {
            return mUdpClient.EndSend(asyncResult);
        }

        /// <summary>
        /// Ends the send.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">Not used.</param>
        /// <returns>Number of sent bytes</returns>
        public int EndSend(IAsyncResult asyncResult, AddressEndPoint remoteEp)
        {
            return mUdpClient.EndSend(asyncResult);
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
            if (this.mDisposed)
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
                mActive = false;
                mUdpClient.Close();
            }
        }

        #endregion

        #region Private method(s)

        private void CheckForBroadcast(IPAddress ipAddress)
        {
            if (((this.Client != null) && !this.mIsBroadcast) && IPAddress.Broadcast.Equals(ipAddress))
            {
                this.mIsBroadcast = true;
                this.mUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            }
        }

        #endregion

    }

}
