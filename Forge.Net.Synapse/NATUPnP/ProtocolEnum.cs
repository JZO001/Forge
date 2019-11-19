/* *********************************************************************
 * Date: 14 Aug 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Synapse.NATUPnP
{

    /// <summary>
    /// Represents the supported ip protocol types for the UPnP mappings
    /// </summary>
    [Serializable]
    public enum ProtocolEnum
    {
        /// <summary>
        /// TCP protocol
        /// </summary>
        TCP = 0,

        /// <summary>
        /// UDP protocol
        /// </summary>
        UDP
    }

}
