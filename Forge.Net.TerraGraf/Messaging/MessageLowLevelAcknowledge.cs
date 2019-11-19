/* *********************************************************************
 * Date: 20 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Acknowledge message for a low level Tcp system message
    /// </summary>
    [Serializable]
    internal class MessageLowLevelAcknowledge : TerraGrafMessageBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageLowLevelAcknowledge"/> class.
        /// </summary>
        internal MessageLowLevelAcknowledge()
            : base("*", string.Empty, MessageCodeEnum.LowLevelTcpAcknowledge, -1, MessagePriorityEnum.High, MessageTypeEnum.Udp)
        {
        }

    }

}
