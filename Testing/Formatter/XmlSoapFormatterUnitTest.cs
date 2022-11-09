using System;
using System.IO;
using Forge.Persistence.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.Formatter
{
    /// <summary>
    /// Summary description for XmlSoapFormatterUnitTest
    /// </summary>
    [TestClass]
    public class XmlSoapFormatterUnitTest
    {
        #region Fields
        private TestContext testContextInstance;
        XmlSoapFormatter<string> xmlSoapFormatter = new XmlSoapFormatter<string>();


        #endregion

        #region Constructor

        public XmlSoapFormatterUnitTest()
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

        #region Can Read Test Method
        
        [TestMethod]
        public void CanReadTestMethod()
        {
            string test = "test";
            MemoryStream ms = new MemoryStream();
            xmlSoapFormatter.Write(ms, test);
            ms.Position = 0;
            Assert.IsTrue(xmlSoapFormatter.CanRead(ms));
            xmlSoapFormatter = new XmlSoapFormatter<string>();
        }


        #endregion

        #region Can Write Test Metohd
        
        [TestMethod]
        public void CanWriteTestMethod()
        {
            bool result = xmlSoapFormatter.CanWrite("item");
            Assert.IsTrue(result);
            xmlSoapFormatter = new XmlSoapFormatter<string>();
        }

        [TestMethod]
        public void CanWriteTestMethod2()
        {
            bool result = xmlSoapFormatter.CanWrite(string.Empty);
            Assert.IsTrue(result);
            xmlSoapFormatter = new XmlSoapFormatter<string>();
        }

        [TestMethod]
        public void CanWriteTestMethod3()
        {
            bool result = false;
            try
            {
                xmlSoapFormatter.CanWrite(null);
            }
            catch (ArgumentNullException)
            {
                result = true;
            }
            Assert.IsTrue(result);
            xmlSoapFormatter = new XmlSoapFormatter<string>();
        }


        #endregion

        #region Write Test Metod

        [TestMethod]
        public void WriteTestMethod()
        {
            string test = "test";
            bool result = false;
            MemoryStream ms = new MemoryStream();
            xmlSoapFormatter.Write(ms, test);
            ms.Position = 0;
            Assert.IsTrue(xmlSoapFormatter.CanRead(ms));

            try
            {
                xmlSoapFormatter.Read(ms);
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
            xmlSoapFormatter = new XmlSoapFormatter<string>();
        }

        #endregion

        #region Read Test Method

        [TestMethod]
        public void ReadTestMethod()
        {
            string test = "test";
            bool result = false;
            MemoryStream ms = new MemoryStream();
            xmlSoapFormatter.Write(ms, test);
            ms.Position = 0;
            Assert.IsTrue(xmlSoapFormatter.CanRead(ms));

            try
            {
                xmlSoapFormatter.Read(ms);
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
            var read = xmlSoapFormatter.Read(ms);
            Assert.AreEqual(test, read);
            xmlSoapFormatter = new XmlSoapFormatter<string>();
        }

        #endregion

        #endregion
    }
}
