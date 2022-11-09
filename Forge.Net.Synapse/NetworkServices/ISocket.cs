/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Net;
using System.Net.Sockets;

namespace Forge.Net.Synapse.NetworkServices
{

    /// <summary>
    /// Basic socket functions
    /// </summary>
    public interface ISocket : IDisposable
    {

        #region Public method(s)

        /// <summary>
        /// Begins the accept.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginAccept(AsyncCallback callback, object state);

        /// <summary>
        /// Accepts a new incoming connection.
        /// </summary>
        /// <returns>Socket implementation</returns>
        ISocket Accept();

        /// <summary>
        /// Ends the accept.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Socket implementation</returns>
        ISocket EndAccept(IAsyncResult asyncResult);

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginConnect(EndPoint endPoint, AsyncCallback callBack, object state);

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginConnect(string host, int port, AsyncCallback callBack, object state);

        /// <summary>
        /// Binds the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        void Bind(EndPoint endPoint);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        void Connect(EndPoint endPoint);

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        void Connect(string host, int port);

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        void EndConnect(IAsyncResult asyncResult);

#if IS_WINDOWS

        /// <summary>
        /// Sets the keep alive values.
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <param name="keepAliveTime">The keep alive time.</param>
        /// <param name="keepAliveInterval">The keep alive interval.</param>
        /// <returns>value</returns>
        int SetKeepAliveValues(bool state, int keepAliveTime, int keepAliveInterval);

#endif

        /// <summary>
        /// Begins the receive.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginReceive(byte[] buffer, int offset, int size, AsyncCallback callBack, object state);

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
        IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, ref EndPoint remoteEp, AsyncCallback callBack, object state);

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginSend(byte[] buffer, int offset, int size, AsyncCallback callBack, object state);

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
        IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, EndPoint remoteEp, AsyncCallback callBack, object state);

        /// <summary>
        /// Receives the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Number of received bytes</returns>
        int Receive(byte[] buffer);

        /// <summary>
        /// Receives the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <returns>Number of received bytes</returns>
        int Receive(byte[] buffer, int offset, int size);

        /// <summary>
        /// Receives from.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        int ReceiveFrom(byte[] buffer, ref EndPoint remoteEp);

        /// <summary>
        /// Receives from.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        int ReceiveFrom(byte[] buffer, int offset, int size, ref EndPoint remoteEp);

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Number of sent bytes</returns>
        int Send(byte[] buffer);

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <returns>Number of sent bytes</returns>
        int Send(byte[] buffer, int offset, int size);

        /// <summary>
        /// Sends to.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        int SendTo(byte[] buffer, EndPoint remoteEp);

        /// <summary>
        /// Sends to.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        int SendTo(byte[] buffer, int offset, int size, EndPoint remoteEp);

        /// <summary>
        /// Ends the receive.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of received bytes</returns>
        int EndReceive(IAsyncResult asyncResult);

        /// <summary>
        /// Ends the receive from.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        int EndReceiveFrom(IAsyncResult asyncResult, ref EndPoint remoteEp);

        /// <summary>
        /// Ends the send.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of sent bytes</returns>
        int EndSend(IAsyncResult asyncResult);

        /// <summary>
        /// Ends the send to.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of sent bytes</returns>
        int EndSendTo(IAsyncResult asyncResult);

        /// <summary>
        /// Listens the specified backlog.
        /// </summary>
        /// <param name="backlog">The backlog.</param>
        void Listen(int backlog);

        /// <summary>
        /// Detect the status of the socket
        /// </summary>
        /// <param name="microSeconds">The micro seconds.</param>
        /// <param name="selectMode">The select mode.</param>
        /// <returns>True, if select mode state is valid</returns>
        bool Pool(int microSeconds, SelectMode selectMode);

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

        /// <summary>
        /// Close the socket and wait for the specified time.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        void Close(int timeout);

        /// <summary>
        /// Shutdowns the socket.
        /// </summary>
        /// <param name="how">The how.</param>
        void Shutdown(SocketShutdown how);

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the address family.
        /// </summary>
        AddressFamily AddressFamily { get; }

        /// <summary>
        /// Gets the available.
        /// </summary>
        int Available { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ISocket"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        bool Connected { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable broadcast].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable broadcast]; otherwise, <c>false</c>.
        /// </value>
        bool EnableBroadcast { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [exclusive address use].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclusive address use]; otherwise, <c>false</c>.
        /// </value>
        bool ExclusiveAddressUse { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is bound.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bound; otherwise, <c>false</c>.
        /// </value>
        bool IsBound { get; }

        /// <summary>
        /// Gets the local end point.
        /// </summary>
        AddressEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [no delay].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no delay]; otherwise, <c>false</c>.
        /// </value>
        bool NoDelay { get; set; }

        /// <summary>
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        /// <value>
        /// The size of the receive buffer.
        /// </value>
        int ReceiveBufferSize { get; set; }

        /// <summary>
        /// Gets or sets the receive timeout.
        /// </summary>
        /// <value>
        /// The receive timeout.
        /// </value>
        int ReceiveTimeout { get; set; }

        /// <summary>
        /// Gets or sets the remote end point.
        /// </summary>
        /// <value>
        /// The remote end point.
        /// </value>
        AddressEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Gets or sets the size of the send buffer.
        /// </summary>
        /// <value>
        /// The size of the send buffer.
        /// </value>
        int SendBufferSize { get; set; }

        /// <summary>
        /// Gets or sets the send timeout.
        /// </summary>
        /// <value>
        /// The send timeout.
        /// </value>
        int SendTimeout { get; set; }

        /// <summary>
        /// Gets the type of the protocol.
        /// </summary>
        /// <value>
        /// The type of the protocol.
        /// </value>
        ProtocolType ProtocolType { get; }

        /// <summary>
        /// Gets the type of the socket.
        /// </summary>
        /// <value>
        /// The type of the socket.
        /// </value>
        SocketType SocketType { get; }

        /// <summary>
        /// Gets or sets the TTL.
        /// </summary>
        /// <value>
        /// The TTL.
        /// </value>
        short Ttl { get; set; }

        #endregion

    }

}
