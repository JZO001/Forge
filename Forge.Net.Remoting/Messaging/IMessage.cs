/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;

namespace Forge.Net.Remoting.Messaging
{

    /// <summary>
    /// Represents any message
    /// </summary>
    public interface IMessage
    {

        /// <summary>
        /// Correlation of the message
        /// </summary>
        /// <value>
        /// The correlation id.
        /// </value>
        string CorrelationId { get; }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        MessageTypeEnum MessageType { get; }

        /// <summary>
        /// Get the call context of the message
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        Dictionary<string, object> Context { get; }

        /// <summary>
        /// Gets a value indicating whether [allow parallel execution].
        /// </summary>
        /// <value>
        /// <c>true</c> if [allow parallel execution]; otherwise, <c>false</c>.
        /// </value>
        bool AllowParallelExecution { get; }

    }

}
