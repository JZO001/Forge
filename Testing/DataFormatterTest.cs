using System.IO;
using System.Runtime.CompilerServices;
using Forge.Persistence.Formatters;
using Forge.Testing.FormatterTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing
{

    [TestClass]
    public class DataFormatterTest
    {

        [TestMethod]
        public void BinaryFormatterTest()
        {
            ClassA testObj = new ClassA();
            testObj.MethodA = "A";
            testObj.MethodB = "B";

            BinaryFormatter<ClassA> bf = new BinaryFormatter<ClassA>();
            Assert.IsTrue(bf.CanWrite(testObj));
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Write(ms, testObj);
                ms.Position = 0;
                Assert.IsTrue(bf.CanRead(ms));
                ClassA newObj = bf.Read(ms);
                Assert.AreEqual<ClassA>(testObj, newObj);
                Assert.IsFalse(RuntimeHelpers.Equals(testObj, newObj));
            }
        }

        [TestMethod]
        public void GZipFormatterTest()
        {
            ClassA testObj = new ClassA();
            testObj.MethodA = "A";
            testObj.MethodB = "B";

            BinaryFormatter<ClassA> bf = new BinaryFormatter<ClassA>();
            Assert.IsTrue(bf.CanWrite(testObj));
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Write(ms, testObj);
                ms.Position = 0;
                GZipFormatter gzip = new GZipFormatter();
                using (MemoryStream gzipStream = new MemoryStream())
                {
                    gzip.Write(gzipStream, ms.ToArray());
                    gzipStream.Position = 0;
                    Assert.IsTrue(gzip.CanRead(gzipStream));
                    ms.SetLength(0);
                    byte[] unzipped = gzip.Read(gzipStream);
                    ms.Write(unzipped, 0, unzipped.Length);
                    ms.Position = 0;

                    ClassA newObj = bf.Read(ms);
                    Assert.AreEqual<ClassA>(testObj, newObj);
                    Assert.IsFalse(RuntimeHelpers.Equals(testObj, newObj));
                }
            }
        }

        [TestMethod]
        public void XmlDataFormatterTest()
        {
            ClassA testObj = new ClassA();
            testObj.MethodA = "A";
            testObj.MethodB = "B";

            XmlDataFormatter<ClassA> bf = new XmlDataFormatter<ClassA>();
            Assert.IsTrue(bf.CanWrite(testObj));
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Write(ms, testObj);
                ms.Position = 0;
                Assert.IsTrue(bf.CanRead(ms));
                ClassA newObj = bf.Read(ms);
                Assert.AreNotEqual<ClassA>(testObj, newObj);
                Assert.IsFalse(RuntimeHelpers.Equals(testObj, newObj));
            }
        }

        [TestMethod]
        public void XmlSoapFormatterTest()
        {
            ClassA testObj = new ClassA();
            testObj.MethodA = "A";
            testObj.MethodB = "B";

            XmlSoapFormatter<ClassA> bf = new XmlSoapFormatter<ClassA>();
            Assert.IsTrue(bf.CanWrite(testObj));
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Write(ms, testObj);
                ms.Position = 0;
                Assert.IsTrue(bf.CanRead(ms));
                ClassA newObj = bf.Read(ms);
                Assert.AreEqual<ClassA>(testObj, newObj);
                Assert.IsFalse(RuntimeHelpers.Equals(testObj, newObj));
            }
        }

    }

}
