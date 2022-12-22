using System;
using Forge.Persistence.Collections;
using Forge.Persistence.StorageProviders.HibernateStorageProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.PersistenceCollectionsText
{
	/// <summary>
	/// Summary description for PersistentQueueUnitTest
	/// </summary>
	[TestClass]
	public class PersistentQueueUnitTest
	{
		#region Fields

		private TestContext testContextInstance;

		int number = 0;

		#endregion

		#region Constructor
		
		public PersistentQueueUnitTest()
		{
		}

		#endregion

		#region Properties

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

		#endregion

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		#region Test Methods

		#region queneId, provider, and cacheSize constructor test methods        

		[TestMethod]
		public void EnqueueTestMethod()
		{
			number = 10;
			HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("QueueTest_Id");
			PersistentQueue<string> queue = new PersistentQueue<string>("QueueTest_Id", provider, number);
			queue.Clear();
			for (int i = 0; i < number; i++)
			{
				queue.Enqueue("test" + i.ToString());
			}
			Assert.AreEqual(queue.Count, number);
			Assert.AreEqual(queue.Peek(), "test0");
			queue.Enqueue("New_Item");
			for (int i = 0; i < number; i++)
			{
				queue.Dequeue();
			}
			Assert.AreEqual(queue.Peek(), "New_Item");
			number = 0;
			queue.Dispose();
		}

		[TestMethod]
		public void EnqueueTestMethod2()
		{
			number = 10;
			HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("QueueTest_Id");
			PersistentQueue<string> queue = new PersistentQueue<string>("QueueTest_Id", provider, number);
			queue.Clear();
			for (int i = 0; i < number; i++)
			{
				queue.Enqueue("test" + i.ToString());
			}
			Assert.AreEqual(queue.Count, number);
			Assert.AreEqual(queue.Peek(), "test0");
			for (int i = 0; i < number; i++)
			{
				queue.Enqueue("Item" +i.ToString());
			}
			Assert.AreEqual(queue.Count,number*2);
			Assert.AreEqual(queue.Peek(), "test0");
			for (int i = 0; i < number; i++)
			{
				queue.Dequeue();
			}

			Assert.AreEqual(queue.Peek(), "Item0");

			queue.Dequeue();
			Assert.AreEqual(queue.Peek(), "Item1");

			for (int i = 0; i < number-1; i++)
			{
				queue.Dequeue();
			}

			Assert.IsTrue(queue.IsEmpty);
			Assert.AreEqual(queue.Count,0);

			number = 0;
			queue.Dispose();
		}

		[TestMethod]
		public void DequeueTestMethod()
		{
			number = 10;
			HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("QueueTest2_Id");
			PersistentQueue<string> queue = new PersistentQueue<string>("QueueTest2_Id", provider, number);
			queue.Clear();

			for (int i = 0; i < number; i++)
			{
				queue.Enqueue("test" + i.ToString());
			}
			Assert.AreEqual(queue.Count, number);
			Assert.AreEqual(queue.Peek(), "test0");

			queue.Dequeue();
			Assert.AreEqual(queue.Peek(), "test1");

			for (int i = 0; i < number-3; i++)
			{
				queue.Dequeue();
			}
			Assert.AreEqual(queue.Peek(), "test8");
			queue.Dequeue();
			queue.Dequeue();
			Assert.AreEqual(queue.Count, 0);
			Assert.IsTrue(queue.IsEmpty);

			number = 0;
			queue.Dispose();
		}

		/// <summary>
		/// több elemet szeretnék törölni a listából, mint amennyi benne van 
		/// </summary>
		[TestMethod]
		public void DequeueTestMethod2()
		{
			number = 10;
			HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("QueueTest3_Id");
			PersistentQueue<string> queue = new PersistentQueue<string>("QueueTest3_Id", provider, number);
			queue.Clear();
			bool result = false;
			queue.Enqueue("test_Item");
			Assert.AreEqual(queue.Count, 1);
			try
			{
				for (int i = 0; i < number; i++)
				{
					queue.Dequeue();
				}
			}
			catch (Exception)
			{
				result = true;
			}
			Assert.IsTrue(result);
			Assert.AreEqual(queue.Count, 0);
			Assert.IsTrue(queue.IsEmpty);
			number = 0;
			queue.Dispose();
		}

		[TestMethod]
		public void DequeueTestMethod3()
		{
			number = 10;
			HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("EnqueueQueueTest_Id");
			PersistentQueue<string> queue = new PersistentQueue<string>("EnqueueQueueTest_Id", provider, number);
			queue.Clear();

			for (int i = 0; i < number * 2; i++)
			{
				queue.Enqueue("Item" + i.ToString());
			}
			Assert.AreEqual(queue.Count, number * 2);

			queue.Dequeue();
			queue.Dequeue();
			Assert.AreEqual(queue.Peek(), "Item2");
			
			for (int i = 0; i < number + 7; i++)
			{
				queue.Dequeue();
			}
			Assert.AreEqual(queue.Peek(), "Item19");

			number = 0;
			queue.Dispose();
		}

		[TestMethod]
		public void PeekTestMethod()
		{
			number = 10;
			HibernateStorageProvider<string> provider = new HibernateStorageProvider<string>("QueueTest4_Id");
			PersistentQueue<string> queue = new PersistentQueue<string>("QueueTest4_Id", provider, number);
			queue.Clear();
			bool result = false;
			try
			{
				queue.Peek();
			}
			catch (Exception)
			{
				result = true;
			}
			Assert.IsTrue(result);
			number = 0;
			queue.Dispose();
		}

		#endregion

		#region queueId, and configurationName Constructor Test Method

		[TestMethod]
		public void queueIdAndconfigurationNameTestMethod()
		{
			PersistentQueue<string> queue = new PersistentQueue<string>("queueIdAndconfigurationNameTestMethod", "configurationName");
			Assert.AreEqual(queue.Id, "queueIdAndconfigurationNameTestMethod");
		}

		#endregion

		#endregion
	}
}
