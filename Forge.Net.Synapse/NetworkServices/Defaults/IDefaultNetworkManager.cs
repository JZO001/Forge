using Forge.Net.Synapse.Options;
using Forge.Threading.Tasking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forge.Net.Synapse.NetworkServices.Defaults
{

    /// <summary>
    /// Network manager native interface
    /// </summary>
    public interface IDefaultNetworkManager : INetworkManagerBase, IDisposable
    {

        /// <summary>
        /// Occurs when [network peer connected].
        /// </summary>
        event EventHandler<ConnectionEventArgs> NetworkPeerConnected;

        /// <summary>
        /// Gets the network factory.
        /// </summary>
        /// <value>
        /// The network factory.
        /// </value>
        INetworkFactoryBase NetworkFactory { get; }

        /// <summary>
        /// Gets or sets the server stream factory.
        /// </summary>
        /// <value>
        /// The server stream factory.
        /// </value>
        IServerStreamFactory ServerStreamFactory { get; set; }

        /// <summary>
        /// Gets or sets the client stream factory.
        /// </summary>
        /// <value>
        /// The client stream factory.
        /// </value>
        IClientStreamFactory ClientStreamFactory { get; set; }

        /// <summary>
        /// Gets or sets the socket keep alive time.
        /// </summary>
        /// <value>
        /// The socket keep alive time.
        /// </value>
        int SocketKeepAliveTime { get; set; }

        /// <summary>
        /// Gets or sets the socket keep alive time interval.
        /// </summary>
        /// <value>
        /// The socket keep alive time interval.
        /// </value>
        int SocketKeepAliveTimeInterval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use socket keep alive].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use socket keep alive]; otherwise, <c>false</c>.
        /// </value>
        bool UseSocketKeepAlive { get; set; }

        /// <summary>
        /// Gets or sets the default size of the buffer.
        /// </summary>
        /// <value>
        /// The default size of the buffer.
        /// </value>
        int SocketReceiveBufferSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the socket send buffer.
        /// </summary>
        /// <value>
        /// The size of the socket send buffer.
        /// </value>
        int SocketSendBufferSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [no delay].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no delay]; otherwise, <c>false</c>.
        /// </value>
        bool NoDelay { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        bool IsDisposed { get; }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="localPort">The local port.</param>
        /// <returns>Identifier of the listener</returns>
        long StartServer(int localPort);

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>Identifier of the listener</returns>
        long StartServer(AddressEndPoint endPoint);

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="serverStreamFactory">The server stream factory.</param>
        /// <returns>Identifier of the listener</returns>
        long StartServer(AddressEndPoint endPoint, IServerStreamFactory serverStreamFactory);

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="backlog">The maximum number of pending connections allowed in the waiting queue.</param>
        /// <param name="serverStreamFactory">The server stream factory.</param>
        /// <returns>Identifier of the listener</returns>
        long StartServer(AddressEndPoint endPoint, int backlog, IServerStreamFactory serverStreamFactory);

        /// <summary>Starts a set of TCP servers from options</summary>
        /// <param name="serverOptions">The server options.</param>
        /// <returns>
        ///   Dictionary with the original ServerOption and server id
        /// </returns>
        Dictionary<ServerOptions, long> StartTCPServers(IEnumerable<ServerOptions> serverOptions);

        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <param name="serverId">The server id.</param>
        /// <returns>True, if the server stopped, otherwise False.</returns>
        bool StopServer(long serverId);

        /// <summary>
        /// Stops the servers.
        /// </summary>
        void StopServers();

#if NET40

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginConnect(AddressEndPoint endPoint, AsyncCallback callback, object state);

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginConnect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory, AsyncCallback callback, object state);

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginConnect(AddressEndPoint endPoint, int bufferSize, AsyncCallback callback, object state);

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginConnect(AddressEndPoint endPoint, int bufferSize, IClientStreamFactory clientStreamFactory, AsyncCallback callback, object state);

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>
        /// Network Stream instance
        /// </returns>
        NetworkStream EndConnect(IAsyncResult asyncResult);

#endif

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        ITaskResult BeginConnect(AddressEndPoint endPoint, ReturnCallback callback, object state);

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        ITaskResult BeginConnect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory, ReturnCallback callback, object state);

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        ITaskResult BeginConnect(AddressEndPoint endPoint, int bufferSize, ReturnCallback callback, object state);

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        ITaskResult BeginConnect(AddressEndPoint endPoint, int bufferSize, IClientStreamFactory clientStreamFactory, ReturnCallback callback, object state);

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>
        /// Network Stream instance
        /// </returns>
        NetworkStream EndConnect(ITaskResult asyncResult);

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>Network Stream instance</returns>
        Task<NetworkStream> ConnectAsync(AddressEndPoint endPoint);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <returns>Network Stream instance</returns>
        Task<NetworkStream> ConnectAsync(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns>Network Stream instance</returns>
        Task<NetworkStream> ConnectAsync(AddressEndPoint endPoint, int bufferSize);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <returns>
        /// Network Stream instance
        /// </returns>
        Task<NetworkStream> ConnectAsync(AddressEndPoint endPoint, int bufferSize, IClientStreamFactory clientStreamFactory);

#endif

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>Network Stream instance</returns>
        NetworkStream Connect(AddressEndPoint endPoint);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <returns>Network Stream instance</returns>
        NetworkStream Connect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns>Network Stream instance</returns>
        NetworkStream Connect(AddressEndPoint endPoint, int bufferSize);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <returns>
        /// Network Stream instance
        /// </returns>
        NetworkStream Connect(AddressEndPoint endPoint, int bufferSize, IClientStreamFactory clientStreamFactory);

        /// <summary>
        /// Gets the server end point.
        /// </summary>
        /// <param name="serverId">The server id.</param>
        /// <returns>AddressEndPoint</returns>
        AddressEndPoint GetServerEndPoint(long serverId);

        /// <summary>
        /// Determines whether [is server end point exist] [the specified end point].
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>
        ///   <c>true</c> if [is server end point exist] [the specified end point]; otherwise, <c>false</c>.
        /// </returns>
        bool IsServerEndPointExist(AddressEndPoint endPoint);

    }

}
