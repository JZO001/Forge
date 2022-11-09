using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HVS.Forge.Configuration.Shared;
using HVS.Forge.Persistence.StorageProviders.ConfigSection;

namespace HVS.Forge.Testing.ConfigSectionTests.HVS.Forge.Configuration.SharedTests
{
    /// <summary>
    /// Summary description for ConfigurationSectionStandardUnitTest
    /// </summary>
    [TestClass]
    public class ConfigurationSectionStandardUnitTest
    {
        #region Fields

        private TestContext testContextInstance;

        ConfigurationSectionStandard configurationSectionStandard;

        #endregion

        #region Constructor

        public ConfigurationSectionStandardUnitTest()
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
        public void EncryptSectionTestMethod()
        {
        }

        [TestMethod]
        public void CategoryPropertyItemsTestMethod() //Object reference not set to an instance of an object.
        {
            configurationSectionStandard.CategoryPropertyItems = null;//StorageConfiguration.Settings.CategoryPropertyItems;
            Assert.IsNull(configurationSectionStandard.CategoryPropertyItems);
            
            //configurationSectionStandard.CategoryPropertyItems = null;
            //Assert.IsNull(configurationSectionStandard.CategoryPropertyItems);
        }

        #endregion
    }
}
