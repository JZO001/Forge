/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.Serialization;

namespace Forge.ORM.NHibernateExtension
{

    /// <summary>
    /// Base exception type
    /// </summary>
    [Serializable]
    public abstract class ORMException : Exception
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ORMException"/> class.
        /// </summary>
        protected ORMException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ORMException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        protected ORMException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ORMException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        protected ORMException(string message, Exception ex)
            : base(message, ex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ORMException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        ///   
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected ORMException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

    }

}
