using Forge.Reflection;

namespace Testing.Reflection
{

    [TestClass]
    public class TypeHelperTest
    {

        [TestMethod]
        public void TestTypeHelper()
        {
            Type type = null;

            type = TypeHelper.GetTypeFromString("System.Int32", true);
            Assert.IsFalse(type == null);

            type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]]", true);
            Assert.IsFalse(type == null);

            type = TypeHelper.GetTypeFromString("System.Collections.Generic.Dictionary`2[[System.Int32],[System.Int64]][,]", true);
            Assert.IsFalse(type == null);

            type = TypeHelper.GetTypeFromString(typeof(Dictionary<int, Dictionary<int, string>>).AssemblyQualifiedName);
            Assert.IsFalse(type == null);

            type = TypeHelper.GetTypeFromString(typeof(Dictionary<int, Dictionary<int, string>>[]).AssemblyQualifiedName);
            Assert.IsFalse(type == null);

            type = TypeHelper.GetTypeFromString(typeof(HashSet<int?>[][,]).AssemblyQualifiedName, true);
            Assert.IsFalse(type == null);

            type = TypeHelper.GetTypeFromString(typeof(HashSet<int?>[][,]).AssemblyQualifiedName, TypeLookupModeEnum.AllowAll, true, false, false);
            Assert.IsFalse(type == null);
        }

        [TestMethod]
        public void TestTypeHelperMultiArray()
        {
            Type type = TypeHelper.GetTypeFromString(typeof(Dictionary<int, object[,,]>).AssemblyQualifiedName, true);

            Assert.IsFalse(type == null);
        }

    }

}