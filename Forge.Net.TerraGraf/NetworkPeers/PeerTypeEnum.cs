/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represents the type of a peer
    /// </summary>
    [Serializable]
    public enum PeerTypeEnum : int
    {
        /// <summary>
        /// Local host
        /// </summary>
        Local = 0,

        /// <summary>
        /// Remote host
        /// </summary>
        Remote
    }

}
