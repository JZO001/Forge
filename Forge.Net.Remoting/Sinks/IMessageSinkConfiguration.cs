/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.Net.Remoting.Sinks
{

    /// <summary>
    /// Represents the configuration of a message
    /// </summary>
    public interface IMessageSinkConfiguration
    {

        /// <summary>
        /// Gets the message sink id.
        /// </summary>
        /// <value>
        /// The message sink id.
        /// </value>
        string MessageSinkId { get; }

    }

}
