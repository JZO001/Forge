using System.IO;
using System.Runtime.CompilerServices;
using Forge.Persistence.Serialization;
using Forge.Testing.FormatterTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing
{

    [TestClass]
    public class SerializationTest
    {

        [TestMethod]
        public void SerializationFileInfoTest()
        {
            FileInfo fi = new FileInfo("serializationFi.bin");

            ClassA testObj = new ClassA();
            testObj.MethodA = "A";
            testObj.MethodB = "B";

            SerializationHelper.Write<ClassA>(testObj, fi);
            ClassA newObj = SerializationHelper.Read<ClassA>(fi);

            Assert.AreEqual<ClassA>(testObj, newObj);
            Assert.IsFalse(RuntimeHelpers.Equals(testObj, newObj));
        }

    }

}
