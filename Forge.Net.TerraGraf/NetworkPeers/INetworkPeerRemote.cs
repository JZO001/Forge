/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represents a public remote peer
    /// </summary>
    public interface INetworkPeerRemote : INetworkPeer
    {

        /// <summary>
        /// Gets the distance.
        /// </summary>
        /// <value>
        /// The distance.
        /// </value>
        int Distance { get; }

        /// <summary>
        /// Gets the reply time.
        /// </summary>
        /// <value>
        /// The reply time.
        /// </value>
        long ReplyTime { get; }

        /// <summary>
        /// Gets the active network connection.
        /// </summary>
        /// <value>
        /// The active network connection.
        /// </value>
        INetworkConnection ActiveNetworkConnection { get; }

        /// <summary>
        /// Gets the network connections.
        /// </summary>
        /// <value>
        /// The network connections.
        /// </value>
        ICollection<INetworkConnection> NetworkConnections { get; }

    }

}
