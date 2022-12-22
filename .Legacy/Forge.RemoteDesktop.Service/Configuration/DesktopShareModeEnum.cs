/* *********************************************************************
 * Date: 12 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.RemoteDesktop.Service.Configuration
{

    /// <summary>
    /// Represents the desktop share mode
    /// </summary>
    [Serializable]
    public enum DesktopShareModeEnum
    {
        /// <summary>
        /// Share desktop for all clients
        /// </summary>
        Shared = 0,

        /// <summary>
        /// Desktop accessible for the last logged in client. Current client will be disconnected.
        /// </summary>
        ExclusiveForLastLogin,

        /// <summary>
        /// Desktop accessible for the firstly logged in client. The service will be busy while the current client connected.
        /// </summary>
        ExclusiveForFirstLogin

    }

}
