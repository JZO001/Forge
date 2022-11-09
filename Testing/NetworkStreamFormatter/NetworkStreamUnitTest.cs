using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.NetworkStreamFormatter
{
    /// <summary>
    /// Summary description for NetworkStreamUnitTest
    /// </summary>
    [TestClass]
    public class NetworkStreamUnitTest
    {
        #region Fields

        private TestContext testContextInstance;

        #endregion

        #region Constructor
        
        public NetworkStreamUnitTest()
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
        
        [TestMethod]
        public void CanReadTestMethod()
        {
            MyData test = new MyData();
            test.message = "test_messenge";
            test.sendData = "send_data";
            test.data = test.ToByte();
            NetworkStreamFormatter nsf = new NetworkStreamFormatter();           
            MemoryStream ms = new MemoryStream();
            nsf.Write(ms, test);
            ms.Position = 0;
            bool result = nsf.CanRead(ms);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanWriteTestMethod()
        {
            MyData test = new MyData();
            test.message = "test_messenge";
            test.sendData = "send_data";
            test.data = test.ToByte();
            NetworkStreamFormatter nsf = new NetworkStreamFormatter();
            MemoryStream ms = new MemoryStream();
            bool result = nsf.CanWrite(test);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReadTesteMethod()
        {
            MyData test = new MyData();
            test.message = "test";
            test.sendData = "send_data";
            test.data = test.ToByte();
            NetworkStreamFormatter nsf = new NetworkStreamFormatter();
            MemoryStream ms = new MemoryStream();
            nsf.Write(ms, test);
            ms.Position = 0;
            MyData result = nsf.Read(ms);
            Assert.IsNotNull(result);
            Assert.AreEqual(test.message, result.message);
            Assert.AreEqual(test.sendData, result.sendData);
        }

        [TestMethod]
        public void WriteTestMethod()
        {
            MyData test = new MyData();
            test.message = "test";
            test.sendData = "send_data";
            test.data = test.ToByte();
            NetworkStreamFormatter nsf = new NetworkStreamFormatter();
            MemoryStream ms = new MemoryStream();
            bool result = false;
            try
            {
                nsf.Write(ms, test);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.IsFalse(result);
        }
        
        #endregion
    }
}
