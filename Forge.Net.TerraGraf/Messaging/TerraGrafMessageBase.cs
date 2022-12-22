/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Text;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Represent a base message for high level terragraf messages
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, SenderId = '{SenderId}', MessageId = '{MessageId}', Priority = '{Priority}', MessageType = '{MessageType}']")]
    internal abstract class TerraGrafMessageBase : MessageBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MessagePriorityEnum mPriority = MessagePriorityEnum.Normal;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MessageTypeEnum mMessageType = MessageTypeEnum.Tcp;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mTargetId = string.Empty;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafMessageBase"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="targetId">The target id.</param>
        /// <param name="messageCode">The message code.</param>
        /// <param name="messageId">The message id.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="messageType">Type of the message.</param>
        protected TerraGrafMessageBase(string senderId, string targetId, MessageCodeEnum messageCode, long messageId,
            MessagePriorityEnum priority, MessageTypeEnum messageType)
            : base(senderId, messageId, messageCode)
        {
            mPriority = priority;
            mMessageType = messageType;
            mTargetId = (targetId == null ? string.Empty : targetId);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Priortiy level of the message
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        [DebuggerHidden]
        internal MessagePriorityEnum Priority
        {
            get { return mPriority; }
        }

        /// <summary>
        /// Type of the message
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        [DebuggerHidden]
        internal MessageTypeEnum MessageType
        {
            get { return mMessageType; }
        }

        /// <summary>
        /// Gets the target id.
        /// </summary>
        /// <value>
        /// The target id.
        /// </value>
        [DebuggerHidden]
        internal string TargetId
        {
            get { return mTargetId; }
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
                TerraGrafMessageBase other = (TerraGrafMessageBase)obj;
                result = other.mPriority == mPriority && other.mMessageType == mMessageType && other.mTargetId == mTargetId;
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
            hash = 31 * hash + mPriority.GetHashCode();
            hash = 31 * hash + mMessageType.GetHashCode();
            hash = 31 * hash + mTargetId.GetHashCode();
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
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append(", TargetId: [");
            sb.Append(mTargetId);
            sb.Append("], Priority: [");
            sb.Append(mPriority.ToString());
            sb.Append("], MessageType: [");
            sb.Append(mMessageType.ToString());
            sb.Append("]");
            return sb.ToString();
        }

        #endregion

    }

}
