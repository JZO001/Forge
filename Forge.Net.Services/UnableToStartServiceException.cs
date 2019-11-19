/* *********************************************************************
 * Date: 19 Dec 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.Serialization;
using System.Security;

namespace Forge.Net.Services
{

    /// <summary>
    /// Occurs when a service cannot start
    /// </summary>
    [Serializable]
    public class UnableToStartServiceException : ApplicationException
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToStartServiceException" /> class.
        /// </summary>
        public UnableToStartServiceException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToStartServiceException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnableToStartServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToStartServiceException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        [SecuritySafeCritical]
        protected UnableToStartServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToStartServiceException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnableToStartServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }

}
