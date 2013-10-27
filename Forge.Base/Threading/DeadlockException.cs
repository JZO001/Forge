/* *********************************************************************
 * Date: 27 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.Serialization;
using System.Threading;

namespace Forge.Threading
{

    /// <summary>
    /// Exception for deadlock fail scenario
    /// </summary>
    [Serializable]
    public class DeadlockException : Exception
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="DeadlockException"/> class.
        /// </summary>
        public DeadlockException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeadlockException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DeadlockException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeadlockException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public DeadlockException(string message, Exception ex)
            : base(message, ex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeadlockException" /> class.
        /// </summary>
        /// <param name="currentLockName">Name of the current lock.</param>
        /// <param name="conflictedLockName">Name of the conflicted lock.</param>
        /// <param name="conflictedThread">The conflicted thread.</param>
        /// <param name="conflictedThreadStackTrace">The conflicted thread stack trace.</param>
        public DeadlockException(string currentLockName, string conflictedLockName, Thread conflictedThread, String conflictedThreadStackTrace)
            : base(String.Format("Deadlock detected on lock '{0}' with the following thread '{1}', id: {2}, conflicted lock name: '{3}'. Conflicted thread current stack trace: {4}", currentLockName, conflictedThread == null ? "<null>" : conflictedThread.Name, conflictedThread == null ? "<null>" : conflictedThread.ManagedThreadId.ToString(), conflictedLockName, conflictedThreadStackTrace))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeadlockException" /> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> parameter is null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0).</exception>
        protected DeadlockException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

    }

}
