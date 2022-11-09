using System;
using System.Threading;
using Forge.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.ThreadingTest
{
    /// <summary>
    /// Summary description for ThreadingUnitTest
    /// </summary>
    [TestClass]
    public class ThreadingUnitTest
    {
        #region Fields

        private TestContext testContextInstance;

        public static DeadlockSafeLock dlsl = new DeadlockSafeLock("TestThread");

        public static DeadlockSafeLock dlsl2 = new DeadlockSafeLock("TestThread2");

        public static AutoResetEvent event1 = new AutoResetEvent(false);

        public static AutoResetEvent TestThreadEvent = new AutoResetEvent(false);

        public static AutoResetEvent TestThreadEvent2 = new AutoResetEvent(false);

        public static AutoResetEvent TestThreadEvent3 = new AutoResetEvent(false);

        public static AutoResetEvent event2 = new AutoResetEvent(false);
        public static int lockNumber = 0;

        public static int tryLockNumber = 0;

        public static int unlockNumber = 0;

        #endregion

        #region Constructor

        public ThreadingUnitTest()
        {

        }

        #endregion

        #region Propertyes
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        ///

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

        #region Property TestMethod

        [TestMethod]
        public void IsLockedTestMethod()
        {
            dlsl.Lock();
            Assert.IsTrue(dlsl.IsLocked);
            dlsl.Unlock();
            Assert.IsFalse(dlsl.IsLocked);
        }

        [TestMethod]
        public void LockIdTestMethod()
        {
            dlsl.Lock();
            Assert.IsNotNull(dlsl.LockId);
            Assert.IsInstanceOfType(dlsl.LockId, typeof(String));
            dlsl = new DeadlockSafeLock("TestThread");
        }

        [TestMethod]
        public void IsDisposedTestMethod()
        {
            dlsl.Dispose();
            Assert.IsTrue(dlsl.IsDisposed);
            dlsl = new DeadlockSafeLock("TestThread");
            Assert.IsFalse(dlsl.IsDisposed);
        }

        [TestMethod]
        public void NameTestMethod()
        {
            dlsl.Lock();
            Assert.IsNotNull(dlsl.Name);
            Assert.IsInstanceOfType(dlsl.Name, typeof(String));
            dlsl.Unlock();
            Assert.IsFalse(dlsl.IsLocked);
        }

        [TestMethod]
        public void IsHeldByCurrentThreadTestMethod()
        {
            dlsl.Lock();
            Assert.IsTrue(dlsl.IsHeldByCurrentThread);
            dlsl.Unlock();
            Assert.IsFalse(dlsl.IsHeldByCurrentThread);
        }

        #endregion

        #region Test Methods

        #region Lock Test Methods

        /// <summary>
        /// Help Method for the LockTestMethod
        /// </summary>
        /// <returns></returns>
        public static bool LockMethodA()
        {
            bool result = false;
            dlsl.Lock();
            event2.Set();
            event1.WaitOne();
            try
            {
                dlsl2.Lock();
            }
            catch (DeadlockException)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Help Method for the LockTestMethod
        /// </summary>
        /// <returns></returns>
        public static bool LockMethodB()
        {
            bool result = false;
            dlsl2.Lock();
            event1.Set();
            event2.WaitOne();
            try
            {
                dlsl.Lock();
            }
            catch (DeadlockException)
            {
                result = true;
            }
            return result;
        }

        [TestMethod]
        public void LockTestMethod()
        {
            bool LockMethodAResult = false;
            bool LockMethodBResult = false;
            Thread threadOne = new Thread(delegate()
            {
                LockMethodAResult = LockMethodA();
                TestThreadEvent.Set();

            });
            threadOne.Name = "First_Test_Thread";

            Thread threadTwo = new Thread(delegate()
            {
                LockMethodBResult = LockMethodB();
                TestThreadEvent.Set();
            });
            threadTwo.Name = "Second_Test_Thread";

            threadOne.Start();
            threadTwo.Start();

            TestThreadEvent.WaitOne();

            Assert.IsTrue(LockMethodAResult || LockMethodBResult);
            Assert.IsFalse(LockMethodAResult && LockMethodBResult);
            TestThreadEvent = new AutoResetEvent(false);
            dlsl = new DeadlockSafeLock("TestThread");
            dlsl2 = new DeadlockSafeLock("TestThread2");

        }

        /// <summary>
        /// Help Method for the LockTestMethod
        /// </summary>
        public static void LockHelpMethod()
        {
            lockNumber = 3;
            for (int i = 0; i < lockNumber; i++)
            {
                dlsl.Lock();
            }
            string trheadName = Thread.CurrentThread.Name;
            TestThreadEvent3.Set();
        }

        /// <summary>
        /// Help Method for the LockTestMethod
        /// </summary>
        /// <returns></returns>
        public static bool LockHelpMethod2()
        {
            bool result = false;
            try
            {
                string threadName = Thread.CurrentThread.Name;
                dlsl.Lock();
            }

            finally
            {
                dlsl.Unlock();
                dlsl = new DeadlockSafeLock("TestThread");
                TestThreadEvent = new AutoResetEvent(false);
                TestThreadEvent3 = new AutoResetEvent(false);
                result = true;
            }
            return result;

        }


        [TestMethod]
        public void LockTestMethod2()
        {
            bool result = false;

            LockHelpMethod();

            TestThreadEvent3.WaitOne();
            Assert.IsTrue(dlsl.IsLocked);


            Thread thread2 = new Thread(delegate()
            {
                result = LockHelpMethod2();
            });
            thread2.Name = "thread2";
            thread2.Start();
            TestThreadEvent.WaitOne(5000);
            for (int i = 0; i < lockNumber; i++)
            {
                dlsl.Unlock();
            }
            thread2.Join();

            Assert.IsTrue(result);
            dlsl = new DeadlockSafeLock("TestThread");
            TestThreadEvent = new AutoResetEvent(false);
            TestThreadEvent3 = new AutoResetEvent(false);
        }

        /// <summary>
        /// Help Method for the LockAndUnlockEqualsTestMethod
        /// </summary>
        public static void LockAndUnlockEqualsHelpMethod()
        {
            lockNumber = 3;
            for (int i = 0; i < lockNumber; i++)
            {
                dlsl.Lock();
            }

            Assert.IsTrue(dlsl.IsLocked);
            Assert.IsNotNull(lockNumber);

            for (int i = 0; i < lockNumber; i++)
            {
                dlsl.Unlock();
            }

            Assert.IsFalse(dlsl.IsLocked);
            TestThreadEvent.Set();
        }

        /// <summary>
        /// Help Method for the LockAndUnlockEqualsTestMethod
        /// </summary>
        public static void LockAndUnlockEqualsHelpMethod2()
        {
            dlsl.Lock();
            string currentThread = Thread.CurrentThread.Name;

            Assert.IsTrue(dlsl.IsLocked);
            dlsl.Unlock();
            Assert.IsFalse(dlsl.IsLocked);
            TestThreadEvent2.Set();
        }


        [TestMethod]
        public void LockAndUnlockEqualsTestMethod()
        {
            Thread thread = new Thread(delegate()
            {
                LockAndUnlockEqualsHelpMethod();
            });
            thread.Start();
            thread.Name = "thread";
            TestThreadEvent.WaitOne();

            Thread thread2 = new Thread(delegate()
            {
                LockAndUnlockEqualsHelpMethod2();
            });
            thread2.Start();
            thread2.Name = "thread2";
            TestThreadEvent2.WaitOne();

            dlsl = new DeadlockSafeLock("TestThread");
            TestThreadEvent2 = new AutoResetEvent(false);
            TestThreadEvent = new AutoResetEvent(false);
            lockNumber = 0;
        }

        #endregion

        #region InvalidOperationException Test Method

        [TestMethod]
        public void MoreCallUnlockTestMethod()
        {
            unlockNumber = 3;
            bool result = false;
            try
            {
                for (int i = 0; i < unlockNumber; i++)
                {
                    dlsl.Unlock();
                }
            }
            catch (InvalidOperationException)
            {
                result = true;
            }

            Assert.IsTrue(result);
            dlsl = new DeadlockSafeLock("TestThread");
            unlockNumber = 0;
        }

        #endregion

        #region TryLock Test Method

        [TestMethod]
        public void BoolTryLockTestMethod()
        {
            object isTry = dlsl.TryLock(1000);
            Assert.IsInstanceOfType(isTry, typeof(bool));
            Assert.IsTrue(dlsl.IsLocked);
            dlsl.Unlock();
            Assert.IsFalse(dlsl.IsLocked);
        }

        [TestMethod]
        public void BoolTryLockTestMethod2()
        {
            bool isTry = dlsl.TryLock(1000);
            Assert.IsTrue(dlsl.IsLocked);
            dlsl.Unlock();
            Assert.IsFalse(dlsl.IsLocked);
            dlsl = new DeadlockSafeLock("TestThread");
        }

        [TestMethod]
        public void BoolTryLockTestMethod3()
        {
            bool isTry = dlsl.TryLock(TimeSpan.FromSeconds(1));
            Assert.IsTrue(isTry);
            Assert.IsTrue(dlsl.IsLocked);
            dlsl.Unlock();
            Assert.IsFalse(dlsl.IsLocked);
        }

        [TestMethod]
        public void BoolTryLockTestMethod4()
        {
            bool isTry = dlsl.TryLock(TimeSpan.FromSeconds(0));
            Assert.IsTrue(isTry);
            Assert.IsTrue(dlsl.IsLocked);
            dlsl.Unlock();
            Assert.IsFalse(dlsl.IsLocked);
        }


        [TestMethod]
        public void BoolTryLockTestMethod5()
        {
            dlsl.Lock();
            dlsl.Unlock();
            dlsl.TryLock(TimeSpan.FromSeconds(2));
            Assert.IsTrue(dlsl.IsLocked);
            Assert.IsFalse(dlsl.IsDisposed);
            dlsl.Unlock();
            dlsl = new DeadlockSafeLock("TestThread");
            Assert.IsFalse(dlsl.IsDisposed);
        }

        [TestMethod]
        public void MoreEnterTryLockTestMethod()
        {
            tryLockNumber = 3;
            for (int i = 0; i < tryLockNumber; i++)
            {
                dlsl.TryLock(1000);
            }
            Assert.IsTrue(dlsl.IsLocked);

            for (int i = 0; i < tryLockNumber; i++)
            {
                dlsl.TryLock(TimeSpan.FromSeconds(1));
            }
            Assert.IsTrue(dlsl.IsLocked);
            for (int i = 0; i < tryLockNumber * 2; i++)
            {
                dlsl.Unlock();
            }
            Assert.IsFalse(dlsl.IsLocked);
            tryLockNumber = 0;
        }


        /// <summary>
        /// Help Method for the TryLockTestMethod
        /// </summary>
        public static void TryLockHelpMethod()
        {
            lockNumber = 3;
            for (int i = 0; i < lockNumber; i++)
            {
                dlsl.TryLock(1000);
            }
            string trheadName = Thread.CurrentThread.Name;
            TestThreadEvent3.Set();

        }

        /// <summary>
        /// Help Method for the TryLockTestMethod
        /// </summary>
        /// <returns></returns>
        public static bool TryLockHelpMethod2()
        {
            bool result = false;
           
                string threadName = Thread.CurrentThread.Name;
                dlsl.TryLock(1000);
                Assert.IsTrue(dlsl.IsLocked);
                dlsl.Unlock();
                TestThreadEvent.Set();
                result = true;
                return result;
        }

        [TestMethod]
        public void TryLockTestMethod()
        {
            bool result = false;

            TryLockHelpMethod();

            TestThreadEvent3.WaitOne();
            Assert.IsTrue(dlsl.IsLocked);


            Thread thread2 = new Thread(delegate()
            {
                result = TryLockHelpMethod2();
            });
            thread2.Name = "thread2";
            thread2.Start();
            
            for (int i = 0; i < lockNumber; i++)
            {
                dlsl.Unlock();
            }
            TestThreadEvent.WaitOne(5000);
            thread2.Join();

            Assert.IsTrue(result);
            dlsl = new DeadlockSafeLock("TestThread");
            TestThreadEvent = new AutoResetEvent(false);
            TestThreadEvent3 = new AutoResetEvent(false);
        }

        /// <summary>
        /// Help Method for the LockAndUnlockEqualsTestMethod
        /// </summary>
        public static void TryLockAndUnlockEqualsHelpMethod()
        {
            lockNumber = 3;
            for (int i = 0; i < lockNumber; i++)
            {
                dlsl.Lock();
            }

            Assert.IsTrue(dlsl.IsLocked);
            Assert.IsNotNull(lockNumber);

            for (int i = 0; i < lockNumber; i++)
            {
                dlsl.Unlock();
            }

            Assert.IsFalse(dlsl.IsLocked);
            TestThreadEvent.Set();
        }

        /// <summary>
        /// Help Method for the LockAndUnlockEqualsTestMethod
        /// </summary>
        public static void TryLockAndUnlockEqualsHelpMethod2()
        {
            dlsl.Lock();
            string currentThread = Thread.CurrentThread.Name;

            Assert.IsTrue(dlsl.IsLocked);
            dlsl.Unlock();
            Assert.IsFalse(dlsl.IsLocked);
            TestThreadEvent2.Set();
        }


        [TestMethod]
        public void TryLockAndUnlockEqualsTestMethod()
        {
            Thread thread = new Thread(delegate()
            {
                TryLockAndUnlockEqualsHelpMethod();
            });
            thread.Start();
            thread.Name = "thread";
            TestThreadEvent.WaitOne();

            Thread thread2 = new Thread(delegate()
            {
                TryLockAndUnlockEqualsHelpMethod2();
            });
            thread2.Start();
            thread2.Name = "thread2";
            TestThreadEvent2.WaitOne();

            dlsl = new DeadlockSafeLock("TestThread");
            TestThreadEvent2 = new AutoResetEvent(false);
            TestThreadEvent = new AutoResetEvent(false);
            lockNumber = 0;
        }

        #endregion

        #region ObjectDisposedException Test Method

        [TestMethod]
        public void LockDisposeTestMethod()
        {
            bool lockMethodResult = false;

            dlsl.Dispose();
            Assert.IsTrue(dlsl.IsDisposed);

            try
            {
                dlsl.Lock();
            }
            catch (ObjectDisposedException)
            {
                lockMethodResult = true;
            }
            Assert.IsTrue(lockMethodResult);
            dlsl = new DeadlockSafeLock("TestThread");
        }

        [TestMethod]
        public void UnlockkDisposeTestMethod()
        {
            bool unlockMethodResult = false;
            dlsl.Dispose();
            Assert.IsTrue(dlsl.IsDisposed);

            try
            {
                dlsl.Unlock();
            }
            catch (ObjectDisposedException)
            {
                unlockMethodResult = true;
            }
            Assert.IsTrue(unlockMethodResult);
            dlsl = new DeadlockSafeLock("TestThread");
        }

        [TestMethod]
        public void TrylockkDisposeTestMethod()
        {
            bool tryLockMethodResult = false;
            dlsl.Dispose();
            Assert.IsTrue(dlsl.IsDisposed);

            try
            {
                dlsl.TryLock(1000);
            }
            catch (ObjectDisposedException)
            {
                tryLockMethodResult = true;
            }

            Assert.IsTrue(tryLockMethodResult);
            dlsl = new DeadlockSafeLock("TestThread");
        }

        #endregion

        #region Unlock Test Method

        [TestMethod]
        public void UnlockTestMethod()
        {
            dlsl.Lock();
            Assert.IsTrue(dlsl.IsLocked);
            Assert.IsFalse(dlsl.IsDisposed);
            dlsl = new DeadlockSafeLock("TestThread");
            Assert.IsFalse(dlsl.IsDisposed);
        }

        #endregion

        #region Equals Test Methods

        [TestMethod]
        public void EqualsTestMethod()
        {
            object obj = null;
            Assert.IsFalse(dlsl.Equals(obj));
            dlsl = new DeadlockSafeLock("TestThread");
        }

        [TestMethod]
        public void EqualsTestMethod2()
        {
            string testString = "test";
            dlsl = new DeadlockSafeLock("test");
            Assert.IsFalse(dlsl.Equals(testString));
            dlsl = new DeadlockSafeLock("TestThread");
        }

        [TestMethod]
        public void EqualsTestMethod3()
        {
            string testString = string.Empty;
            Assert.IsFalse(dlsl.Equals(testString));
            dlsl = new DeadlockSafeLock("TestThread");
        }

        [TestMethod]
        public void EqualsTestMethod4()
        {
            DeadlockSafeLock obj = null;
            Assert.IsFalse(dlsl.Equals(obj));
            dlsl = new DeadlockSafeLock("TestThread");
        }

        [TestMethod]
        public void EqualsTestMethod5()
        {
            DeadlockSafeLock obj = new DeadlockSafeLock("Test");
            Assert.IsFalse(dlsl.Equals(obj));
            dlsl = new DeadlockSafeLock("TestThread");
        }

        #endregion

        #endregion
    }
}
