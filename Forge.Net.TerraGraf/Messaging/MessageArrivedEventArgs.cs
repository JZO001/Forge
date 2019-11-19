/* *********************************************************************
 * Date: 10 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Event args for message arrived event
    /// </summary>
    [Serializable]
    internal class MessageArrivedEventArgs : EventArgs
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TerraGrafMessageBase mMessage = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageArrivedEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        internal MessageArrivedEventArgs(TerraGrafMessageBase message)
        {
            if (message == null)
            {
                ThrowHelper.ThrowArgumentNullException("message");
            }
            this.mMessage = message;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the message.
        /// </summary>
        [DebuggerHidden]
        internal TerraGrafMessageBase Message
        {
            get { return mMessage; }
        }

        #endregion

    }

}
