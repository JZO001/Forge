using System.Collections.Generic;
using Forge.Persistence.Collections;
using Forge.Persistence.StorageProviders.HibernateStorageProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing
{

    /// <summary>
    /// Summary description for PersistentCollectionTest
    /// </summary>
    [TestClass]
    public class PersistentCollectionTest
    {

        static PersistentCollectionTest()
        {
            Forge.Logging.Log4net.Log4NetManager.InitializeFromAppConfig();
        }

        public PersistentCollectionTest()
        {
        }

        [TestMethod]
        public void PersistentQueueTest()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("HSP_Queue");
            PersistentQueue<string> queue = new PersistentQueue<string>("HSP_Queue", provider);
            queue.Clear();

            queue.Enqueue("A");
            queue.Enqueue("B");
            queue.Enqueue("C");

            Assert.IsTrue(queue.Count == 3);
            Assert.IsTrue("A".Equals(queue.Dequeue()));
            Assert.IsTrue("B".Equals(queue.Dequeue()));
            Assert.IsTrue("C".Equals(queue.Dequeue()));
            Assert.IsTrue(queue.Count == 0);

            queue.Dispose();
        }

        [TestMethod]
        public void PersistentStackTest()
        {
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("HSP_Stack");
            PersistentStack<string> stack = new PersistentStack<string>("HSP_Stack", provider);
            stack.Clear();

            Assert.IsTrue(stack.Count == 0);

            stack.Push("A");
            stack.Push("B");
            stack.Push("C");

            Assert.IsTrue(stack.Count == 3);
            Assert.IsTrue("C".Equals(stack.Peek()));
            Assert.IsTrue(stack.Count == 3);

            Assert.IsTrue("C".Equals(stack.Pop()));
            Assert.IsTrue("B".Equals(stack.Pop()));
            Assert.IsTrue("A".Equals(stack.Pop()));

            Assert.IsTrue(stack.Count == 0);

            stack.Dispose();
        }

        [TestMethod]
        public void PersistentListTest()
        {
            //FileStorageProvider<string>.DefaultBaseUrl = @"C:\Temp";
            HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("HSP_List");
            PersistentList<string> list = new PersistentList<string>("HSP_List", provider);
            list.Clear();

            Assert.IsTrue(list.Count == 0);

            list.Add("A");
            list.Add("B");
            list.Add("C");

            Assert.IsTrue(list.Count == 3);
            Assert.IsTrue("A".Equals(list[0]));
            Assert.IsTrue("B".Equals(list[1]));
            Assert.IsTrue("C".Equals(list[2]));

            foreach (string s in list)
            {
                Assert.IsTrue(list.Contains(s));
            }

            list.Remove("A");
            list.RemoveAt(0);
            list.Clear();

            Assert.IsTrue(list.Count == 0);

            list.Dispose();
        }

        [TestMethod]
        public void PersistentDictionaryTest()
        {
            HibernateStorageProvider<KeyValuePair<string, int>> provider = new HibernateStorageProvider<KeyValuePair<string, int>>("HSP_Dictionary");
            PersistentDictionary<string, int> dict = new PersistentDictionary<string, int>("HSP_Dictionary", provider);
            dict.Clear();

            Assert.IsTrue(dict.Count == 0);

            dict.Add("A", 0);
            dict.Add("B", 1);
            dict.Add("C", 2);
            dict.Add("D", 2);

            Assert.IsTrue(dict.Count == 4);
            Assert.IsTrue(dict["D"] == 2);

            dict["D"] = 3;

            Assert.IsTrue(dict["D"] == 3);

            foreach (KeyValuePair<string, int> kv in dict)
            {
                Assert.IsTrue(dict.Contains(kv));
                Assert.IsTrue(dict.ContainsKey(kv.Key));
            }

            dict.Remove("A");
            Assert.IsTrue(dict.Count == 3);

            dict.Clear();
            Assert.IsTrue(dict.Count == 0);

            dict.Dispose();
        }

    }

}
