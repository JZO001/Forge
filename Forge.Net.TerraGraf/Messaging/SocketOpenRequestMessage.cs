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
    /// Message for request to open a new TCP socket connection
    /// </summary>
    [Serializable]
    internal sealed class SocketOpenRequestMessage : SocketMessage
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketOpenRequestMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="targetId">The target id.</param>
        /// <param name="senderPort">The sender port.</param>
        /// <param name="targetPort">The target port.</param>
        /// <param name="senderSocketId">The sender socket id.</param>
        internal SocketOpenRequestMessage(string senderId, string targetId, int senderPort, int targetPort, long senderSocketId)
            : base(senderId, targetId, MessageCodeEnum.SocketOpenRequest, MessageTypeEnum.Tcp, senderPort, targetPort, senderSocketId, -1)
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
        }

        #endregion

    }

}
