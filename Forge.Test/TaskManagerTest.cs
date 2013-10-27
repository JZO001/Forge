using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge.Threading.Tasking;
using System.Threading;

namespace Forge.Test
{

    [TestClass]
    public class TaskManagerTest
    {

        private AutoResetEvent mTestResult1 = new AutoResetEvent(false);

        private AutoResetEvent mTestResult2 = new AutoResetEvent(false);

        private AutoResetEvent mTestResult3 = new AutoResetEvent(false);

        private AutoResetEvent mTestResult4 = new AutoResetEvent(false);

        [TestMethod]
        public void TestTaskManagerOrderByTaskTarget()
        {
            TaskManager taskManager = new TaskManager(ChaosTheoryEnum.OrderByTaskDelegateTarget);
            taskManager.Execute<int, string, bool>(TaskGeneric, TaskGenericFinished, 1, "2");
            taskManager.Execute<int, string, bool>(TaskGenericException, TaskGenericExceptionFinished, 1, "2");
            taskManager.Execute<int, string>(TaskNonGeneric, TaskNonGenericFinished, 1, "2");
            taskManager.Execute<int, string>(TaskNonGenericException, TaskNonGenericExceptionFinished, 1, "2");

            mTestResult1.WaitOne();
            mTestResult2.WaitOne();
            mTestResult3.WaitOne();
            mTestResult4.WaitOne();
        }

        [TestMethod]
        public void TestTaskManagerOrderByReturnTarget()
        {
            TaskManager taskManager = new TaskManager(ChaosTheoryEnum.OrderByReturnDelegateTarget);
            taskManager.Execute<int, string, bool>(TaskGeneric, TaskGenericFinished, 1, "2");
            taskManager.Execute<int, string, bool>(TaskGenericException, TaskGenericExceptionFinished, 1, "2");
            taskManager.Execute<int, string>(TaskNonGeneric, TaskNonGenericFinished, 1, "2");
            taskManager.Execute<int, string>(TaskNonGenericException, TaskNonGenericExceptionFinished, 1, "2");

            mTestResult1.WaitOne();
            mTestResult2.WaitOne();
            mTestResult3.WaitOne();
            mTestResult4.WaitOne();
        }

        [TestMethod]
        public void TestTaskManagerChaos()
        {
            TaskManager taskManager = new TaskManager(ChaosTheoryEnum.Chaos);
            taskManager.Execute<int, string, bool>(TaskGeneric, TaskGenericFinished, 1, "2");
            taskManager.Execute<int, string, bool>(TaskGenericException, TaskGenericExceptionFinished, 1, "2");
            taskManager.Execute<int, string>(TaskNonGeneric, TaskNonGenericFinished, 1, "2");
            taskManager.Execute<int, string>(TaskNonGenericException, TaskNonGenericExceptionFinished, 1, "2");

            mTestResult1.WaitOne();
            mTestResult2.WaitOne();
            mTestResult3.WaitOne();
            mTestResult4.WaitOne();
        }

        private bool TaskGeneric(int a, string b)
        {
            Assert.IsTrue(a == 1);
            Assert.IsTrue("2".Equals(b));
            return false;
        }

        private bool TaskGenericException(int a, string b)
        {
            Assert.IsTrue(a == 1);
            Assert.IsTrue("2".Equals(b));
            throw new Exception("TaskGenericException");
        }

        private void TaskNonGeneric(int a, string b)
        {
            Assert.IsTrue(a == 1);
            Assert.IsTrue("2".Equals(b));
        }

        private void TaskNonGenericException(int a, string b)
        {
            Assert.IsTrue(a == 1);
            Assert.IsTrue("2".Equals(b));
            throw new Exception("TaskNonGenericException");
        }

        private void TaskGenericFinished(TaskResult<bool> result)
        {
            Assert.IsNotNull(result);
            Assert.IsTrue(!result.Result);
            Assert.IsTrue(result.Exception == null);

            mTestResult1.Set();
        }

        private void TaskGenericExceptionFinished(TaskResult<bool> result)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Exception);

            mTestResult2.Set();
        }

        private void TaskNonGenericFinished(TaskResult result)
        {
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Exception == null);

            mTestResult3.Set();
        }

        private void TaskNonGenericExceptionFinished(TaskResult result)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Exception);

            mTestResult4.Set();
        }

    }

}
