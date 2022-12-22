using System;
using System.Collections.Generic;
using Forge.Collections;
using Forge.Persistence.Collections;
using Forge.Persistence.StorageProviders.HibernateStorageProvider;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Collection;
using NHibernate.Mapping.Attributes;
using NHibernate.Tool.hbm2ddl;
using Forge.Testing.PersistenceCollectionsText.Entities;

namespace Forge.Testing.PersistenceCollectionsText
{
    /// <summary>
    /// Summary description for PersistentListUnitTest
    /// </summary>
    [TestClass]
    public class PersistentListUnitTest
    {
        #region Fileds

        private TestContext testContextInstance;

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(EFTest));

        private static ISessionFactory sessionFactory = null;

        int number;

        #endregion

        #region Constructor

        public PersistentListUnitTest()
        {
        }

        static PersistentListUnitTest()
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
            // define mapping schema
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

        #region Listid and provider constructor test methods        

        [TestMethod]
        public void AddTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("TestAdd_Id");
            PersistentList<string> pList = new PersistentList<string>("TestAdd_Id", provider);
            pList.Clear();
            number = 10;

            Assert.AreEqual(pList.Count, 0);

            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void VersionTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_Version_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_Version_Id", provider);
            pList.Clear();
            number = 10;

            Assert.AreEqual(pList.Count, 0);

            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            Assert.IsNotNull(pList.Version);
            Assert.AreEqual(pList.Version, number);
            number = 0;
            pList.Dispose();
        }

        /// <summary>
        /// 10 elemmel való feltöltés, egy elem kitörlése, és utána vizsgálata 
        /// //10 removolok egy olyan elemet, ami csak egyszer van benen
        /// </summary>
        [TestMethod]
        public void RemoveTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_Remove_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_Remove_Id", provider);
            pList.Clear();
            number = 10;
            Assert.AreEqual(pList.Count, 0);
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            //10 szer törlök egy olyan elemet, ami csak egyszer van benne
            for (int i = 0; i < number; i++)
            {
                pList.Remove("test0");
            }
            Assert.AreEqual(pList.Count, number - 1);
            Assert.AreEqual(pList[0], "test1");

            pList.Remove("test1");
            Assert.AreEqual(pList.Count, number - 2);

            number = 0;
            pList.Dispose();
        }

        /// <summary>
        /// remove null, és nem létező elem törlés
        /// </summary>
        [TestMethod]
        public void RemoveTestMethod2()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_Remove2_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_Remove2_Id", provider);
            pList.Clear();
            number = 10;
            bool result = false;
            bool result2 = false;
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            result = pList.Remove("testt");
            Assert.AreEqual(pList.Count, number);
            Assert.IsFalse(result);
            result2 = pList.Remove(null);
            Assert.IsFalse(result2);
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void InsertTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_Insert_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_Insert_Id", provider);
            pList.Clear();
            number = 10;

            Assert.AreEqual(pList.Count, 0);
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);

            pList.Insert(4, "New_Item");

            Assert.AreEqual(pList.Count, number + 1);
            Assert.AreEqual(pList[4], "New_Item");

            pList.Remove("New_Item");
            Assert.AreEqual(pList.Count, number);
            Assert.AreEqual(pList[3], "test3");
            Assert.AreEqual(pList[4], "test4");
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void InsertTestMethod2()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_Insert_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_Insert_Id", provider);
            pList.Clear();
            number = 10;
            bool result = false;
            try
            {
                pList.Insert(4, "item");
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            result = false;
            number = 0;
            pList.Dispose();
        }

        /// <summary>
        /// Nagyobb index törlése, mint ami a listában van
        /// </summary>
        [TestMethod]
        public void RemoveAtTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_RemoveAt_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_RemoveAt_Id", provider);
            pList.Clear();
            number = 10;
            bool result = false;

            Assert.AreEqual(pList.Count, 0);
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);

            pList.RemoveAt(2);

            Assert.AreEqual(pList.Count, number - 1);
            Assert.AreEqual(pList[2], "test3");

            try
            {
                pList.RemoveAt(43);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            Assert.AreEqual(pList.Count, number - 1);

            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void AddRageTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("TestAddRange_Id");
            PersistentList<string> pList = new PersistentList<string>("TestAddRange_Id", provider);
            pList.Clear();
            number = 10;
            List<string> pl = new List<string>();
            for (int i = 0; i < number; i++)
            {
                pl.Add("test" + i.ToString());
            }
            IEnumerable<string> enumerable = pl;
            pList.AddRange(enumerable);
            Assert.AreEqual(pList.Count, number);
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void AddRageTestMethod2()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("TestAddRange2_Id");
            PersistentList<string> pList = new PersistentList<string>("TestAddRange_Id", provider);
            pList.Clear();
            IEnumerable<string> enumerable = null;
            bool result = false;
            try
            {
                pList.AddRange(enumerable);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            pList.Dispose();
        }

        [TestMethod]
        public void RemoveAllTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_RemoveAll_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_RemoveAll_Id", provider);
            pList.Clear();
            number = 10;
            List<string> collectionList = new List<string>();
            Assert.AreEqual(pList.Count, 0);
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            for (int i = 0; i < number; i++)
            {
                collectionList.Add("test" + i.ToString());
            }
            ICollection<string> collection = collectionList;
            Assert.AreEqual(collection.Count, number);
            pList.RemoveAll(collection);
            Assert.AreEqual(pList.Count, 0);
            number = 0;
            pList.Dispose();
        }

        /// <summary>
        /// törlés null collectionnal
        /// </summary>
        [TestMethod]
        public void RemoveAllTestMethod2()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_RemoveAll2_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_RemoveAll2_Id", provider);
            pList.Clear();
            bool result = false;
            number = 10;
            ICollection<string> collecton = null;
            try
            {
                pList.RemoveAll(collecton);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void RemoveAllTestMethod3()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_RemoveAll3_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_RemoveAl3_Id", provider);
            pList.Clear();
            number = 10;
            List<string> collectionList = new List<string>();
            ICollection<string> collection = null;
            for (int i = 0; i < number * 2; i++)
            {
                collectionList.Add("test" + i.ToString());
            }
            Assert.AreEqual(collectionList.Count, number * 2);
            collection = collectionList;
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            pList.RemoveAll(collection);
            Assert.AreEqual(pList.Count, 0);
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void RetainAllTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_RetainAll_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_RetainAll_Id", provider);
            pList.Clear();
            number = 10;
            List<string> collectionList = new List<string>();
            for (int i = 0; i < number; i++)
            {
                collectionList.Add("Item" + i.ToString());
            }
            ICollection<string> collection = collectionList;
            Assert.AreEqual(collection.Count, number);
            Assert.AreEqual(pList.Count, 0);
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            pList.RetainAll(collection);
            Assert.AreEqual(pList.Count, 0);

            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void RetainAllTestMethod2()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_RetainAll2_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_RetainAll2_Id", provider);
            pList.Clear();
            number = 10;
            List<string> collectionList = new List<string>();
            for (int i = 0; i < number; i++)
            {
                collectionList.Add("test" + i.ToString());
            }
            ICollection<string> collection = collectionList;
            Assert.AreEqual(collection.Count, number);
            Assert.AreEqual(pList.Count, 0);
            for (int i = 0; i < number; i++)
            {
                pList.Add("test");
            }
            Assert.AreEqual(pList.Count, number);
            pList.RetainAll(collection);
            Assert.AreEqual(pList.Count, 0);

            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void RetainAllTestMethod3()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_RetainAll3_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_RetainAll3s_Id", provider);
            pList.Clear();
            number = 10;
            bool result = false;
            ICollection<string> collection = null;
            try
            {
                pList.RetainAll(collection);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void LastIndexOfTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_LastOF_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_LastOF_Id", provider);
            pList.Clear();
            number = 10;

            Assert.AreEqual(pList.Count, 0);
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);

            int lastIndex = pList.LastIndexOf("test9");
            Assert.IsNotNull(lastIndex);
            Assert.AreEqual(pList.Count, lastIndex + 1);
            Assert.AreEqual(lastIndex, 9);
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void LastIndexOfTestMethod2()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_LastOF2_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_LastOF2_Id", provider);
            pList.Clear();
            number = 10;
            int lastIndex = 0;
            Assert.AreEqual(pList.Count, 0);
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            lastIndex = pList.LastIndexOf("not_item");
            Assert.IsTrue(lastIndex == -1);
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void IndexOfTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_IndexOF2_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_IndexOF2_Id", provider);
            pList.Clear();
            number = 10;
            Assert.AreEqual(pList.Count, 0);
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            int index = pList.IndexOf("test0");
            int index2 = pList.IndexOf("test5");
            Assert.AreEqual(index, 0);
            Assert.AreEqual(index2, 5);

            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void IndexOfTestMethod2()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_IndexOF2_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_IndexOF2_Id", provider);
            pList.Clear();
            number = 10;
            int index = 0;
            Assert.AreEqual(pList.Count, 0);
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            index = pList.IndexOf(null);
            Assert.AreEqual(-1, index);
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void ConstainAllTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_ConstainAll_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_ConstainAll_Id", provider);
            pList.Clear();
            number = 10;
            Assert.AreEqual(pList.Count, 0);
            List<string> collectionList = new List<string>();
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());

            }
            Assert.AreEqual(pList.Count, number);
            for (int i = 0; i < number; i++)
            {
                collectionList.Add("test" + i.ToString());
            }

            ICollection<string> collection = collectionList;
            Assert.AreEqual(collection.Count, number);

            bool result = pList.ContainsAll(collection);
            Assert.IsTrue(result);
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void ConstainAllTestMethod2()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Test_ConstainAll2_Id");
            PersistentList<string> pList = new PersistentList<string>("Test_ConstainAll2_Id", provider);
            pList.Clear();
            number = 10;
            Assert.AreEqual(pList.Count, 0);
            List<string> collectionList = new List<string>();
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());

            }
            Assert.AreEqual(pList.Count, number);
            for (int i = 0; i < number; i++)
            {
                collectionList.Add("Item" + i.ToString());
            }
            ICollection<string> collection = collectionList;
            Assert.AreEqual(collection.Count, number);
            bool result = pList.ContainsAll(collection);
            Assert.IsFalse(result);
            number = 0;
            pList.Dispose();
        }

        /// <summary>
        /// Elem beinsertálása
        /// </summary>
        [TestMethod]
        public void InsertRageTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Insert_Test_Id");
            PersistentList<string> pList = new PersistentList<string>("Insert_Test_Id", provider);
            pList.Clear();
            number = 10;
            List<string> collectionList = new List<string>();
            for (int i = 0; i < number; i++)
            {
                collectionList.Add("test" + i.ToString());
            }
            ICollection<string> collection = collectionList;
            Assert.AreEqual(collection.Count, number);

            pList.InsertRange(0, collection);

            Assert.AreEqual(pList.Count, number);
            Assert.AreEqual(pList[0], "test0");
            Assert.AreEqual(pList[9], "test9");
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void InsertRageTestMethod2()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("Insert2_Test_Id");
            PersistentList<string> pList = new PersistentList<string>("Insert2_Test_Id", provider);
            pList.Clear();
            number = 10;
            bool result = false;
            bool result2 = false;
            ICollection<string> collectoin = null;
            List<string> collectionList = new List<string>();
            try
            {
                pList.InsertRange(0, collectoin);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            for (int i = 0; i < number; i++)
            {
                collectionList.Add("test" + i.ToString());
            }
            collectoin = collectionList;
            try
            {
                pList.InsertRange(5, collectoin);
            }
            catch (Exception)
            {
                result2 = true;
            }
            Assert.IsTrue(result2);
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void ISublistTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("ISublist_Test_Id");
            PersistentList<string> pList = new PersistentList<string>("ISublist_Test_Id", provider);
            pList.Clear();
            number = 10;
            ISubList<string> sublist = null;
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            sublist = pList.SubList(3, 6);
            Assert.AreEqual(sublist.Count, 3);
            Assert.AreEqual(sublist[0], "test3");
            Assert.AreEqual(sublist[2], "test5");
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void ISublistTestMethod2()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("ISublist2_Test_Id");
            PersistentList<string> pList = new PersistentList<string>("ISublist2_Test_Id", provider);
            pList.Clear();
            number = 10;
            bool result = false;
            ISubList<string> sublist = null;
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            try
            {
                sublist = pList.SubList(0, 40);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            result = false;

            try
            {
                sublist = pList.SubList(4, 0);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void CopyToTestMethod()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("CopyTo_Test_Id");
            PersistentList<string> pList = new PersistentList<string>("CopyTo_Test_Id", provider);
            pList.Clear();
            number = 10;
            string[] t = new string[number + 2];
            for (int i = 0; i < number; i++)
            {
                t[i] = "Item" + i.ToString();
            }
            Assert.AreEqual(t.Length, number + 2);
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            pList.CopyTo(t, 2);
            Assert.AreEqual(t[0], "Item0");
            Assert.AreEqual(t[2], "test0");
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void CopyToTestMethod2()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("CopyTo2_Test_Id");
            PersistentList<string> pList = new PersistentList<string>("CopyTo2_Test_Id", provider);
            pList.Clear();
            number = 10;
            string[] t = new string[number + 2];
            for (int i = 0; i < number; i++)
            {
                t[i] = "" + i.ToString();
            }
            Assert.AreEqual(t.Length, number + 2);

            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            pList.CopyTo(7, t, 3, 1);
            Assert.IsNotNull(t);
            Assert.AreEqual(t[3], "test7");
            number = 0;
            pList.Dispose();
        }

        [TestMethod]
        public void CopyToTestMethod3()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("CopyTo3_Test_Id");
            PersistentList<string> pList = new PersistentList<string>("CopyTo3_Test_Id", provider);
            pList.Clear();
            number = 10;
            bool result = false;
            string[] t = new string[number + 2];
            for (int i = 0; i < number; i++)
            {
                t[i] = "" + i.ToString();
            }
            Assert.AreEqual(t.Length, number + 2);
            for (int i = 0; i < number; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number);
            try
            {
                pList.CopyTo(t, 13);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            result = false;
            pList.Clear();
            t = null;
            Assert.AreEqual(pList.Count, 0);
            try
            {
                pList.CopyTo(t, 2);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            result = false;
            t = new string[number + 2];
            for (int i = 0; i < number; i++)
            {
                t[i] = "" + i.ToString();
            }
            Assert.AreEqual(t.Length, number + 2);
            try
            {
                pList.CopyTo(11, t, 2, 1);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            result = false;
            try
            {
                pList.CopyTo(11, t, 22, 1);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            number = 0;
            pList.Dispose();
        }

        #endregion

        #region  Listid ,Provider, and cacheSize constructor test methods

        [TestMethod]
        public void CacheSizeAddTestMethod()
        {
            number = 10;
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("CacheSizeAdd_Test_Id");
            PersistentList<string> pList = new PersistentList<string>("CacheSizeAdd_Test_Id", provider, number);

            pList.Clear();
            for (int i = 0; i < number * 2; i++)
            {
                pList.Add("test" + i.ToString());
            }
            Assert.AreEqual(pList.Count, number * 2);
            pList.RemoveAt(0);
            Assert.AreEqual(pList[0], "test1");
            Assert.AreEqual(pList[9], "test10");

            pList.RemoveAt(14);
            Assert.AreEqual(pList[14], "test16");

            for (int i = 0; i < number - 7; i++)
            {
                pList.RemoveAt(i);
            }
            Assert.AreEqual(pList[0], "test2");
            Assert.AreEqual(pList[1], "test4");
            Assert.AreEqual(pList[2], "test6");
            number = 0;
            pList.Dispose();
        }


        #endregion

        #region Listid, cacheStrategy, cacheSize constructor test methods

        [TestMethod]
        public void cacheStrategyAddTestMethod()
        {
            PersistentList<string> pList = new PersistentList<string>("cacheStrategyAdd_Test_Id", "test_Configuration");
            Assert.AreEqual(pList.Id, "cacheStrategyAdd_Test_Id");
        }


        #endregion

        #endregion
    }
}
