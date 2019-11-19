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
    /// Represent a viewpoint of current network peer
    /// </summary>
    [Serializable]
    internal sealed class TerraGrafNetworkInformation
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafNetworkInformation"/> class.
        /// </summary>
        internal TerraGrafNetworkInformation()
        {
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Myself
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        [DebuggerHidden]
        internal NetworkPeer Sender
        {
            get;
            set;
        }

        /// <summary>
        /// Known network computers
        /// </summary>
        /// <value>
        /// The known network peers.
        /// </value>
        [DebuggerHidden]
        internal NetworkPeer[] KnownNetworkPeers
        {
            get;
            set;
        }

        #endregion

    }

}
