/* *********************************************************************
 * Date: 27 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Forge.Threading
{

    /// <summary>
    /// Reentrance lock with deadlock detection ability
    /// </summary>
    [DebuggerDisplay("[{GetType()}, LockId = {LockId}, Name = {Name}]")]
    public sealed class DeadlockSafeLock : MBRBase, IDisposable, IEquatable<DeadlockSafeLock>, ILock
    {

        #region Field(s)

        private static readonly Dictionary<DeadlockSafeLock, object> EXISTING_LOCKS = new Dictionary<DeadlockSafeLock, object>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly String mLockId = Guid.NewGuid().ToString();

        private readonly Dictionary<int, String> mQueuedThreads = new Dictionary<int, String>();

        private readonly Dictionary<int, String> mOwnerThreads = new Dictionary<int, String>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly String mName = string.Empty;

        private readonly Semaphore mSemaphore = new Semaphore(1, 1);

        private Thread mOwner = null;

        private int mOwnerCounter = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mDisposed = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="DeadlockSafeLock"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DeadlockSafeLock(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                ThrowHelper.ThrowArgumentNullException("name");
            }
            this.mName = name;
            lock (EXISTING_LOCKS)
            {
                EXISTING_LOCKS.Add(this, null);
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="DeadlockSafeLock"/> is reclaimed by garbage collection.
        /// </summary>
        ~DeadlockSafeLock()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DebuggerHidden]
        public String Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DeadlockSafeLock"/> is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsDisposed
        {
            get { return mDisposed; }
        }

        /// <summary>
        /// Gets the lock id.
        /// </summary>
        /// <value>
        /// The lock id.
        /// </value>
        [DebuggerHidden]
        public String LockId
        {
            get { return mLockId; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is held by current thread.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is held by current thread; otherwise, <c>false</c>.
        /// </value>
        public bool IsHeldByCurrentThread
        {
            get
            {
                return Thread.CurrentThread.Equals(this.mOwner);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is locked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is locked; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocked
        {
            get
            {
                return this.mOwner != null;
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Locks this instance.
        /// </summary>
        /// <exception cref="DeadlockException">Occurs when deadlock detected.</exception>
        /// <exception cref="ObjectDisposedException">Occurs when this instance has disposed.</exception>
        public void Lock()
        {
            DoDisposeCheck();

            bool isHeldByCurrentThread = IsHeldByCurrentThread;
            if (!isHeldByCurrentThread)
            {
                AdminThread();
            }

            LockInner(Timeout.Infinite);

            if (!isHeldByCurrentThread)
            {
                lock (EXISTING_LOCKS)
                {
                    mQueuedThreads.Remove(Thread.CurrentThread.ManagedThreadId);
                    mOwnerThreads.Add(Thread.CurrentThread.ManagedThreadId, GetStackTrace(new StackTrace()));
                }
            }
        }

        /// <summary>
        /// Tries the lock.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <exception cref="DeadlockException">Occurs when deadlock detected.</exception>
        /// <exception cref="ObjectDisposedException">Occurs when this instance has disposed.</exception>
        /// <exception cref="ArgumentNullException">Occurs when the TimeSpan parameter is null.</exception>
        /// <returns>True, if the lock acquired successfuly, otherwise False.</returns>
        public bool TryLock(TimeSpan timeout)
        {
            if (timeout == null)
            {
                ThrowHelper.ThrowArgumentNullException("timeout");
            }
            return TryLock(Convert.ToInt32(timeout.TotalMilliseconds));
        }

        /// <summary>
        /// Tries the lock.
        /// </summary>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        /// <exception cref="DeadlockException">Occurs when deadlock detected.</exception>
        /// <exception cref="ObjectDisposedException">Occurs when this instance has disposed.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Occurs when int value is lower then Timeout.Infinite (-1)</exception>
        /// <returns>True, if the lock acquired successfuly, otherwise False.</returns>
        public bool TryLock(int millisecondsTimeout)
        {
            DoDisposeCheck();
            if (millisecondsTimeout < Timeout.Infinite)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("millisecondsTimeout");
            }

            bool result = false;
            bool isHeldByCurrentThread = IsHeldByCurrentThread;
            if (!isHeldByCurrentThread)
            {
                AdminThread();
            }
            result = LockInner(millisecondsTimeout);

            if (!isHeldByCurrentThread)
            {
                lock (EXISTING_LOCKS)
                {
                    mQueuedThreads.Remove(Thread.CurrentThread.ManagedThreadId);
                    if (result)
                    {
                        mOwnerThreads.Add(Thread.CurrentThread.ManagedThreadId, GetStackTrace(new StackTrace()));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Unlocks this instance.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Occurs when this instance has disposed.</exception>
        public void Unlock()
        {
            DoDisposeCheck();

            if (!Thread.CurrentThread.Equals(this.mOwner))
            {
                throw new InvalidOperationException("Current thread does not the owner of the lock.");
            }

            if (this.mOwnerCounter == 1)
            {
                lock (EXISTING_LOCKS)
                {
                    mOwnerThreads.Remove(Thread.CurrentThread.ManagedThreadId);
                    this.mOwner = null;
                    this.mOwnerCounter = 0;
                }
                this.mSemaphore.Release();
            }
            else
            {
                this.mOwnerCounter--;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            DeadlockSafeLock other = (DeadlockSafeLock)obj;
            return other.mLockId == mLockId && other.mName == mName;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True, if the other class is equals with this.</returns>
        public bool Equals(DeadlockSafeLock other)
        {
            return Equals((object)other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private method(s)

        private bool LockInner(int millisecondsTimeout)
        {
            DoDisposeCheck();
            bool result = true;

            if (IsHeldByCurrentThread)
            {
                this.mOwnerCounter++;
            }
            else
            {
                result = this.mSemaphore.WaitOne(millisecondsTimeout); // request access
                if (result)
                {
                    this.mOwner = Thread.CurrentThread; // store owner
                    this.mOwnerCounter = 1;
                }
            }

            return result;
        }

        private void AdminThread()
        {
            lock (EXISTING_LOCKS) // globális lock
            {
                DoDisposeCheck();
                String stackTrace = GetStackTrace(new StackTrace());
                if (EXISTING_LOCKS.Count > 1 && !IsHeldByCurrentThread && IsLocked)
                {
                    // lockolva van és nem az enyém
                    Thread currentOwner = this.mOwner; // itt a tulaj
                    // végignézem azokat a lockokat, amelyeknek Én vagyok a tulaja és megnézem, hogy a tulaj várakozik-e valamelyiknek a Queue-jában
                    foreach (DeadlockSafeLock dsl in EXISTING_LOCKS.Keys)
                    {
                        // a jelenlegi lock-ot nem vizsgálom
                        if (!this.Equals(dsl))
                        {
                            // vizsgálat: ez a vizsgált lock már az enyém és a jelenlegi lock tulaja várakozik belépésre
                            if (dsl.IsHeldByCurrentThread && dsl.mQueuedThreads.ContainsKey(currentOwner.ManagedThreadId))
                            {
                                // egy általam birtokolt lock-on ez a tulaj már várakozik, ez pedig deadlock
                                throw new DeadlockException(this.mName, dsl.Name, currentOwner, dsl.mQueuedThreads[currentOwner.ManagedThreadId]);
                            }
                        }
                    }
                }
                // ha nem észleltem deadlock-ot és nem én vagyok a tulaj, felveszem magam a várakozók listájára
                if (!IsHeldByCurrentThread)
                {
                    mQueuedThreads.Add(Thread.CurrentThread.ManagedThreadId, stackTrace);
                }
            }
        }

        private static String GetStackTrace(StackTrace st)
        {
            StringBuilder sb = new StringBuilder(st.ToString());
            sb.AppendLine();
            return sb.ToString();
        }

        private void DoDisposeCheck()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(String.Format("{0}, Name: {1}", this.GetType().FullName, this.mName));
            }
        }

        private void Dispose(bool disposing)
        {
            this.mDisposed = true;
            lock (EXISTING_LOCKS)
            {
                EXISTING_LOCKS.Remove(this);
            }
            if (disposing)
            {
                this.mSemaphore.Close();
            }
        }

        #endregion

    }

}
