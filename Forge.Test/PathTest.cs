using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge.IO;

namespace Forge.Test
{

    [TestClass]
    public class PathTest
    {

        [TestMethod]
        public void TestAbsolutePath()
        {
            Assert.IsFalse(PathHelper.IsAbsolutePath("Documents\\Photos"));
            Assert.IsTrue(PathHelper.IsAbsolutePath("C:\\Windows"));
        }

        [TestMethod]
        public void TestCutoff()
        {
            string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string systemFolderWithBackslash = string.Format("{0}\\", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            Assert.IsTrue(systemFolder.Equals(PathHelper.CutoffBackslashFromPathEnd(systemFolder)));
            Assert.IsTrue(systemFolder.Equals(PathHelper.CutoffBackslashFromPathEnd(systemFolderWithBackslash)));
        }

        [TestMethod]
        public void TestEnvironmentPath()
        {
            string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = string.Format("${0}\\test.docx", Environment.SpecialFolder.MyDocuments.ToString());

            string expectedResult = string.Format("{0}\\test.docx", systemFolder);
            Assert.IsTrue(expectedResult.Equals(PathHelper.ResolveEnvironmentSpecialFolder(path)));
        }

    }

}
