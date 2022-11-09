using System;
using Forge.Threading.ConfigSection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing
{
    class ThreadPoolTestPoxy : MBRBase
    {

        #region Fields

        int minThreadNumber = 0;

        int maxThreadNumber = 0;

        int maxConcurrentExecution = 0;

        int ShutDownIdleThreadTime = 0;

        int MaxStackSize = 0;

        bool SetReadOnlyFlag = true;

        #endregion

        #region Constructor

        public ThreadPoolTestPoxy()
        {

        }

        #endregion

        #region Initialize

        public void Initialize()
        {
            ThreadPoolConfiguration.SectionHandler.OnConfigurationChanged +=new EventHandler<EventArgs>(SectionHandler_OnConfigurationChanged);
            ThreadPoolConfiguration.Refresh();
        }

        #endregion

        #region Event

        void SectionHandler_OnConfigurationChanged(object sender, EventArgs e)
        {
            foreach (ThreadPoolItem item in ThreadPoolConfiguration.Settings.ThreadPools)
            {
                string name = "TerraGraf_Network_Send";
                if (name == item.Name)
                {
                    Assert.AreEqual(item.MinThreadNumber, minThreadNumber);
                    Assert.AreEqual(item.MaxThreadNumber, maxThreadNumber);
                    Assert.AreEqual(item.MaxConcurrentExecution, maxConcurrentExecution);
                    Assert.AreEqual(item.ShutDownIdleThreadTime, ShutDownIdleThreadTime);
                    Assert.AreEqual(item.MaxStackSize, MaxStackSize);
                    Assert.AreEqual(item.SetReadOnlyFlag, SetReadOnlyFlag);
                    break;
                }
            }
        }

        #endregion

        #region Public Methods

        public void SetupValue(int MinThreadNumber, int MaxThreadNumber, int MaxConcurrentExecution, int ShutDownIdleThreadTimeParam, int MaxStackSizeParam, bool SetReadOnlyFlagParam)
        {
            minThreadNumber = MinThreadNumber;
            maxThreadNumber = MaxThreadNumber;
            maxConcurrentExecution = MaxConcurrentExecution;
            ShutDownIdleThreadTime = ShutDownIdleThreadTimeParam;
            MaxStackSize = MaxStackSizeParam;
            SetReadOnlyFlag = SetReadOnlyFlagParam;
        }

        public void ProxyTest(int minThreadNumber, int maxThreadNumber, int maxConcurrentExecution, int ShutDownIdleThreadTime, int MaxStackSize, bool SetReadOnlyFlag)
        {
            foreach (ThreadPoolItem item in ThreadPoolConfiguration.Settings.ThreadPools)
            {
                string name = "TerraGraf_Network_Send";
                if (name == item.Name)
                {
                    Assert.AreEqual(item.MinThreadNumber, minThreadNumber);
                    Assert.AreEqual(item.MaxThreadNumber, maxThreadNumber);
                    Assert.AreEqual(item.MaxConcurrentExecution, maxConcurrentExecution);
                    Assert.AreEqual(item.ShutDownIdleThreadTime, ShutDownIdleThreadTime);
                    Assert.AreEqual(item.MaxStackSize, MaxStackSize);
                    Assert.AreEqual(item.SetReadOnlyFlag, SetReadOnlyFlag);
                    break;
                }
            }
        }
        #endregion
    }
}
