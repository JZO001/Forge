/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using Forge.Net.Remoting.Sinks;

namespace Forge.Net.Remoting.Messaging
{

    /// <summary>
    /// Represents the header of a message
    /// </summary>
    [Serializable]
    public class MessageHeader
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mMessageSinkId = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mMessageLength = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IMessageSinkConfiguration mMessageSinkConfiguration = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mMessageSinkConfigurationClassName = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeader"/> class.
        /// </summary>
        protected MessageHeader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeader"/> class.
        /// </summary>
        /// <param name="messageSinkId">The message sink id.</param>
        /// <param name="messageLength">Length of the message.</param>
        /// <param name="messageSinkConfiguration">The message sink configuration.</param>
        public MessageHeader(String messageSinkId, int messageLength, IMessageSinkConfiguration messageSinkConfiguration)
        {
            if (string.IsNullOrEmpty(messageSinkId))
            {
                ThrowHelper.ThrowArgumentNullException("messageSinkId");
            }
            if (messageLength < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("messageLength");
            }

            this.mMessageSinkId = messageSinkId;
            this.mMessageLength = messageLength;
            this.mMessageSinkConfiguration = messageSinkConfiguration;
            if (messageSinkConfiguration != null)
            {
                this.mMessageSinkConfigurationClassName = messageSinkConfiguration.GetType().FullName;
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the message sink id.
        /// </summary>
        /// <value>
        /// The message sink id.
        /// </value>
        public string MessageSinkId
        {
            get { return mMessageSinkId; }
        }

        /// <summary>
        /// Gets the length of the message.
        /// </summary>
        /// <value>
        /// The length of the message.
        /// </value>
        public int MessageLength
        {
            get { return mMessageLength; }
        }

        /// <summary>
        /// Gets the message sink configuration.
        /// </summary>
        /// <value>
        /// The message sink configuration.
        /// </value>
        public IMessageSinkConfiguration MessageSinkConfiguration
        {
            get { return mMessageSinkConfiguration; }
        }

        /// <summary>
        /// Gets the name of the message sink configuration class.
        /// </summary>
        /// <value>
        /// The name of the message sink configuration class.
        /// </value>
        public string MessageSinkConfigurationClassName
        {
            get { return mMessageSinkConfigurationClassName; }
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

            MessageHeader other = (MessageHeader)obj;
            return other.mMessageSinkId == mMessageSinkId && 
                other.mMessageLength == mMessageLength && 
                other.mMessageSinkConfiguration == mMessageSinkConfiguration &&
                other.mMessageSinkConfigurationClassName == mMessageSinkConfigurationClassName;
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

        #endregion

    }

}
