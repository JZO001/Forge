using System.Linq;
using System.Threading;
using Forge.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing
{

    [TestClass]
    public class LockTest
    {

        private ILock[] mLocks = new ILock[] { new DeadlockSafeLock("A"), new DeadlockSafeLock("B") };
        private Thread mDaemon = null;
        private AutoResetEvent mEventA = new AutoResetEvent(false);
        private AutoResetEvent mEventB = new AutoResetEvent(false);

        [TestMethod]
        public void LockAllAgressiveTest()
        {
            LockHandle.LockAllAgressive(mLocks);

            mLocks.ToList<ILock>().ForEach(l => l.Unlock());
        }

        [TestMethod]
        public void LockAllAgressiveFailTest()
        {
            mDaemon = new Thread(new ThreadStart(DaemonTest));
            mDaemon.IsBackground = true;
            mDaemon.Name = "DAEMON";
            mDaemon.Start();

            mEventA.WaitOne(); 
            LockHandle.LockAllAgressive(mLocks);

            mLocks.ToList<ILock>().ForEach(l => l.Unlock());
            mEventB.Set(); // ez nem futhat le
        }

        [TestMethod]
        public void LockAllGentleTest()
        {
            LockHandle.LockAllGentle(mLocks);

            mLocks.ToList<ILock>().ForEach(l => l.Unlock());
        }

        private void DaemonTest()
        {
            mLocks[1].Lock();
            mEventA.Set();
            mEventB.WaitOne();
        }

    }

}
