using System;
using System.Collections.Generic;
using System.Threading;
using Forge.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.LockHandle_Test
{
    /// <summary>
    /// Summary description for LockHandleUnitTest
    /// </summary>
    [TestClass]
    public class LockHandleUnitTest
    {
        #region Fields

        private TestContext testContextInstance;

        public static AutoResetEvent threadEvent = new AutoResetEvent(false);

        public static AutoResetEvent threadEvent2 = new AutoResetEvent(false);

        public static AutoResetEvent threadEvent3 = new AutoResetEvent(false);

        public DeadlockSafeLock dlsl = new DeadlockSafeLock("TestThread");

        public static DeadlockSafeLock dlsl2 = new DeadlockSafeLock("TestThread2");

        public static DeadlockSafeLock dlsl3 = new DeadlockSafeLock("TestThread3");

        public static List<ILock> lockCollection = new List<ILock>();

        #endregion

        #region Constructor

        public LockHandleUnitTest()
        {
        }

        #endregion

        #region Propertyes

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region Test Method

        #region TryLockTestMethods

        [TestMethod]
        public void TryLockParameterTestMethod()
        {
            bool lockResult = false;
            try
            {
                lockResult = LockHandle.TryLockAll(lockCollection, 5000);
            }
            catch (ArgumentNullException)
            {
                lockResult = true;
            }
            Assert.IsFalse(lockResult);
        }

        [TestMethod]
        public void TryLockTestMethod()
        {
            bool result = false;
            dlsl.Lock();

            lockCollection.Add(dlsl);
            lockCollection.Add(dlsl2);
            lockCollection.Add(dlsl3);

            result = LockHandle.TryLockAll(lockCollection, 5000);
            dlsl.Unlock();
            dlsl2.Unlock();
            dlsl3.Unlock();
            Assert.IsTrue(result);
            lockCollection.Clear();
        }

        /// <summary>
        /// TryLockTestMethod2 HelpMetohd
        /// </summary>
        /// <returns></returns>
        public bool TryLockHelpMethod()
        {
            LockHandle.TryLockAll(lockCollection, 3000);
            dlsl2.Unlock();
            dlsl3.Unlock();
            return true;
        }

        [TestMethod]
        public void TryLockTestMethod2()
        {
            bool result = false;
            dlsl.Lock();
            lockCollection.Add(dlsl);
            lockCollection.Add(dlsl2);
            lockCollection.Add(dlsl3);

            Thread t = new Thread(delegate()
                {
                    result = TryLockHelpMethod();
                    threadEvent.Set();
                });
            t.Name = "thread";
            t.Start();
            threadEvent.WaitOne(1500);
            Assert.IsFalse(result);

            dlsl.Unlock();
            threadEvent.WaitOne();
            Assert.IsTrue(result);

            lockCollection.Clear();
            threadEvent = new AutoResetEvent(false);
            dlsl = new DeadlockSafeLock("TestThread");
            dlsl2 = new DeadlockSafeLock("TestThread2");
            dlsl3 = new DeadlockSafeLock("TestThread3");
        }

        public bool TryLockHelpMethod2()
        {
            return LockHandle.TryLockAll(lockCollection);
            dlsl2.Unlock();
            dlsl3.Unlock();
        }

        [TestMethod]
        public void TryLockTestMethod3()
        {
            bool result = false;
            dlsl.Lock();
            lockCollection.Add(dlsl);
            lockCollection.Add(dlsl2);
            lockCollection.Add(dlsl3);
            Exception e = null;

            Thread t = new Thread(delegate()
                {
                    try
                    {
                        result = TryLockHelpMethod2();
                    }
                    catch (Exception ex)
                    {
                        e = ex;
                    }
                    finally
                    {
                        threadEvent.Set();
                    }
                });
            t.Name = "thread";
            t.Start();
            threadEvent.WaitOne();
            if (e != null)
            {
                throw new Exception(e.ToString(), e);
            }
            Assert.IsNull(e);

            dlsl.Unlock();
            threadEvent.WaitOne(2000);
            Thread t2 = new Thread(delegate()
            {
                try
                {
                    result = TryLockHelpMethod2();
                }
                catch (Exception ex)
                {
                    e = ex;
                }
                finally
                {
                    threadEvent.Set();
                }
            });
            t2.Name = "thread";
            t2.Start();
            threadEvent.WaitOne();
            Assert.IsTrue(result);

            lockCollection.Clear();
            threadEvent = new AutoResetEvent(false);

            dlsl = new DeadlockSafeLock("TestThread");
            dlsl2 = new DeadlockSafeLock("TestThread2");
            dlsl3 = new DeadlockSafeLock("TestThread3");
        }

        #endregion

        #region Timeout Test Method

        /// <summary>
        /// TimeOut AgressiveLockTestMethod HelpMetohd
        /// </summary>
        /// <returns></returns>
        public bool TimeOutAgressiveLockHelpMethod()
        {
            LockHandle.LockAllAgressive(lockCollection, 4000, 1000);
            return true;
        }

        [TestMethod]
        public void TimeOutLockAgressiveTestMethod()
        {
            bool result = false;
            dlsl.Lock();
            lockCollection.Add(dlsl);
            lockCollection.Add(dlsl2);
            lockCollection.Add(dlsl3);
            Exception e = null;

            Thread t = new Thread(delegate()
            {
                try
                {
                    result = TimeOutAgressiveLockHelpMethod();

                }
                catch (Exception ex)
                {
                    e = ex;
                }
                finally
                {
                    threadEvent.Set();
                }

            });
            t.Name = "thread";
            t.Start();
            threadEvent.WaitOne(2000);
            if (e != null)
            {
                throw new Exception(e.ToString(), e);
            }
            Assert.IsNull(e);

            Assert.IsFalse(result);

            dlsl.Unlock();
            threadEvent.WaitOne();
            Assert.IsTrue(result);

            dlsl = new DeadlockSafeLock("TestThread");
            dlsl2 = new DeadlockSafeLock("TestThread2");
            dlsl3 = new DeadlockSafeLock("TestThread3");
            lockCollection.Clear();
            threadEvent = new AutoResetEvent(false);
        }

        #endregion

        #region Lock Aggressiv Test Method

        public bool AgressiveLockHelpMethod()
        {
            LockHandle.LockAllAgressive(lockCollection, 4000, 1000);
            return true;
        }

        [TestMethod]
        public void LockAgressiveTestMethod()
        {
            bool result = false;
            dlsl.Lock();
            lockCollection.Add(dlsl);
            lockCollection.Add(dlsl2);
            lockCollection.Add(dlsl3);
            Exception e = null;

            Thread t = new Thread(delegate()
            {
                try
                {
                    result = AgressiveLockHelpMethod();

                }
                catch (Exception ex)
                {
                    e = ex;
                }
                finally
                {
                    threadEvent.Set();
                }

            });
            t.Name = "thread";
            t.Start();
            threadEvent.WaitOne(2000);
            if (e != null)
            {
                throw new Exception(e.ToString(), e);
            }
            Assert.IsNull(e);

            Assert.IsFalse(result);

            dlsl.Unlock();
            threadEvent.WaitOne();
            Assert.IsTrue(result);

            dlsl = new DeadlockSafeLock("TestThread");
            dlsl2 = new DeadlockSafeLock("TestThread2");
            dlsl3 = new DeadlockSafeLock("TestThread3");
            lockCollection.Clear();
            threadEvent = new AutoResetEvent(false);
        }

        public bool AgressiveLockHelpMethod2()
        {
            LockHandle.LockAllAgressive(lockCollection);
            return true;
        }

        [TestMethod]
        public void LockAgressiveTestMethod2()
        {
            bool result = false;
            dlsl.Lock();
            lockCollection.Add(dlsl);
            lockCollection.Add(dlsl2);
            lockCollection.Add(dlsl3);

            Thread t = new Thread(delegate()
                {
                    result = AgressiveLockHelpMethod2();
                    threadEvent.Set();
                });
            t.Name = "thread";
            t.Start();
            threadEvent.WaitOne(2000);

            Assert.IsFalse(result);
            dlsl.Unlock();
            threadEvent.WaitOne();
            Assert.IsTrue(result);

            dlsl = new DeadlockSafeLock("TestThread");
            dlsl2 = new DeadlockSafeLock("TestThread2");
            dlsl3 = new DeadlockSafeLock("TestThread3");
            lockCollection.Clear();
            threadEvent = new AutoResetEvent(false);
        }


        /// <summary>
        /// LockAgressiver DeadLock Help Method
        /// </summary>
        /// <returns></returns>
        public bool LockAgressiverDeadLockHelpMethod()
        {
            bool result = false;
            dlsl.Lock();
            threadEvent2.Set();
            threadEvent3.WaitOne();
            try
            {
                dlsl2.Lock();
                LockHandle.LockAllAgressive(lockCollection, 4000, 1000);
            }
            catch (DeadlockException)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// LockAgressiverDead Lock Help Method2
        /// </summary>
        /// <returns></returns>
        public bool LockAgressiverDeadLockHelpMethod2()
        {
            bool result = false;
            dlsl2.Lock();
            threadEvent3.Set();
            threadEvent2.WaitOne();
            try
            {
                dlsl.Lock();
                LockHandle.LockAllAgressive(lockCollection, 4000, 1000);
            }
            catch (DeadlockException)
            {
                result = true;
            }

            return result;
        }

        [TestMethod]
        public void LockAgressiveDeadLockTestMethod()
        {
            bool LockAgressiverDeadLockHelpMethodResult = false;
            bool LockAgressiverDeadLockHelpMethod2Result = false;

            lockCollection.Add(dlsl);
            lockCollection.Add(dlsl2);
            lockCollection.Add(dlsl3);

            Thread t = new Thread(delegate()
                {
                    LockAgressiverDeadLockHelpMethodResult = LockAgressiverDeadLockHelpMethod();
                    threadEvent.Set();
                });

            t.Name = "thread";

            Thread t2 = new Thread(delegate()
                {
                    LockAgressiverDeadLockHelpMethod2Result = LockAgressiverDeadLockHelpMethod2();
                    threadEvent.Set();
                });

            t2.Name = "thread2";

            t.Start();
            t2.Start();

            threadEvent.WaitOne();

            Assert.IsTrue(LockAgressiverDeadLockHelpMethodResult || LockAgressiverDeadLockHelpMethod2Result);
            Assert.IsFalse(LockAgressiverDeadLockHelpMethodResult && LockAgressiverDeadLockHelpMethod2Result);

            lockCollection.Clear();
            dlsl = new DeadlockSafeLock("TestThread");
            dlsl2 = new DeadlockSafeLock("TestThread2");
            dlsl3 = new DeadlockSafeLock("TestThread3");
            threadEvent = new AutoResetEvent(false);
            threadEvent2 = new AutoResetEvent(false);
            threadEvent3 = new AutoResetEvent(false);
        }

        #endregion

        #region GentleLockAll Test Methods

        public bool GentleLockAllHelpMethod()
        {
            LockHandle.LockAllGentle(lockCollection);
            return true;
        }

        [TestMethod]
        public void GentleLockAllTestMethod()
        {
            bool result = false;
            dlsl.Lock();
            lockCollection.Add(dlsl);
            lockCollection.Add(dlsl2);
            lockCollection.Add(dlsl3);

            Thread t = new Thread(delegate()
                {
                    result = GentleLockAllHelpMethod();
                    threadEvent.Set();
                });
            t.Name = "thread";
            t.Start();
            threadEvent.WaitOne(2000);

            Assert.IsFalse(result);

            dlsl.Unlock();
            threadEvent.WaitOne();
            Assert.IsTrue(result);
            
            dlsl = new DeadlockSafeLock("TestThread");
            dlsl2 = new DeadlockSafeLock("TestThread2");
            dlsl3 = new DeadlockSafeLock("TestThread3");
            lockCollection.Clear();
            threadEvent = new AutoResetEvent(false);
        }

        public bool GentleLockAllHelpMethod2()
        {
            LockHandle.LockAllGentle(lockCollection, 1000);
            dlsl2.Unlock();
            dlsl3.Unlock();
            return true;
        }


        [TestMethod]
        public void GentleLockAllTestMethod2()
        {
            bool result = false;
            dlsl.Lock();
            lockCollection.Add(dlsl);
            lockCollection.Add(dlsl2);
            lockCollection.Add(dlsl3);

            Thread t = new Thread(delegate()
                {
                    result = GentleLockAllHelpMethod2();
                    threadEvent.Set();
                });
            t.Name = "thread";
            t.Start();
            threadEvent.WaitOne(2000);
            Assert.IsFalse(result);

            dlsl.Unlock();
            threadEvent.WaitOne();
            Assert.IsTrue(result);

            
            dlsl = new DeadlockSafeLock("TestThread");
            threadEvent = new AutoResetEvent(false);
            lockCollection.Clear();
        }


        /// <summary>
        /// GentleDaeadLock Help Method
        /// </summary>
        /// <returns></returns>
        public bool GentleDaeadLockTestMethod()
        {
            bool result = false;
            dlsl.Lock();
            threadEvent2.Set();
            threadEvent3.WaitOne();
            try
            {
                dlsl2.Lock();
                LockHandle.LockAllGentle(lockCollection, 1000);
            }
            catch (DeadlockException)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// GentleDaeadLock Help Method2
        /// </summary>
        /// <returns></returns>
        public bool GentleDaeadLockTestMethod2()
        {
            bool result = false;
            dlsl2.Lock();
            threadEvent3.Set();
            threadEvent2.WaitOne();
            try
            {
                dlsl.Lock();
                LockHandle.LockAllGentle(lockCollection, 1000);
            }
            catch (DeadlockException)
            {
                result = true;
            }

            return result;
        }

        [TestMethod]
        public void GentleLcokAllTestMethodDeadLock()
        {
            bool GentleDaeadLockTestMethodResult = false;
            bool GentleDaeadLockTestMethod2Result = false;
            lockCollection.Add(dlsl);
            lockCollection.Add(dlsl2);
            lockCollection.Add(dlsl3);

            Thread t = new Thread(delegate() 
                {
                    GentleDaeadLockTestMethodResult = GentleDaeadLockTestMethod();
                    threadEvent.Set();
                });

            t.Name = "thread";
         

            Thread t2 = new Thread(delegate() 
                {
                   GentleDaeadLockTestMethod2Result = GentleDaeadLockTestMethod2();
                   threadEvent.Set();
                });

            t2.Name = "thread2";
            t.Start();
            t2.Start();

            threadEvent.WaitOne();

            Assert.IsTrue(GentleDaeadLockTestMethodResult || GentleDaeadLockTestMethod2Result);
            Assert.IsFalse(GentleDaeadLockTestMethodResult && GentleDaeadLockTestMethod2Result);
            lockCollection.Clear();
            threadEvent = new AutoResetEvent(false);
            threadEvent2 = new AutoResetEvent(false);
            threadEvent3 = new AutoResetEvent(false);
            dlsl = new DeadlockSafeLock("TestThread");
            dlsl2 = new DeadlockSafeLock("TestThread2");
            dlsl3 = new DeadlockSafeLock("TestThread2");
        }

        #endregion

        #endregion

    }
}
