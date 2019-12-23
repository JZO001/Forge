/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Forge.Threading
{

    /// <summary>
    /// Lock handle helper class
    /// </summary>
    public static class LockHandle
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(LockHandle));

        /// <summary>
        /// If value lower than 0, the wait value will be randomized
        /// </summary>
        public static int DEFAULT_WAIT_CYCLE_IN_MS = -1; // if value lower than 0, the wait value will be randomized

        /// <summary>
        /// Default call timeout
        /// </summary>
        public static int DEFAULT_CALL_TIMEOUT_IN_MS = 0;

        /// <summary>
        /// Default period timeout
        /// </summary>
        public static int DEFAULT_PERIOD_TIMEOUT_IN_MS = 5000;

        #endregion

        #region Public method(s)

        /// <summary>
        /// Locks provided lock agressively. This means the method locks all available lock and keep them locked.
        /// Next round tries to lock the others. The cycle will be retry until timeout or all lock acquired successfully.
        /// </summary>
        /// <param name="locks">The locks.</param>
        public static void LockAllAgressive(ICollection<ILock> locks)
        {
            LockAllAgressive(locks, Timeout.Infinite, DEFAULT_PERIOD_TIMEOUT_IN_MS);
        }

        /// <summary>
        /// Locks provided locks agressively. This means the method locks all available lock and keep them locked.
        /// Next round tries to lock the others. The cycle will be retry until timeout or all lock acquired successfully.
        /// </summary>
        /// <param name="locks">The locks.</param>
        /// <param name="callTimeout">The call timeout.</param>
        /// <param name="periodTimeout">The period timeout.</param>
        public static void LockAllAgressive(ICollection<ILock> locks, int callTimeout, int periodTimeout)
        {
            if (locks == null)
            {
                ThrowHelper.ThrowArgumentNullException("locks");
            }
            if (callTimeout < Timeout.Infinite)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("callTimeout");
            }
            if (periodTimeout < Timeout.Infinite)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("periodTimeout");
            }
            AgressiveLockAll(locks, callTimeout, periodTimeout);
        }

        /// <summary>
        /// Locks provided locks like a gentleman. This means the method lock all available lock and if remains
        /// unavailable locks in the current lock cycle, release all locked locks and try again all.
        /// </summary>
        /// <param name="locks">The locks.</param>
        public static void LockAllGentle(ICollection<ILock> locks)
        {
            LockAllGentle(locks, DEFAULT_WAIT_CYCLE_IN_MS);
        }

        /// <summary>
        /// Locks provided locks like a gentleman. This means the method lock all available lock and if remains
        /// unavailable locks in the current lock cycle, release all locked locks and try again all.
        /// </summary>
        /// <param name="locks">The locks.</param>
        /// <param name="waitCycleInMs">The wait cycle in ms.</param>
        public static void LockAllGentle(ICollection<ILock> locks, int waitCycleInMs)
        {
            if (locks == null)
            {
                ThrowHelper.ThrowArgumentNullException("locks");
            }

            if (locks.Count == 0)
            {
                return;
            }

            Random rnd = new Random(DateTime.Now.Millisecond);
            bool allLocked = false;
            while (!allLocked)
            {
                allLocked = GentleLockAll(locks, DEFAULT_CALL_TIMEOUT_IN_MS);
                if (!allLocked)
                {
                    Thread.Sleep(waitCycleInMs < 0 ? rnd.Next(10) : waitCycleInMs);
                }
            }
        }

        /// <summary>
        /// Tries the lock all.
        /// </summary>
        /// <param name="locks">The locks.</param>
        /// <returns>
        /// True, if all lock successfully acquired, otherwise False.
        /// </returns>
        public static bool TryLockAll(ICollection<ILock> locks)
        {
            return GentleLockAll(locks, DEFAULT_CALL_TIMEOUT_IN_MS);
        }

        /// <summary>
        /// Tries the lock all.
        /// </summary>
        /// <param name="locks">The locks.</param>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        /// <returns>
        /// True, if all lock successfully acquired, otherwise False.
        /// </returns>
        public static bool TryLockAll(ICollection<ILock> locks, int millisecondsTimeout)
        {
            if (locks == null)
            {
                ThrowHelper.ThrowArgumentNullException("locks");
            }

            if (locks.Count == 0)
            {
                return false;
            }

            return GentleLockAll(locks, millisecondsTimeout);
        }

        #endregion

        #region Private method(s)

        private static void AgressiveLockAll(ICollection<ILock> locks, int callTimeout, int periodTimeout)
        {
            DateTime startCallTime = DateTime.MinValue;
            if (callTimeout > Timeout.Infinite)
            {
                // -1 means no timeout
                startCallTime = DateTime.Now.AddMilliseconds(callTimeout);
            }

            DateTime startTryLock = DateTime.MinValue;
            if (periodTimeout > Timeout.Infinite)
            {
                // -1 means no timeout
                startTryLock = DateTime.Now.AddMilliseconds(periodTimeout);
            }

            Random rnd = new Random(DateTime.Now.Millisecond);
            int lockedCounter = 0;
            List<ILock> lockedByMe = new List<ILock>();
            while (lockedCounter < locks.Count)
            {
                lockedCounter = 0;
                try
                {
                    foreach (ILock lockable in locks)
                    {
                        if (lockedByMe.Contains(lockable))
                        {
                            lockedCounter++;
                        }
                        else if (lockable.TryLock(0))
                        {
                            lockedByMe.Add(lockable); // általam lockolt resource feljegyzése
                            lockedCounter++;
                        }
                        else
                        {
                            if (startTryLock != DateTime.MinValue && DateTime.Now.Ticks > startTryLock.Ticks)
                            {
                                // tryLock has timed out, set next timeout value
                                startTryLock = DateTime.Now.AddMilliseconds(periodTimeout);

                                if (lockedCounter < locks.Count)
                                {
                                    if (lockedByMe.Count > 0)
                                    {
                                        lockedByMe.ForEach(l => l.Unlock());
                                        lockedByMe.Clear();
                                    }
                                    lockedCounter = 0;
                                    if (LOGGER.IsInfoEnabled) LOGGER.Info("Unable to lock ILock's, timed out, trying again.");
                                    break;
                                }
                            }
                        }
                    }
                    if (lockedCounter < locks.Count)
                    {
                        Thread.Sleep(rnd.Next(10));
                    }
                }
                catch (Exception)
                {
                    // Csak DeadlockException és ObjectDisposedException-re számítok itt

                    // Csak azokat unlock-olom, akiket eredetileg is itt lockoltam le. Amelyek már lockolva érkeztek, azokat nem.
                    // Erre figyelni kell fejlesztés közben.
                    if (lockedByMe.Count > 0)
                    {
                        lockedByMe.ForEach(l => l.Unlock());
                        lockedByMe.Clear();
                    }
                    throw;
                }

                if (lockedCounter < locks.Count)
                {
                    if (startCallTime != DateTime.MinValue && DateTime.Now.Ticks > startCallTime.Ticks)
                    {
                        if (lockedByMe.Count > 0)
                        {
                            lockedByMe.ForEach(l => l.Unlock());
                            lockedByMe.Clear();
                        }
                        throw new TimeoutException("LockHandle unable to lock all ILock's.");
                    }
                }
            }
            lockedByMe.Clear();
        }

        private static bool GentleLockAll(ICollection<ILock> locks, int timeout)
        {
            bool allLocked = true;
            List<ILock> lockedByMe = new List<ILock>();
            foreach (ILock lc in locks)
            {
                bool result = false;
                result = lc.TryLock(timeout);
                if (result)
                {
                    lockedByMe.Add(lc);
                }
                else
                {
                    allLocked = false;
                    break;
                }
            }
            if (!allLocked)
            {
                lockedByMe.ForEach(l => l.Unlock());
                lockedByMe.Clear();
            }
            return allLocked;
        }

        #endregion

    }

}
