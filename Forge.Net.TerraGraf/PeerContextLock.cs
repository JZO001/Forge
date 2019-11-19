/* *********************************************************************
 * Date: 24 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Threading;

namespace Forge.Net.TerraGraf
{

    /// <summary>
    /// Wrapper for lock. This instance cannot be disposed.
    /// </summary>
    internal sealed class PeerContextLock : MBRBase, ILock
    {

        #region Field(s)

        private readonly DeadlockSafeLock mLock = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PeerContextLock"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        internal PeerContextLock(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ThrowHelper.ThrowArgumentNullException("id");
            }
            mLock = new DeadlockSafeLock(id);
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets a value indicating whether this instance is locked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is locked; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocked
        {
            get { return mLock.IsLocked; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is held by current thread.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is held by current thread; otherwise, <c>false</c>.
        /// </value>
        public bool IsHeldByCurrentThread
        {
            get { return mLock.IsHeldByCurrentThread; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ILock"/> is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get { return mLock.IsDisposed; }
        }

        /// <summary>
        /// Locks this instance.
        /// </summary>
        public void Lock()
        {
            mLock.Lock();
        }

        /// <summary>
        /// Tries the lock.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>True, if the lock was successful</returns>
        public bool TryLock(TimeSpan timeout)
        {
            return mLock.TryLock(timeout);
        }

        /// <summary>
        /// Tries the lock.
        /// </summary>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        /// <returns>True, if the lock was successful</returns>
        public bool TryLock(int millisecondsTimeout)
        {
            return mLock.TryLock(millisecondsTimeout);
        }

        /// <summary>
        /// Unlocks this instance.
        /// </summary>
        public void Unlock()
        {
            mLock.Unlock();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mLock")]
        public void Dispose()
        {
        }

        #endregion

    }

}
