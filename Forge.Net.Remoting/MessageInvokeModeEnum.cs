/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Remoting
{

    /// <summary>
    /// Represents the direction of the call of a method.
    /// </summary>
    [Serializable]
    public enum MessageInvokeModeEnum
    {
        /// <summary>
        /// Client calls a remote method
        /// </summary>
        RequestService = 0,

        /// <summary>
        /// Remote service calls a client method
        /// </summary>
        RequestCallback
    }

}
