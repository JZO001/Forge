/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;

namespace Forge.Net.Remoting.Sinks
{

    /// <summary>
    /// Parameters for a serialized contents
    /// </summary>
    [Serializable]
    public sealed class MessageSinkParameters
    {

        #region Constructor(s)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSinkParameters"/> class.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="data">The data.</param>
        public MessageSinkParameters(IMessageSinkConfiguration config, byte[] data)
        {
            if (config == null)
            {
                ThrowHelper.ThrowArgumentNullException("config");
            }
            if (data == null)
            {
                ThrowHelper.ThrowArgumentNullException("data");
            }
            ConfigurationToDeserialize = config;
            SerializedData = data;
        } 

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the configuration to deserialize.
        /// </summary>
        /// <value>
        /// The configuration to deserialize.
        /// </value>
        public IMessageSinkConfiguration ConfigurationToDeserialize { get; private set; }

        /// <summary>
        /// Gets the serialized data.
        /// </summary>
        /// <value>
        /// The serialized data.
        /// </value>
        public byte[] SerializedData { get; private set; }

        #endregion

    }

}
