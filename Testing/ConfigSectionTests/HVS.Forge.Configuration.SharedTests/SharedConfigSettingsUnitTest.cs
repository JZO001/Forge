using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HVS.Forge.Configuration.Shared.Interfaces;
using HVS.Forge.Configuration.Shared;

namespace HVS.Forge.Testing.ConfigSectionTests.HVS.Forge.Configuration.SharedTests
{
    /// <summary>
    /// Summary description for SharedConfigSettingsUnitTest
    /// </summary>
    [TestClass]
    public class SharedConfigSettingsUnitTest 
    {
        #region Fields

        private TestContext testContextInstance;

        ConfigurationSectionBase configurationSectionBase;
 
        //SharedConfigSettings<> shared;

        #endregion

        #region Constructor

        public SharedConfigSettingsUnitTest()
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {

        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {

        }
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

        #region Test Methods

        [TestMethod]
        public void DefaultConfigurationFileTestMethod()
        {

        }

        #endregion
    }
}
