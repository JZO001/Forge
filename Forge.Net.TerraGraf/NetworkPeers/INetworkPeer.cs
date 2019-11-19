/* *********************************************************************
 * Date: 21 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.TerraGraf.Contexts;

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represent shared properties of a network peer
    /// </summary>
    public interface INetworkPeer
    {

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        string Id { get; }

        /// <summary>
        /// Gets the name of the host.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        string HostName { get; }

        /// <summary>
        /// Gets the network context.
        /// </summary>
        NetworkContext NetworkContext { get; }

        /// <summary>
        /// Gets the peer context.
        /// </summary>
        /// <value>
        /// The peer context.
        /// </value>
        NetworkPeerDataContext PeerContext { get; }

        /// <summary>
        /// Gets the type of the peer.
        /// </summary>
        /// <value>
        /// The type of the peer.
        /// </value>
        PeerTypeEnum PeerType { get; }

    }

}
