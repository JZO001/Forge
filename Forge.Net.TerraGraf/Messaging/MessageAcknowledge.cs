/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Diagnostics;
using System.Threading;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Represents an acknowledge message which confirms a request
    /// </summary>
    [Serializable]
    internal sealed class MessageAcknowledge : TerraGrafMessageBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mMessageIdToAck = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageAcknowledge"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="targetId">The target id.</param>
        /// <param name="messageIdToAck">The message id to ack.</param>
        public MessageAcknowledge(string senderId, string targetId, long messageIdToAck)
            : base(senderId, targetId, MessageCodeEnum.Acknowledge, Interlocked.Increment(ref mGlobalMessageId), MessagePriorityEnum.High, MessageTypeEnum.Tcp)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                ThrowHelper.ThrowArgumentNullException("targetId");
            }
            mMessageIdToAck = messageIdToAck;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the message id to ack.
        /// </summary>
        /// <value>
        /// The message id to ack.
        /// </value>
        [DebuggerHidden]
        public long MessageIdToAck
        {
            get { return mMessageIdToAck; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            bool result = false;
            if (base.Equals(obj))
            {
                MessageAcknowledge other = (MessageAcknowledge)obj;
                result = other.mMessageIdToAck == mMessageIdToAck;
            }
            return result;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash = 30 * hash + mMessageIdToAck.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, MessageToAck: {1}", base.ToString(), mMessageIdToAck);
        }

        #endregion

    }

}
