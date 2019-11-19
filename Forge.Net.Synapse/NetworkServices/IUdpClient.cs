/* *********************************************************************
 * Date: 08 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Synapse.NetworkServices
{

    /// <summary>
    /// Represents UdpClient services
    /// </summary>
    public interface IUdpClient : IDisposable
    {

        /// <summary>
        /// Gets the available.
        /// </summary>
        /// <value>
        /// The available.
        /// </value>
        int Available { get; }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        ISocket Client { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable broadcast].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable broadcast]; otherwise, <c>false</c>.
        /// </value>
        bool EnableBroadcast { get; set; }

        /// <summary>
        /// Gets or sets the TTL.
        /// </summary>
        /// <value>
        /// The TTL.
        /// </value>
        short Ttl { get; set; }

        /// <summary>
        /// Begins the receive.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginReceive(AsyncCallback callback, object state);

        /// <summary>
        /// Receives the specified remote ep.
        /// </summary>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        byte[] Receive(ref AddressEndPoint remoteEp);

        /// <summary>
        /// Ends the receive.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of received bytes</returns>
        byte[] EndReceive(IAsyncResult asyncResult, ref AddressEndPoint remoteEp);

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginSend(byte[] buffer, AsyncCallback callback, object state);

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginSend(byte[] buffer, int offset, int size, AsyncCallback callback, object state);

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginSend(byte[] buffer, int size, AsyncCallback callback, object state);

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginSend(byte[] buffer, AddressEndPoint remoteEp, AsyncCallback callback, object state);

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginSend(byte[] buffer, int size, AddressEndPoint remoteEp, AsyncCallback callback, object state);

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
        IAsyncResult BeginSend(byte[] buffer, int offset, int size, AddressEndPoint remoteEp, AsyncCallback callback, object state);

        /// <summary>
        /// Begins the send.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginSend(byte[] buffer, string hostName, int port, AsyncCallback callback, object state);

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
        IAsyncResult BeginSend(byte[] buffer, int size, string hostName, int port, AsyncCallback callback, object state);

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
        IAsyncResult BeginSend(byte[] buffer, int offset, int size, string hostName, int port, AsyncCallback callback, object state);

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
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <returns>Number of sent bytes</returns>
        int Send(byte[] buffer, int size);

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        int Send(byte[] buffer, AddressEndPoint remoteEp);

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        int Send(byte[] buffer, int size, AddressEndPoint remoteEp);

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        int Send(byte[] buffer, int offset, int size, AddressEndPoint remoteEp);

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <returns>Number of sent bytes</returns>
        int Send(byte[] buffer, string hostName, int port);

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <returns>Number of sent bytes</returns>
        int Send(byte[] buffer, int size, string hostName, int port);

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        /// <returns>Number of sent bytes</returns>
        int Send(byte[] buffer, int offset, int size, string hostName, int port);

        /// <summary>
        /// Ends the receive.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Number of sent bytes</returns>
        int EndSend(IAsyncResult asyncResult);

        /// <summary>
        /// Ends the send.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>Number of sent bytes</returns>
        int EndSend(IAsyncResult asyncResult, AddressEndPoint remoteEp);

        /// <summary>
        /// Connects the specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        void Connect(int port);

        /// <summary>
        /// Connects the specified local ep.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        void Connect(AddressEndPoint localEp);

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

    }

}
