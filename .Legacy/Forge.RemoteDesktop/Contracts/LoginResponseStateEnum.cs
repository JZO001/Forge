/* *********************************************************************
 * Date: 06 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the response code of the login process
    /// </summary>
    [Serializable]
    public enum LoginResponseStateEnum
    {

        /// <summary>
        /// Login access denied
        /// </summary>
        AccessDenied = 0,

        /// <summary>
        /// Access granted for service
        /// </summary>
        AccessGranted,

        /// <summary>
        /// The service is busy
        /// </summary>
        ServiceBusy,

        /// <summary>
        /// The remote service state is stopped
        /// </summary>
        ServiceInactive

    }

}
