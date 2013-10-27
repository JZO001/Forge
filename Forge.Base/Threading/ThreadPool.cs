/* *********************************************************************
 * Date: 23 Jul 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using Forge.Threading.ConfigSection;
using log4net;

namespace Forge.Threading
{

    /// <summary>
    /// ThreadPool implementation for specific use cases
    /// </summary>
    public sealed class ThreadPool : MBRBase, IDisposable
    {

        #region Variables

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(ThreadPool));

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private String mName = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mIsReadOnly = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Int32 mMinThreadNumber = Environment.ProcessorCount;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Int32 mMaxThreadNumber = Environment.ProcessorCount * 25;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Int32 mShutDownIdleThreadTime = 120000; // 2 minutes

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Int32 mMaxStackSize = 0;

        private readonly Queue<TaskContainer> mTaskContainerQueue = new Queue<TaskContainer>();

        private readonly List<WorkerThread> mExistingThreads = new List<WorkerThread>();

        private readonly List<WorkerThread> mInactiveThreads = new List<WorkerThread>();

        private int mThreadId = 0;

        private int mThreadNumber = 0;

        private readonly object mLockObject = new object();

        private bool mDisposed = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPool"/> class.
        /// </summary>
        public ThreadPool()
        {
            OpenConfiguration();
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPool"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ThreadPool(String name)
        {
            if (name == null)
            {
                ThrowHelper.ThrowArgumentNullException("name");
            }

            this.mName = name;
            OpenConfiguration();
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPool"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="minThreadNumber">The min thread number.</param>
        /// <param name="maxThreadNumber">The max thread number.</param>
        /// <param name="maxStackSize">Size of the max stack.</param>
        public ThreadPool(String name, Int32 minThreadNumber, Int32 maxThreadNumber, Int32 maxStackSize)
        {
            if (name == null)
            {
                ThrowHelper.ThrowArgumentNullException("name");
            }

            this.mName = name;
            OpenConfiguration();
            if (minThreadNumber >= 0)
            {
                mMinThreadNumber = minThreadNumber;
            }
            if (maxThreadNumber > 0)
            {
                this.MaxThreadNumber = maxThreadNumber;
                if (mMaxThreadNumber < mMinThreadNumber)
                {
                    mMaxThreadNumber = mMinThreadNumber;
                }
            }
            if (maxStackSize > 0)
            {
                this.mMaxStackSize = maxStackSize;
            }
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPool"/> class.
        /// </summary>
        /// <param name="minThreadNumber">The min thread number.</param>
        /// <param name="maxThreadNumber">The max thread number.</param>
        /// <param name="maxStackSize">Size of the max stack.</param>
        public ThreadPool(Int32 minThreadNumber, Int32 maxThreadNumber, Int32 maxStackSize)
        {
            OpenConfiguration();
            if (minThreadNumber >= 0)
            {
                mMinThreadNumber = minThreadNumber;
            }
            if (maxThreadNumber > 0)
            {
                this.MaxThreadNumber = maxThreadNumber;
                if (mMaxThreadNumber < mMinThreadNumber)
                {
                    mMaxThreadNumber = mMinThreadNumber;
                }
            }
            if (maxStackSize > 0)
            {
                this.mMaxStackSize = maxStackSize;
            }
            Initialize();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ThreadPool"/> is reclaimed by garbage collection.
        /// </summary>
        ~ThreadPool()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the min thread number.
        /// </summary>
        /// <value>
        /// The min thread number.
        /// </value>
        [DebuggerHidden]
        [XmlElement(Order = 1)]
        public Int32 MinThreadNumber
        {
            get { DoDisposeCheck(); return mMinThreadNumber; }
            set
            {
                DoDisposeCheck();
                if (!IsReadOnly)
                {
                    if (value >= 0 && value <= mMaxThreadNumber)
                    {
                        if (value > mMinThreadNumber)
                        {
                            lock (mLockObject)
                            {
                                for (int i = mMinThreadNumber; i < value; i++)
                                {
                                    StartNewThread();
                                }
                                mMinThreadNumber = value;
                            }
                        }
                        else
                        {
                            mMinThreadNumber = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the max thread number.
        /// </summary>
        /// <value>
        /// The max thread number.
        /// </value>
        [DebuggerHidden]
        [XmlElement(Order = 2)]
        public Int32 MaxThreadNumber
        {
            get { DoDisposeCheck(); return mMaxThreadNumber; }
            set
            {
                DoDisposeCheck();
                if (!IsReadOnly)
                {
                    if (value > 0 && value >= mMinThreadNumber)
                    {
                        if (value < mMaxThreadNumber)
                        {
                            // kevesebb szál engedélyezett
                            lock (mLockObject)
                            {
                                if (mInactiveThreads.Count > 0)
                                {
                                    List<WorkerThread> wtList = new List<WorkerThread>(mInactiveThreads);
                                    for (int i = value; i < mMaxThreadNumber; i++)
                                    {
                                        if (wtList.Count == 0)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            wtList[0].WakeUpEvent.Set();
                                            wtList.RemoveAt(0);
                                        }
                                    }
                                }
                            }
                        }
                        mMaxThreadNumber = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the shut down idle thread time.
        /// </summary>
        /// <value>
        /// The shut down idle thread time.
        /// </value>
        [DebuggerHidden]
        [XmlElement(Order = 3)]
        public Int32 ShutDownIdleThreadTime
        {
            get { return mShutDownIdleThreadTime; }
            set
            {
                DoDisposeCheck();
                if (value > 0 && !IsReadOnly)
                {
                    mShutDownIdleThreadTime = value;
                }
            }
        }

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
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsReadOnly
        {
            [DebuggerStepThrough]
            get { DoDisposeCheck(); return mIsReadOnly; }
            //set { mIsReadOnly = value; }
        }

        /// <summary>
        /// Gets or sets the size of the max stack.
        /// </summary>
        /// <value>
        /// The size of the max stack.
        /// </value>
        [DebuggerHidden]
        [XmlElement(Order = 4)]
        public Int32 MaxStackSize
        {
            get { return mMaxStackSize; }
            set { DoDisposeCheck(); if (value >= 0 && !mIsReadOnly) mMaxStackSize = value; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Queues the user work item.
        /// </summary>
        /// <param name="callBack">The call back.</param>
        [DebuggerStepThrough]
        public void QueueUserWorkItem(WaitCallback callBack)
        {
            QueueUserWorkItem(callBack, null);
        }

        /// <summary>
        /// Queues the user work item.
        /// </summary>
        /// <param name="callBack">The call back.</param>
        /// <param name="state">The state.</param>
        [DebuggerStepThrough]
        public void QueueUserWorkItem(WaitCallback callBack, Object state)
        {
            DoDisposeCheck();
            if (callBack == null)
            {
                ThrowHelper.ThrowArgumentNullException("callBack");
            }
            lock (mLockObject)
            {
                DoDisposeCheck();
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(String.Format("THREADPOOL({0}): enqueue new task for execution. Currently waiting task(s) in the queue: {1}", string.IsNullOrEmpty(mName) ? GetHashCode().ToString() : mName, mTaskContainerQueue.Count));

                TaskContainer tc = new TaskContainer() { WaitCallback = callBack, State = state };
                WorkerThread wt = null;
                if (mInactiveThreads.Count > 0)
                {
                    foreach (WorkerThread _wt in mInactiveThreads)
                    {
                        if (_wt.Task == null &&
                            _wt.Thread.ThreadState != System.Threading.ThreadState.Aborted &&
                            _wt.Thread.ThreadState != System.Threading.ThreadState.Stopped)
                        {
                            wt = _wt;
                        }
                    }
                }
                else if (mThreadNumber < mMaxThreadNumber)
                {
                    wt = StartNewThread();
                }
                if (wt == null)
                {
                    // nincs szabad végrehajtó
                    mTaskContainerQueue.Enqueue(tc);
                    if (mMaxThreadNumber > mThreadNumber)
                    {
                        // teli vagyunk, tartalék thread indítása
                        StartNewThread();
                    }
                }
                else
                {
                    // egyből hozzárendelem valakihez
                    mInactiveThreads.Remove(wt);
                    wt.Task = tc;
                    wt.WakeUpEvent.Set();
                }
            }
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

        #region Private helpers

        private void DoDisposeCheck()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!mDisposed && ThreadPoolConfiguration.SectionHandler.RestartOnExternalChanges)
            {
                ThreadPoolConfiguration.SectionHandler.OnConfigurationChanged -= new EventHandler<EventArgs>(SectionHandler_OnConfigurationChanged);
            }
            mDisposed = true;
            if (disposing)
            {
                int maxItemNumber = 64;
                List<WorkerThread> list = new List<WorkerThread>();
                List<EventWaitHandle> shutdownEvents = new List<EventWaitHandle>();
                List<List<EventWaitHandle>> seList = new List<List<EventWaitHandle>>();
                seList.Add(shutdownEvents);
                lock (mLockObject)
                {
                    foreach (WorkerThread wt in mExistingThreads)
                    {
                        if (wt.Thread.ThreadState != System.Threading.ThreadState.Aborted &&
                            wt.Thread.ThreadState != System.Threading.ThreadState.Stopped)
                        {
                            wt.IsShutdownForce = true;
                            wt.WakeUpEvent.Set();
                            list.Add(wt);
                            if (shutdownEvents.Count == maxItemNumber)
                            {
                                shutdownEvents = new List<EventWaitHandle>();
                                seList.Add(shutdownEvents);
                            }
                            shutdownEvents.Add(wt.ShutdownEvent);
                        }
                    }
                }
                foreach (List<EventWaitHandle> l in seList)
                {
                    if (l.Count > 0)
                    {
                        if (WaitHandle.WaitAll(l.ToArray(), 5000))
                        {
                            l.ForEach(i => i.Close());
                        }
                        l.Clear();
                    }
                }
                seList.Clear();
                if (list.Count > 0)
                {
                    list.ForEach(i => i.Dispose());
                    list.Clear();
                }
            }
        }

        private void OpenConfiguration()
        {
            if (ThreadPoolConfiguration.SectionHandler.RestartOnExternalChanges)
            {
                ThreadPoolConfiguration.SectionHandler.OnConfigurationChanged += new EventHandler<EventArgs>(SectionHandler_OnConfigurationChanged);
            }
            SectionHandler_OnConfigurationChanged(null, null);
        }

        private void SectionHandler_OnConfigurationChanged(object sender, EventArgs e)
        {
            if (LOGGER.IsInfoEnabled) LOGGER.Info(String.Format("THREADPOOL[{0}]: reading values from configuration...", string.IsNullOrEmpty(mName) ? GetHashCode().ToString() : mName));
            foreach (ThreadPoolItem item in ThreadPoolConfiguration.Settings.ThreadPools)
            {
                if (this.mName.Equals(item.Name) || string.IsNullOrEmpty(item.Name))
                {
                    if (item.MinThreadNumber >= 0)
                    {
                        this.mMinThreadNumber = item.MinThreadNumber;
                    }
                    if (item.MaxThreadNumber > 0)
                    {
                        this.mMaxThreadNumber = item.MaxThreadNumber;
                    }
                    if (item.ShutDownIdleThreadTime == -1 || item.ShutDownIdleThreadTime >= 1000)
                    {
                        this.mShutDownIdleThreadTime = item.ShutDownIdleThreadTime;
                    }
                    if (item.MaxStackSize > 0)
                    {
                        this.mMaxStackSize = item.MaxStackSize;
                    }
                    this.mIsReadOnly = item.SetReadOnlyFlag;
                    if (mMaxThreadNumber < mMinThreadNumber)
                    {
                        mMaxThreadNumber = mMinThreadNumber;
                    }

                    //if ( !this.mIsReadOnly )
                    //{
                    //    ThreadPoolConfiguration.SectionHandler.OnConfigurationChanged += new JZO.Forge.Configuration.Shared.ConfigurationChangedHandler( SectionHandler_OnConfigurationChanged );
                    //}
                    if (item.Name.Equals(this.Name))
                    {
                        // csak akkor állok le a kereséssel, ha a pontosan rám vonatkozó beállításokat találtam meg
                        break;
                    }
                }
            }
        }

        private void Initialize()
        {
            if (mMinThreadNumber > 0)
            {
                for (int i = 0; i < mMinThreadNumber; i++)
                {
                    StartNewThread();
                }
            }
            if (LOGGER.IsInfoEnabled) LOGGER.Info(String.Format("THREADPOOL: initialized, Name: {0}, MinThreadNumber: {1}, MaxThreadNumber: {2}, ShutDownIdleThreadTime: {3}, MaxStackSize: {4}, IsReadOnly: {5}", string.IsNullOrEmpty(mName) ? GetHashCode().ToString() : mName, MinThreadNumber, MaxThreadNumber, ShutDownIdleThreadTime, MaxStackSize, IsReadOnly.ToString()));
        }

        private WorkerThread StartNewThread()
        {
            Thread t = new Thread(new ParameterizedThreadStart(WorkerThreadMain));
            t.IsBackground = true;
            t.Name = string.Format("ThreadPool({0})-Thread{1}", string.IsNullOrEmpty(mName) ? GetHashCode().ToString() : mName, Interlocked.Increment(ref mThreadId));
            WorkerThread w = new WorkerThread(t);
            mInactiveThreads.Add(w);
            mExistingThreads.Add(w);
            Interlocked.Increment(ref mThreadNumber);
            t.Start(w);
            return w;
        }

        private void WorkerThreadMain(object state)
        {
            WorkerThread wt = (WorkerThread)state;

            if (LOGGER.IsDebugEnabled) LOGGER.Debug(String.Format("THREADPOOL[{0}]: starting new ThreadPool thread ({1}).", string.IsNullOrEmpty(mName) ? GetHashCode().ToString() : mName, Thread.CurrentThread.Name));

            try
            {
                while (!mDisposed && !wt.IsShutdownForce)
                {
                    if (wt.Task == null)
                    {
                        lock (mLockObject)
                        {
                            if (!mDisposed && !wt.IsShutdownForce)
                            {
                                // nincs leállás
                                if (wt.Task == null)
                                {
                                    // nincs kiosztott feladat
                                    if (mMaxThreadNumber < mThreadNumber)
                                    {
                                        // túl sok thread van, nekem nincs feladatom, leállok
                                        mInactiveThreads.Remove(wt);
                                        break;
                                    }
                                    else if (mTaskContainerQueue.Count > 0)
                                    {
                                        // van meló, kérek magamnak
                                        wt.Task = mTaskContainerQueue.Dequeue();
                                        mInactiveThreads.Remove(wt);
                                    }
                                }
                            }
                            else
                            {
                                // leállítás parancs van érvényben
                                mInactiveThreads.Remove(wt);
                                break;
                            }
                        }
                    }
                    if (wt.Task != null || wt.WakeUpEvent.WaitOne(mShutDownIdleThreadTime))
                    {
                        // jelzést kaptam vagy osztottak rám melót
                        if (wt.Task != null && !mDisposed && !wt.IsShutdownForce)
                        {
                            Execute(wt);
                        }
                    }
                    else
                    {
                        // nincs jelzésem
                        if (!mDisposed && !wt.IsShutdownForce)
                        {
                            lock (mLockObject)
                            {
                                if (wt.Task == null && mMinThreadNumber > mThreadNumber)
                                {
                                    // nincs feladatom, leállás...
                                    mInactiveThreads.Remove(wt);
                                    break;
                                }
                            }
                            if (wt.Task != null && !mDisposed && !wt.IsShutdownForce)
                            {
                                // közben kaptam feladatot...
                                Execute(wt);
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }

            lock (mLockObject)
            {
                Interlocked.Decrement(ref mThreadNumber);
                mInactiveThreads.Remove(wt);
                mExistingThreads.Remove(wt);
                if (wt.IsShutdownForce)
                {
                    // leállítási parancs érvényben, jelzés...
                    wt.ShutdownEvent.Set();
                }
                else
                {
                    wt.Dispose();
                }
            }

            if (LOGGER.IsDebugEnabled) LOGGER.Debug(String.Format("THREADPOOL[{0}]: thread ({1}) has exited.", string.IsNullOrEmpty(mName) ? GetHashCode().ToString() : mName, Thread.CurrentThread.Name));
        }

        private void Execute(WorkerThread wt)
        {
            try
            {
                TaskContainer tc = wt.Task;
                wt.Task = null;
                tc.WaitCallback.Invoke(tc.State);
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format("THREADPOOL[{0}]: an exception was thrown while executing a queued task. Exception: {1}", string.IsNullOrEmpty(mName) ? GetHashCode().ToString() : mName, ex.ToString()));
            }
            lock (mLockObject)
            {
                mInactiveThreads.Add(wt);
            }
        }

        #endregion

        #region Nested class(es)

        private class TaskContainer
        {

            /// <summary>
            /// Gets or sets the wait callback.
            /// </summary>
            /// <value>
            /// The wait callback.
            /// </value>
            internal WaitCallback WaitCallback { get; set; }

            /// <summary>
            /// Gets or sets the state.
            /// </summary>
            /// <value>
            /// The state.
            /// </value>
            internal object State { get; set; }

            /// <summary>
            /// Gets or sets the worker thread.
            /// </summary>
            /// <value>
            /// The worker thread.
            /// </value>
            internal WorkerThread WorkerThread { get; set; }

        }

        private class WorkerThread : IDisposable
        {

            /// <summary>
            /// Initializes a new instance of the <see cref="WorkerThread"/> class.
            /// </summary>
            /// <param name="thread">The thread.</param>
            internal WorkerThread(Thread thread)
            {
                this.Thread = thread;
                this.WakeUpEvent = new AutoResetEvent(false);
                this.ShutdownEvent = new ManualResetEvent(false);
            }

            /// <summary>
            /// Gets or sets the thread.
            /// </summary>
            /// <value>
            /// The thread.
            /// </value>
            internal Thread Thread { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is shutdown force.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is shutdown force; otherwise, <c>false</c>.
            /// </value>
            internal bool IsShutdownForce { get; set; }

            /// <summary>
            /// Gets or sets the wake up event.
            /// </summary>
            /// <value>
            /// The wake up event.
            /// </value>
            internal AutoResetEvent WakeUpEvent { get; private set; }

            /// <summary>
            /// Gets or sets the shutdown event.
            /// </summary>
            /// <value>
            /// The shutdown event.
            /// </value>
            internal ManualResetEvent ShutdownEvent { get; private set; }

            /// <summary>
            /// Gets or sets the task.
            /// </summary>
            /// <value>
            /// The task.
            /// </value>
            internal TaskContainer Task { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            [MethodImpl(MethodImplOptions.Synchronized)]
            public override bool Equals(object obj)
            {
                if (this.Thread == null)
                {
                    return false;
                }
                return this.Thread.Name.Equals(((WorkerThread)obj).Thread.Name);
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
            [MethodImpl(MethodImplOptions.Synchronized)]
            public void Dispose()
            {
                this.Thread = null;
                this.WakeUpEvent.Close();
                this.ShutdownEvent.Close();
            }

        }

        #endregion

    }

}
