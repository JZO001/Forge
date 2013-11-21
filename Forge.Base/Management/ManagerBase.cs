/* *********************************************************************
 * Date: 22 Jan 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Forge.EventRaiser;

namespace Forge.Management
{

    internal delegate ManagerStateEnum ManagerStartDelegate();

    internal delegate ManagerStateEnum ManagerStopDelegate();

    /// <summary>
    /// Represents the base methods and properties of a manager service
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), Serializable]
    public abstract class ManagerBase : MBRBase, IManager
    {

        #region Field(s)

        [NonSerialized]
        private int mAsyncActiveStartCount = 0;

        [NonSerialized]
        private AutoResetEvent mAsyncActiveStartEvent = null;

        [NonSerialized]
        private ManagerStartDelegate mStartDelegate = null;

        [NonSerialized]
        private int mAsyncActiveStopCount = 0;

        [NonSerialized]
        private AutoResetEvent mAsyncActiveStopEvent = null;

        [NonSerialized]
        private ManagerStopDelegate mStopDelegate = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object mLockObjectForEvents = new object();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NonSerialized]
        private ManagerStateEnum mManagerstate = ManagerStateEnum.Uninitialized;

        private static readonly object ASYNC_BEGIN_LOCK = new object();

        [NonSerialized]
        private EventHandler<ManagerEventStateEventArgs> mEventStartDelegate;

        [NonSerialized]
        private EventHandler<ManagerEventStateEventArgs> mEventStopDelegate;

        /// <summary>
        /// Occurs when [event start].
        /// </summary>
        public event EventHandler<ManagerEventStateEventArgs> EventStart
        {
            add
            {
                lock (mLockObjectForEvents)
                {
                    mEventStartDelegate = (EventHandler<ManagerEventStateEventArgs>)Delegate.Combine(mEventStartDelegate, value);
                }
            }
            remove
            {
                lock (mLockObjectForEvents)
                {
                    mEventStartDelegate = (EventHandler<ManagerEventStateEventArgs>)Delegate.Remove(mEventStartDelegate, value);
                }
            }
        }

        /// <summary>
        /// Occurs when [event stop].
        /// </summary>
        public event EventHandler<ManagerEventStateEventArgs> EventStop
        {
            add
            {
                lock (mLockObjectForEvents)
                {
                    mEventStopDelegate = (EventHandler<ManagerEventStateEventArgs>)Delegate.Combine(mEventStopDelegate, value);
                }
            }
            remove
            {
                lock (mLockObjectForEvents)
                {
                    mEventStopDelegate = (EventHandler<ManagerEventStateEventArgs>)Delegate.Remove(mEventStopDelegate, value);
                }
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerBase" /> class.
        /// </summary>
        protected ManagerBase()
            : base()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the state of the manager.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public ManagerStateEnum ManagerState
        {
            get
            {
                return mManagerstate;
            }
            protected set
            {
                mManagerstate = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [event sync invocation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [event sync invocation]; otherwise, <c>false</c>.
        /// </value>
        public bool EventSyncInvocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [event UI invocation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [event UI invocation]; otherwise, <c>false</c>.
        /// </value>
        public bool EventUIInvocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [event parallel invocation].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [event parallel invocation]; otherwise, <c>false</c>.
        /// </value>
        public bool EventParallelInvocation
        {
            get;
            set;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Starts the manager asyncronously.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        [DebuggerHidden]
        public IAsyncResult BeginStart(AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveStartCount);
            ManagerStartDelegate d = new ManagerStartDelegate(this.Start);
            if (this.mAsyncActiveStartEvent == null)
            {
                lock (ASYNC_BEGIN_LOCK)
                {
                    if (this.mAsyncActiveStartEvent == null)
                    {
                        this.mAsyncActiveStartEvent = new AutoResetEvent(true);
                    }
                }
            }
            this.mAsyncActiveStartEvent.WaitOne();
            this.mStartDelegate = d;
            return d.BeginInvoke(callback, state);
        }

        /// <summary>
        /// Starts this manager instance.
        /// </summary>
        /// <returns>
        /// Manager State
        /// </returns>
        public abstract ManagerStateEnum Start();

        /// <summary>
        /// Ends the asynchronous start process.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>
        /// Manager State
        /// </returns>
        [DebuggerHidden]
        public ManagerStateEnum EndStart(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (this.mStartDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndStart called multiple times.", "asyncResult");
            }
            try
            {
                return this.mStartDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                this.mStartDelegate = null;
                this.mAsyncActiveStartEvent.Set();
                CloseAsyncActiveStartEvent(Interlocked.Decrement(ref mAsyncActiveStartCount));
            }
        }

        /// <summary>
        /// Stops the manager asyncronously.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Async property
        /// </returns>
        [DebuggerHidden]
        public IAsyncResult BeginStop(AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref mAsyncActiveStopCount);
            ManagerStopDelegate d = new ManagerStopDelegate(this.Stop);
            if (this.mAsyncActiveStopEvent == null)
            {
                lock (ASYNC_BEGIN_LOCK)
                {
                    if (this.mAsyncActiveStopEvent == null)
                    {
                        this.mAsyncActiveStopEvent = new AutoResetEvent(true);
                    }
                }
            }
            this.mAsyncActiveStopEvent.WaitOne();
            this.mStopDelegate = d;
            return d.BeginInvoke(callback, state);
        }

        /// <summary>
        /// Stops this manager instance.
        /// </summary>
        /// <returns>
        /// Manager State
        /// </returns>
        public abstract ManagerStateEnum Stop();

        /// <summary>
        /// Ends the asynchronous stop process.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>
        /// Manager State
        /// </returns>
        [DebuggerHidden]
        public ManagerStateEnum EndStop(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                ThrowHelper.ThrowArgumentNullException("asyncResult");
            }
            if (this.mStopDelegate == null)
            {
                ThrowHelper.ThrowArgumentException("Wrong async result or EndStop called multiple times.", "asyncResult");
            }
            try
            {
                return this.mStopDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                this.mStopDelegate = null;
                this.mAsyncActiveStopEvent.Set();
                CloseAsyncActiveStopEvent(Interlocked.Decrement(ref mAsyncActiveStopCount));
            }
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Raises the event.
        /// </summary>
        /// <param name="del">The delegae (event).</param>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void RaiseEvent(Delegate del, object sender, EventArgs e)
        {
            if (EventSyncInvocation)
            {
                Raiser.CallDelegatorBySync(del, new object[] { sender, e }, EventUIInvocation, EventParallelInvocation);
            }
            else
            {
                Raiser.CallDelegatorByAsync(del, new object[] { sender, e }, EventUIInvocation, EventParallelInvocation);
            }
        }

        /// <summary>
        /// Called when [start].
        /// </summary>
        /// <param name="state">The state.</param>
        protected virtual void OnStart(ManagerEventStateEnum state)
        {
            RaiseEvent(mEventStartDelegate, this, new ManagerEventStateEventArgs(state));
        }

        /// <summary>
        /// Raises the <see cref="E:StartWithCustomEventArgs" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ManagerEventStateEventArgs" /> instance containing the event data.</param>
        protected virtual void OnStartWithCustomEventArgs(ManagerEventStateEventArgs e)
        {
            RaiseEvent(mEventStartDelegate, this, e);
        }

        /// <summary>
        /// Called when [stop].
        /// </summary>
        /// <param name="state">The state.</param>
        protected virtual void OnStop(ManagerEventStateEnum state)
        {
            RaiseEvent(mEventStopDelegate, this, new ManagerEventStateEventArgs(state));
        }

        /// <summary>
        /// Raises the <see cref="E:StopWithCustomEventArgs" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ManagerEventStateEventArgs" /> instance containing the event data.</param>
        protected virtual void OnStopWithCustomEventArgs(ManagerEventStateEventArgs e)
        {
            RaiseEvent(mEventStopDelegate, this, e);
        }

        #endregion

        #region Private method(s)

        private void CloseAsyncActiveStartEvent(int asyncActiveCount)
        {
            if ((this.mAsyncActiveStartEvent != null) && (asyncActiveCount == 0))
            {
                this.mAsyncActiveStartEvent.Close();
                this.mAsyncActiveStartEvent = null;
            }
        }

        private void CloseAsyncActiveStopEvent(int asyncActiveCount)
        {
            if ((this.mAsyncActiveStopEvent != null) && (asyncActiveCount == 0))
            {
                this.mAsyncActiveStopEvent.Close();
                this.mAsyncActiveStopEvent = null;
            }
        }

        #endregion

    }

}
