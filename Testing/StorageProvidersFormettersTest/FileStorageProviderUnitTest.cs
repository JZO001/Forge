using System;
using System.Collections.Generic;
using System.Threading;
using Forge.Collections;
using Forge.Persistence.Formatters;
using Forge.Persistence.StorageProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.StorageProvidersFormettersTest
{
    /// <summary>
    /// Summary description for FileStorageProviderUnitTest
    /// </summary>
    [TestClass]
    public class FileStorageProviderUnitTest
    {
        #region Fields

        private TestContext testContextInstance;

        public static string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap";

        public static string id = Guid.NewGuid().ToString();

        public static FileStorageProvider<string> fileStorageProvider = new FileStorageProvider<string>(id, source);

        public static ListSpecialized<string> listSpecialized = new ListSpecialized<string>();

        int listNumber = 0;

        #endregion

        #region Constructor
        
        public FileStorageProviderUnitTest()
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

        #region TestMethods

        #region id and source Constructor And Add Test Methods

        [TestMethod]
        public void  IdAndSourceConstructorAddTestMethod()
        {
            string test = "test";
            string idNew = "1";
            fileStorageProvider.Add(test);

            fileStorageProvider = new FileStorageProvider<string>(idNew, source);
        }

        [TestMethod]
        public void IdAndSourceConstructorAddTestMethod2()
        {
            string sourceNew = @"C:\Users\GABOR\Downloads\Pictur";
            string test = "test";
            string idNew = "1";
            FileStorageProvider<string> fileStorageProvider2 = new FileStorageProvider<string>(idNew, sourceNew);
            fileStorageProvider2.Add(test);
            fileStorageProvider2 = new FileStorageProvider<string>(id, source);
        }

        [TestMethod]
        public void IdAndSourceConstructorAddTestMethod3()
        {
            string sourceNew = string.Empty;
            string test = "test";
            string idNew = "1";
            FileStorageProvider<string> fileStorageProvider2 = new FileStorageProvider<string>(idNew, sourceNew);
            fileStorageProvider2.Add(test);
            fileStorageProvider2 = new FileStorageProvider<string>(id, source);
        }

        [TestMethod]
        public void IdAndSourceConstructorAddTestMethod4()
        {
            string sourceNew = string.Empty;
            string test = string.Empty;
            bool result = false;
            try
            {
                FileStorageProvider<string> fileStorageProvider2 = new FileStorageProvider<string>(string.Empty, sourceNew);
                fileStorageProvider2.Add(test);
            }
            catch (ArgumentNullException)
            {
                result = true;
            }
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IdAndSourceConstructorAddTestMethod5()
        {
            string sourceNew = null;
            string test = null;
            bool result = false;
            try
            {
                FileStorageProvider<string> fileStorageProvider2 = new FileStorageProvider<string>(null, sourceNew);
                fileStorageProvider2.Add(test);
            }
            catch (ArgumentNullException)
            {
                result = true;
            }
            Assert.IsTrue(result);
        }

        #region AddRage Test Methods

        [TestMethod]
        public void AddRageTestMethod()
        {
            listNumber = 10;
            IEnumerable<string> iEnumerable = new string[] { ("Test2000") };
            string idNew = "1";
            FileStorageProvider<string> fileStorageProvider = new FileStorageProvider<string>(idNew, @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest");
            for (int i = 0; i < listNumber; i++)
            {
                fileStorageProvider.AddRange(iEnumerable);
            }
            IEnumeratorSpecialized<string> iterator = (IEnumeratorSpecialized<string>)listSpecialized.GetEnumerator();
            while (iterator.MoveNext())
            {
                string item = iterator.Current;
            }

            listNumber = 0;
            fileStorageProvider.Clear();
            Assert.AreEqual(0, fileStorageProvider.Count);
            Assert.IsTrue(fileStorageProvider.IsEmpty);
        }

        #endregion

        #endregion

        #region id, source and compress Constructor Test Methods

        [TestMethod]
        public void IdSourceAndCompressConstructorTestMethod()
        {
            string id ="1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            bool result = false;
            IEnumerable<string> iEnumerable = new string[] { ("Test2000") };
            FileStorageProvider<string> FileStorageProviderCompress = new FileStorageProvider<string>(id, source, true);
            listNumber = 10;

            for (int i = 0; i < listNumber; i++)
            {
                FileStorageProviderCompress.AddRange(iEnumerable);
            }
            Thread.Sleep(1500);
            try
            {
                for (int i = 0; i < listNumber*2; i++)
                {
                    FileStorageProviderCompress.RemoveAt(i);
                }
            }

            catch (ArgumentOutOfRangeException)
            {
                result = true;
            }
            Assert.IsTrue(result);
            listNumber = 0;
            FileStorageProviderCompress.Clear();
            Assert.IsTrue(FileStorageProviderCompress.IsEmpty);
            Assert.AreEqual(FileStorageProviderCompress.Count, 0);
        }

        [TestMethod]
        public void IdSourceAndCompressConstructorTestMethod2()
        {
            string id = "1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            bool result = false;
            string test = "test";
            FileStorageProvider<string> FileStorageProviderCompress = new FileStorageProvider<string>(id, source, true);
            listNumber = 10;
            for (int i = 0; i < listNumber; i++)
            {
                FileStorageProviderCompress.Add(test);
            }

            Thread.Sleep(1500);
            try
            {
                for (int i = 0; i < listNumber*2; i++)
                {
                    FileStorageProviderCompress.Remove(test);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                result = true;
            }
            
            Assert.IsFalse(result);
            FileStorageProviderCompress.Clear();
            Assert.IsTrue(FileStorageProviderCompress.IsEmpty);
            Assert.AreEqual(FileStorageProviderCompress.Count, 0);
            listNumber = 0;
        }

        [TestMethod]
        public void IdSourceAndCompressConstructorTestMethod3()
        {
            string id = "1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            bool result = false;
            string test = "test";
            FileStorageProvider<string> FileStorageProviderCompress = new FileStorageProvider<string>(id, source, true);
            listNumber = 10;

            for (int i = 0; i < listNumber; i++)
            {
                FileStorageProviderCompress.Add(test);
            }

            Thread.Sleep(1500);

            try
            {
                for (int i = 0; i < listNumber*2; i++)
                {
                    FileStorageProviderCompress.RemoveAt(0);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                result = true;
            }                        

            Assert.IsTrue(result);
            FileStorageProviderCompress.Clear();
            Assert.IsTrue(FileStorageProviderCompress.IsEmpty);
            Assert.AreEqual(FileStorageProviderCompress.Count, 0);
            listNumber = 0;
        }

        [TestMethod]
        public void IdSourceAndCompressConstructorTestMethod4()
        {
            string id = "1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            string test = "test";
            FileStorageProvider<string> FileStorageProviderCompress = new FileStorageProvider<string>(id, source, true);
            listNumber = 10;

            for (int i = 0; i < listNumber; i++)
            {
                FileStorageProviderCompress.Add(test);
            }

            Thread.Sleep(1500);

            FileStorageProviderCompress.Clear();

            int fileNumber = FileStorageProviderCompress.Count;
            Assert.AreEqual(0, fileNumber);
            Assert.IsTrue(FileStorageProviderCompress.IsEmpty);
            listNumber = 0;
        }

        [TestMethod]
        public void IdSourceAndCompressConstructorTestMethod5()
        {
            string id = "1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            string test = "test";
            FileStorageProvider<string> FileStorageProviderCompress = new FileStorageProvider<string>(id, source, true);
            listNumber = 10;

            for (int i = 0; i < listNumber; i++)
            {
                FileStorageProviderCompress.Add(test);
            }

            Assert.AreEqual(FileStorageProviderCompress.Count, listNumber);
            Thread.Sleep(1500);
            FileStorageProviderCompress.Insert(5, test+ "Item");
            Thread.Sleep(1500);
            Assert.AreEqual(FileStorageProviderCompress[5], "testItem");
            FileStorageProviderCompress.Clear();
            Assert.IsTrue(FileStorageProviderCompress.IsEmpty);
            Assert.AreEqual(FileStorageProviderCompress.Count, 0);
            listNumber = 0;
        }

        [TestMethod]
        public void IdSourceAndCompressConstructorTestMethod6()
        {
            string id = "1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            string test = "test";
            FileStorageProvider<string> FileStorageProviderCompress = new FileStorageProvider<string>(id, source, true);
            listNumber = 10;
            for (int i = 0; i < listNumber; i++)
            {
                FileStorageProviderCompress.Add(test);
            }

            if (FileStorageProviderCompress.Count == listNumber)
            {
                Assert.AreEqual(FileStorageProviderCompress.Count, listNumber);
            }

            Assert.IsTrue(FileStorageProviderCompress.CompressContent);

            FileStorageProviderCompress.Clear();
            Assert.IsTrue(FileStorageProviderCompress.IsEmpty);
            Assert.AreEqual(FileStorageProviderCompress.Count, 0);
            listNumber = 0;
        }

        [TestMethod]
        public void IdSourceAndCompressConstructorTestMethod7()
        {
            string id = "1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            string test = "test";
            FileStorageProvider<string> FileStorageProviderCompress = new FileStorageProvider<string>(id, source, true);
            listNumber = 10;

            for (int i = 0; i < listNumber; i++)
            {
                FileStorageProviderCompress.Add(test + i.ToString());
            }

            FileStorageProviderCompress.RemoveAt(1);
            if (FileStorageProviderCompress.Count == listNumber - 1)
            {
                Assert.AreEqual(FileStorageProviderCompress.Count, listNumber - 1);
                Assert.AreEqual(FileStorageProviderCompress[1], "test2");
            }
            FileStorageProviderCompress.Clear();
            Assert.IsTrue(FileStorageProviderCompress.IsEmpty);
            Assert.AreEqual(FileStorageProviderCompress.Count, 0);
            listNumber = 0;
        }

        [TestMethod]
        public void IdSourceAndCompressConstructorTestMethod8()
        {
            string id = "1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            string test = "test";
            FileStorageProvider<string> FileStorageProviderCompress = new FileStorageProvider<string>(id, source, true);
            listNumber = 10;

            for (int i = 0; i < listNumber; i++)
            {
                FileStorageProviderCompress.Add(test);
            }

            Assert.AreEqual(FileStorageProviderCompress.Count, listNumber);
            FileStorageProviderCompress.Insert(4, "OtherText");
            Assert.AreEqual(FileStorageProviderCompress.Count, listNumber + 1);
            Assert.AreEqual(FileStorageProviderCompress[4], "OtherText");
            Assert.IsTrue(FileStorageProviderCompress.CompressContent);

            FileStorageProviderCompress.Clear();
            Assert.IsTrue(FileStorageProviderCompress.IsEmpty);
            Assert.AreEqual(FileStorageProviderCompress.Count, 0);
            listNumber = 0;
        }

        [TestMethod]
        public void IdSourceAndCompressConstructorTestMethod9()
        {
            string id = "1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            listNumber = 10;
            ListSpecialized<string> listSpecialized = new ListSpecialized<string>();
            ListSpecialized<string> templistSpecialized = new ListSpecialized<string>();

            for (int i = 0; i < listNumber; i++)
            {
                listSpecialized.Add("Test2000" + i.ToString());
            }

            FileStorageProvider<string> FileStorageProviderCompress = new FileStorageProvider<string>(id, source, true);

            for (int i = 0; i < listNumber; i++)
            {
                FileStorageProviderCompress.AddRange(listSpecialized);
            }

            FileStorageProviderCompress.Insert(9, "Test2010");

            Assert.AreEqual(FileStorageProviderCompress.Count, listNumber * listNumber + 1);
            Assert.AreEqual(FileStorageProviderCompress[9], "Test2010");

            for (int i = 0; i < listNumber; i++)
            {
                templistSpecialized.Add(listSpecialized[i]);
            }

            Assert.AreEqual(templistSpecialized.Count, listNumber);
            int FileStorageProviderCompressCount = FileStorageProviderCompress.Count;
            for (int i = 0; i < listNumber; i++)
            {
                FileStorageProviderCompress.Remove(templistSpecialized[i]);
            }

            Assert.AreEqual(FileStorageProviderCompressCount, FileStorageProviderCompress.Count + listNumber);

            FileStorageProviderCompress.Clear();
            Assert.IsTrue(FileStorageProviderCompress.IsEmpty);
            Assert.AreEqual(FileStorageProviderCompress.Count, 0);
            listNumber = 0;
        }

        [TestMethod]
        public void IdSourceAndCompressConstructorTestMethod10()
        {
            string id = "1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            listNumber = 10;
            string test = "test";
            FileStorageProvider<string> FileStorageProviderCompress = new FileStorageProvider<string>(id, source, true);

            for (int i = 0; i < listNumber; i++)
            {
                FileStorageProviderCompress.Add(test + i.ToString());
            }

            Assert.AreEqual(FileStorageProviderCompress.Count, listNumber);
            FileStorageProviderCompress.Clear();
            IDataFormatter<string> IDataFormatter = FileStorageProviderCompress.DataFormatter;
            Assert.IsInstanceOfType(IDataFormatter, typeof(BinaryFormatter<string>));
            


            FileStorageProviderCompress.Clear();
            Assert.IsTrue(FileStorageProviderCompress.IsEmpty);
            Assert.AreEqual(FileStorageProviderCompress.Count, 0);
            listNumber = 0;

        }

        #endregion

        #region sorageId and IDateformatter Constructor Test Methods

        [TestMethod]
        public void StorageAndIDataFormatterTestMethod()
        {
            string id = "1";
            BinaryFormatter<string> binaryFormatter = new BinaryFormatter<string>();
            XmlSoapFormatter<string> xmlSoapFormatter = new XmlSoapFormatter<string>();
            FileStorageProvider<string> fileStorageProviderIDataFormatter = new FileStorageProvider<string>(id, xmlSoapFormatter);
            string test = "test";
            listNumber = 10;

            for (int i = 0; i < listNumber; i++)
            {
                fileStorageProviderIDataFormatter.Add(test);
            }

            Thread.Sleep(1000);

            Assert.IsFalse(fileStorageProviderIDataFormatter.CompressContent);
            Assert.IsInstanceOfType(fileStorageProviderIDataFormatter.DataFormatter, typeof(XmlSoapFormatter<string>));

            fileStorageProviderIDataFormatter.Clear();
            Assert.IsTrue(fileStorageProviderIDataFormatter.IsEmpty);
            Assert.AreEqual(fileStorageProviderIDataFormatter.Count, 0);
            listNumber = 0;
        }

        #endregion

        #region SourceId, IDataFormatter, compressContent Counstructor Test Metods

        [TestMethod]
        public void SourceIdIDataFormatterCompressContentCounstructorTestMetod()
        {
            string id = "1";
            BinaryFormatter<string> binaryFormatter = new BinaryFormatter<string>();
            XmlSoapFormatter<string> xmlSoapFormatter = new XmlSoapFormatter<string>();
            FileStorageProvider<string> fileStorageProviderIDataFormattercompressContent = new FileStorageProvider<string>(id, xmlSoapFormatter, true);
            string test = "test";
            listNumber = 10;

            for (int i = 0; i < listNumber; i++)
            {
                fileStorageProviderIDataFormattercompressContent.Add(test);
            }
            Assert.AreEqual(fileStorageProviderIDataFormattercompressContent.Count, listNumber);
            Assert.IsInstanceOfType(fileStorageProviderIDataFormattercompressContent.DataFormatter, typeof(XmlSoapFormatter<string>));
            Assert.IsTrue(fileStorageProviderIDataFormattercompressContent.CompressContent);

            fileStorageProviderIDataFormattercompressContent.Clear();
            Assert.IsTrue(fileStorageProviderIDataFormattercompressContent.IsEmpty);
            Assert.AreEqual(fileStorageProviderIDataFormattercompressContent.Count, 0);
            listNumber = 0;
        }

        #endregion

        #region StorageId, IDataFormatter, Source, and compressContent Constructor Test Methods

        [TestMethod]
        public void StorageIdIDataFormatterSourceAndCompressContentConstructorTestMethod()
        {
            string id = "1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            BinaryFormatter<string> binaryFormatter = new BinaryFormatter<string>();
            FileStorageProvider<string> fileStorageProviderIDataFormatterExtendedParameter = new FileStorageProvider<string>(id, binaryFormatter, source, true);
            string test = "test";
            listNumber = 10;

            for (int i = 0; i < listNumber; i++)
            {
                fileStorageProviderIDataFormatterExtendedParameter.Add(test);
            }
            Thread.Sleep(1500);
            fileStorageProviderIDataFormatterExtendedParameter.Clear();
            Assert.AreEqual(fileStorageProviderIDataFormatterExtendedParameter.Count, 0);
            Assert.IsTrue(fileStorageProviderIDataFormatterExtendedParameter.IsEmpty);
            listNumber = 0;

        }

        [TestMethod]
        public void StorageIdIDataFormatterSourceAndCompressContentConstructorTestMethod2()
        {
            string id = "1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            XmlDataFormatter<string> xmlDataFormatter = new XmlDataFormatter<string>();
            FileStorageProvider<string> fileStorageProviderIDataFormatterExtendedParameter = new FileStorageProvider<string>(id, xmlDataFormatter, source, true);
            string test = "test";
            listNumber = 10;
            for (int i = 0; i < listNumber; i++)
            {
                fileStorageProviderIDataFormatterExtendedParameter.Add(test);
            }
            Thread.Sleep(1000);
            Assert.AreEqual(fileStorageProviderIDataFormatterExtendedParameter.Count, listNumber);
            Assert.IsInstanceOfType(fileStorageProviderIDataFormatterExtendedParameter.DataFormatter, typeof(XmlDataFormatter<string>));
            fileStorageProviderIDataFormatterExtendedParameter.Clear();
            Assert.AreEqual(fileStorageProviderIDataFormatterExtendedParameter.Count, 0);
            Assert.IsTrue(fileStorageProviderIDataFormatterExtendedParameter.IsEmpty);
            listNumber = 0;
        }

        [TestMethod]
        public void StorageIdIDataFormatterSourceAndCompressContentConstructorTestMethod3()
        {
            string id = "1";
            string source = @"C:\Users\GABOR\Downloads\Pictures\FixMap\NewTest\NewMap";
            XmlDataFormatter<string> xmlDataFormatter = new XmlDataFormatter<string>();
            FileStorageProvider<string> fileStorageProviderIDataFormatterExtendedParameter = new FileStorageProvider<string>(id, xmlDataFormatter, source, true);
            string test = "test";
            listNumber = 10;

            for (int i = 0; i < listNumber; i++)
            {
                fileStorageProviderIDataFormatterExtendedParameter.Add(test);
            }

            Assert.IsTrue(fileStorageProviderIDataFormatterExtendedParameter.CompressContent);
            fileStorageProviderIDataFormatterExtendedParameter.Clear();
            Assert.AreEqual(fileStorageProviderIDataFormatterExtendedParameter.Count, 0);
            Assert.IsTrue(fileStorageProviderIDataFormatterExtendedParameter.IsEmpty);
            listNumber = 0;
        }

        #endregion

        #endregion
    }
}
