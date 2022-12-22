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
    /// Represents TcpClient services
    /// </summary>
    public interface ITcpClient : IDisposable
    {

#if NET40

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginConnect(string host, int port, AsyncCallback callback, object state);

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginConnect(AddressEndPoint localEp, AsyncCallback callback, object state);

        /// <summary>Ends the connect.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        void EndConnect(IAsyncResult asyncResult);

#endif

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        ITaskResult BeginConnect(string host, int port, ReturnCallback callback, object state);

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        ITaskResult BeginConnect(AddressEndPoint localEp, ReturnCallback callback, object state);

        /// <summary>Ends the connect.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        void EndConnect(ITaskResult asyncResult);

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        Task ConnectAsync(string host, int port);

        /// <summary>
        /// Connects the specified local ep.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        Task ConnectAsync(AddressEndPoint localEp);

#endif

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        void Connect(string host, int port);

        /// <summary>
        /// Connects the specified local ep.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        void Connect(AddressEndPoint localEp);

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <returns>Network Stream instance</returns>
        Synapse.NetworkStream GetStream();

        /// <summary>
        /// Gets the available.
        /// </summary>
        /// <value>
        /// The available bytes.
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
        /// Gets a value indicating whether this <see cref="ITcpClient"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        bool Connected { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [exclusive address use].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclusive address use]; otherwise, <c>false</c>.
        /// </value>
        bool ExclusiveAddressUse { get; set; }

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

    }

}
