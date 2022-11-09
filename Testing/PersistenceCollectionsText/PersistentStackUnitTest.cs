using System;
using Forge.Persistence.Collections;
using Forge.Persistence.StorageProviders.HibernateStorageProvider;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Mapping.Attributes;
using NHibernate.Tool.hbm2ddl;
using Forge.Testing.NHibernateStorageProviderTest.Entities;

namespace Forge.Testing.PersistenceCollectionsText
{
    /// <summary>
    /// Summary description for PersistentStackUnitTest
    /// </summary>
    [TestClass]
    public class PersistentStackUnitTest
    {
        #region Fields

        private TestContext testContextInstance;

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(EFTest));

        private static ISessionFactory sessionFactory = null;

        int number = 0;

        #endregion

        #region Constructor
     
        public PersistentStackUnitTest()
        {
        }

        static PersistentStackUnitTest()
        {
            Forge.Logging.Log4net.Log4NetManager.InitializeFromAppConfig();
            LOGGER.Warn("START::WARN");
            LOGGER.Debug("START::DEBUG");
            LOGGER.Info("START::INFO");
            LOGGER.Error("START::ERROR");
            LOGGER.Fatal("START::FATAL");
        }
        #endregion

        #region Properties
        
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
            HbmSerializer.Default.HbmAssembly = typeof(TestEntityClass).Assembly.GetName().FullName;
            //HbmSerializer.Default.HbmNamespace = typeof( Product ).Namespace;
            HbmSerializer.Default.HbmAutoImport = true;
            HbmSerializer.Default.Validate = true;
            HbmSerializer.Default.WriteDateComment = false;
            HbmSerializer.Default.HbmDefaultAccess = "field";
            //HbmSerializer.Default.Serialize(typeof(People).Assembly, "output.hbm.xml"); // serialize mapping xml into file to spectate it

            // create configuration and load assembly
            NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();
            //cfg.Properties[NHibernate.Cfg.Environment.CollectionTypeFactoryClass] = typeof(Net4CollectionTypeFactory).AssemblyQualifiedName;
            cfg.Configure();
            //cfg.AddAssembly( typeof( People ).Assembly ); // use this only, if hbm.xml exists in the assembly
            cfg.AddInputStream(HbmSerializer.Default.Serialize(typeof(TestEntityClass).Assembly)); // ez bármikor müxik, de lassabb

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
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
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

        #region StackId, provider, and cacheSize constructor test methods

        [TestMethod]
        public void PushTestMethod()
        {
            number = 10;
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("StackTestPush_Id");
            PersistentStack<string> stack = new PersistentStack<string>("StackTestPush_Id", provider, number);
            stack.Clear();
            for (int i = 0; i < number; i++)
            {
                stack.Push("test" +i.ToString());
            }
            Assert.AreEqual(stack.Count, number);
            string currnet = stack.Peek();
            Assert.AreEqual(currnet, "test9");
            number = 0;
            stack.Dispose();
        }

        [TestMethod]
        public void PushTestMethod2()
        {
            number = 10;
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("StackTestPush2_Id");
            PersistentStack<string> stack = new PersistentStack<string>("StackTestPush2_Id", provider, number);
            stack.Clear();
            for (int i = 0; i < number ; i++)
            {
                stack.Push("test" + i.ToString());
            }
            Assert.AreEqual(stack.Count, number);

            for (int i = 0; i < number; i++)
            {
                stack.Push("Item" +i.ToString());
            }
            Assert.AreEqual(stack.Count, number*2);
            Assert.AreEqual(stack.Peek(), "Item9");
            Assert.AreEqual(stack.Pop(), "Item9");
            Assert.AreEqual(stack.Pop(), "Item8");
            Assert.AreEqual(stack.Pop(), "Item7");
            number = 0;
            stack.Dispose();
        }

