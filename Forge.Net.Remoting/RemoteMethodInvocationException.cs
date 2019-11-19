/* *********************************************************************
 * Date: 12 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Remoting
{

    /// <summary>
    /// Occurs when a remote method invocation throws an exception
    /// </summary>
    [Serializable]
    public class RemoteMethodInvocationException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMethodInvocationException"/> class.
        /// </summary>
        public RemoteMethodInvocationException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMethodInvocationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RemoteMethodInvocationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMethodInvocationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public RemoteMethodInvocationException(string message, Exception ex) : base(message, ex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMethodInvocationException" /> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> parameter is null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0).</exception>
        protected RemoteMethodInvocationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

    }

}
