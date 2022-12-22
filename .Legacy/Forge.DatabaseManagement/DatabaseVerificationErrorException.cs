/* *********************************************************************
 * Date: 08 Aug 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.Serialization;

namespace Forge.DatabaseManagement
{

    /// <summary>
    /// Occurs when the database verification failed.
    /// </summary>
    [Serializable]
    public class DatabaseVerificationErrorException : ApplicationException
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseVerificationErrorException"/> class.
        /// </summary>
        public DatabaseVerificationErrorException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseVerificationErrorException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DatabaseVerificationErrorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseVerificationErrorException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public DatabaseVerificationErrorException(string message, Exception ex)
            : base(message, ex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseVerificationErrorException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected DatabaseVerificationErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }

}
