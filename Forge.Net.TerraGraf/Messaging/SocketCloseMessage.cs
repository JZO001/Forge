/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Net.Synapse;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Message for close an active TCP socket connection
    /// </summary>
    [Serializable]
    internal sealed class SocketCloseMessage : SocketMessage
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketCloseMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="targetId">The target id.</param>
        /// <param name="senderPort">The sender port.</param>
        /// <param name="targetPort">The target port.</param>
        /// <param name="senderSocketId">The sender socket id.</param>
        /// <param name="targetSocketId">The target socket id.</param>
        /// <param name="messageType">Type of the message.</param>
        internal SocketCloseMessage(string senderId, string targetId, int senderPort, int targetPort, long senderSocketId, long targetSocketId, MessageTypeEnum messageType)
            : base(senderId, targetId, MessageCodeEnum.SocketClose, messageType, senderPort, targetPort, senderSocketId, targetSocketId)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                ThrowHelper.ThrowArgumentNullException("targetId");
            }
            AddressEndPoint.ValidateTcpPort(senderPort);
            AddressEndPoint.ValidateTcpPort(targetPort);
            if (senderSocketId < 1)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("senderSocketId");
            }
            if (targetSocketId < 1)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("targetSocketId");
            }
        }

        #endregion

    }

}
