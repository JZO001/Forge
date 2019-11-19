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
    /// Abstract base class for system messages
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, SenderId = '{SenderId}', MessageId = '{MessageId}', MessageCode = '{MessageCode}']")]
    internal abstract class MessageBase : IEquatable<MessageBase>
    {

        #region Field(s)

        protected static long mGlobalMessageId = DateTime.UtcNow.Ticks;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mSenderId = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mMessageId = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MessageCodeEnum mMessageCode = MessageCodeEnum.Undefinied;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="messageId">The message id.</param>
        /// <param name="messageCode">The message code.</param>
        protected MessageBase(string senderId, long messageId, MessageCodeEnum messageCode)
        {
            if (string.IsNullOrEmpty(senderId))
            {
                ThrowHelper.ThrowArgumentNullException("senderId");
            }
            this.mSenderId = senderId;
            this.mMessageId = messageId;
            this.mMessageCode = messageCode;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Üzenet küldő azonosítója
        /// </summary>
        /// <value>
        /// The sender id.
        /// </value>
        [DebuggerHidden]
        public string SenderId
        {
            get { return mSenderId; }
        }

        /// <summary>
        /// Üzenet azonosítója
        /// </summary>
        /// <value>
        /// The message id.
        /// </value>
        [DebuggerHidden]
        public long MessageId
        {
            get { return mMessageId; }
        }

        /// <summary>
        /// Üzenet kódja
        /// </summary>
        /// <value>
        /// The message code.
        /// </value>
        [DebuggerHidden]
        public MessageCodeEnum MessageCode
        {
            get { return mMessageCode; }
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
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            MessageBase other = (MessageBase)obj;
            return other.mSenderId == mSenderId && other.mMessageId == mMessageId && other.mMessageCode == mMessageCode;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True, if the other class is equals with this.</returns>
        public bool Equals(MessageBase other)
        {
            return Equals((object)other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = GetType().AssemblyQualifiedName.GetHashCode();
            hash = 39 * hash + mSenderId.GetHashCode();
            hash = 39 * hash + mMessageId.GetHashCode();
            hash = 39 * hash + mMessageCode.GetHashCode();
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
            StringBuilder sb = new StringBuilder(GetType().Name);
            sb.Append(", SenderId: [");
            sb.Append(mSenderId);
            sb.Append("], MessageId: ");
            sb.Append(mMessageId.ToString());
            sb.Append(", MessageCode: ");
            sb.Append(mMessageCode.ToString());
            return sb.ToString();
        }

        #endregion

    }

}
