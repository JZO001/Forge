/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Delivery mode for a message
    /// </summary>
    [Serializable]
    internal enum MessageTypeEnum : int
    {
        /// <summary>
        /// Guaranteed and keep the order of the packages
        /// </summary>
        Tcp = 0,

        /// <summary>
        /// Not guaranted package type
        /// </summary>
        Udp
    }

}
