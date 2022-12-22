/* *********************************************************************
 * Date: 06 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the response information of the login
    /// </summary>
    [Serializable]
    public class LoginResponseArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginResponseArgs"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        public LoginResponseArgs(LoginResponseStateEnum state)
        {
            ResponseState = state;
        }

        /// <summary>
        /// Gets the state of the response.
        /// </summary>
        /// <value>
        /// The state of the response.
        /// </value>
        public LoginResponseStateEnum ResponseState { get; private set; }

    }

}
