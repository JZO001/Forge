/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Remoting.Messaging
{

    /// <summary>
    /// Acknowledge message
    /// </summary>
    [Serializable]
    public class AcknowledgeMessage : Message
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AcknowledgeMessage"/> class.
        /// </summary>
        protected AcknowledgeMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AcknowledgeMessage" /> class.
        /// </summary>
        /// <param name="correlationId">The correlation id.</param>
        public AcknowledgeMessage(string correlationId) : base(correlationId, MessageTypeEnum.Acknowledge)
        {
            if (string.IsNullOrEmpty(correlationId))
            {
                ThrowHelper.ThrowArgumentNullException("correlationId");
            }
        }

    }

}
