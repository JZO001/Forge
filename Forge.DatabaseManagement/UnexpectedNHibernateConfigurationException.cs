/* *********************************************************************
 * Date: 05 Aug 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.Serialization;

namespace Forge.DatabaseManagement
{

    /// <summary>
    /// Occurs when a Database Manager unable to handle the provided database type
    /// </summary>
    [Serializable]
    public class UnexpectedNHibernateConfigurationException : ApplicationException
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedNHibernateConfigurationException"/> class.
        /// </summary>
        public UnexpectedNHibernateConfigurationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedNHibernateConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnexpectedNHibernateConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedNHibernateConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public UnexpectedNHibernateConfigurationException(string message, Exception ex)
            : base(message, ex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedNHibernateConfigurationException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected UnexpectedNHibernateConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }

}
