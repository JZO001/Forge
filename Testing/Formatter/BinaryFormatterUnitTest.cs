using System;
using System.IO;
using Forge.Persistence.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.Formatter
{
    /// <summary>
    /// Summary description for BinaryFormatterUnitTest
    /// </summary>
    [TestClass]
    public class BinaryFormatterUnitTest
    {
        #region Fields

        private TestContext testContextInstance;
       
        BinaryFormatter<string> binaryFormatter = new BinaryFormatter<string>();  
   
        #endregion

        #region Constructor

        public BinaryFormatterUnitTest()
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
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region Test Methods

        #region Can Read Test Metohds

        [TestMethod]
        public void CanReadTestMethod()
        {
            string test = "test";
            MemoryStream ms = new MemoryStream();
            binaryFormatter.Write(ms, test);
            ms.Position = 0;
            Assert.IsTrue(binaryFormatter.CanRead(ms));
            binaryFormatter = new BinaryFormatter<string>();
            
        }
        #endregion

        #region Can Write Test Methods

        /// <summary>
        /// binaryFormatter can write with correct value
        /// </summary>
        [TestMethod]
        public void CanWriteTestMethod()
        {
            bool result = binaryFormatter.CanWrite("item");
            Assert.IsTrue(result);
            binaryFormatter = new BinaryFormatter<string>();
        }

        /// <summary>
        /// binaryFormatter can write with empty string value
        /// </summary>
        [TestMethod]
        public void CanWriteTestMethod2()
        {
            bool result = binaryFormatter.CanWrite(string.Empty);
            Assert.IsTrue(result);
            binaryFormatter = new BinaryFormatter<string>();
        }

        /// <summary>
        /// binaryFormatter can write with null value
        /// </summary>
        [TestMethod]
        public void CanWriteTestMethod3()
        {
            bool result = false;
            try
            {
                binaryFormatter.CanWrite(null);
            }
            catch (ArgumentNullException)
            {
                result = true;
            }
            Assert.IsTrue(result);
            binaryFormatter = new BinaryFormatter<string>();
        }

        #endregion

        #region Write Test Methods

        [TestMethod]
        public void WriteTestMethod()
        {
            string test = "test";
            bool result = false;
            MemoryStream ms = new MemoryStream();
            binaryFormatter.Write(ms, test);
            ms.Position = 0;
            Assert.IsTrue(binaryFormatter.CanRead(ms));

            try
            {
                binaryFormatter.Read(ms);
            }
            catch (ArgumentNullException)
            {
                result = true;   
            }
            catch (FormatException)
            {
                result = true;
            }
            catch(Exception)
            {
                result = true;
            }
            Assert.IsFalse(result);
            binaryFormatter = new BinaryFormatter<string>();
        }

        #endregion

        #region Read Test Method

        [TestMethod]
        public void ReadTestMethod()
        {
            string test = "test";
            bool result = false;
            MemoryStream ms = new MemoryStream();
            binaryFormatter.Write(ms, test);
            ms.Position = 0;
            Assert.IsTrue(binaryFormatter.CanRead(ms));

            try
            {
                binaryFormatter.Read(ms);
            }
            catch (ArgumentNullException)
            {
                result = true;
            }
            catch (FormatException)
            {
                result = true;
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsFalse(result);
            ms.Position = 0;
            var read = binaryFormatter.Read(ms);
            Assert.AreEqual(test, read);
            binaryFormatter = new BinaryFormatter<string>();
        }

        #endregion

        #endregion
    }
}
