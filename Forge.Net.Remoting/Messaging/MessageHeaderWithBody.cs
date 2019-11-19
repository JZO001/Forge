/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;

namespace Forge.Net.Remoting.Messaging
{

    /// <summary>
    /// Message Header with body
    /// </summary>
    [Serializable]
    public sealed class MessageHeaderWithBody : MessageHeader
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte[] mData = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeaderWithBody"/> class.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="data">The data.</param>
        public MessageHeaderWithBody(MessageHeader messageHeader, byte[] data)
            : base(messageHeader.MessageSinkId, messageHeader.MessageLength, messageHeader.MessageSinkConfiguration)
        {
            if (data == null)
            {
                ThrowHelper.ThrowArgumentNullException("data");
            }
            this.mData = data;
        }

        #endregion
        
        #region Public properties

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [DebuggerHidden]
        public byte[] Data
        {
            get { return mData; }
        } 

        #endregion

    }

}
