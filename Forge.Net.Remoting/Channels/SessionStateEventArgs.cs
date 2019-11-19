/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;

namespace Forge.Net.Remoting.Channels
{

    /// <summary>
    /// Represents the event arguments of the session state event
    /// </summary>
    [Serializable]
    public sealed class SessionStateEventArgs : EventArgs
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mSessionId = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mConnected = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionStateEventArgs"/> class.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="connected">if set to <c>true</c> [connected].</param>
        public SessionStateEventArgs(string sessionId, bool connected)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                ThrowHelper.ThrowArgumentNullException("sessionId");
            }
            this.mSessionId = sessionId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the session id.
        /// </summary>
        /// <value>
        /// The session id.
        /// </value>
        [DebuggerHidden]
        public string SessionId
        {
            get { return mSessionId; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsConnected
        {
            get { return mConnected; }
        }

        #endregion

    }

}
