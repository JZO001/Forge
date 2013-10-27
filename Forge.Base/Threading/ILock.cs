/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Threading
{

    /// <summary>
    /// Common interface for locks
    /// </summary>
    public interface ILock : IDisposable
    {

        /// <summary>
        /// Gets a value indicating whether this instance is locked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is locked; otherwise, <c>false</c>.
        /// </value>
        bool IsLocked { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is held by current thread.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is held by current thread; otherwise, <c>false</c>.
        /// </value>
        bool IsHeldByCurrentThread { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ILock"/> is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        bool IsDisposed { get; }

        /// <summary>
        /// Locks this instance.
        /// </summary>
        void Lock();

        /// <summary>
        /// Tries the lock.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>True, if the lock acquired successfuly, otherwise False.</returns>
        bool TryLock(TimeSpan timeout);

        /// <summary>
        /// Tries the lock.
        /// </summary>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        /// <returns>True, if the lock acquired successfuly, otherwise False.</returns>
        bool TryLock(int millisecondsTimeout);

        /// <summary>
        /// Unlocks this instance.
        /// </summary>
        void Unlock();

    }

}
