using Forge.Management;
using Forge.Net.Synapse;
using Forge.Net.Synapse.NetworkServices;
using Forge.Net.TerraGraf.Configuration;
using Forge.Net.TerraGraf.Contexts;
using Forge.Net.TerraGraf.NetworkPeers;
using Forge.Threading.Tasking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Forge.Net.TerraGraf
{

    /// <summary>
    /// TerraGraf Network manager interface
    /// </summary>
    public interface ITerraGrafNetworkManager : INetworkManagerBase, IManager
    {

        /// <summary>
        /// Occurs when [network peer discovered].
        /// </summary>
        event EventHandler<NetworkPeerChangedEventArgs> NetworkPeerDiscovered;

        /// <summary>
        /// Occurs when [network peer distance changed].
        /// </summary>
        event EventHandler<NetworkPeerDistanceChangedEventArgs> NetworkPeerDistanceChanged;

        /// <summary>
        /// Occurs when [network peer unaccessible].
        /// </summary>
        event EventHandler<NetworkPeerChangedEventArgs> NetworkPeerUnaccessible;

        /// <summary>
        /// Occurs when [network peer context changed].
        /// </summary>
        event EventHandler<NetworkPeerContextEventArgs> NetworkPeerContextChanged;

        /// <summary>
        /// Gets the localhost.
        /// </summary>
        /// <value>
        /// The localhost.
        /// </value>
        INetworkPeerLocal Localhost { get; }

        /// <summary>
        /// Gets the current network context.
        /// </summary>
        /// <value>
        /// The local network context.
        /// </value>
        NetworkContext LocalNetworkContext { get; }

        /// <summary>
        /// Gets the active sockets.
        /// </summary>
        /// <value>
        /// The active sockets.
        /// </value>
        ICollection<ISocketSafeHandle> ActiveSockets { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is shutdown.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is shutdown; otherwise, <c>false</c>.
        /// </value>
        bool IsShutdown { get; }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>Identifier of the endpoint</returns>
        long StartServer(AddressEndPoint endPoint);

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="serverStreamFactory">The server stream factory.</param>
        /// <returns>Identifier of the endpoint</returns>
        long StartServer(AddressEndPoint endPoint, IServerStreamFactory serverStreamFactory);

        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <param name="serverId">The server id.</param>
        /// <returns>True, if the server stopped, otherwise False.</returns>
        bool StopServer(long serverId);

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
        /// <param name="timeoutInMS">The timeout in MS.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginConnect(AddressEndPoint endPoint, int timeoutInMS, AsyncCallback callback, object state);

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="timeoutInMS">The timeout in MS.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        IAsyncResult BeginConnect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory, int timeoutInMS, AsyncCallback callback, object state);

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>NetworkPeerRemote</returns>
        INetworkPeerRemote EndConnect(IAsyncResult asyncResult);

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
        /// <param name="timeoutInMS">The timeout in MS.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        ITaskResult BeginConnect(AddressEndPoint endPoint, int timeoutInMS, ReturnCallback callback, object state);

        /// <summary>
        /// Begins the connect.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="timeoutInMS">The timeout in MS.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async property</returns>
        ITaskResult BeginConnect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory, int timeoutInMS, ReturnCallback callback, object state);

        /// <summary>
        /// Ends the connect.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>NetworkPeerRemote</returns>
        INetworkPeerRemote EndConnect(ITaskResult asyncResult);

#if NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>NetworkPeerRemote</returns>
        Task<INetworkPeerRemote> ConnectAsync(AddressEndPoint endPoint);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <returns>NetworkPeerRemote</returns>
        Task<INetworkPeerRemote> ConnectAsync(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="timeoutInMS">The timeout in MS.</param>
        /// <returns>NetworkPeerRemote</returns>
        Task<INetworkPeerRemote> ConnectAsync(AddressEndPoint endPoint, int timeoutInMS);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="timeoutInMS">The timeout in MS.</param>
        /// <returns>NetworkPeerRemote</returns>
        Task<INetworkPeerRemote> ConnectAsync(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory, int timeoutInMS);

#endif

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>NetworkPeerRemote</returns>
        INetworkPeerRemote Connect(AddressEndPoint endPoint);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <returns>NetworkPeerRemote</returns>
        INetworkPeerRemote Connect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="timeoutInMS">The timeout in MS.</param>
        /// <returns>NetworkPeerRemote</returns>
        INetworkPeerRemote Connect(AddressEndPoint endPoint, int timeoutInMS);

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="clientStreamFactory">The client stream factory.</param>
        /// <param name="timeoutInMS">The timeout in MS.</param>
        /// <returns>NetworkPeerRemote</returns>
        INetworkPeerRemote Connect(AddressEndPoint endPoint, IClientStreamFactory clientStreamFactory, int timeoutInMS);

        /// <summary>
        /// Creates the socket TCP.
        /// </summary>
        /// <returns>Socket</returns>
        ISocket CreateSocketTcp();

        /// <summary>
        /// Creates the socket UDP.
        /// </summary>
        /// <returns>Socket</returns>
        ISocket CreateSocketUdp();

    }

}
