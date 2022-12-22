/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Text;
using Forge.Net.Synapse;
using Forge.Shared;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Message for send socket user raw data
    /// </summary>
    [Serializable]
    internal sealed class SocketRawDataMessage : SocketMessage
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ulong mPacketOrderNumber = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte[] mData = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// UDP Broadcast, initializes a new instance of the <see cref="SocketRawDataMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="senderPort">The sender port.</param>
        /// <param name="targetPort">The target port.</param>
        /// <param name="data">The data.</param>
        internal SocketRawDataMessage(string senderId, int senderPort, int targetPort, byte[] data)
            : base(senderId, string.Empty, MessageCodeEnum.SocketRawData, MessageTypeEnum.Udp, senderPort, targetPort, -1, -1)
        {
            if (data == null)
            {
                ThrowHelper.ThrowArgumentNullException("data");
            }
            AddressEndPoint.ValidateTcpPort(senderPort);
            AddressEndPoint.ValidateTcpPort(targetPort);
            mData = data;
        }

        /// <summary>
        /// UDP, initializes a new instance of the <see cref="SocketRawDataMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="targetId">The target id.</param>
        /// <param name="senderPort">The sender port.</param>
        /// <param name="targetPort">The target port.</param>
        /// <param name="data">The data.</param>
        internal SocketRawDataMessage(string senderId, string targetId, int senderPort, int targetPort, byte[] data)
            : base(senderId, targetId, MessageCodeEnum.SocketRawData, MessageTypeEnum.Udp, senderPort, targetPort, -1, -1)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                ThrowHelper.ThrowArgumentNullException("targetId");
            }
            if (data == null)
            {
                ThrowHelper.ThrowArgumentNullException("data");
            }
            AddressEndPoint.ValidateTcpPort(senderPort);
            AddressEndPoint.ValidateTcpPort(targetPort);
            mData = data;
        }

        /// <summary>
        /// TCP, initializes a new instance of the <see cref="SocketRawDataMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="targetId">The target id.</param>
        /// <param name="senderPort">The sender port.</param>
        /// <param name="targetPort">The target port.</param>
        /// <param name="senderSocketId">The sender socket id.</param>
        /// <param name="targetSocketId">The target socket id.</param>
        /// <param name="packetOrderNumber">The packet order number.</param>
        /// <param name="data">The data.</param>
        internal SocketRawDataMessage(string senderId, string targetId, int senderPort, int targetPort, long senderSocketId, long targetSocketId, ulong packetOrderNumber, byte[] data)
            : base(senderId, targetId, MessageCodeEnum.SocketRawData, MessageTypeEnum.Tcp, senderPort, targetPort, senderSocketId, targetSocketId)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                ThrowHelper.ThrowArgumentNullException("targetId");
            }
            if (data == null)
            {
                ThrowHelper.ThrowArgumentNullException("data");
            }
            if (senderSocketId < 1)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("senderSocketId");
            }
            if (targetSocketId < 1)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("targetSocketId");
            }
            AddressEndPoint.ValidateTcpPort(senderPort);
            AddressEndPoint.ValidateTcpPort(targetPort);
            mPacketOrderNumber = packetOrderNumber;
            mData = data;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the packet order number.
        /// </summary>
        /// <value>
        /// The packet order number.
        /// </value>
        [DebuggerHidden]
        internal ulong PacketOrderNumber
        {
            get { return mPacketOrderNumber; }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [DebuggerHidden]
        internal byte[] Data
        {
            get { return mData; }
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
                result = other.mData == mData && other.mPacketOrderNumber == mPacketOrderNumber;
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
            hash = 11 * hash + mData.GetHashCode();
            hash = 11 * hash + mPacketOrderNumber.GetHashCode();
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
            sb.Append(", ParketOrderNumber: [");
            sb.Append(PacketOrderNumber.ToString());
            sb.Append("], Data size: [");
            sb.Append(Data == null ? "(null)" : Data.LongLength.ToString());
            sb.Append("]");
            return sb.ToString();
        }

        #endregion

    }

}
