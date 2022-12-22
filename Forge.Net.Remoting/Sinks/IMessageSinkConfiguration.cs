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

        /// <summary>Gets a value indicating whether [decompress data].</summary>
        /// <value>
        ///   <c>true</c> if [decompress data]; otherwise, <c>false</c>.</value>
        bool DecompressData { get; }

    }

}
