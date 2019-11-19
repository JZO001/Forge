/* *********************************************************************
 * Date: 06 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the login request arguments
    /// </summary>
    [Serializable]
    public sealed class LoginRequestArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginRequestArgs" /> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public LoginRequestArgs(string userName, string password)
        {
            this.DeviceId = ApplicationHelper.ApplicationId;
            this.UserName = userName;
            this.Password = password;
        }

        /// <summary>
        /// Gets the device id.
        /// </summary>
        /// <value>
        /// The device id.
        /// </value>
        public string DeviceId { get; private set; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; private set; }

    }

}
