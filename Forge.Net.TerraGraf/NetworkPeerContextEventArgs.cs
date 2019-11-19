/* *********************************************************************
 * Date: 22 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using Forge.Net.TerraGraf.NetworkPeers;

namespace Forge.Net.TerraGraf
{

    /// <summary>
    /// Network peer data context change event argument
    /// </summary>
    [Serializable]
    public sealed class NetworkPeerContextEventArgs : EventArgs
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private INetworkPeerRemote mNetworkPeer = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPeerContextEventArgs"/> class.
        /// </summary>
        /// <param name="networkPeer">The network peer.</param>
        public NetworkPeerContextEventArgs(INetworkPeerRemote networkPeer)
        {
            if (networkPeer == null)
            {
                ThrowHelper.ThrowArgumentNullException("networkPeer");
            }
            this.mNetworkPeer = networkPeer;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets the network peer.
        /// </summary>
        /// <value>
        /// The network peer.
        /// </value>
        public INetworkPeerRemote NetworkPeer
        {
            get { return mNetworkPeer; }
        }

        #endregion

    }

}
