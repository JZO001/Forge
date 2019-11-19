/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration;
using Forge.Net.Remoting.Messaging;

namespace Forge.Net.Remoting.Sinks
{

    /// <summary>
    /// Represents the services if a message sink
    /// </summary>
    public interface IMessageSink : IInitializable, IDisposable
    {

        /// <summary>
        /// Get the unique identifier of the sink.
        /// </summary>
        /// <value>
        /// The message sink id.
        /// </value>
        string MessageSinkId { get;}

        /// <summary>
        /// Serialize data and save it into the return object. Return object optionaly stores information to deserialization.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Serialized message content</returns>
        MessageSinkParameters Serialize(IMessage message);

        /// <summary>
        /// Deserialize data from the parameters with the provided configuration.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Deserialized message content</returns>
        IMessage Deserialize(MessageSinkParameters parameters);

    }

}
