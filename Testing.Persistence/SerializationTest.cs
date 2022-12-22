using System.Runtime.CompilerServices;
using Forge.Persistence.Serialization;

namespace Testing.Persistence
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

            SerializationHelper.Write(testObj, fi);
            ClassA newObj = SerializationHelper.Read<ClassA>(fi);

            Assert.AreEqual(testObj, newObj);
            Assert.IsFalse(RuntimeHelpers.Equals(testObj, newObj));
        }

    }

}
