using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HVS.Forge.Threading.ConfigSection;

namespace HVS.Forge.Testing.ConfigSectionTests.HVS.Forge.Threading
{
    /// <summary>
    /// Summary description for ThreadPoolSection
    /// </summary>
    [TestClass]
    public class ThreadPoolSection
    {
        #region Fields

        private TestContext testContextInstance;

        ThreadPoolItem threadPoolItem = new ThreadPoolItem();

        #endregion

        #region Constructor
        
        public ThreadPoolSection()
        {
            //
            // TODO: Add constructor logic here
            //
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
         [ClassInitialize()]
         public static void MyClassInitialize(TestContext testContext) { }
        
         //Use ClassCleanup to run code after all tests in a class have run
         [ClassCleanup()]
         public static void MyClassCleanup() { }
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

        [TestMethod]
        public void NameTestMethod()
        {
            threadPoolItem.Name = string.Empty;
            Int32 maxValue = Int32.MaxValue;
            Int32 minValue = Int32.MinValue + maxValue * maxValue * maxValue;
            threadPoolItem.MinThreadNumber = Int32.MaxValue + maxValue ; //???
            threadPoolItem.MinThreadNumber = Int32.MinValue + maxValue * maxValue ;
            double a = -2.0;
            threadPoolItem.MaxConcurrentExecution = Int32.MaxValue * maxValue * (int)a;
            threadPoolItem.ShutDownIdleThreadTime = Int32.MaxValue;
            threadPoolItem.MaxStackSize = Int32.MinValue; 
            threadPoolItem.SetReadOnlyFlag = true;
        }
    }
}
