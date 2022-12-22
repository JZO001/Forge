/* *********************************************************************
 * Date: 06 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the arguments of a disconnect event
    /// </summary>
    [Serializable]
    public class DisconnectEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectEventArgs" /> class.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        public DisconnectEventArgs(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                ThrowHelper.ThrowArgumentNullException("sessionId");
            }

            SessionId = sessionId;
        }

        /// <summary>
        /// Gets the session id.
        /// </summary>
        /// <value>
        /// The session id.
        /// </value>
        public string SessionId { get; private set; }

    }

}
