/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the authentication modes
    /// </summary>
    [Serializable]
    public enum AuthenticationModeEnum
    {

        /// <summary>
        /// A password required for all incoming authentication request
        /// </summary>
        OnlyPassword = 0,

        /// <summary>
        /// A user name and password required for the authentication
        /// </summary>
        UsernameAndPassword,

        /// <summary>
        /// No authentication required
        /// </summary>
        Off

    }

}
