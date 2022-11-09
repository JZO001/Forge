using System;
using System.Collections.Generic;
using Forge.Persistence.Collections;
using Forge.Persistence.StorageProviders.HibernateStorageProvider;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Forge.Testing.PersistenceCollectionsText.Entities;

namespace Forge.Testing.PersistenceCollectionsText
{
    /// <summary>
    /// Summary description for PersistentDictionaryUnitTest
    /// </summary>
    [TestClass]
    public class PersistentDictionaryUnitTest
    {
        #region Fields

        private TestContext testContextInstance;

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(EFTest));

        private static ISessionFactory sessionFactory = null;

        int number = 0;

        #endregion

        #region Constructor

        public PersistentDictionaryUnitTest()
        {

        }

        static PersistentDictionaryUnitTest()
        {
            Forge.Logging.Log4net.Log4NetManager.InitializeFromAppConfig();
            LOGGER.Warn("START::WARN");
            LOGGER.Debug("START::DEBUG");
            LOGGER.Info("START::INFO");
            LOGGER.Error("START::ERROR");
            LOGGER.Fatal("START::FATAL");
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

        //
        // Use ClassCleanup to run code after all tests in a class have run
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

        #region MapId Test Methods

        [TestMethod]
        public void MapIdAddTestMethod()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdAddTestMethod");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdAddTestMethod", hsp);
            dict.Clear();
            number = 10;

            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);
            Assert.AreEqual(dict["test3"], 3);

            Assert.AreEqual(dict["test9"], 9);
            number = 0;
            dict.Dispose();
        }


        [TestMethod]
        public void MapIdAddEntityTestMethod()
        {
            //HibernateStorageProvider<KeyValuePair<PersistentDictionaryEntity, int>> hsp = new HibernateStorageProvider<KeyValuePair<PersistentDictionaryEntity, int>>("MyTestId");
            //PersistentDictionary<PersistentDictionaryEntity, int> dict = new PersistentDictionary<PersistentDictionaryEntity, int>("MyTestId", hsp);
            ////dict.Clear();
            //Random rnd = new Random();
            //PersistentDictionaryEntity pce = new PersistentDictionaryEntity();
            //pce.Id = new EntityFramework.Model.EntityId(1, 1, rnd.Next(-100, 100));
            //pce.Name = "MY_TEST_NAME";
            //pce.TestInt = 0;
            //pce.Version = new EntityFramework.Model.EntityVersion(1);
            //dict.Add(pce, 1);
            //dict.Dispose();
        }

        [TestMethod]
        public void MapIdAddTestMethod2()
        {
            PersistentDictionary<PresistenceCacheEntities, int> persistentDictionary = new PersistentDictionary<PresistenceCacheEntities, int>("MapIdAddTestMethod2");
            PresistenceCacheEntities tec = new PresistenceCacheEntities();
            tec = null;
            bool result = false;

            try
            {
                persistentDictionary.Add(tec, 0);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MapIdAddTestMethod3()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdAddTestMethod3");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdAddTestMethod3", hsp);
            dict.Clear();
            bool result = false;
            try
            {
                dict.Add(null, 1);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            dict.Dispose();
        }


        /// <summary>
        /// Constains, és ContainsKey metódusok ellenörzése
        /// </summary>
        [TestMethod]
        public void MapIdAddListTestMethod5()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdAddListTestMethod5");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdAddListTestMethod5", hsp);
            dict.Clear();
            number = 10;
            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);

            foreach (KeyValuePair<string, int> item in dict)
            {
                Assert.IsNotNull(item.Value);
                Assert.IsTrue(dict.Contains(item));
                Assert.IsTrue(dict.ContainsKey(item.Key));
            }
            number = 0;
            dict.Dispose();
        }

        /// <summary>
        /// A dict-hez a KeyValuePair-t haszáltam az addoláshoz
        /// </summary>
        [TestMethod]
        public void MapIdAddListTestMethod6()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdAddListTestMethod6");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdAddListTestMethod6", hsp);
            KeyValuePair<string, int> addList = new KeyValuePair<string, int>("test0", 0);
            dict.Clear();
            Assert.AreEqual(dict.Count, 0);
            dict.Add(addList);
            Assert.AreEqual(dict.Count, 1);
            dict.Dispose();
        }

        [TestMethod]
        public void MapIdAddListTestMethod7()
        {
            number = 10;
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdAddListTestMethod7");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdAddListTestMethod7", hsp, number);
            dict.Clear();

            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);
            Assert.AreEqual(dict["test3"], 3);

            for (int i = 0; i < number; i++)
            {
                dict.Add("Item" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number * 2);
            Assert.AreEqual(dict["test2"], 2);
            Assert.AreEqual(dict["test9"], 9);
            Assert.AreEqual(dict["Item2"], 2);
            Assert.AreEqual(dict["Item6"],6);

            number = 0;
            dict.Dispose();
        }

        [TestMethod]
        public void MapIdAddListTestMethod8()
        {
            number = 10;
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdAddListTestMethod8");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdAddListTestMethod8", hsp, number);
            dict.Clear();
            bool result = false;
            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);

            try
            {
                Assert.AreEqual(dict[null], 0);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);

            number = 0;
            dict.Dispose();
        }

        [TestMethod]
        public void MapIdAddListTestMethod9()
        {
            number = 10;
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdAddListTestMethod9");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdAddListTestMethod9", hsp, number);
            dict.Clear();
            bool result = false;
            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);
            try
            {
                Assert.AreEqual(dict["test0"], 43);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);

        }

        [TestMethod]
        public void MapIdRemoveTestMethod()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdRemoveTestMethod");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdRemoveTestMethod", hsp);
            dict.Clear();
            number = 10;
            List<string> dictList = new List<string>();
            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);

            dict.Remove("test0");
            Assert.AreEqual(dict.Count, number - 1);

            foreach (KeyValuePair<string, int> item in dict)
            {
                dictList.Add(item.Key);
            }
            Assert.AreEqual(dictList[0], "test1");
            number = 0;
            dict.Dispose();
        }

        [TestMethod]
        public void MapIdRemoveTestMethod2()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdRemoveTestMethod2");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdRemoveTestMethod2", hsp);
            dict.Clear();
            bool result = false;

            try
            {
                dict.Remove(null);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            dict.Dispose();
        }

        /// <summary>
        /// Értelmetlen key-t akarok törölni
        /// </summary>
        [TestMethod]
        public void MapIdRemoveTestMethod3()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdRemoveTestMethod3");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdRemoveTestMethod3", hsp);
            dict.Clear();
            number = 10;
            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);
            dict.Remove("testrr");

            Assert.AreEqual(dict.Count, number);
            number = 0;
            dict.Dispose();
        }


        [TestMethod]
        public void MapIdRemoveTestMethod4()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdRemoveTestMethod4");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdRemoveTestMethod4", hsp);
            KeyValuePair<string, int> removeKey = new KeyValuePair<string, int>("test0", 0);
            dict.Clear();
            number = 10;
            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);
            dict.Remove(removeKey);
            Assert.AreEqual(dict.Count, number - 1);
            removeKey = new KeyValuePair<string, int>("test3", 3);
            dict.Remove(removeKey);
            Assert.AreEqual(dict.Count, number - 2);

            removeKey = new KeyValuePair<string, int>("test1", 1);
            dict.Remove(removeKey);
            removeKey = new KeyValuePair<string, int>("test2", 2);
            dict.Remove(removeKey);
            removeKey = new KeyValuePair<string, int>("test4", 4);
            dict.Remove(removeKey);
            removeKey = new KeyValuePair<string, int>("test5", 5);
            dict.Remove(removeKey);
            removeKey = new KeyValuePair<string, int>("test6", 6);
            dict.Remove(removeKey);
            removeKey = new KeyValuePair<string, int>("test7", 7);
            dict.Remove(removeKey);
            removeKey = new KeyValuePair<string, int>("test8", 8);
            dict.Remove(removeKey);
            removeKey = new KeyValuePair<string, int>("test9", 9);
            dict.Remove(removeKey);
            Assert.AreEqual(dict.Count, 0);
            Assert.IsTrue(dict.IsEmpty);
            number = 0;
            dict.Dispose();
        }

        /// <summary>
        /// Egy olyan KeyValuePairt akarok törölni, ami nincs a dict-ben
        /// </summary>
        [TestMethod]
        public void MapIdRemoveTestMethod5()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdRemoveTestMethod5");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdRemoveTestMethod5", hsp);
            KeyValuePair<string, int> removeKey = new KeyValuePair<string, int>("test0", 3);
            dict.Clear();
            number = 10;
            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);
            dict.Remove(removeKey);
            Assert.AreEqual(dict.Count, number);
            number = 0;
            dict.Dispose();
        }

        [TestMethod]
        public void MapIdRemoveTestMethod6()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdRemoveTestMethod6");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdRemoveTestMethod6", hsp);
            KeyValuePair<string, int> removeKey = new KeyValuePair<string, int>(null, 3);
            dict.Clear();
            number = 10;

            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);
            dict.Remove(removeKey);
            Assert.AreEqual(dict.Count, number);
            number = 0;
            dict.Dispose();
        }

        [TestMethod]
        public void MapIdContainsKeyTestMethod()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("MapIdContainsKeyTestMethod");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("MapIdContainsKeyTestMethod", hsp);
            dict.Clear();
            number = 10;
            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);

            foreach (KeyValuePair<string, int> item in dict)
            {
                Assert.IsTrue(dict.Contains(item));
                Assert.IsTrue(dict.ContainsKey(item.Key));
            }
            number = 0;
            dict.Dispose();
        }

        [TestMethod]
        public void TryGetValueTestMethod()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("TryGetValueTestMethod");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("TryGetValueTestMethod", hsp);
            dict.Clear();
            number = 10;
            bool result = false;
            bool result2 = false;
            int value = 0;

            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);

            result = dict.TryGetValue("test0", out value);
            Assert.IsNotNull(value);
            result2 = dict.TryGetValue("test1", out value);
            Assert.IsTrue(result);
            Assert.IsTrue(result2);
            dict.TryGetValue("test6", out value);
            //itt a value=6
            Assert.IsNotNull(value);
            number = 0;
            dict.Dispose();
        }

        /// <summary>
        /// Egy olyan elem értékét szeretném majd kerresni, ami nincs benne a dict-ben 
        /// Ilyenkor a TryGetValue false értékkel tér vissza 
        /// </summary>
        [TestMethod]
        public void TryGetValueTestMethod2()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("TryGetValueTestMethod2");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("TryGetValueTestMethod2", hsp);
            dict.Clear();
            number = 10;
            bool result = true;
            int value = 0;

            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("Item" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);
            result = dict.TryGetValue("Test", out value);
            Assert.IsFalse(result);
            dict.Dispose();
            number = 0;
        }

        [TestMethod]
        public void TryGetValueTestMethod3()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("TryGetValueTestMethod3");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("TryGetValueTestMethod3", hsp);
            dict.Clear();
            number = 10;
            bool result = true;
            int value = 0;
            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("Item" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);
            try
            {
                dict.TryGetValue(null, out value);

            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            number = 0;
            dict.Dispose();
        }

        /// <summary>
        /// null érték ellenőrzése
        /// </summary>
        [TestMethod]
        public void CopyToTestMethod()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("CopyToTestMethod");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("CopyToTestMethod", hsp);
            dict.Clear();
            number = 10;
            bool result = false;

            try
            {
                dict.CopyTo(null, 10);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsTrue(result);
            dict.Dispose();
        }

        [TestMethod]
        public void CopyToTestMethod2()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> hsp = new HibernateStorageProvider<KeyValuePair<string, int>>("CopyToTestMethod2");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("CopyToTestMethod2", hsp);
            dict.Clear();
            number = 10;
            KeyValuePair<string, int>[] t = new KeyValuePair<string, int>[number + 2];
            Assert.AreEqual(dict.Count, 0);
            for (int i = 0; i < number; i++)
            {
                dict.Add("test" + i.ToString(), i);
            }
            Assert.AreEqual(dict.Count, number);
            dict.CopyTo(t, 2);
            Assert.AreEqual(t.Length, number + 2);
            number = 0;
            dict.Dispose();
        }

        #endregion

        #endregion
    }
}
