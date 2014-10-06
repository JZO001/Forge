using System;
using System.Collections.Generic;
using Forge.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Test
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

        //[TestMethod]
        //public void TestTypeHelperDiffVersion()
        //{
        //    string typeStr = string.Format("{0}, Iesi.Collections, Version=1.0.0.0, Culture=neutral, PublicKeyToken=aa95f207798dfdb4", typeof(Iesi.Collections.HashedSet).FullName);

        //    Type type = TypeHelper.GetTypeFromString(typeStr, true);

        //    Assert.IsFalse(type == null);
        //}

        [TestMethod]
        public void TestTypeHelperMultiArray()
        {
            Type type = TypeHelper.GetTypeFromString(typeof(Dictionary<int, object[, ,]>).AssemblyQualifiedName, true);

            Assert.IsFalse(type == null);
        }

        //[TestMethod]
        //public void TestTypeHelperArray()
        //{
        //    string typeStr = string.Format("{0}[], Iesi.Collections, Version=1.0.0.0, Culture=neutral, PublicKeyToken=aa95f207798dfdb4", typeof(Iesi.Collections.HashedSet).FullName);

        //    Type type = TypeHelper.GetTypeFromString(typeStr, true);

        //    Assert.IsFalse(type == null);
        //}

        //[TestMethod]
        //public void TestTypeHelperGenericAndArray()
        //{
        //    string typeStr = string.Format("{0}[,], Iesi.Collections, Version=1.0.0.0, Culture=neutral, PublicKeyToken=aa95f207798dfdb4", typeof(Iesi.Collections.Generic.ISet<>).FullName);

        //    Type type = TypeHelper.GetTypeFromString(typeStr, true);

        //    Assert.IsFalse(type == null);
        //}

    }

}
