using Forge.Persistence.StorageProviders.HibernateStorageProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing
{

    [TestClass]
    public class HSPTest
    {

        public HSPTest()
        {
            Forge.Logging.Log4net.Log4NetManager.InitializeFromAppConfig();
        }

        [TestMethod]
        public void TestMethod1()
        {
            //HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("TESTING");
            //provider.Add("A");
            //provider.Add("B");
            //provider.AddRange(new string[] { "C", "D", "E" });
            
            //Assert.IsTrue(provider.Count == 5);

            //provider.Remove("A");
            //provider.RemoveAt(2);
            
            //IEnumeratorSpecialized<string> e = provider.GetEnumerator();
            //while (e.MoveNext())
            //{
            //    string data = e.Current;
            //}

            //provider.Clear();

            
            HibernateStorageProvider<byte[]> providerBlob = new HibernateStorageProvider<byte[]>("TESTING2");

            byte[] blob = new byte[10000000];
            providerBlob.Add(blob);

            providerBlob.Clear();

        }

    }

}
