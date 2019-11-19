/* *********************************************************************
 * Date: 25 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Represents a socket communication message
    /// </summary>
    [Serializable]
    internal abstract class SocketMessage : TerraGrafMessageBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mSenderSocketId = -1; // TCP üzeneteknél használatos

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mSenderPort = -1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mTargetSocketId = -1; // TCP üzeneteknél használatos

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mTargetPort = -1;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="targetId">The target id.</param>
        /// <param name="messageCode">The message code.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="senderPort">The sender port.</param>
        /// <param name="targetPort">The target port.</param>
        /// <param name="senderSocketId">The sender socket id.</param>
        /// <param name="targetSocketId">The target socket id.</param>
        internal SocketMessage(string senderId, string targetId, MessageCodeEnum messageCode, MessageTypeEnum messageType, int senderPort, int targetPort, long senderSocketId, long targetSocketId)
            : base(senderId, targetId, messageCode, Interlocked.Increment(ref mGlobalMessageId), MessagePriorityEnum.Low, messageType)
        {
            this.mSenderPort = senderPort;
            this.mSenderSocketId = senderSocketId;
            this.mTargetPort = targetPort;
            this.mTargetSocketId = targetSocketId;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the sender socket id.
        /// </summary>
        /// <value>
        /// The sender socket id.
        /// </value>
        internal long SenderSocketId
        {
            get { return mSenderSocketId; }
        }

        /// <summary>
        /// Gets the sender port.
        /// </summary>
        /// <value>
        /// The sender port.
        /// </value>
        internal int SenderPort
        {
            get { return mSenderPort; }
        }

        /// <summary>
        /// Gets the target socket id.
        /// </summary>
        /// <value>
        /// The target socket id.
        /// </value>
        internal long TargetSocketId
        {
            get { return mTargetSocketId; }
        }

        /// <summary>
        /// Gets the target port.
        /// </summary>
        /// <value>
        /// The target port.
        /// </value>
        internal int TargetPort
        {
            get { return mTargetPort; }
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
                SocketRawDataMessage other = (SocketRawDataMessage)obj;
                result = other.mSenderSocketId == mSenderSocketId && other.mTargetSocketId == mTargetSocketId && other.mSenderPort == mSenderPort && other.mTargetPort == mTargetPort;
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
            hash = 11 * hash + mSenderSocketId.GetHashCode();
            hash = 11 * hash + mTargetSocketId.GetHashCode();
            hash = 11 * hash + mSenderPort.GetHashCode();
            hash = 11 * hash + mTargetPort.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append(", SenderSocketId: [");
            sb.Append(SenderSocketId.ToString());
            sb.Append("], SenderPort: [");
            sb.Append(SenderPort.ToString());
            sb.Append("], TargetSocketId: [");
            sb.Append(TargetSocketId.ToString());
            sb.Append("], TargetPort: [");
            sb.Append(TargetPort.ToString());
            sb.Append("]");
            return sb.ToString();
        }

        #endregion

    }

}
