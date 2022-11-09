using System;
using System.IO;
using System.Text;
using Forge.Persistence.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.Formatter
{
    /// <summary>
    /// Summary description for GZipFormatterUnitTest
    /// </summary>
    [TestClass]
    public class GZipFormatterUnitTest
    {
        #region Fields 

        private TestContext testContextInstance;
        public static GZipFormatter gZipFormatter = new GZipFormatter();

        #endregion

        #region Constructor 

        public GZipFormatterUnitTest()
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
            MemoryStream ms = new MemoryStream();
            byte[] bite = ASCIIEncoding.Default.GetBytes("test");
            gZipFormatter.Write(ms, bite);
            ms.Position = 0;
            Assert.IsTrue(gZipFormatter.CanRead(ms));
            gZipFormatter = new GZipFormatter();
        }
        #endregion

        #region Can Write Test Method

        [TestMethod]
        public void CanWriteTestMethod()
        {
            byte[] bite = ASCIIEncoding.Default.GetBytes("test");
            bool result = gZipFormatter.CanWrite(bite);
            Assert.IsTrue(result);
            gZipFormatter = new GZipFormatter();
        }

        [TestMethod]
        public void CanWriteTestMethod2()
        {
            byte[] bite = ASCIIEncoding.Default.GetBytes(string.Empty);
            bool result = gZipFormatter.CanWrite(bite);
            Assert.IsTrue(result);
            gZipFormatter = new GZipFormatter();
        }

        [TestMethod]
        public void  CanWriteTestMethod3()
        {
            bool result = false;
            try
            {
                gZipFormatter.CanWrite(null);
            }
            catch (ArgumentNullException)
            {
                result = true;
            }
            Assert.IsTrue(result);
            gZipFormatter = new GZipFormatter();
        }


        #endregion

        #region Write Test Method
        
        [TestMethod]
        public void WriteTestMethod()
        {
            byte[] bite = ASCIIEncoding.Default.GetBytes("test");
            bool result = false;
            MemoryStream ms = new MemoryStream();
            gZipFormatter.Write(ms, bite);
            ms.Position = 0;
            Assert.IsTrue(gZipFormatter.CanRead(ms));

            try
            {
                gZipFormatter.Read(ms);
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
            gZipFormatter = new GZipFormatter();
        }

        #endregion

        #region Read Test Method 

        [TestMethod]
        public void ReadTestMethod()
        {
            byte[] bite = ASCIIEncoding.Default.GetBytes("test");
            bool result = false;
            MemoryStream ms = new MemoryStream();
            gZipFormatter.Write(ms, bite);
            ms.Position = 0;
            Assert.IsTrue(gZipFormatter.CanRead(ms));

            try
            {
                gZipFormatter.Read(ms);
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
            byte[] resutlbyte = gZipFormatter.Read(ms);
            Assert.AreEqual(bite.ToString(), resutlbyte.ToString());
            gZipFormatter = new GZipFormatter();
        }

        #endregion

        #endregion
    }
}
