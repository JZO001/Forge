/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.Serialization;

namespace Forge.Configuration.Shared
{

    /// <summary>
    /// Exception for invalid configuration scenario
    /// </summary>
    [Serializable]
    public class InvalidConfigurationValueException : InvalidConfigurationException
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationValueException" /> class.
        /// </summary>
        public InvalidConfigurationValueException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationValueException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidConfigurationValueException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationValueException" /> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> parameter is null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0).</exception>
        protected InvalidConfigurationValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationValueException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidConfigurationValueException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }

}
