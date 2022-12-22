/* *********************************************************************
 * Date: 08 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Threading.Tasking;
using System;
using System.Threading.Tasks;

namespace Forge.Net.Synapse.NetworkServices
{

    /// <summary>
    /// Represents a TcpListener service methods
    /// </summary>
    public interface ITcpListener
    {

#if NETCOREAPP3_1_OR_GREATER
        /// <summary>
        /// Accepts the socket.
        /// </summary>
        /// <returns>Socket instance</returns>
        Task<ISocket> AcceptSocketAsync();

        /// <summary>
        /// Accepts the TCP client.
        /// </summary>
        /// <returns>TcpClient implementation</returns>
        Task<ITcpClient> AcceptTcpClientAsync();
#endif

        /// <summary>
        /// Accepts the socket.
        /// </summary>
        /// <returns>Socket implementation</returns>
        ISocket AcceptSocket();

        /// <summary>
        /// Accepts the TCP client.
        /// </summary>
        /// <returns>TcpClient implementation</returns>
        ITcpClient AcceptTcpClient();

#if NET40

        /// <summary>
        /// Begins the accept socket.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginAcceptSocket(AsyncCallback callback, object state);

        /// <summary>
        /// Begins the accept TCP client.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginAcceptTcpClient(AsyncCallback callback, object state);

        /// <summary>
        /// Ends the accept socket.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Socket implementation</returns>
        ISocket EndAcceptSocket(IAsyncResult asyncResult);

        /// <summary>
        /// Ends the accept TCP client.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>TcpClient implementation</returns>
        ITcpClient EndAcceptTcpClient(IAsyncResult asyncResult);

#endif

        /// <summary>
        /// Begins the accept socket.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        ITaskResult BeginAcceptSocket(ReturnCallback callback, object state);

        /// <summary>
        /// Begins the accept TCP client.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        ITaskResult BeginAcceptTcpClient(ReturnCallback callback, object state);

        /// <summary>
        /// Ends the accept socket.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>Socket implementation</returns>
        ISocket EndAcceptSocket(ITaskResult asyncResult);

        /// <summary>
        /// Ends the accept TCP client.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>TcpClient implementation</returns>
        ITcpClient EndAcceptTcpClient(ITaskResult asyncResult);

        /// <summary>
        /// Pendings this instance.
        /// </summary>
        /// <returns>True, if an incoming connection is waiting</returns>
        bool Pending();

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start();

        /// <summary>
        /// Sets the specified backlog.
        /// </summary>
        /// <param name="backlog">The backlog.</param>
        void Start(int backlog);

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();

        /// <summary>
        /// Gets or sets a value indicating whether [exclusive address use].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclusive address use]; otherwise, <c>false</c>.
        /// </value>
        bool ExclusiveAddressUse { get; set; }

        /// <summary>
        /// Gets the local endpoint.
        /// </summary>
        /// <value>
        /// The local endpoint.
        /// </value>
        AddressEndPoint LocalEndpoint { get; }

        /// <summary>
        /// Gets the server socket.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        ISocket Server { get; }

    }

}
