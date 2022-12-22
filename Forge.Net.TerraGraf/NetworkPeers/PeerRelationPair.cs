/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using Forge.Net.TerraGraf.NetworkInfo;
using Forge.Shared;

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represents a relation between two network peer
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, StateId = '{StateId}', PeerA = '{PeerA.Id}', PeerB = '{PeerB.Id}', Connected = '{Connected}']")]
    internal sealed class PeerRelationPair
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkPeerRemote mPeerA = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkPeerRemote mPeerB = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mConnected = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mStateId = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PeerRelationPair"/> class.
        /// </summary>
        /// <param name="peerA">The peer A.</param>
        /// <param name="peerB">The peer B.</param>
        internal PeerRelationPair(NetworkPeerRemote peerA, NetworkPeerRemote peerB)
        {
            if (peerA == null)
            {
                ThrowHelper.ThrowArgumentNullException("peerA");
            }
            if (peerB == null)
            {
                ThrowHelper.ThrowArgumentNullException("peerB");
            }
            mPeerA = peerA;
            mPeerB = peerB;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the peer A.
        /// </summary>
        /// <value>
        /// The peer A.
        /// </value>
        [DebuggerHidden]
        internal NetworkPeerRemote PeerA
        {
            get { return mPeerA; }
        }

        /// <summary>
        /// Gets the peer B.
        /// </summary>
        /// <value>
        /// The peer B.
        /// </value>
        [DebuggerHidden]
        internal NetworkPeerRemote PeerB
        {
            get { return mPeerB; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PeerRelationPair"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        internal bool Connected
        {
            get { return mConnected; }
            set { mConnected = value; }
        }

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

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Builds the peer relation.
        /// </summary>
        /// <returns>PeerRelation</returns>
        internal PeerRelation BuildPeerRelation()
        {
            return new PeerRelation(this.mStateId, this.mPeerA.Id, this.mPeerB.Id, this.mConnected);
        }

        #endregion

    }

}
