/* *********************************************************************
 * Date: 09 Jul 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.Net.TerraGraf.NetworkPeers;
using Forge.Shared;

namespace Forge.Net.TerraGraf
{

    /// <summary>
    /// Peer Distance changed eventArgs
    /// </summary>
    [Serializable]
    public class NetworkPeerDistanceChangedEventArgs : NetworkPeerChangedEventArgs
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IDictionary<INetworkPeerRemote, int> mNetworkPeersDistanceBefore = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IDictionary<INetworkPeerRemote, int> mNetworkPeersDistanceAfter = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPeerDistanceChangedEventArgs"/> class.
        /// </summary>
        /// <param name="networkPeers">The network peers.</param>
        /// <param name="networkPeersDistanceBefore">The network peers distance before.</param>
        /// <param name="networkPeersDistanceAfter">The network peers distance after.</param>
        public NetworkPeerDistanceChangedEventArgs(ICollection<INetworkPeerRemote> networkPeers,
            IDictionary<INetworkPeerRemote, int> networkPeersDistanceBefore,
            IDictionary<INetworkPeerRemote, int> networkPeersDistanceAfter)
            : base(networkPeers)
        {
            if (networkPeersDistanceBefore == null)
            {
                ThrowHelper.ThrowArgumentNullException("networkPeersDistanceBefore");
            }
            if (networkPeersDistanceAfter == null)
            {
                ThrowHelper.ThrowArgumentNullException("networkPeersDistanceAfter");
            }
            mNetworkPeersDistanceBefore = new Dictionary<INetworkPeerRemote, int>(networkPeersDistanceBefore);
            mNetworkPeersDistanceAfter = new Dictionary<INetworkPeerRemote, int>(networkPeersDistanceAfter);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the distance before.
        /// </summary>
        /// <value>
        /// The distance before.
        /// </value>
        [DebuggerHidden]
        public IDictionary<INetworkPeerRemote, int> DistanceBefore
        {
            get { return new Dictionary<INetworkPeerRemote, int>(mNetworkPeersDistanceBefore); }
        }

        /// <summary>
        /// Gets the distance after.
        /// </summary>
        /// <value>
        /// The distance after.
        /// </value>
        [DebuggerHidden]
        public IDictionary<INetworkPeerRemote, int> DistanceAfter
        {
            get { return new Dictionary<INetworkPeerRemote, int>(mNetworkPeersDistanceAfter); }
        }

        #endregion

    }

}
