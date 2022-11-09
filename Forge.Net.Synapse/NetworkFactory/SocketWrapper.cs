/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using Forge.Net.Synapse.NetworkServices;

namespace Forge.Net.Synapse.NetworkFactory
{

    /// <summary>
    /// Wrapper class for Socket.NET
    /// </summary>
    public sealed class SocketWrapper : MBRBase, ISocket
    {

        #region Field(s)

        private Socket mSocket = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketWrapper"/> class.
        /// </summary>
        /// <param name="socket">The socket.</param>
        public SocketWrapper(Socket socket)
        {
            if (socket == null)
            {
                ThrowHelper.ThrowArgumentNullException("socket");
            }
            this.mSocket = socket;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SocketWrapper"/> is reclaimed by garbage collection.
        /// </summary>
        ~SocketWrapper()
        {
            Dispose(false);
        }

        #endregion

        #region Public member(s)

        /// <summary>
        /// Gets the address family.
        /// </summary>
        public AddressFamily AddressFamily
        {
            get { return mSocket.AddressFamily; }
        }

        /// <summary>
        /// Begins the accept.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            return mSocket.BeginAccept(callback, state);
        }

        /// <summary>
        /// Accepts a new incoming connection.
        /// </summary>
        /// <returns>Socket implementation</returns>
        public ISocket Accept()
        {
            return new SocketWrapper(mSocket.Accept());
        }

        /// <summary>
        /// Ends the accept.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Socket implementation</returns>
        public ISocket EndAccept(IAsyncResult asyncResult)
        {
            return new SocketWrapper(mSocket.EndAccept(asyncResult));
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(System.Net.EndPoint endPoint, AsyncCallback callBack, object state)
        {
            return mSocket.BeginConnect(endPoint, callBack, state);
        }

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginConnect(string host, int port, AsyncCallback callBack, object state)
        {
            return mSocket.BeginConnect(host, port, callBack, state);
        }

        /// <summary>
        /// Binds the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        public void Bind(System.Net.EndPoint endPoint)
        {
            mSocket.Bind(endPoint);
        }

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        public void Connect(System.Net.EndPoint endPoint)
        {
            mSocket.Connect(endPoint);
        }

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public void Connect(string host, int port)
        {
            mSocket.Connect(host, port);
        }

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        public void EndConnect(IAsyncResult asyncResult)
        {
            mSocket.EndConnect(asyncResult);
        }

        /// <summary>
        /// Begins the receive.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, AsyncCallback callBack, object state)
        {
            return mSocket.BeginReceive(buffer, offset, size, SocketFlags.None, callBack, state);
        }

        /// <summary>
        /// Receives from.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, ref System.Net.EndPoint remoteEp, AsyncCallback callBack, object state)
        {
            return mSocket.BeginReceiveFrom(buffer, offset, size, SocketFlags.None, ref remoteEp, callBack, state);
        }

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, AsyncCallback callBack, object state)
        {
            return mSocket.BeginSend(buffer, offset, size, SocketFlags.None, callBack, state);
        }

        /// <summary>
        /// Begins the send to.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        public IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, System.Net.EndPoint remoteEp, AsyncCallback callBack, object state)
        {
            return mSocket.BeginSendTo(buffer, offset, size, SocketFlags.None, remoteEp, callBack, state);
        }

        /// <summary>
        /// Receives the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Number of received bytes</returns>
        public int Receive(byte[] buffer)
        {
            return mSocket.Receive(buffer);
        }

        /// <summary>
        /// Receives the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <returns>Number of received bytes</returns>
        public int Receive(byte[] buffer, int offset, int size)
        {
            return mSocket.Receive(buffer, offset, size, SocketFlags.None);
        }

        /// <summary>
        /// Receives from.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        public int ReceiveFrom(byte[] buffer, ref System.Net.EndPoint remoteEp)
        {
            return mSocket.ReceiveFrom(buffer, ref remoteEp);
        }

