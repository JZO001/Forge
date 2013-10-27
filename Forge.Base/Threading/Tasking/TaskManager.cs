/* *********************************************************************
 * Date: 28 Feb 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using log4net;

namespace Forge.Threading.Tasking
{

    /// <summary>
    /// Executes a task and than call back with the result
    /// </summary>
    public sealed class TaskManager : MBRBase
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TaskManager));

        private static readonly ThreadPool mThreadPool = new ThreadPool("TaskManager_ThreadPool", 1, 10, 256);

        private readonly Dictionary<int, List<QueueItem>> mOrderedItems = new Dictionary<int, List<QueueItem>>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskManager"/> class.
        /// </summary>
        public TaskManager()
        {
            this.ChaosTheoryMode = ChaosTheoryEnum.OrderByTaskDelegateTarget;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskManager"/> class.
        /// </summary>
        public TaskManager(ChaosTheoryEnum chaosTheory)
        {
            this.ChaosTheoryMode = chaosTheory;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the chaos theory mode.
        /// </summary>
        /// <value>
        /// The chaos theory mode.
        /// </value>
        public ChaosTheoryEnum ChaosTheoryMode { get; private set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        public void Execute(Action taskDelegate, ReturnCallback returnDelegate)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        public void Execute<T1>(Action<T1> taskDelegate, ReturnCallback returnDelegate, T1 p1)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        public void Execute<T1, T2>(Action<T1, T2> taskDelegate, ReturnCallback returnDelegate, T1 p1, T2 p2)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        public void Execute<T1, T2, T3>(Action<T1, T2, T3> taskDelegate, ReturnCallback returnDelegate, T1 p1, T2 p2, T3 p3)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2, p3);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        public void Execute<T1, T2, T3, T4>(Action<T1, T2, T3, T4> taskDelegate, ReturnCallback returnDelegate, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2, p3, p4);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="p5">The p5.</param>
        public void Execute<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> taskDelegate, ReturnCallback returnDelegate, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2, p3, p4, p5);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="p5">The p5.</param>
        /// <param name="p6">The p6.</param>
        public void Execute<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> taskDelegate, ReturnCallback returnDelegate, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2, p3, p4, p5, p6);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="T7">The type of the 7.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="p5">The p5.</param>
        /// <param name="p6">The p6.</param>
        /// <param name="p7">The p7.</param>
        public void Execute<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> taskDelegate, ReturnCallback returnDelegate, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2, p3, p4, p5, p6, p7);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="T7">The type of the 7.</typeparam>
        /// <typeparam name="T8">The type of the 8.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="p5">The p5.</param>
        /// <param name="p6">The p6.</param>
        /// <param name="p7">The p7.</param>
        /// <param name="p8">The p8.</param>
        public void Execute<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> taskDelegate, ReturnCallback returnDelegate, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2, p3, p4, p5, p6, p7, p8);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        public void Execute<TResult>(Func<TResult> taskDelegate, ReturnCallback<TResult> returnDelegate)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        public void Execute<T1, TResult>(Func<T1, TResult> taskDelegate, ReturnCallback<TResult> returnDelegate, T1 p1)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        public void Execute<T1, T2, TResult>(Func<T1, T2, TResult> taskDelegate, ReturnCallback<TResult> returnDelegate, T1 p1, T2 p2)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        public void Execute<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> taskDelegate, ReturnCallback<TResult> returnDelegate, T1 p1, T2 p2, T3 p3)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2, p3);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        public void Execute<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> taskDelegate, ReturnCallback<TResult> returnDelegate, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2, p3, p4);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="p5">The p5.</param>
        public void Execute<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> taskDelegate, ReturnCallback<TResult> returnDelegate, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2, p3, p4, p5);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="p5">The p5.</param>
        /// <param name="p6">The p6.</param>
        public void Execute<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> taskDelegate, ReturnCallback<TResult> returnDelegate, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2, p3, p4, p5, p6);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="T7">The type of the 7.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="p5">The p5.</param>
        /// <param name="p6">The p6.</param>
        /// <param name="p7">The p7.</param>
        public void Execute<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> taskDelegate, ReturnCallback<TResult> returnDelegate, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2, p3, p4, p5, p6, p7);
        }

        /// <summary>
        /// Executes the specified task delegate.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="T7">The type of the 7.</typeparam>
        /// <typeparam name="T8">The type of the 8.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="taskDelegate">The task delegate.</param>
        /// <param name="returnDelegate">The return delegate.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="p5">The p5.</param>
        /// <param name="p6">The p6.</param>
        /// <param name="p7">The p7.</param>
        /// <param name="p8">The p8.</param>
        public void Execute<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> taskDelegate, ReturnCallback<TResult> returnDelegate, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
        {
            Execute((Delegate)taskDelegate, (Delegate)returnDelegate, p1, p2, p3, p4, p5, p6, p7, p8);
        }

        #endregion

        #region Private method(s)

        private void Execute(Delegate taskDelegate, Delegate returnDelegate, params object[] inParameters)
        {
            if (taskDelegate == null)
            {
                ThrowHelper.ThrowArgumentNullException("taskDelegate");
            }
            if (returnDelegate == null && ChaosTheoryMode == ChaosTheoryEnum.OrderByReturnDelegateTarget)
            {
                ThrowHelper.ThrowArgumentNullException("returnDelegate");
            }

            QueueItem item = new QueueItem() { TaskDelegate = taskDelegate, ReturnDelegate = returnDelegate, InParameters = inParameters };
            switch (ChaosTheoryMode)
            {
                case ChaosTheoryEnum.Chaos:
                    {
                        mThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ExecuteTask), item);
                    }
                    break;

                case ChaosTheoryEnum.OrderByTaskDelegateTarget:
                    {
                        BeginExecution(taskDelegate.Target.GetHashCode(), item);
                    }
                    break;

                case ChaosTheoryEnum.OrderByReturnDelegateTarget:
                    {
                        BeginExecution(returnDelegate.Target.GetHashCode(), item);
                    }
                    break;
            }
        }

        private void BeginExecution(int targetHash, QueueItem item)
        {
            lock (mOrderedItems)
            {
                List<QueueItem> list = null;
                if (mOrderedItems.ContainsKey(targetHash))
                {
                    list = mOrderedItems[targetHash];
                }
                else
                {
                    list = new List<QueueItem>();
                    mOrderedItems[targetHash] = list;
                }

                list.Add(item);
                if (list.Count == 1)
                {
                    mThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ExecuteTask), item);
                }
            }
        }

        private void ExecuteTask(object state)
        {
            QueueItem item = state as QueueItem;
            Exception exception = null;
            object methodResult = null;

            try
            {
                methodResult = item.TaskDelegate.Method.Invoke(item.TaskDelegate.Target, item.InParameters);
            }
            catch (TargetInvocationException ex)
            {
                exception = ex.InnerException;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            TaskResult result = null;
            if (item.TaskDelegate.Method.ReturnType.Equals(typeof(void)))
            {
                result = new TaskResult(item.InParameters, exception);
            }
            else if (exception == null)
            {
                result = (TaskResult)typeof(TaskResult<>).MakeGenericType(item.TaskDelegate.Method.ReturnType).GetConstructor(new Type[] { typeof(object[]), typeof(Exception), item.TaskDelegate.Method.ReturnType }).Invoke(new object[] { item.InParameters, exception, methodResult });
            }
            else
            {
                result = (TaskResult)typeof(TaskResult<>).MakeGenericType(item.TaskDelegate.Method.ReturnType).GetConstructor(new Type[] { typeof(object[]), typeof(Exception) }).Invoke(new object[] { item.InParameters, exception });
            }

            if (item.ReturnDelegate != null)
            {
                try
                {
                    if (item.ReturnDelegate.Target is Control)
                    {
                        ((Control)item.ReturnDelegate.Target).Invoke(item.ReturnDelegate, new object[] { result });
                    }
                    else if (item.ReturnDelegate.Target is DependencyObject)
                    {
                        DependencyObject ctrl = (DependencyObject)item.ReturnDelegate.Target;
                        ctrl.Dispatcher.Invoke(item.ReturnDelegate, new object[] { result });
                    }
                    else
                    {
                        item.ReturnDelegate.Method.Invoke(item.ReturnDelegate.Target, new object[] { result });
                    }
                }
                catch (Exception e)
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(e.Message, e);
                }
            }

            if (ChaosTheoryMode != ChaosTheoryEnum.Chaos)
            {
                lock (mOrderedItems)
                {
                    int hashCode = ChaosTheoryMode == ChaosTheoryEnum.OrderByTaskDelegateTarget ? item.TaskDelegate.Target.GetHashCode() : item.ReturnDelegate.Target.GetHashCode();
                    List<QueueItem> list = mOrderedItems[hashCode];
                    list.RemoveAt(0);
                    if (list.Count > 0)
                    {
                        mThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ExecuteTask), list[0]);
                    }
                }
            }
        }

        #endregion

        #region Nested class(es)

        private sealed class QueueItem
        {

            /// <summary>
            /// Gets or sets the task delegate.
            /// </summary>
            /// <value>
            /// The task delegate.
            /// </value>
            public Delegate TaskDelegate { get; set; }

            /// <summary>
            /// Gets or sets the return delegate.
            /// </summary>
            /// <value>
            /// The return delegate.
            /// </value>
            public Delegate ReturnDelegate { get; set; }

            /// <summary>
            /// Gets or sets the in parameters.
            /// </summary>
            /// <value>
            /// The in parameters.
            /// </value>
            public object[] InParameters { get; set; }

        }

        #endregion

    }

}
