/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;
using Forge.Net.TerraGraf.Contexts;
using Forge.Threading;

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represents a public local network peer
    /// </summary>
    public interface INetworkPeerLocal : INetworkPeer
    {

        /// <summary>
        /// Gets the peer context lock.
        /// </summary>
        /// <value>
        /// The peer context lock.
        /// </value>
        ILock PeerContextLock { get; }

        /// <summary>
        /// Sets the peer context.
        /// </summary>
        /// <value>
        /// The peer context.
        /// </value>
        new NetworkPeerDataContext PeerContext { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is black hole.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is black hole; otherwise, <c>false</c>.
        /// </value>
        bool IsBlackHole { get; set; }

        /// <summary>
        /// Gets the TCP servers.
        /// </summary>
        /// <value>
        /// The TCP servers.
        /// </value>
        ICollection<TCPServer> TCPServers { get; }

    }

}
