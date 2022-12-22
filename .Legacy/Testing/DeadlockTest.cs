using System;
using System.Threading;
using Forge.Threading;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing
{

    [TestClass]
    public class DeadlockTest
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(DeadlockTest));

        private enum TestMode
        {
            Deadlock,
            LockUnlock,
            MultipleLock,
            TryLock
        }

        private TestMode testMode = TestMode.TryLock;

        private AutoResetEvent mEvent = new AutoResetEvent(false);
        private AutoResetEvent mGo1 = new AutoResetEvent(false);
        private AutoResetEvent mGo2 = new AutoResetEvent(false);

        private DeadlockSafeLock _lockA = new DeadlockSafeLock("Lock_A");
        private DeadlockSafeLock _lockB = new DeadlockSafeLock("Lock_B");

        static DeadlockTest()
        {
            Forge.Logging.Log4net.Log4NetManager.InitializeFromAppConfig();
        }

        [TestMethod]
        public void TestMethod1()
        {
            Thread t = new Thread(new ThreadStart(ThreadMain));
            t.Name = "T1";
            t.IsBackground = true;
            t.Start();
            t = new Thread(new ThreadStart(ThreadMain));
            t.Name = "T2";
            t.IsBackground = true;
            t.Start();
            mEvent.WaitOne();
        }

        private void ThreadMain()
        {
            try
            {
                switch (testMode)
                {
                    case TestMode.Deadlock:
                        // deadlock test
                        if (Thread.CurrentThread.Name.Equals("T1"))
                        {
                            _lockA.Lock();
                            mGo1.Set();
                            mGo2.WaitOne();
                            _lockB.Lock();
                        }
                        else
                        {
                            _lockB.Lock();
                            mGo2.Set();
                            mGo1.WaitOne();
                            _lockA.Lock();
                        }
                        mEvent.WaitOne();
                        break;

                    case TestMode.LockUnlock:
                        _lockA.Lock();
                        _lockA.Unlock();
                        break;

                    case TestMode.MultipleLock:
                        _lockA.Lock();
                        _lockA.Lock();
                        _lockA.Lock();
                        _lockA.Unlock();
                        _lockA.Unlock();
                        _lockA.Unlock();
                        break;

                    case TestMode.TryLock:
                        if (Thread.CurrentThread.Name.Equals("T1"))
                        {
                            bool resultA = _lockA.TryLock(1000);
                            bool resultB = _lockA.TryLock(1000);
                            Thread.Sleep(1000);
                            if (resultA) _lockA.Unlock();
                            if (resultB) _lockA.Unlock();

                            _lockA.Lock();
                            mGo2.Set();
                            mGo1.WaitOne();
                            _lockA.Unlock();
                        }
                        else
                        {
                            mGo2.WaitOne();
                            bool result = _lockA.TryLock(1000);
                            if (result) _lockA.Unlock();
                            mGo1.Set();
                        }
                        break;

                }
                
            }
            catch (Exception ex)
            {
                LOGGER.Error("", ex);
            }
            mEvent.Set();
        }

    }

}