        [TestMethod]
        public void PeekTestMethod()
        {
            number = 10;
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("StackTestPeek_Id");
            PersistentStack<string> stack = new PersistentStack<string>("StackTestPeek_Id", provider, number);
            stack.Clear();
            bool result = false;
            Assert.AreEqual(stack.Count, 0);
            try
            {
                string currnet = stack.Peek();
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            number = 0;
            stack.Dispose();
        }


        /// <summary>
        /// stack push-olása, aztán egyenként, majd több elem pop-ása a stac-ból
        /// </summary>
        [TestMethod]
        public void PopTestMethod()
        {
            number = 10;
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("StackTestPop_Id");
            PersistentStack<string> stack = new PersistentStack<string>("StackTestPop_Id", provider, number);
            stack.Clear();
            for (int i = 0; i < number; i++)
            {
                stack.Push("test" + i.ToString());
            }
            Assert.AreEqual(stack.Count, number);
            stack.Pop();
            Assert.AreEqual(stack.Count, number-1);
            string current = stack.Peek();
            Assert.AreEqual(current, "test8");

            for (int i = 0; i < number-1; i++)
            {
                stack.Pop();
            }
            Assert.AreEqual(stack.Count, 0);

            stack.Push("Nem_Item");
            Assert.AreEqual(stack.Count, 1);
            Assert.AreEqual(stack.Peek(), "Nem_Item");
            number = 0;
            stack.Dispose();
        }

        /// <summary>
        /// több elemet pop-ok mint amennyit bele push-oltam a stack-be
        /// </summary>
        [TestMethod]
        public void PopTestMethod2()
        {
            number = 10;
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("StackTestPop_Id");
            PersistentStack<string> stack = new PersistentStack<string>("StackTestPop_Id", provider, number);
            stack.Clear();
            bool result = true;
            for (int i = 0; i < number; i++)
            {
                stack.Push("test" + i.ToString());
            }
            Assert.AreEqual(stack.Count, number);

            try
            {
                for (int i = 0; i < number * 2; i++)
                {
                    stack.Pop();
                }
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            Assert.IsTrue(stack.IsEmpty);
            number = 0;
            stack.Dispose();
        }

        /// <summary>
        /// Stack pop, push, és elemek rendezettségének ellenörzése
        /// </summary>
        [TestMethod]
        public void CacheTestMethod()
        {
            number = 10;
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("cache");
            PersistentStack<string> stack = new PersistentStack<string>("cache", provider, number);
            stack.Clear();
            
            for (int i = 0; i < number*2; i++)
            {
                stack.Push("test" + i.ToString());
            }
            Assert.AreEqual(stack.Count, number*2);

            stack.Pop();
            stack.Pop();
            Assert.AreEqual(stack.Peek(), "test17");

            for (int i = 0; i < number-6; i++)
            {
                stack.Pop();
            }
            Assert.AreEqual(stack.Peek(), "test13");

            for (int i = 0; i < number-3; i++)
            {
                stack.Pop();
            }
            Assert.AreEqual(stack.Count, 7);
            Assert.AreEqual(stack.Peek(), "test6");

            for (int i = 0; i < number-4; i++)
            {
                stack.Push("Item" +i.ToString());
            }
            Assert.AreEqual(stack.Count, 13);
            Assert.AreEqual(stack.Peek(), "Item5");

            for (int i = 0; i < number+3; i++)
            {
                stack.Pop();
            }
            Assert.IsTrue(stack.IsEmpty);
            Assert.AreEqual(stack.Count, 0);
            number = 0;
            stack.Dispose();
        }

        #endregion

        #region StackId, and configurationName constructor test method

        [TestMethod]
        public void ConfigurationNameTestMethod()
        {
            number = 10;
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("ConfigurationNameID");
            PersistentStack<string> stack = new PersistentStack<string>("ConfigurationNameID", "ConfigurationName");
            stack.Clear();

            Assert.AreEqual(stack.Id, "ConfigurationNameID");

            number = 0;
            stack.Dispose();
        }

        #endregion

        #endregion
    }
}
