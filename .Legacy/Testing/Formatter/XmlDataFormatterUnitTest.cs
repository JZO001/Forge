using System;
using System.IO;
using Forge.Persistence.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.Formatter
{
    /// <summary>
    /// Summary description for XmlDataFormatterUnitTest
    /// </summary>
    [TestClass]
    public class XmlDataFormatterUnitTest
    {
        #region Fields

        private TestContext testContextInstance;

        XmlDataFormatter<string> xmlDataFormatter = new XmlDataFormatter<string>();

        #endregion

        #region Contructor

        public XmlDataFormatterUnitTest()
        {
        }

        #endregion

        #region Properies

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

        #region Can Run Test Method

        [TestMethod]
        public void TestMethod()
        {
            string test = "test";
            MemoryStream ms = new MemoryStream();
            xmlDataFormatter.Write(ms, test);
            ms.Position = 0;
            Assert.IsTrue(xmlDataFormatter.CanRead(ms));
            xmlDataFormatter = new XmlDataFormatter<string>();
        }

        #endregion

        #region Can Write Test Method
        [TestMethod]
        public void CanWriteTestMethod()
        {
            string test = "test";
            bool result = xmlDataFormatter.CanWrite(test);
            Assert.IsTrue(result);
            xmlDataFormatter = new XmlDataFormatter<string>();
        }

        [TestMethod]
        public void CanWriteTestMethod2()
        {
            bool result = xmlDataFormatter.CanWrite(string.Empty);
            Assert.IsTrue(result);
            xmlDataFormatter = new XmlDataFormatter<string>();
        }

        [TestMethod]
        public void CanWriteTestMethod3()
        {
            bool result = false;
            try
            {
                xmlDataFormatter.CanWrite(null);
            }
            catch (ArgumentNullException)
            {
                result = true;
            }
            Assert.IsTrue(result);
            xmlDataFormatter = new XmlDataFormatter<string>();
        }

        #endregion

        #region Write Test Method

        [TestMethod]
        public void WriteTestMethod()
        {
            string test = "test";
            bool result = false;
            MemoryStream ms = new MemoryStream();
            xmlDataFormatter.Write(ms, test);
            ms.Position = 0;
            Assert.IsTrue(xmlDataFormatter.CanRead(ms));

            try
            {
                xmlDataFormatter.Read(ms);
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
            xmlDataFormatter = new XmlDataFormatter<string>();
        }

        #endregion

        #region Read Test Method

        [TestMethod]
        public void ReadTestMethod()
        {
            string test = "test";
            bool result = false;
            MemoryStream ms = new MemoryStream();
            xmlDataFormatter.Write(ms, test);
            ms.Position = 0;
            Assert.IsTrue(xmlDataFormatter.CanRead(ms));

            try
            {
                var read = xmlDataFormatter.Read(ms);
                Assert.AreEqual(test, read);
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
            xmlDataFormatter = new XmlDataFormatter<string>();
        }

        #endregion
 
        #endregion
    }
}