        /// <summary>
        /// Receives from.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        public int ReceiveFrom(byte[] buffer, int offset, int size, ref System.Net.EndPoint remoteEp)
        {
            return mSocket.ReceiveFrom(buffer, offset, size, SocketFlags.None, ref remoteEp);
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Number of sent bytes</returns>
        public int Send(byte[] buffer)
        {
            return mSocket.Send(buffer);
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
            return mSocket.Send(buffer, offset, size, SocketFlags.None);
        }

        /// <summary>
        /// Sends to.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        public int SendTo(byte[] buffer, System.Net.EndPoint remoteEp)
        {
            return mSocket.SendTo(buffer, remoteEp);
        }

        /// <summary>
        /// Sends to.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        public int SendTo(byte[] buffer, int offset, int size, System.Net.EndPoint remoteEp)
        {
            return mSocket.SendTo(buffer, offset, size, SocketFlags.None, remoteEp);
        }

        /// <summary>
        /// Ends the receive.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of received bytes</returns>
        public int EndReceive(IAsyncResult asyncResult)
        {
            return mSocket.EndReceive(asyncResult);
        }

        /// <summary>
        /// Ends the receive from.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        public int EndReceiveFrom(IAsyncResult asyncResult, ref System.Net.EndPoint remoteEp)
        {
            return mSocket.EndReceiveFrom(asyncResult, ref remoteEp);
        }

        /// <summary>
        /// Ends the send.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of received bytes</returns>
        public int EndSend(IAsyncResult asyncResult)
        {
            return mSocket.EndSend(asyncResult);
        }

        /// <summary>
        /// Ends the send to.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of received bytes</returns>
        public int EndSendTo(IAsyncResult asyncResult)
        {
            return mSocket.EndSendTo(asyncResult);
        }

        /// <summary>
        /// Listens the specified backlog.
        /// </summary>
        /// <param name="backlog">The backlog.</param>
        public void Listen(int backlog)
        {
            mSocket.Listen(backlog);
        }

        /// <summary>
        /// Detect the status of the socket
        /// </summary>
        /// <param name="microSeconds">The micro seconds.</param>
        /// <param name="selectMode">The select mode.</param>
        /// <returns>True, if the socket is active, otherwise False.</returns>
        public bool Pool(int microSeconds, SelectMode selectMode)
        {
            return mSocket.Poll(microSeconds, selectMode);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            mSocket.Close();
        }

        /// <summary>
        /// Close the socket and wait for the specified time.
        /// </summary>
        /// <param name="timeout">The timeout value.</param>
        public void Close(int timeout)
        {
            mSocket.Close(timeout);
        }

        /// <summary>
        /// Gets the available bytes.
        /// </summary>
        public int Available
        {
            get { return mSocket.Available; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ISocket"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected
        {
            get { return mSocket.Connected; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable broadcast].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable broadcast]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableBroadcast
        {
            get { return mSocket.EnableBroadcast; }
            set { mSocket.EnableBroadcast = value; }
        }

        /// <summary>
        /// Gets the local end point.
        /// </summary>
        public AddressEndPoint LocalEndPoint
        {
            get { return new AddressEndPoint(((IPEndPoint)mSocket.LocalEndPoint).Address.ToString(), ((IPEndPoint)mSocket.LocalEndPoint).Port, ((IPEndPoint)mSocket.LocalEndPoint).AddressFamily); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [no delay].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no delay]; otherwise, <c>false</c>.
        /// </value>
        public bool NoDelay
        {
            get { return mSocket.NoDelay; }
            set { mSocket.NoDelay = value; }
        }

        /// <summary>
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        /// <value>
        /// The size of the receive buffer.
        /// </value>
        public int ReceiveBufferSize
        {
            get { return mSocket.ReceiveBufferSize; }
            set { mSocket.ReceiveBufferSize = value; }
        }

        /// <summary>
        /// Gets or sets the receive timeout.
        /// </summary>
        /// <value>
        /// The receive timeout.
        /// </value>
        public int ReceiveTimeout
        {
            get { return mSocket.ReceiveTimeout; }
            set { mSocket.ReceiveTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the remote end point.
        /// </summary>
        /// <value>
        /// The remote end point.
        /// </value>
        public AddressEndPoint RemoteEndPoint
        {
            get { return new AddressEndPoint(((IPEndPoint)mSocket.RemoteEndPoint).Address.ToString(), ((IPEndPoint)mSocket.RemoteEndPoint).Port, ((IPEndPoint)mSocket.RemoteEndPoint).AddressFamily); }
        }

        /// <summary>
        /// Gets or sets the size of the send buffer.
        /// </summary>
        /// <value>
        /// The size of the send buffer.
        /// </value>
        public int SendBufferSize
        {
            get { return mSocket.SendBufferSize; }
            set { mSocket.SendBufferSize = value; }
        }

        /// <summary>
        /// Gets or sets the send timeout.
        /// </summary>
        /// <value>
        /// The send timeout.
        /// </value>
        public int SendTimeout
        {
            get { return mSocket.SendTimeout; }
            set { mSocket.SendTimeout = value; }
        }

        /// <summary>
        /// Gets the type of the protocol.
        /// </summary>
        /// <value>
        /// The type of the protocol.
        /// </value>
        public ProtocolType ProtocolType
        {
            get { return mSocket.ProtocolType; }
        }

        /// <summary>
        /// Gets the type of the socket.
        /// </summary>
        /// <value>
        /// The type of the socket.
        /// </value>
        public SocketType SocketType
        {
            get { return mSocket.SocketType; }
        }

        /// <summary>
        /// Gets or sets the TTL.
        /// </summary>
        /// <value>
        /// The TTL.
        /// </value>
        public short Ttl
        {
            get { return mSocket.Ttl; }
            set { mSocket.Ttl = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [exclusive address use].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclusive address use]; otherwise, <c>false</c>.
        /// </value>
        public bool ExclusiveAddressUse
        {
            get { return mSocket.ExclusiveAddressUse; }
            set { mSocket.ExclusiveAddressUse = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is bound.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bound; otherwise, <c>false</c>.
        /// </value>
        public bool IsBound
        {
            get { return mSocket.IsBound; }
        }

        /// <summary>
        /// Shutdowns the socket.
        /// </summary>
        /// <param name="how">The how.</param>
        public void Shutdown(SocketShutdown how)
        {
            mSocket.Shutdown(how);
        }

#if IS_WINDOWS

        /// <summary>
        /// Sets the keep alive values.
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <param name="keepAliveTime">The keep alive time.</param>
        /// <param name="keepAliveInterval">The keep alive interval.</param>
        /// <returns></returns>
        public int SetKeepAliveValues(bool state, int keepAliveTime, int keepAliveInterval)
        {
            if (keepAliveTime < 1000)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("keepAliveTime");
            }
            if (keepAliveInterval < 1000)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("keepAliveInterval");
            }

            TcpKeepAlive keepAlive = new TcpKeepAlive();
            keepAlive.State = Convert.ToUInt32(state);
            keepAlive.KeepAliveTime = Convert.ToUInt32(keepAliveTime);
            keepAlive.KeepAliveInterval = Convert.ToUInt32(keepAliveInterval);

            return mSocket.IOControl(IOControlCode.KeepAliveValues, keepAlive.ToArray(), null);
        }

#endif

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private method(s)

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                mSocket.Dispose();
            }
        }

        #endregion

    }

}
