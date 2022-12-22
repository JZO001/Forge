/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Legacy;
using System;
using System.Diagnostics;

namespace Forge.Net.TerraGraf.NetworkInfo
{

    /// <summary>
    /// Represents a peer relation container with state
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, StateId = '{StateId}']")]
    internal sealed class PeerRelationContainer : MBRBase
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PeerRelationContainer"/> class.
        /// </summary>
        internal PeerRelationContainer()
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
        internal long StateId { get; set; }

        /// <summary>
        /// Gets or sets the peer relations.
        /// </summary>
        /// <value>
        /// The peer relations.
        /// </value>
        [DebuggerHidden]
        internal PeerRelation[] PeerRelations { get; set; }

        #endregion

    }

}
