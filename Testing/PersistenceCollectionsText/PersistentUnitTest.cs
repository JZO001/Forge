using System;
using System.Collections.Generic;
using Forge.Persistence.StorageProviders.HibernateStorageProvider;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Mapping.Attributes;
using NHibernate.Tool.hbm2ddl;
using Forge.Testing.PersistenceCollectionsText.Entities;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.PersistenceCollectionsText
{
    /// <summary>
    /// Summary description for PersistenceUnitTest
    /// </summary>
    [TestClass]
    public class PersistentUnitTest
    {
        #region Fields

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(EFTest));

        private static ISessionFactory sessionFactory = null;

        private TestContext testContextInstance;

        #endregion

        #region Constructor
        
        static PersistentUnitTest()
        {
            Forge.Logging.Log4net.Log4NetManager.InitializeFromAppConfig();
            LOGGER.Warn("START::WARN");
            LOGGER.Debug("START::DEBUG");
            LOGGER.Info("START::INFO");
            LOGGER.Error("START::ERROR");
            LOGGER.Fatal("START::FATAL");
        }
        public PersistentUnitTest()
        {
        }

        #endregion

        #region Propreties

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
            HbmSerializer.Default.HbmAssembly = typeof(PersistentListEntity).Assembly.GetName().FullName;
            //HbmSerializer.Default.HbmNamespace = typeof( Product ).Namespace;
            HbmSerializer.Default.HbmAutoImport = true;
            HbmSerializer.Default.Validate = true;
            HbmSerializer.Default.WriteDateComment = false;
            HbmSerializer.Default.HbmDefaultAccess = "field";
            //HbmSerializer.Default.Serialize(typeof(Product).Assembly, "output.hbm.xml"); // serialize mapping xml into file to spectate it

            // create configuration and load assembly
            NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();
            //cfg.Properties[NHibernate.Cfg.Environment.CollectionTypeFactoryClass] = typeof(Net4CollectionTypeFactory).AssemblyQualifiedName;
            cfg.Configure();
            //cfg.AddAssembly( typeof( Product ).Assembly ); // use this only, if hbm.xml exists in the assembly
            cfg.AddInputStream(HbmSerializer.Default.Serialize(typeof(PersistentListEntity).Assembly)); // ez bármikor müxik, de lassabb

            try
            {
                SchemaValidator schemaValidator = new SchemaValidator(cfg);
                schemaValidator.Validate(); // validate the database schema
            }
            catch (Exception)
            {
                SchemaUpdate schemaUpdater = new SchemaUpdate(cfg); // try to update schema
                schemaUpdater.Execute(false, true);
                if (schemaUpdater.Exceptions.Count > 0)
                {
                    throw new Exception("FAILED TO UPDATE SCHEMA");
                }
            }

            //SchemaExport export = new SchemaExport(cfg);
            //export.Execute( false, true, false );
            //new SchemaExport( cfg ).Execute( false, true, false );
            sessionFactory = cfg.BuildSessionFactory();
        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
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
        public void AddEntityTestMethod()
        {
            HibernateStorageProvider<PersistentListEntity> provider = new HibernateStorageProvider<PersistentListEntity>("TestAdd_IdEN");
            PersistentListEntity ple = new PersistentListEntity();
            ple.Id = new EntityId(8, 1, 3);
            ple.Name = "Test_Name" + Guid.NewGuid().ToString();
            ple.TestInt = 0;
            ple.Version = new EntityVersion(1);
            provider.Add(ple);
            Assert.IsNotNull(provider);
            provider.Dispose();
            Assert.IsNotNull(provider.StorageId);
        }

        [TestMethod]
        public void RemoveEntityTestMethod()
        {
            HibernateStorageProvider<PersistentListEntity> provider = new HibernateStorageProvider<PersistentListEntity>("TestRemove_IdEN");
            PersistentListEntity ple = new PersistentListEntity();
            ple.Id = new EntityId(7, 50, 3);
            ple.Name = "Remove_TestName" + Guid.NewGuid().ToString();
            ple.TestInt = 0;
            ple.Version = new EntityVersion(1);
            provider.Add(ple);
            provider.Remove(ple);
            Assert.IsNotNull(provider);
            provider.Dispose();
        }

        [TestMethod]
        public void InsertEntityTestMethod()
        {
            HibernateStorageProvider<PersistentListEntity> provider = new HibernateStorageProvider<PersistentListEntity>("TestInsert_IdEN");
            PersistentListEntity ple = new PersistentListEntity();
            ple.Id = new EntityId(1, 56, 3);
            ple.Name = "Remove_TestName" + Guid.NewGuid().ToString();
            ple.TestInt = 0;
            ple.Version = new EntityVersion(1);
            provider.Insert(0, ple);
            Assert.IsNotNull(provider);
            provider.Dispose();
        }

        [TestMethod]
        public void AddRangeEntityTestMethod()
        {
            HibernateStorageProvider<PersistentListEntity> provider = new HibernateStorageProvider<PersistentListEntity>("TestAddRange_IdEN");
            PersistentListEntity ple = new PersistentListEntity();
            List<PersistentListEntity> persistentListEntityList = new List<PersistentListEntity>();
            IEnumerable<PersistentListEntity> enumberable;
            ple.Id = new EntityId(5, 50, 3);
            ple.Name = "AddRange_test_name" + Guid.NewGuid().ToString();
            ple.TestInt = 0;
            ple.Version = new EntityVersion(1);
            persistentListEntityList.Add(ple);
            enumberable = persistentListEntityList;
            provider.AddRange(enumberable);
            Assert.IsNotNull(provider);
            provider.Dispose();
        }

        [TestMethod]
        public void RemoverAtTestMethod()
        {
            HibernateStorageProvider<PersistentListEntity> provider = new HibernateStorageProvider<PersistentListEntity>("TestRemoveAt_IdEN");
            PersistentListEntity ple = new PersistentListEntity();
            ple.Id = new EntityId(1, 5, 3);
            ple.Name = "RemoverAt_test_name" + Guid.NewGuid().ToString();
            ple.TestInt = 0;
            ple.Version = new EntityVersion(1);
            provider.Add(ple);
            provider.RemoveAt(0);
            Assert.IsNotNull(provider);
            provider.Dispose();
        }

        #endregion
    }
}
