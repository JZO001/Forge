/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Net.Synapse;
using Forge.Shared;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Message for request to open a new TCP socket connection response
    /// </summary>
    [Serializable]
    internal sealed class SocketOpenResponseMessage : SocketMessage
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketOpenResponseMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="targetId">The target id.</param>
        /// <param name="senderPort">The sender port.</param>
        /// <param name="targetPort">The target port.</param>
        /// <param name="senderSocketId">The sender socket id.</param>
        /// <param name="targetSocketId">The target socket id.</param>
        public SocketOpenResponseMessage(string senderId, string targetId, int senderPort, int targetPort, long senderSocketId, long targetSocketId)
            : base(senderId, targetId, MessageCodeEnum.SocketOpenResponse, MessageTypeEnum.Tcp, senderPort, targetPort, senderSocketId, targetSocketId)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                ThrowHelper.ThrowArgumentNullException("targetId");
            }
            if (senderPort != -1)
            {
                AddressEndPoint.ValidateTcpPort(senderPort);
            }
            AddressEndPoint.ValidateTcpPort(targetPort);
            if (senderSocketId < 1 && senderSocketId != -1)
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
