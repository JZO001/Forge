using System;
using System.Collections;
using System.Collections.Generic;
using Forge.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.Collection
{
    /// <summary>
    /// Summary description for ISubListUnitTest
    /// </summary>
    [TestClass]
    public class ISubListUnitTest
    {
        #region Fields

        private TestContext testContextInstance;

        public int listNumber = 0;

        public static ListSpecialized<string> listSpecialized = new ListSpecialized<string>();

        #endregion

        #region Constructor

        public ISubListUnitTest()
        {
           
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
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 

        //[TestInitialize()]
        //public void MyTestInitialize() 
        //{
            
            
        //}

        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region Test Methods

        [TestMethod]
        public void SublistTestMethod()
        {
            listNumber = 10;
            ISubList<string> sublistTemp;
            ISubList<string> sublist;
            IEnumerable<string> enumerable;

            for (int i = 0; i < listNumber; i++)
            {
                listSpecialized.Add(Guid.NewGuid().ToString());
            }
            sublist = listSpecialized.SubList(3, 6);
            sublistTemp = sublist;
            Assert.IsNotNull(sublist);

            enumerable = new string[] { (Guid.NewGuid().ToString()) };
            sublist.AddRange(enumerable);
            Assert.IsNotNull(sublist);
            Assert.AreEqual(sublistTemp, sublist);
            Assert.IsNotNull(listSpecialized);
            listSpecialized.Clear();
            listNumber = 0;
            Assert.AreEqual(0, listSpecialized.Count);
        }

        #region AddRage Test Method

        [TestMethod]
        public void AddRageTestMethod()
        {
            listNumber = 10;
            ISubList<string> sublistTemp;
            ISubList<string> sublist;
            IEnumerable<string> enumerable;

            for (int i = 0; i < listNumber; i++)
            {
                listSpecialized.Add(Guid.NewGuid().ToString());
            }
            sublist = listSpecialized.SubList(3, 6);
            sublistTemp = sublist;
            for (int i = 0; i < listNumber-5; i++)
            {
                enumerable = new string[] { (Guid.NewGuid().ToString()) };
                sublist.AddRange(enumerable);
            }
            Assert.IsNotNull(sublist);
            Assert.AreEqual(sublist, sublistTemp);
            Assert.AreEqual(sublistTemp.Count +7, listSpecialized.Count);
            listSpecialized.Clear();
            listNumber = 0;
            Assert.AreEqual(0, listSpecialized.Count);            
        }

        [TestMethod]
        public void AddRageTestMethod2()
        {
            listNumber = 10;
            ISubList<string> sublist;
            IEnumerable<string> enumerable =  new string[] { ("Test2000") };


            for (int i = 0; i < listNumber; i++)
            {
                listSpecialized.Add("Test"+i.ToString());
            }

            sublist = listSpecialized.SubList(3,6);
            sublist.AddRange(enumerable);

            Assert.AreEqual(sublist[0], listSpecialized[3]);
            Assert.AreEqual(listSpecialized.Count, listNumber+1);
            Assert.AreEqual(sublist.Count, listNumber-6);
            Assert.AreEqual(listSpecialized[listNumber-4], "Test2000");
            Assert.AreEqual(sublist[3], "Test2000");
            Assert.IsNotNull(sublist[3]);
            listNumber = 0;
            listSpecialized.Clear();
            Assert.AreEqual(0, listSpecialized.Count);
        }

        #endregion

        #region Remove Test Methods

        [TestMethod]
        public void RemoveAtTestMetod()
        {
            listNumber = 10;
            ISubList<string> sublistTemp;
            ISubList<string> sublist;

            for (int i = 0; i < listNumber; i++)
            {
                listSpecialized.Add(Guid.NewGuid().ToString());
            }
            Assert.AreEqual(listSpecialized.Count, listNumber);

            sublist = listSpecialized.SubList(3,6);
            sublistTemp = sublist;

            int sublistTempCount = sublistTemp.Count;
            sublistTemp.RemoveAt(sublistTempCount-1);
            Assert.AreEqual(sublistTemp.Count, 2);
            Assert.AreEqual(listSpecialized.Count, 9);
            listNumber = 0;
            listSpecialized.Clear();
            Assert.AreEqual(0, listSpecialized.Count);
        }

        [TestMethod]
        public void RemmoveMoreItems()
        {
            listNumber = 20;
            ISubList<string> sublist;

            for (int i = 0; i < listNumber; i++)
            {
                listSpecialized.Add("test" + i.ToString());
            }
            Assert.AreEqual(listSpecialized.Count, listNumber);
            
            sublist = listSpecialized.SubList(5, 15);
            Assert.AreEqual(sublist.Count, listNumber/2);

            Assert.AreEqual(listSpecialized[6], "test6");
            for (int i = 0; i < listNumber-10; i++)
            {
                sublist.RemoveAt(0);
            }
            Assert.AreEqual(sublist.Count, 0);
            Assert.AreEqual(listSpecialized.Count, listNumber - 10);

            Assert.AreEqual(listSpecialized[6], "test16");
            Assert.AreNotEqual(listSpecialized[6], "test6");
            listNumber = 0;
            listSpecialized.Clear();
            Assert.AreEqual(0, listSpecialized.Count);
        }

        [TestMethod]
        public void RemoveTestMethod()
        {
            listNumber = 10;
            for (int i = 0; i < listNumber; i++)
            {
                listSpecialized.Add("a" + i.ToString());
            }
            Assert.AreEqual(listSpecialized.Count, listNumber);
            string searchItem = "a0";
            IEnumeratorSpecialized<string> iterator = (IEnumeratorSpecialized<string>)listSpecialized.GetEnumerator();
            while (iterator.MoveNext())
            {
                string item = iterator.Current;
                if (item == searchItem)
                {
                    iterator.Remove();
                    break;
                }
            }
            Assert.AreNotEqual(listSpecialized[0], "a0");
            listNumber = 0;
            listSpecialized.Clear();
        }

        #endregion

        [TestMethod]
        public void InsertTestMethod()
        {
            listNumber = 10;
            ISubList<string> sublist;
            for (int i = 0; i < listNumber; i++)
            {
                listSpecialized.Add(Guid.NewGuid().ToString());
            }
            Assert.AreEqual(listSpecialized.Count, listNumber);

            sublist = listSpecialized.SubList(3,6);
            Assert.AreEqual(sublist.Count, 3);

            sublist.Insert(2, "TestString");
            Assert.AreEqual(sublist.Count, 4);
            Assert.AreEqual(listSpecialized.Count, listNumber+1);
            Assert.AreEqual(sublist[2], "TestString");
            Assert.AreEqual(listSpecialized[5], "TestString");
            listNumber = 0;
            listSpecialized.Clear();
            Assert.AreEqual(0, listSpecialized.Count);
        }

        [TestMethod]
        public void SearchReplaceTestMethod()
        {
            listNumber = 10;
            ISubList<string> sublist;
            for (int i = 0; i < listNumber; i++)
            {
                listSpecialized.Add("TestItem" + i.ToString());
            }
            Assert.AreEqual(listSpecialized.Count, listNumber);
            sublist = listSpecialized.SubList(3, 6);

            string searchItem = "TestItem3";

            IEnumeratorSpecialized<string> iterator = (IEnumeratorSpecialized<string>)sublist.GetEnumerator();
            while (iterator.MoveNext())
            {
                string item = iterator.Current;
                if (item == searchItem)
                {
                    listSpecialized[0] = searchItem;
                    break;
                }
            }
            Assert.AreEqual(sublist[0], searchItem);
            Assert.AreEqual(listSpecialized[3], searchItem);
            listNumber = 0;
            listSpecialized.Clear();
        }

        [TestMethod]
        public void SeacrhAndAddTestMethod()
        {
            listNumber = 10;
            for (int i = 0; i < listNumber; i++)
            {
                listSpecialized.Add("TestItem" + i.ToString());
            }
            Assert.AreEqual(listSpecialized.Count, listNumber);
            string addItem = "TestItem5";

            IEnumeratorSpecialized<string> iterator = (IEnumeratorSpecialized<string>)listSpecialized.GetEnumerator();
            while (iterator.MoveNext())
            {
                string item = iterator.Current;
                if (item == addItem)
                {
                    listSpecialized.Add(addItem);
                    break;
                }
            }

            Assert.AreEqual(listSpecialized.Count, listNumber + 1);
            listNumber = 0;
            listSpecialized.Clear();
        }


        #endregion
    }
}
