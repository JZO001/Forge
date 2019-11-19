/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using Forge.Net.TerraGraf.Contexts;

namespace Forge.Net.TerraGraf.NetworkInfo
{

    /// <summary>
    /// Represents a peer context
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, StateId = '{StateId}']")]
    internal sealed class PeerContextContainer
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PeerContextContainer"/> class.
        /// </summary>
        internal PeerContextContainer()
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
        /// Gets or sets the peer context.
        /// </summary>
        /// <value>
        /// The peer context.
        /// </value>
        [DebuggerHidden]
        internal NetworkPeerDataContext PeerContext { get; set; }

        #endregion

    }

}
