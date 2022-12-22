using Forge.Collections;
using Forge.Persistence.StorageProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.StorageProvidersFormettersTest
{
	/// <summary>
	/// Summary description for MemoryStorageProviderUnitTest
	/// </summary>
	[TestClass]
	public class MemoryStorageProviderUnitTest
	{
		#region Fields

		private TestContext testContextInstance;

		public static string id = "1";

		public static MemoryStorageProvider<string> memoryStorageProvider = new MemoryStorageProvider<string>(id);

		int listNumber = 0;

		#endregion

		#region Constructor
		
		public MemoryStorageProviderUnitTest()
		{
		}

		#endregion

		#region Propertyes
		
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

		#region Add Method

		[TestMethod]
		public void AddTestMethod()
		{
			string test ="test";
			listNumber = 10;

			for (int i = 0; i < listNumber; i++)
			{
				memoryStorageProvider.Add(test);
			}

			Assert.AreEqual(memoryStorageProvider.Count, listNumber);
			memoryStorageProvider.Clear();
			listNumber = 0;
		}

		#endregion

		#region AddRage Test method

		[TestMethod]
		public void AddRageTestMethod()
		{
			listNumber = 10;
			ListSpecialized<string> listSpecialized = new ListSpecialized<string>();

			for (int i = 0; i < listNumber; i++)
			{
				listSpecialized.Add("Test2000" + i.ToString());
			}

			for (int i = 0; i < listNumber; i++)
			{
				memoryStorageProvider.AddRange(listSpecialized);
			}
			Assert.AreEqual(memoryStorageProvider.Count, listNumber * listNumber);



			listNumber = 0;
			memoryStorageProvider.Clear();
			listSpecialized.Clear();
			Assert.AreEqual(memoryStorageProvider.Count, 0);
			Assert.IsTrue(memoryStorageProvider.IsEmpty);
		}

		#endregion

		#region Remove Test Method 

		[TestMethod]
		public void RemoveTestMethod()
		{
			string test = "test";
			listNumber = 10;

			for (int i = 0; i < listNumber; i++)
			{
				memoryStorageProvider.Add(test+i.ToString());
			}
			Assert.AreEqual(memoryStorageProvider.Count, listNumber);

			memoryStorageProvider.Remove("test2");
			Assert.AreEqual(memoryStorageProvider.Count, listNumber-1);
			Assert.AreEqual(memoryStorageProvider[3], "test4");

			listNumber = 0;
			memoryStorageProvider.Clear();
			Assert.AreEqual(memoryStorageProvider.Count, 0);
			Assert.IsTrue(memoryStorageProvider.IsEmpty);
		}

		#endregion

		#region RemoveAt Test Method

		[TestMethod]
		public void RemoveAtTestMethod()
		{
			string test = "test";
			listNumber = 10;

			for (int i = 0; i < listNumber; i++)
			{
				memoryStorageProvider.Add(test + i.ToString());
			}
			Assert.AreEqual(memoryStorageProvider.Count, listNumber);
			
			
			memoryStorageProvider.RemoveAt(2);
			Assert.AreEqual(memoryStorageProvider.Count, listNumber-1);
			Assert.AreEqual(memoryStorageProvider[3], "test4");

			listNumber = 0;
			memoryStorageProvider.Clear();
			Assert.AreEqual(memoryStorageProvider.Count, 0);
			Assert.IsTrue(memoryStorageProvider.IsEmpty);
		}

		[TestMethod]
		public void RemoveAtTestMethod2()
		{
			listNumber = 10;
			ListSpecialized<string> listSpecialized = new ListSpecialized<string>();

			for (int i = 0; i < listNumber; i++)
			{
				listSpecialized.Add("Test2000" + i.ToString());
			}

			for (int i = 0; i < listNumber; i++)
			{
				memoryStorageProvider.AddRange(listSpecialized);
			}
			Assert.AreEqual(memoryStorageProvider.Count, listNumber * listNumber);

			int memoryStorageProviderCount = memoryStorageProvider.Count;
			memoryStorageProvider.RemoveAt(2);

			Assert.AreEqual(memoryStorageProvider.Count+1, memoryStorageProviderCount);
			Assert.AreEqual(memoryStorageProvider[3], "Test20004");


			listNumber = 0;
			memoryStorageProvider.Clear();
			listSpecialized.Clear();
			Assert.AreEqual(memoryStorageProvider.Count, 0);
			Assert.IsTrue(memoryStorageProvider.IsEmpty);

		}

		#endregion

		#region Insert Test Method

		[TestMethod]
		public void InsertTestMethod()
		{
			string test = "test";
			listNumber = 10;

			for (int i = 0; i < listNumber; i++)
			{
				memoryStorageProvider.Add(test + i.ToString());
			}
			Assert.AreEqual(memoryStorageProvider.Count, listNumber);


			memoryStorageProvider.Insert(2, "InsertTest");

			Assert.AreEqual(memoryStorageProvider.Count, listNumber +1);
			Assert.AreEqual(memoryStorageProvider[2], "InsertTest");
			Assert.AreEqual(memoryStorageProvider[3], "test2");

			listNumber = 0;
			memoryStorageProvider.Clear();
			Assert.AreEqual(memoryStorageProvider.Count, 0);
			Assert.IsTrue(memoryStorageProvider.IsEmpty);
		}

		#endregion

		#region Clear Test Method

		[TestMethod]
		public void ClearTestMethod()
		{
			string test = "test";
			listNumber = 10;

			for (int i = 0; i < listNumber; i++)
			{
				memoryStorageProvider.Add(test + i.ToString());
			}
			Assert.AreEqual(memoryStorageProvider.Count, listNumber);

			listNumber = 0;
			memoryStorageProvider.Clear();
			Assert.AreEqual(memoryStorageProvider.Count, 0);
			Assert.IsTrue(memoryStorageProvider.IsEmpty);
		}

		#endregion

		#region Count Test Method

		[TestMethod]
		public void CountTestMethod()
		{
			string test = "test";
			listNumber = 10;

			for (int i = 0; i < listNumber; i++)
			{
				memoryStorageProvider.Add(test + i.ToString());
			}
			Assert.AreEqual(memoryStorageProvider.Count, listNumber);

			listNumber = 0;
			memoryStorageProvider.Clear();
			Assert.AreEqual(memoryStorageProvider.Count, 0);
			Assert.IsTrue(memoryStorageProvider.IsEmpty);
		}

		#endregion

		#region IsEmpty Test Method 

		[TestMethod]
		public void IsEmptyTestMethod()
		{
			string test = "test";
			listNumber = 10;

			for (int i = 0; i < listNumber; i++)
			{
				memoryStorageProvider.Add(test + i.ToString());
			}
			Assert.AreEqual(memoryStorageProvider.Count, listNumber);

			listNumber = 0;
			memoryStorageProvider.Clear();
			Assert.AreEqual(memoryStorageProvider.Count, 0);
			Assert.IsTrue(memoryStorageProvider.IsEmpty);
		}

		#endregion

		#endregion
	}
}
