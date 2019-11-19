/* *********************************************************************
 * Date: 11 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.Serialization;
using System.Security;

namespace Forge.Net.Services
{

    /// <summary>
    /// Occurs when service is not available
    /// </summary>
    [Serializable]
    public class ServiceNotAvailableException : ApplicationException
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotAvailableException"/> class.
        /// </summary>
        public ServiceNotAvailableException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotAvailableException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ServiceNotAvailableException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotAvailableException" /> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        [SecuritySafeCritical]
        protected ServiceNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotAvailableException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServiceNotAvailableException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }

}
