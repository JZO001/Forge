/* *********************************************************************
 * Date: 23 May 2007
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using log4net;

namespace Forge.EventRaiser
{

    /// <summary>
    /// Sync and async delegate caller which keep the order of the caller threads
    /// </summary>
    /// <param name="result">The result.</param>
    public delegate void AsyncRaiserCallback(DelegateInvokeResult result);

    /// <summary>
    /// Sync and async delegate caller which keep the order of the caller threads
    /// </summary>
    public sealed class Raiser
    {

        #region Static variables

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(Raiser));
        internal static Forge.Threading.ThreadPool RaiserThreadPool = new Forge.Threading.ThreadPool("Forge.EventRaiser.ThreadPool");
        private static Dictionary<String, TypeContainer> mTypeContainers = new Dictionary<String, TypeContainer>();
        private static Int32 mExecutionThreadRunningTime = 15; // value is in milliseconds

        #endregion

        #region Constructor(s)

        private Raiser()
        {
        }

        #endregion

        #region Private method

        private static void EventExecutorThreadMain()
        {
            TypeContainer container = null;

            lock (mTypeContainers)
            {
                container = mTypeContainers[Thread.CurrentThread.Name];
            }

            //int counter = 0; // this is for performance testing
            Stopwatch swExecutionStopper = null;
            while (true)
            {
                container.ExecutorThreadSemaphore.WaitOne();
                //counter++; // this is for performance testing

                if (swExecutionStopper == null)
                {
                    // need to instantiate here, because WaitOne occurs false result for measurement
                    swExecutionStopper = Stopwatch.StartNew();
                }

                EventDataContainer cnt = null;
                lock (container.Queue)
                {
                    if (container.Queue.Count > 0)
                    {
                        cnt = container.Queue.Dequeue();
                    }
                }

                if (cnt != null)
                {
                    if (cnt.Delegate != null)
                    {
                        List<Object> result = CallDelegatorBySync(cnt.Delegate, cnt.Parameters, cnt.ControlInvoke, cnt.ParallelInvocation);
                        if (cnt.CallBack != null)
                        {
                            try
                            {
                                cnt.CallBack.Method.Invoke(cnt.CallBack.Target, new Object[] { new DelegateInvokeResult(cnt.Delegate, cnt.Parameters, result, cnt.ControlInvoke, cnt.ParallelInvocation, cnt.State) });
                            }
                            catch (Exception ex)
                            {
                                if (LOGGER.IsErrorEnabled) LOGGER.Error("RAISER, an error occured while AsyncRaiserCallBack invoked.", ex);
                            }
                            //Raiser.RaiserThreadPool.QueueUserWorkItem( cnt.CallBack, new DelegateInvokeResult( cnt.Delegate, cnt.Parameters, cnt.CallBack, result, cnt.ControlInvoke, cnt.ParallelInvocation ) );
                        }
                    }
                    cnt.Dispose();
                }

                // check execution time
                if (swExecutionStopper.ElapsedMilliseconds > mExecutionThreadRunningTime && mExecutionThreadRunningTime != Timeout.Infinite)
                {
                    swExecutionStopper.Stop();

                    //StaticLogger.Logger.WriteInfo( swExecutionStopper.ElapsedMilliseconds.ToString( ) + ", counter: " + counter.ToString( ) ); // this is for performance testing
                    //counter = 0; // this is for performance testing

                    swExecutionStopper = null;
                    Thread.Sleep(0);
                }

            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Calls the provided delegate asynchronously.
        /// </summary>
        /// <param name="dl">Delegate to raise</param>
        /// <param name="parameters">Parameter list of the target method</param>
        [DebuggerStepThrough]
        public static void CallDelegatorByAsync(Delegate dl, Object[] parameters)
        {
            CallDelegatorByAsync(dl, parameters, false, false, null, null);
        }

        /// <summary>
        /// Calls the provided delegate asynchronously.
        /// </summary>
        /// <param name="dl">Delegate to raise</param>
        /// <param name="parameters">Parameter list of the target method</param>
        /// <param name="callback">Specify a callback method, if you want to receive results</param>
        /// <param name="state">The state.</param>
        [DebuggerStepThrough]
        public static void CallDelegatorByAsync(Delegate dl, Object[] parameters, AsyncRaiserCallback callback, Object state)
        {
            CallDelegatorByAsync(dl, parameters, false, false, callback, state);
        }

        /// <summary>
        /// Calls the provided delegate asynchronously.
        /// </summary>
        /// <param name="dl">Delegate to raise</param>
        /// <param name="parameters">Parameter list of the target method</param>
        /// <param name="controlInvoke">Call UI thread if the target object is a control in the InvocationList</param>
        [DebuggerStepThrough]
        public static void CallDelegatorByAsync(Delegate dl, Object[] parameters, bool controlInvoke)
        {
            CallDelegatorByAsync(dl, parameters, controlInvoke, false, null, null);
        }

        /// <summary>
        /// Calls the provided delegate asynchronously.
        /// </summary>
        /// <param name="dl">Delegate to raise</param>
        /// <param name="parameters">Parameter list of the target method</param>
        /// <param name="controlInvoke">Call UI thread if the target object is a control in the InvocationList</param>
        /// <param name="parallelInvocation">Execute invocation list within a separated thread</param>
        [DebuggerStepThrough]
        public static void CallDelegatorByAsync(Delegate dl, Object[] parameters, bool controlInvoke, bool parallelInvocation)
        {
            CallDelegatorByAsync(dl, parameters, controlInvoke, parallelInvocation, null, null);
        }

        /// <summary>
        /// Calls the provided delegate asynchronously.
        /// </summary>
        /// <param name="dl">Delegate to raise</param>
        /// <param name="parameters">Parameter list of the target method</param>
        /// <param name="controlInvoke">Call UI thread if the target object is a control in the InvocationList</param>
        /// <param name="parallelInvocation">Execute invocation list within a separated thread</param>
        /// <param name="callback">Specify a callback method, if you want to receive results</param>
        /// <param name="state">The state.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), DebuggerStepThrough]
        public static void CallDelegatorByAsync(Delegate dl, Object[] parameters, bool controlInvoke, bool parallelInvocation, AsyncRaiserCallback callback, Object state)
        {
            TypeContainer container = null;
            TypeNameResolver name_res = new TypeNameResolver();

            lock (mTypeContainers)
            {
                if (mTypeContainers.ContainsKey(name_res.CallerTypeName))
                {
                    container = mTypeContainers[name_res.CallerTypeName];
                }
                else
                {
                    container = new TypeContainer();
                    mTypeContainers[name_res.CallerTypeName] = container;
                }

                if (container.Thread == null)
                {
                    container.Thread = new Thread(new ThreadStart(EventExecutorThreadMain));
                    container.Thread.IsBackground = true;
                    container.Thread.Name = name_res.CallerTypeName;
                    container.Thread.Start();
                }
            }

            lock (container.Queue)
            {
                container.Queue.Enqueue(new EventDataContainer(dl, parameters, controlInvoke, parallelInvocation, callback, state));
            }

            container.ExecutorThreadSemaphore.Release();
        }

        /// <summary>
        /// Calls the provided delegate synchronously.
        /// </summary>
        /// <param name="dl">Delegate to raise</param>
        /// <param name="parameters">Parameter list of the target method</param>
        /// <returns>List of results which collected from the subscribers</returns>
        [DebuggerStepThrough]
        public static List<Object> CallDelegatorBySync(Delegate dl, Object[] parameters)
        {
            return CallDelegatorBySync(dl, parameters, false, false);
        }

        /// <summary>
        /// Calls the provided delegate synchronously.
        /// </summary>
        /// <param name="dl">Delegate to raise</param>
        /// <param name="parameters">Parameter list of the target method</param>
        /// <param name="controlInvoke">Call UI thread if the target object is a control in the InvocationList</param>
        /// <returns>List of results which collected from the subscribers</returns>
        [DebuggerStepThrough]
        public static List<Object> CallDelegatorBySync(Delegate dl, Object[] parameters, bool controlInvoke)
        {
            return CallDelegatorBySync(dl, parameters, controlInvoke, false);
        }

        /// <summary>
        /// Calls the provided delegate synchronously.
        /// </summary>
        /// <param name="dl">Delegate to raise</param>
        /// <param name="parameters">Parameter list of the target method</param>
        /// <param name="controlInvoke">Call UI thread if the target object is a control in the InvocationList</param>
        /// <param name="parallelInvocation">Execute invocation list within a separated thread</param>
        /// <returns>
        /// List of results which collected from the subscribers
        /// </returns>
        [DebuggerStepThrough]
        public static List<Object> CallDelegatorBySync(Delegate dl, Object[] parameters, bool controlInvoke, bool parallelInvocation)
        {
            List<Object> result = new List<Object>();
            if (dl != null)
            {
                Delegate[] list = dl.GetInvocationList();
                if (parallelInvocation)
                {
                    // parallel execution
                    using (AsyncInvocationContainer aic = new AsyncInvocationContainer())
                    {
                        result.AddRange(aic.Execute(list, parameters, controlInvoke));
                    }
                }
                else
                {
                    // sequential execution
                    foreach (Delegate d in list)
                    {
                        try
                        {
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("RAISER, before invoke on '{0}' with method '{1}'. Invoke on control: {2}, parallel: {3}", d.Target == null ? d.Method.DeclaringType.FullName : d.Target.GetType().FullName, d.Method.Name, controlInvoke.ToString(), parallelInvocation.ToString()));
                            if (controlInvoke && d.Target is Control)
                            {
                                result.Add(((Control)d.Target).Invoke(d, parameters));
                            }
                            else if (controlInvoke && d.Target is DependencyObject)
                            {
                                DependencyObject ctrl = (DependencyObject)d.Target;
                                result.Add(ctrl.Dispatcher.Invoke(d, parameters));
                            }
                            else
                            {
                                result.Add(d.Method.Invoke(d.Target, parameters));
                            }
                        }
                        catch (Exception e)
                        {
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("RAISER, failed to invoke on '{0}' with method '{1}'. Invoke on control: {2}, parallel: {3}. Reason: {4}", d.Target == null ? d.Method.DeclaringType.FullName : d.Target.GetType().FullName, d.Method.Name, controlInvoke.ToString(), parallelInvocation.ToString(), e.Message), e);
                            result.Add(e);
                        }
                        finally
                        {
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("RAISER, after invoke on '{0}' with method '{1}'. Invoke on control: {2}, parallel: {3}", d.Target == null ? d.Method.DeclaringType.FullName : d.Target.GetType().FullName, d.Method.Name, controlInvoke.ToString(), parallelInvocation.ToString()));
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get or set the maximum execution time of event invoker thread in milliseconds
        /// </summary>
        /// <value>
        /// The execution thread running time.
        /// </value>
        /// <exception cref="System.ArgumentException">Set value greater than zero</exception>
        public static Int32 ExecutionThreadRunningTime
        {
            get { return Raiser.mExecutionThreadRunningTime; }
            set
            {
                if (value <= 0 && value != Timeout.Infinite)
                {
                    throw new ArgumentException("Set value greater than zero");
                }
                Raiser.mExecutionThreadRunningTime = value;
            }
        }

        #endregion

        #region Nested classes

        [Serializable]
        private sealed class TypeContainer : IDisposable
        {

            #region Variables

            [NonSerialized]
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Thread mThread = null;

            [NonSerialized]
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Semaphore mExecutorThreadSemaphore = new Semaphore(0, Int32.MaxValue);

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Queue<EventDataContainer> mQueue = new Queue<EventDataContainer>();

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="TypeContainer"/> class.
            /// </summary>
            internal TypeContainer()
            {
            }

            #endregion

            #region Internal properties

            /// <summary>
            /// Gets or sets the thread.
            /// </summary>
            /// <value>
            /// The thread.
            /// </value>
            [DebuggerHidden]
            internal Thread Thread
            {
                get { return mThread; }
                set { mThread = value; }
            }

            /// <summary>
            /// Gets the executor thread semaphore.
            /// </summary>
            [DebuggerHidden]
            internal Semaphore ExecutorThreadSemaphore
            {
                get { return mExecutorThreadSemaphore; }
                //set { mExecutorThreadSemaphore = value; }
            }

            /// <summary>
            /// Gets the queue.
            /// </summary>
            [DebuggerHidden]
            internal Queue<EventDataContainer> Queue
            {
                get { return mQueue; }
                //set { mQueue = value; }
            }

            #endregion

            #region Public method(s)

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                mExecutorThreadSemaphore.Close();
            }

            #endregion

        }

        [Serializable]
        private sealed class EventDataContainer : IDisposable
        {

            #region Variables

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Delegate mDelegate = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Object[] mParameters = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private AsyncRaiserCallback mCallBack = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool mControlInvoke = true;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool mParallelInvocation = true;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Object mState = null;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="EventDataContainer"/> class.
            /// </summary>
            internal EventDataContainer()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="EventDataContainer"/> class.
            /// </summary>
            /// <param name="del">The del.</param>
            /// <param name="parameters">The parameters.</param>
            /// <param name="controlInvoke">if set to <c>true</c> [control invoke].</param>
            /// <param name="parallelInvocation">if set to <c>true</c> [parallel invocation].</param>
            /// <param name="callBack">The call back.</param>
            /// <param name="state">The state.</param>
            internal EventDataContainer(Delegate del,
                Object[] parameters,
                bool controlInvoke,
                bool parallelInvocation,
                AsyncRaiserCallback callBack,
                Object state)
            {
                this.mDelegate = del;
                this.mParameters = parameters;
                this.mCallBack = callBack;
                this.mControlInvoke = controlInvoke;
                this.mParallelInvocation = parallelInvocation;
                this.mState = state;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the delegate.
            /// </summary>
            [DebuggerHidden]
            internal Delegate Delegate
            {
                get { return mDelegate; }
                //set { mDelegate = value; }
            }

            /// <summary>
            /// Gets the parameters.
            /// </summary>
            [DebuggerHidden]
            internal Object[] Parameters
            {
                get { return mParameters; }
                //set { mParameters = value; }
            }

            /// <summary>
            /// Gets the call back.
            /// </summary>
            [DebuggerHidden]
            internal AsyncRaiserCallback CallBack
            {
                get { return mCallBack; }
                //set { mCallBack = value; }
            }

            /// <summary>
            /// Gets a value indicating whether [control invoke].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [control invoke]; otherwise, <c>false</c>.
            /// </value>
            [DebuggerHidden]
            internal bool ControlInvoke
            {
                get { return mControlInvoke; }
                //set { mControlInvoke = value; }
            }

            /// <summary>
            /// Gets a value indicating whether [parallel invocation].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [parallel invocation]; otherwise, <c>false</c>.
            /// </value>
            [DebuggerHidden]
            internal bool ParallelInvocation
            {
                get { return mParallelInvocation; }
                //set { mParallelInvocation = value; }
            }

            /// <summary>
            /// Gets the state.
            /// </summary>
            [DebuggerHidden]
            internal Object State
            {
                get { return mState; }
                //set { mState = value; }
            }

            #endregion

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                mDelegate = null;
                mParameters = null;
                mCallBack = null;
                mState = null;
            }

            #endregion

        }

        [Serializable]
        private sealed class AsyncInvocationContainer : IDisposable
        {

            #region Variables

            [NonSerialized]
            private Semaphore mExecutorThreadSemaphore = null;

            private Object[] mResultObjects = null;

            private Object[] mParameters = null;

            private bool mControlInvoke = true;

            #endregion

            #region Constructors

            public AsyncInvocationContainer()
            {
            }

            #endregion

            #region Private methods

            private void InvokerThreadMain(Object param)
            {
                Object[] data = (Object[])param;
                Delegate dl = (Delegate)data[1];
                Int32 index = (Int32)data[0];
                try
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("RAISER, before invoke on '{0}' with method '{1}'. Invoke on control: {2}, parallel: true", dl.Target == null ? dl.Method.DeclaringType.FullName : dl.Target.GetType().FullName, dl.Method.Name, mControlInvoke.ToString()));

                    if (dl.Target is Control && mControlInvoke)
                    {
                        mResultObjects[index] = ((Control)dl.Target).Invoke(dl, mParameters);
                    }
                    else if (dl.Target is DependencyObject && mControlInvoke)
                    {
                        DependencyObject ctrl = (DependencyObject)dl.Target;
                        mResultObjects[index] = ctrl.Dispatcher.Invoke(dl, mParameters);
                    }
                    else
                    {
                        mResultObjects[index] = dl.Method.Invoke(dl.Target, mParameters);
                    }
                }
                catch (Exception e)
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("RAISER, failed to invoke on '{0}' with method '{1}'. Invoke on control: {2}, parallel: true. Reason: {3}", dl.Target == null ? dl.Method.DeclaringType.FullName : dl.Target.GetType().FullName, dl.Method.Name, mControlInvoke.ToString(), e.Message), e);
                    mResultObjects[index] = e;
                }
                finally
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("RAISER, after invoke on '{0}' with method '{1}'. Invoke on control: {2}, parallel: true", dl.Target == null ? dl.Method.DeclaringType.FullName : dl.Target.GetType().FullName, dl.Method.Name, mControlInvoke.ToString()));
                }
                mExecutorThreadSemaphore.Release();
            }

            #endregion

            #region Public methods

            /// <summary>
            /// Executes the specified invocation list.
            /// </summary>
            /// <param name="invocationList">The invocation list.</param>
            /// <param name="parameters">The parameters.</param>
            /// <param name="controlInvoke">if set to <c>true</c> [control invoke].</param>
            /// <returns></returns>
            internal List<Object> Execute(Delegate[] invocationList, Object[] parameters, bool controlInvoke)
            {
                this.mParameters = parameters;
                this.mControlInvoke = controlInvoke;

                mExecutorThreadSemaphore = new Semaphore(0, invocationList.Length);
                mResultObjects = new Object[invocationList.Length];
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Raiser.RaiserThreadPool.QueueUserWorkItem(new WaitCallback(InvokerThreadMain), new Object[] { i, invocationList[i] });
                }

                for (int i = 0; i < invocationList.Length; i++)
                {
                    mExecutorThreadSemaphore.WaitOne();
                }

                return new List<Object>(mResultObjects);
            }

            #endregion

            #region Public properties

            //public List<Object> ResultObjects
            //{
            //    get { return new List<Object>( mResultObjects ); }
            //    set { mResultObjects = value.ToArray( ); }
            //}

            #endregion

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                mExecutorThreadSemaphore.Close();
                mResultObjects = null;
            }

            #endregion

        }

        #endregion

    }

}
