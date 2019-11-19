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
    /// Represents gateway addresses of a peer
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, StateId = '{StateId}']")]
    internal sealed class NATGatewayContainer : MBRBase
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NATGatewayContainer"/> class.
        /// </summary>
        internal NATGatewayContainer()
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
        /// Gets or sets the gateways.
        /// </summary>
        /// <value>
        /// The gateways.
        /// </value>
        [DebuggerHidden]
        internal AddressEndPoint[] Gateways { get; set; }

        #endregion

    }

}
