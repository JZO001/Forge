using System;
using Forge.Collections;
using Forge.Persistence.StorageProviders;
using Forge.Testing.FormatterTest;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing
{

    [TestClass]
    public sealed class StorageProviderTest
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(StorageProviderTest));

        [TestMethod]
        public void MemoryStorageProviderTest()
        {
            Forge.Logging.Log4net.Log4NetManager.InitializeFromAppConfig();
            LOGGER.Info("LOG-INFO");
            LOGGER.Debug("LOG-DEBUG");
            LOGGER.Error("LOG-ERROR");
            LOGGER.Fatal("LOG-FATAL");
            LOGGER.Warn("LOG-WARN");

            using (MemoryStorageProvider<string> ms = new MemoryStorageProvider<string>("VS_TEST_MS"))
            {
                ms.Add("A");
                ms.Add("B");
                ms.Add("C");
                ms.Add("D");
                ms.AddRange(new string[] {"E", "F"});
                Assert.IsTrue(ms.Count == 6);

                IEnumeratorSpecialized<string> myEnum = ms.GetEnumerator();
                while (myEnum.MoveNext())
                {
                    string s = myEnum.Current;
                    myEnum.Remove();
                }

                Assert.IsTrue(ms.Count == 0);
            }

        }

        [TestMethod]
        public void FileStorageProviderTest()
        {
            using (FileStorageProvider<ClassA> fs = new FileStorageProvider<ClassA>("VS_TEST_FS"))
            {
                fs.Add(new ClassA() { MethodA = Guid.NewGuid().ToString(), MethodB = Guid.NewGuid().ToString() });
                fs.Add(new ClassA() { MethodA = Guid.NewGuid().ToString(), MethodB = Guid.NewGuid().ToString() });
                fs.Add(new ClassA() { MethodA = Guid.NewGuid().ToString(), MethodB = Guid.NewGuid().ToString() });
                fs.Add(new ClassA() { MethodA = Guid.NewGuid().ToString(), MethodB = Guid.NewGuid().ToString() });
                fs.Add(null);

                IEnumeratorSpecialized<ClassA> myEnum = fs.GetEnumerator();
                while (myEnum.MoveNext())
                {
                    ClassA s = myEnum.Current;
                    myEnum.Remove();
                }

                Assert.IsTrue(fs.Count == 0);
            }
        }

    }

}
