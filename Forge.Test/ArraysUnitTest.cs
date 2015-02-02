using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
﻿
namespace Forge.Test
{
    [TestClass]
    public class ArraysUnitTest
    {

        public ArraysUnitTest()
        {
            Console.WriteLine(Environment.Version.ToString());
        }

        [TestMethod]
        public void TestEquals()
        {
            int[] a = new int[] { 1, 2, 3, 4 };
            int[] b = new int[] { 1, 2, 3, 4 };

            Assert.IsTrue(Arrays.DeepEquals(a, b));
        }

        [TestMethod]
        public void TestDiffLength()
        {
            int[] a = new int[] { 1, 2, 3, 4 };
            int[] b = new int[] { 1, 2, 3, 4, 5 };

            Assert.IsFalse(Arrays.DeepEquals(a, b));
        }

        [TestMethod]
        public void TestDiffTypes()
        {
            int[] a = new int[] { 1, 2, 3, 4 };
            long[] b = new long[] { 1, 2, 3, 4, 5 };

            Assert.IsFalse(Arrays.DeepEquals(a, b));
        }

        [TestMethod]
        public void TestNulls()
        {
            int?[] a = new int?[] { 1, 2, 3, 4, null };
            int?[] b = new int?[] { 1, 2, 3, 4, null };

            Assert.IsTrue(Arrays.DeepEquals(a, b));
        }

    }
}
