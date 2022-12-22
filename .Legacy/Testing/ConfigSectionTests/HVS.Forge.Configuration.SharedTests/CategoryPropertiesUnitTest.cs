using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HVS.Forge.Configuration.Shared;
using HVS.Forge.Persistence.StorageProviders.ConfigSection;
using System.Configuration;
using HVS.Forge.EntityFramework.Model;
using NHibernate;

namespace HVS.Forge.Testing.ConfigSectionTests.HVS.Forge.Configuration.SharedTests
{
    /// <summary>
    /// Summary description for CategoryPropertiesUnitTest
    /// </summary>
    [TestClass]
    public class CategoryPropertiesUnitTest
    {

        #region Fields

        private TestContext testContextInstance;

        CategoryPropertyItem categoryPropertyItem = new CategoryPropertyItem();
        CategoryPropertyItems categoryPropertyItems = new CategoryPropertyItems();
        string categoryName = "this is a test string";

        private static ISessionFactory sessionFactory = null;


        #endregion

        #region Constructor

        public CategoryPropertiesUnitTest()
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
            //CategoryPropertyItem categoryPropertyItem = new CategoryPropertyItem();

            //categoryPropertyItem.Id = null;//string.Empty;
        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            if (sessionFactory != null )
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
        public void CategoryPropertyItemIdTestMethod()
        {
            Assert.IsNull(categoryPropertyItem.Id);
            //Assert.IsNotNull(categoryPropertyItem.Id);
        }

        [TestMethod]
        public void CategoryPropertyItemEntryNameTestMethod()
        {
            Assert.IsNull(categoryPropertyItem.EntryName);
            //Assert.IsNotNull(categoryPropertyItem.EntryName);
        }

        [TestMethod]
        public void CategoryPropertyItemEntryValueTestMethod()
        {
            Assert.IsNotNull(categoryPropertyItem.EntryValue);
            Assert.AreEqual(categoryPropertyItem.EntryValue, string.Empty); //default value is ="";
        }

        [TestMethod]
        public void CategoryPropertyItemEncryptedTestMethod() //this property default value is false,
        {
            //Assert.IsTrue(categoryPropertyItem.Encrypted);
            Assert.IsTrue(!categoryPropertyItem.Encrypted);
        }

        [TestMethod]
        public void PropertyItemsTestMethod()
        {
            //Assert.IsNull(categoryPropertyItem.PropertyItems);
            Assert.IsNotNull(categoryPropertyItem.PropertyItems);
        }

        [TestMethod]
        public void CategoryPropertyItemParentTestMethod()
        {
            Assert.IsNull(categoryPropertyItem.Parent);

        }

        [TestMethod]
        public void CategoryPropertyItemGetEnumeratorTestMethod()
        {
            //Assert.AreEqual(categoryPropertyItem.GetEnumerator().Current, null);
            Assert.AreNotEqual(categoryPropertyItem.GetEnumerator().Current, null);
            //Assert.IsNull(categoryPropertyItem.GetEnumerator());
        }

        [TestMethod]
        public void CategoryPropertyItemCloneTestMethod()
        {
            //Assert.IsNull(categoryPropertyItem.Clone());
            Assert.IsNotNull(categoryPropertyItem.Clone());
        }

        [TestMethod]
        public void CategoryPropertyItemsGetCategoryItemsTestMethod()
        {
            //Assert.IsNull(categoryPropertyItems.GetCategoryItems(string.Empty)); //erre igazat ad
            //Assert.IsNull(categoryPropertyItems.GetCategoryItems(null));
            //Assert.IsNull(categoryPropertyItems.GetCategoryItems("test messenge"));
            Assert.IsNotNull(categoryPropertyItems.GetCategoryItems(string.Empty));
            Assert.IsNotNull(categoryPropertyItems.GetCategoryItems("test messenge"));
        }

        [TestMethod]
        public void CategoryPropertyItemsParentTestMethod()
        {
            Assert.IsNull(categoryPropertyItems.Parent);
            //Assert.IsNotNull(categoryPropertyItems.Parent);
        }

        [TestMethod]
        public void CategoryPropertyItemsCloneTestMethod()
        {
            //Assert.AreEqual(categoryPropertyItem.Clone(), categoryPropertyItems.Clone());
            //Assert.IsNull(categoryPropertyItems.Clone());
            Assert.IsNotNull(categoryPropertyItems.Clone());
        }

