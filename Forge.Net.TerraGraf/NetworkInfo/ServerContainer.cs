/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using Forge.Net.Synapse;

namespace Forge.Net.TerraGraf.NetworkInfo
{

    /// <summary>
    /// Represents the TCP listener servers of a peer
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, StateId = '{StateId}']")]
    internal class ServerContainer : MBRBase
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerContainer"/> class.
        /// </summary>
        internal ServerContainer()
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
        /// Gets or sets the servers.
        /// </summary>
        /// <value>
        /// The servers.
        /// </value>
        [DebuggerHidden]
        internal AddressEndPoint[] Servers { get; set; }

        #endregion

    }

}
