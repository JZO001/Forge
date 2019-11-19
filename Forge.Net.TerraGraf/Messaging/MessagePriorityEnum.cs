/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Represents the priority of a message
    /// </summary>
    [Serializable]
    internal enum MessagePriorityEnum : int
    {
        /// <summary>
        /// High priority for acknowledge messages
        /// </summary>
        High = 0,

        /// <summary>
        /// Normal priority for system messages
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Lower priortity for user messages
        /// </summary>
        Low = 2,
    }

}