        [TestMethod]
        public void CategoryPropertyItemsGetEnumeratorTestMethod()
        {
            Assert.IsNotNull(categoryPropertyItems.GetEnumerator());
            int count = int.MaxValue;
            Assert.AreNotEqual(categoryPropertyItems.Count, count);
            Assert.IsNotNull(categoryPropertyItems.GetEnumerator());
            Assert.IsTrue(!categoryPropertyItems.EmitClear);
            Assert.AreNotEqual(categoryPropertyItems.GetEnumerator(), null);
        }

        [TestMethod]
        public void TypeTestMethod()
        {
            CategoryPropertyItems varialbe = new CategoryPropertyItems();
            Assert.IsInstanceOfType(varialbe, typeof(ConfigurationElementCollection));
            Assert.IsInstanceOfType(varialbe, typeof(ICloneable));
            Assert.IsInstanceOfType(varialbe, typeof(IEnumerable<CategoryPropertyItem>));

            CategoryPropertyItem varible2 = new CategoryPropertyItem();
            //Assert.IsInstanceOfType(varible2, typeof(ConfigurationElementCollection));
            Assert.IsInstanceOfType(varible2, typeof(ConfigurationElement));
            Assert.IsInstanceOfType(varible2, typeof(ICloneable));
            Assert.IsInstanceOfType(varible2, typeof(IEnumerable<CategoryPropertyItem>));
            //Assert.IsInstanceOfType(varible2, typeof(IEnumerable<CategoryPropertyItems>));
            //Assert.IsInstanceOfType(varialbe, typeof(IEnumerable<CategoryPropertyItems>));
        }

        [TestMethod]
        public void PropertyesTestMethod()
        {
            //using (ISession session = sessionFactory.OpenSession())
            //{
            //using (ITransaction transaction = session.BeginTransaction())
            //{
            categoryPropertyItem.Id = Guid.NewGuid() + "testMessenge";
            categoryPropertyItem.EntryName = String.Empty;
            categoryPropertyItem.EntryName = Guid.NewGuid().ToString();
            categoryPropertyItem.Encrypted = true;
            categoryPropertyItem.EntryValue = String.Empty + "";
            categoryPropertyItem.PropertyItems = null;
            categoryPropertyItem.Parent = null;

            //transaction.Commit();
            //}
            //}
        }

        [TestMethod]
        public void IEnumeratorGetEnumeratorTestMethod()
        {
            categoryPropertyItem.PropertyItems = StorageConfiguration.Settings.CategoryPropertyItems;
            Assert.IsNotNull(categoryPropertyItem.GetEnumerator());
        }

        [TestMethod]
        public void GetCategoryItemsTestMethod()
        {
            Assert.IsNull(categoryPropertyItems.GetCategoryItems("TestName"));

            //Assert.IsNull(categoryPropertyItems.GetCategoryItems("TestName"));
            //categoryPropertyItems.GetCategoryItems("TestName");
        }

        [TestMethod]
        public void ParentTestMethod()
        {
            bool value = true; //fales don't pass the test
            if(value)
            {
                categoryPropertyItem.Parent = StorageConfiguration.Settings.CategoryPropertyItems;
            }

            Assert.IsNotNull(categoryPropertyItem.Parent);
            Assert.AreNotSame("string", categoryPropertyItem.Parent);
            Assert.AreNotSame(null, categoryPropertyItem.Parent);
            Assert.AreNotSame(int.MaxValue, categoryPropertyItem.Parent);
            Assert.AreNotSame(null, categoryPropertyItem.Parent);
        }

        [TestMethod]
        public void typeTestMthodClone()
        {
            CategoryPropertyItem item = new CategoryPropertyItem();
            Assert.IsInstanceOfType(item, typeof(ICloneable));
        }

        [TestMethod]
        public void CategoryPropertyItemTestMethod()
        {
            CategoryPropertyItem items = new CategoryPropertyItem();
            items.Id = Guid.NewGuid().ToString();
            items.Encrypted = false;
            items.EntryName = string.Empty;
            items.EntryValue = null;
            items.PropertyItems = StorageConfiguration.Settings.CategoryPropertyItems;
            //items.PropertyItems = null;
            Assert.IsNotNull(items.PropertyItems);
            //Assert.IsNull(categoryPropertyItem.Clone());
        }




        #endregion
    }
}
