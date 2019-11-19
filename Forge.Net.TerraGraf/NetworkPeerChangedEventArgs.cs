/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.Net.TerraGraf.NetworkPeers;

namespace Forge.Net.TerraGraf
{

    /// <summary>
    /// EventArgs for network peer changed event
    /// </summary>
    [Serializable]
    public class NetworkPeerChangedEventArgs : EventArgs
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ICollection<INetworkPeerRemote> mNetworkPeers = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPeerChangedEventArgs"/> class.
        /// </summary>
        /// <param name="networkPeers">The network peers.</param>
        public NetworkPeerChangedEventArgs(ICollection<INetworkPeerRemote> networkPeers)
        {
            if (networkPeers == null)
            {
                ThrowHelper.ThrowArgumentNullException("networkPeers");
            }
            this.mNetworkPeers = networkPeers;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the network peers.
        /// </summary>
        /// <value>
        /// The network peers.
        /// </value>
        [DebuggerHidden]
        public ICollection<INetworkPeerRemote> NetworkPeers
        {
            get { return new List<INetworkPeerRemote>(mNetworkPeers); }
        }

        #endregion

    }

}
