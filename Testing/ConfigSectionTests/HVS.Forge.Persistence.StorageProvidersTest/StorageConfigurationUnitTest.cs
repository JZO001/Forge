using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HVS.Forge.Persistence.StorageProviders.ConfigSection;
using log4net;
using NHibernate;
using NHibernate.Mapping.Attributes;
using HVS.Forge.EntityFramework;
using NHibernate.Tool.hbm2ddl;
using HVS.Forge.Configuration.Shared.Interfaces;
using HVS.Forge.Configuration.Shared;

namespace HVS.Forge.Testing.ConfigSectionTests.HVS.Forge.Persistence.StorageProvidersTest
{
    /// <summary>
    /// Summary description for StorageConfigurationUnitTest
    /// </summary>
    [TestClass]
    public class StorageConfigurationUnitTest
    {
        #region Fields

        private TestContext testContextInstance;

        private static ILog LOGGER = LogManager.GetLogger(typeof(StorageConfiguration));

        StorageConfiguration storageConfiguration;

        StorageSection storageSection = new StorageSection();

        private static ISessionFactory sessionFactory = null;

        // = new StorageConfiguration();

        #endregion

        #region Constructor

        static StorageConfigurationUnitTest()
        {
            log4net.Config.XmlConfigurator.Configure();
            LOGGER.Warn("START::WARN");
            LOGGER.Debug("START::DEBUG");
            LOGGER.Info("START::INFO");
            LOGGER.Error("START::ERROR");
            LOGGER.Fatal("START::FATAL");
        }

        public StorageConfigurationUnitTest()
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
            if (sessionFactory != null)
            {
                sessionFactory.Dispose();
            }
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
        public void RefreshInstanceTestMethod()
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    //StorageConfiguration storageConfiguration = new StorageConfiguration();
                    storageConfiguration.RefreshInstance();
                }
            }

        }

        [TestMethod]
        public void SettingsTestMethod()
        {
            //IConfigurationSettingsHandler<StorageSection>
            SharedConfigSettings<StorageSection> mConfigHandler;

            List<ConfigurationSectionStandard> storageSectionList = new List<ConfigurationSectionStandard>();
            ConfigurationSectionStandard result = null;
            StorageSection result2 = null;
            foreach (var item in StorageSection.KnownSections)
            {
                storageSectionList.Add(item);
                result = item;
            }



            //Assert.IsInstanceOfType(result, typeof(StorageSection));
            //Assert.IsInstanceOfType(result2, typeof(StorageConfiguration));

            //Assert.IsNotNull(StorageConfiguration.Settings);
            //Assert.AreEqual(StorageConfiguration.Settings, );

        }

        [TestMethod]
        public void SectionHandlerTestMethod()
        {
            //Assert.IsNull(StorageConfiguration.SectionHandler);
            Assert.IsNotNull(StorageConfiguration.SectionHandler);

            List<ConfigurationSectionStandard> storageSectionList = new List<ConfigurationSectionStandard>();

            foreach (var item in StorageSection.KnownSections)
            {
                storageSectionList.Add(item);
            }

            StorageSection storageSection = new StorageSection();

            

            //StorageConfiguration.SectionHandler = storageSectionList;

        }

        #endregion
    }
}
