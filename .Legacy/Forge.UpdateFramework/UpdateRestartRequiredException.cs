/* *********************************************************************
 * Date: 09 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.Serialization;
using System.Security;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Occurs when the updater client requires a restart to complete the update process.
    /// </summary>
    [Serializable]
    public class UpdateRestartRequiredException : ApplicationException
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRestartRequiredException"/> class.
        /// </summary>
        public UpdateRestartRequiredException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRestartRequiredException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UpdateRestartRequiredException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRestartRequiredException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UpdateRestartRequiredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRestartRequiredException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        [SecuritySafeCritical]
        protected UpdateRestartRequiredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }

}
