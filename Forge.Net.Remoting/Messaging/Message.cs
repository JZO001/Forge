/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Forge.Net.Remoting.Messaging
{

    /// <summary>
    /// Base class for messages
    /// </summary>
    [Serializable]
    public abstract class Message : IMessage
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mCorrelationId = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MessageTypeEnum mMessageType = MessageTypeEnum.Request;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, object> mContext = new Dictionary<string, object>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        protected Message()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        protected Message(MessageTypeEnum messageType)
            : this(CreateNewCorrelationId(), messageType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="correlationId">The correlation id.</param>
        /// <param name="messageType">Type of the message.</param>
        protected Message(string correlationId, MessageTypeEnum messageType)
        {
            this.mCorrelationId = correlationId;
            this.mMessageType = messageType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="correlationId">The correlation unique identifier.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="allowParallelExecution">if set to <c>true</c> [allow parallel execution].</param>
        protected Message(string correlationId, MessageTypeEnum messageType, bool allowParallelExecution)
            : this(correlationId, messageType)
        {
            this.AllowParallelExecution = allowParallelExecution;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Correlation of the message
        /// </summary>
        /// <value>
        /// The correlation id.
        /// </value>
        [DebuggerHidden]
        public string CorrelationId
        {
            get { return mCorrelationId; }
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        [DebuggerHidden]
        public MessageTypeEnum MessageType
        {
            get { return mMessageType; }
        }

        /// <summary>
        /// Get the call context of the message
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        [DebuggerHidden]
        public Dictionary<string, object> Context
        {
            get { return mContext; }
        }

        /// <summary>
        /// Gets a value indicating whether [allow parallel execution].
        /// </summary>
        /// <value>
        /// <c>true</c> if [allow parallel execution]; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        [DebuggerHidden]
        public bool AllowParallelExecution
        {
            get;
            protected set;
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

            Message other = (Message)obj;
            return other.mCorrelationId == mCorrelationId &&
                other.mMessageType == mMessageType;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, CorId: {1}, MessageType: {2}", this.GetType().Name, mCorrelationId == null ? "<none>" : mCorrelationId, mMessageType.ToString());
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Creates the new correlation id.
        /// </summary>
        /// <returns></returns>
        protected static String CreateNewCorrelationId()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion

    }

}
