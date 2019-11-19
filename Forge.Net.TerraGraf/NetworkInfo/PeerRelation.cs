/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;

namespace Forge.Net.TerraGraf.NetworkInfo
{

    /// <summary>
    /// Represent a relation between two network peer
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, StateId = '{StateId}', PeerA = '{PeerA}', PeerB = '{PeerB}', Connected = '{Connected}']")]
    internal sealed class PeerRelation : MBRBase
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PeerRelation"/> class.
        /// </summary>
        /// <param name="stateId">The state id.</param>
        /// <param name="peerA">The peer A.</param>
        /// <param name="peerB">The peer B.</param>
        /// <param name="status">if set to <c>true</c> [status].</param>
        internal PeerRelation(long stateId, string peerA, string peerB, bool status)
        {
            this.StateId = stateId;
            this.PeerA = peerA;
            this.PeerB = peerB;
            this.Connected = status;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets or sets the stream id.
        /// </summary>
        /// <value>
        /// The stream id.
        /// </value>
        [DebuggerHidden]
        internal long StateId { get; set; }

        /// <summary>
        /// Gets or sets the peer A.
        /// </summary>
        /// <value>
        /// The peer A.
        /// </value>
        [DebuggerHidden]
        internal string PeerA { get; set; }

        /// <summary>
        /// Gets or sets the peer B.
        /// </summary>
        /// <value>
        /// The peer B.
        /// </value>
        [DebuggerHidden]
        internal string PeerB { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PeerRelation"/> is status.
        /// </summary>
        /// <value>
        ///   <c>true</c> if status; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        internal bool Connected { get; set; }

        #endregion

    }

}
