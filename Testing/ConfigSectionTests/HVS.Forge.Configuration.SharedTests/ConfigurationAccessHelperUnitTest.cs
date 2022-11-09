using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HVS.Forge.Configuration.Shared;
using System.Configuration;
using HVS.Forge.Persistence.StorageProviders.ConfigSection;

namespace HVS.Forge.Testing.ConfigSectionTests.HVS.Forge.Configuration.SharedTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ConfigurationAccessHelperUnitTest
    {

        #region Fields

        private TestContext testContextInstance;
        CategoryPropertyItems categoryPropertyItems;


        #endregion


        #region Constructor

        public ConfigurationAccessHelperUnitTest()
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

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            CategoryPropertyItems categoryPropertyItems = new CategoryPropertyItems();
            categoryPropertyItems = null;

        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {

        }


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
        public void GetValueByPathTestMethod() //Don't passed, because the "categoryPropertyItems" parameter value is null;
        {
            //Assert.IsNull(ConfigurationAccessHelper.GetValueByPath(null, null));
            //Assert.IsNull(ConfigurationAccessHelper.GetValueByPath(StorageConfiguration.Settings.CategoryPropertyItems, string.Empty));
            //Assert.IsNull(ConfigurationAccessHelper.GetValueByPath(null, ""));
            //Assert.IsNull(ConfigurationAccessHelper.GetValueByPath(StorageConfiguration.Settings.CategoryPropertyItems, ""));
            Assert.IsNull(ConfigurationAccessHelper.GetValueByPath(StorageConfiguration.Settings.CategoryPropertyItems, "test messenge"));
        }

        [TestMethod]
        public void GetCategoryPropertyByPathTestMethod()
        {
            //Assert.IsNull(ConfigurationAccessHelper.GetCategoryPropertyByPath(null, null));
            //Assert.IsNull(ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, string.Empty));
            //Assert.IsNull(ConfigurationAccessHelper.GetCategoryPropertyByPath(null, ""));
            //Assert.IsNull(ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, ""));
            Assert.IsNull(ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, "test messenge"));
        }

        #endregion
    }
}
