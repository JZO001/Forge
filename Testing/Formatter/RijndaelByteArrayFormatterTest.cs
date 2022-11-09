using Forge.Persistence.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Forge.Testing.Formatter
{

    [TestClass]
    public class RijndaelByteArrayFormatterTest
    {

        private TestContext testContextInstance;

        private RijndaelByteArrayFormatter mFormatter = new RijndaelByteArrayFormatter();

        public RijndaelByteArrayFormatterTest()
        {
        }

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

        [TestMethod]
        public void TestMethod()
        {
            string text = "árvíztűrőtükörfúrógépÁRVÍZTŰRŐTÜKÖRFÚRÓGÉP";
            using (MemoryStream sourceStream = new MemoryStream(Encoding.Unicode.GetBytes(text), true))
            {
                using (MemoryStream targetStream = new MemoryStream())
                {
                    mFormatter.Write(targetStream, sourceStream);
                    targetStream.Position = 0;

                    MemoryStream stream = (MemoryStream)mFormatter.Read(targetStream);
                    string decryptedText = Encoding.Unicode.GetString(stream.ToArray());

                    Assert.IsTrue(text == decryptedText);
                }
            }
            //mFormatter.Write()
        }

    }

}
