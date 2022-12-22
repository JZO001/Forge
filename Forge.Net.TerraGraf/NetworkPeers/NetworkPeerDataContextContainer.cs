/* *********************************************************************
 * Date: 10 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using Forge.Legacy;
using Forge.Net.TerraGraf.Contexts;
using Forge.Net.TerraGraf.NetworkInfo;

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represent a peer data context container
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, StateId = '{StateId}']")]
    internal sealed class NetworkPeerDataContextContainer : MBRBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mStateId = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPeerDataContextContainer"/> class.
        /// </summary>
        internal NetworkPeerDataContextContainer()
        {
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets or sets the state id.
        /// </summary>
        /// <value>
        /// The state id.
        /// </value>
        [DebuggerHidden]
        internal long StateId
        {
            get { return mStateId; }
            set { mStateId = value; }
        }

        /// <summary>
        /// Gets or sets the peer context.
        /// </summary>
        /// <value>
        /// The peer context.
        /// </value>
        [DebuggerHidden]
        internal NetworkPeerDataContext PeerContext { get; set; }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Builds the peer context container.
        /// </summary>
        /// <returns>PeerContextContainer</returns>
        internal PeerContextContainer BuildPeerContextContainer()
        {
            PeerContextContainer result = new PeerContextContainer();

            result.StateId = this.StateId;
            result.PeerContext = (PeerContext == null ? null : (NetworkPeerDataContext)PeerContext.Clone());

            return result;
        }

        #endregion

    }

}
