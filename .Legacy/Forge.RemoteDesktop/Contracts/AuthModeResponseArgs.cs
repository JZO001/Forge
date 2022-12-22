/* *********************************************************************
 * Date: 6 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the authentifation information of a service
    /// </summary>
    [Serializable]
    public class AuthModeResponseArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthModeResponseArgs"/> class.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public AuthModeResponseArgs(AuthenticationModeEnum mode)
        {
            this.AuthenticationMode = mode;
        }

        /// <summary>
        /// Gets the authentication mode.
        /// </summary>
        /// <value>
        /// The authentication mode.
        /// </value>
        public AuthenticationModeEnum AuthenticationMode { get; private set; }

    }

}
