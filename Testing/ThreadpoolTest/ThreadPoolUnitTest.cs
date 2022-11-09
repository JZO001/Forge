using System;
using System.Configuration;
using Forge.Threading.ConfigSection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing
{
    [TestClass]
    public class ThreadPoolUnitTest
    {
        #region Fields

        ThreadPoolTestPoxy publicTestPoxy = new ThreadPoolTestPoxy();

        ThreadPoolItem item = new ThreadPoolItem();

        #endregion

        #region Constructor

        static ThreadPoolUnitTest()
        {
            Forge.Logging.Log4net.Log4NetManager.InitializeFromAppConfig();
        }

        #endregion

        #region Test Methods

        [TestMethod]
        public void ThreadPoolUintTestMethod()
        {
            int minThreadNumber = 0;
            int maxThreadNumber = 0 ;
            int maxConcurrentExecution = 0;
            int ShutDownIdleThreadTime = 0;
            int MaxStackSize = 0;
            bool SetReadOnlyFlag = true;

            foreach (ThreadPoolItem item in ThreadPoolConfiguration.Settings.ThreadPools)
            {
                string name = "TerraGraf_Network_Send";
                if(name == item.Name)
                {
                    item.MinThreadNumber = item.MinThreadNumber + 1;
                    item.MaxThreadNumber = item.MaxThreadNumber + 1;
                    item.MaxConcurrentExecution = item.MaxConcurrentExecution + 1;
                    item.ShutDownIdleThreadTime = item.ShutDownIdleThreadTime + 1;
                    item.MaxStackSize = item.MaxStackSize + 1;
                    item.SetReadOnlyFlag = !item.SetReadOnlyFlag;


                    SetReadOnlyFlag = item.SetReadOnlyFlag;
                    maxThreadNumber = item.MaxThreadNumber;
                    maxConcurrentExecution = item.MaxConcurrentExecution;
                    ShutDownIdleThreadTime = item.ShutDownIdleThreadTime;
                    MaxStackSize = item.MaxStackSize;
                    minThreadNumber = item.MinThreadNumber;
                    break;
                }
            }
            ThreadPoolConfiguration.Save(ConfigurationSaveMode.Modified);


            string id = "ThreadPoolUnitTest";
            AppDomainSetup domainInfo = new AppDomainSetup();
            domainInfo.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            domainInfo.ConfigurationFile = ThreadPoolConfiguration.SectionHandler.DefaultConfigurationFile;
            domainInfo.ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;

            //create a new domain
            AppDomain domain = AppDomain.CreateDomain(id, AppDomain.CurrentDomain.Evidence, domainInfo);

            ThreadPoolTestPoxy publicTestPoxy = (ThreadPoolTestPoxy)domain.CreateInstanceAndUnwrap(typeof(ThreadPoolTestPoxy).Assembly.FullName, typeof(ThreadPoolTestPoxy).FullName);
            publicTestPoxy.ProxyTest(minThreadNumber, maxThreadNumber, maxConcurrentExecution, ShutDownIdleThreadTime, MaxStackSize, SetReadOnlyFlag);

        }

        [TestMethod]
        public void ThreadPoolUintTestMethod2()
        {
            string id = "ThreadPoolUnitTest";
            AppDomainSetup domainInfo = new AppDomainSetup();
            domainInfo.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            domainInfo.ConfigurationFile = ThreadPoolConfiguration.SectionHandler.DefaultConfigurationFile;
            domainInfo.ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;

            //create a new domain
            AppDomain domain = AppDomain.CreateDomain(id, AppDomain.CurrentDomain.Evidence, domainInfo);

            ThreadPoolTestPoxy publicTestPoxy = (ThreadPoolTestPoxy)domain.CreateInstanceAndUnwrap(typeof(ThreadPoolTestPoxy).Assembly.FullName, typeof(ThreadPoolTestPoxy).FullName);

            publicTestPoxy.Initialize();

            foreach (ThreadPoolItem item in ThreadPoolConfiguration.Settings.ThreadPools)
            {
                string name = "TerraGraf_Network_Send";
                if (name == item.Name)
                {
                    item.MinThreadNumber = item.MinThreadNumber + 1;
                    item.MaxThreadNumber = item.MaxThreadNumber + 1;
                    item.MaxConcurrentExecution = item.MaxConcurrentExecution + 1;
                    item.ShutDownIdleThreadTime = item.ShutDownIdleThreadTime + 1;
                    item.MaxStackSize = item.MaxStackSize + 1;
                    item.SetReadOnlyFlag = !item.SetReadOnlyFlag;

                    publicTestPoxy.SetupValue(item.MinThreadNumber, item.MaxThreadNumber, item.MaxConcurrentExecution, item.ShutDownIdleThreadTime,
                       item.MaxStackSize, item.SetReadOnlyFlag);
                    break;
                }
            }
            ThreadPoolConfiguration.Save(ConfigurationSaveMode.Modified);
        }

        [TestMethod]
        public void ThreadPoolUintTestMethod3()
        {
            Assert.IsNotNull(item.MaxThreadNumber);
            Assert.IsNotNull(item.MinThreadNumber);
            Assert.AreNotEqual(item.MaxThreadNumber, item.MinThreadNumber);
        }

        [TestMethod]
        public void ThreadPoolUintTestMethod4()
        {
            string name = "TerraGraf_Network_Send";
            if(name == item.Name)
            {
                Assert.IsNotNull(item.MinThreadNumber);
                Assert.IsNotNull(item.MaxThreadNumber);
                Assert.AreNotEqual(item.MinThreadNumber, item.MaxThreadNumber); 
            }
        }

        [TestMethod]
        public void ThreadPoolUintTestMethod5()
        {
            string name = "TerraGraf_Network_Send";
            if (name == item.Name || "TerraGraf_Network_Connection" == item.Name || "TerraGraf_Network_BroadcastServer" == item.Name)
            {
                Assert.IsNotNull(item.MinThreadNumber);
                Assert.IsNotNull(item.MaxThreadNumber);
                Assert.IsNotNull(item.ShutDownIdleThreadTime);
                Assert.IsNotNull(item.MaxStackSize);
                Assert.IsNotNull(item.MaxConcurrentExecution);
                Assert.AreNotEqual(item.MaxThreadNumber, item.MinThreadNumber);
            }
        }

        #endregion
    }
}
