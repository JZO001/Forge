/* *********************************************************************
 * Date: 10 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Diagnostics;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Event arguments for message task sending event
    /// </summary>
    internal sealed class MessageSendEventArgs : EventArgs
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly MessageTask mMessageTask = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly bool mMessageSent = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSendEventArgs"/> class.
        /// </summary>
        /// <param name="messageTask">The message task.</param>
        internal MessageSendEventArgs(MessageTask messageTask)
        {
            if (messageTask == null)
            {
                ThrowHelper.ThrowArgumentNullException("messageTask");
            }
            mMessageTask = messageTask;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSendEventArgs"/> class.
        /// </summary>
        /// <param name="messageTask">The message task.</param>
        /// <param name="messageSent">if set to <c>true</c> [message sent].</param>
        internal MessageSendEventArgs(MessageTask messageTask, bool messageSent)
            : this(messageTask)
        {
            mMessageSent = messageSent;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the message task.
        /// </summary>
        /// <value>
        /// The message task.
        /// </value>
        [DebuggerHidden]
        internal MessageTask MessageTask
        {
            get { return mMessageTask; }
        }

        /// <summary>
        /// Gets a value indicating whether [message sent].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [message sent]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        internal bool MessageSent
        {
            get { return mMessageSent; }
        }

        #endregion

    }

}
