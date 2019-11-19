﻿/* *********************************************************************
 * Date: 12 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Remoting
{

    /// <summary>
    /// Occurs when a remote method invocation failed
    /// </summary>
    [Serializable]
    public class MethodInvocationException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInvocationException"/> class.
        /// </summary>
        public MethodInvocationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInvocationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MethodInvocationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInvocationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public MethodInvocationException(string message, Exception ex)
            : base(message, ex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInvocationException" /> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> parameter is null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0).</exception>
        protected MethodInvocationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

    }

}
