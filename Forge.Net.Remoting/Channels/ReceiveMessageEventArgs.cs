/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using Forge.Net.Remoting.Messaging;

namespace Forge.Net.Remoting.Channels
{

    /// <summary>
    /// Event arguments for message receive
    /// </summary>
    [Serializable]
    public sealed class ReceiveMessageEventArgs : EventArgs
    {

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mSessionId = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IMessage mMessage = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiveMessageEventArgs"/> class.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="message">The message.</param>
        public ReceiveMessageEventArgs(string sessionId, IMessage message)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                ThrowHelper.ThrowArgumentNullException("sessionId");
            }
            if (message == null)
            {
                ThrowHelper.ThrowArgumentNullException("message");
            }
            if (message is AcknowledgeMessage) {
                ThrowHelper.ThrowArgumentException("Invalid message type.");
            }
            this.mSessionId = sessionId;
            this.mMessage = message;
        }

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
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DebuggerHidden]
        public IMessage Message
        {
            get { return mMessage; }
        }

    }

}
