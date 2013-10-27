/* *********************************************************************
 * Date: 18 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using Forge.Collections;
using Forge.Configuration.Shared;
using Forge.Persistence.Formatters;
using Forge.Persistence.Serialization;
using Forge.Reflection;
using log4net;

namespace Forge.Persistence.StorageProviders
{

    /// <summary>
    /// File based storage provider
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [Serializable]
    public sealed class FileStorageProvider<T> : StorageProviderBase<T>
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(FileStorageProvider<T>));

        /// <summary>
        /// Default storage folder
        /// </summary>
        public static String DefaultBaseUrl = "FileStorageRoot";

        /// <summary>
        /// Default assembly style
        /// </summary>
        public static FormatterAssemblyStyle DefaultFormatterAssemblyStyle = FormatterAssemblyStyle.Simple;

        /// <summary>
        /// Default type style
        /// </summary>
        public static FormatterTypeStyle DefaultFormatterTypeStyle = FormatterTypeStyle.XsdString;

        private String mBaseUrl = "";

        private String mStoragePath = "";

        private String mTableFile = "Table.bin";

        private ItemAllocationTable mAllocationTable = null;

        private IDataFormatter<ItemAllocationTable> mTableFormatter = new BinarySerializerFormatter<ItemAllocationTable>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true);

        private bool mCompressContent = false;

        private readonly object LOCK_OBJECT_FOR_ID = new object();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        public FileStorageProvider(string storageId)
            : this(storageId, new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true), DefaultBaseUrl, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="compressContent">if set to <c>true</c> [compress content].</param>
        public FileStorageProvider(string storageId, bool compressContent)
            : this(storageId, new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true), DefaultBaseUrl, compressContent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="baseUrl">The base URL.</param>
        public FileStorageProvider(String storageId, String baseUrl)
            : this(storageId, new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true), baseUrl)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="compressContent">if set to <c>true</c> [compress content].</param>
        public FileStorageProvider(String storageId, String baseUrl, bool compressContent)
            : this(storageId, new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true), baseUrl, compressContent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="dataFormatter">The data formatter.</param>
        public FileStorageProvider(String storageId, IDataFormatter<T> dataFormatter)
            : this(storageId, dataFormatter, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="dataFormatter">The data formatter.</param>
        /// <param name="compressContent">if set to <c>true</c> [compress content].</param>
        public FileStorageProvider(String storageId, IDataFormatter<T> dataFormatter, bool compressContent)
            : this(storageId, dataFormatter, DefaultBaseUrl, compressContent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="dataFormatter">The data formatter.</param>
        /// <param name="baseUrl">The base URL.</param>
        public FileStorageProvider(String storageId, IDataFormatter<T> dataFormatter, String baseUrl)
            : this(storageId, dataFormatter, baseUrl, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="dataFormatter">The data formatter.</param>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="compressContent">if set to <c>true</c> [compress content].</param>
        public FileStorageProvider(String storageId, IDataFormatter<T> dataFormatter, String baseUrl, bool compressContent)
            : base(storageId)
        {
            if (dataFormatter == null)
            {
                throw new ArgumentNullException("dataFormatter");
            }
            if (baseUrl == null)
            {
                throw new ArgumentNullException("baseUrl");
            }

            this.mDataFormatter = dataFormatter;
            this.mBaseUrl = baseUrl;
            this.mCompressContent = compressContent;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="configItem">The config item.</param>
        public FileStorageProvider(String storageId, CategoryPropertyItem configItem)
            : base(storageId, configItem)
        {
            String url = String.Empty;
            CategoryPropertyItem item = configItem.PropertyItems == null ? null : configItem.PropertyItems["BaseUrl"];
            if (item != null)
            {
                url = item.EntryValue;
            }
            this.mBaseUrl = url;

            Initialize();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="FileStorageProvider&lt;T&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~FileStorageProvider()
        {
            Dispose(false);
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Adds the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        public override void Add(T o)
        {
            DoDisposeCheck();
            String itemFileName = String.Format("{0}.bin", GetNextFileUid());
            mAllocationTable.FileItemNames.Add(itemFileName);
            SaveObject(itemFileName, o);
            SaveAllocationTable();
            IncVersion();
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="o">The o.</param>
        public override void AddRange(IEnumerable<T> o)
        {
            DoDisposeCheck();
            if (o == null)
            {
                ThrowHelper.ThrowArgumentNullException("o");
            }
            IEnumerator<T> myEnum = o.GetEnumerator();
            while (myEnum.MoveNext())
            {
                String itemFileName = String.Format("{0}.bin", GetNextFileUid());
                mAllocationTable.FileItemNames.Add(itemFileName);
                SaveObject(itemFileName, myEnum.Current);
            }
            SaveAllocationTable();
            IncVersion();
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="o">The o.</param>
        public override void Insert(int index, T o)
        {
            DoDisposeCheck();
            if (index > Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            String itemFileName = String.Format("{0}.bin", GetNextFileUid());
            mAllocationTable.FileItemNames.Insert(index, itemFileName);
            SaveObject(itemFileName, o);
            SaveAllocationTable();
            IncVersion();
        }

        /// <summary>
        /// Removes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        /// True, if the collection modified, otherwise False.
        /// </returns>
        public override bool Remove(T o)
        {
            DoDisposeCheck();
            bool result = false;
            IEnumeratorSpecialized<T> i = GetEnumerator();
            while (i.MoveNext())
            {
                T next = i.Current;
                if (next == null ? o == null : next.Equals(o))
                {
                    i.Remove();
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index</exception>
        public override void RemoveAt(int index)
        {
            DoDisposeCheck();
            if (index > Count - 1)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            String fileName = mAllocationTable.FileItemNames[index];
            mAllocationTable.FileItemNames.RemoveAt(index);
            if (mAllocationTable.FileItemNames.Count == 0)
            {
                mAllocationTable.FileUid = 0;
            }
            SaveAllocationTable();
            RemoveObject(fileName);
            IncVersion();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public override void Clear()
        {
            DoDisposeCheck();
            foreach (FileInfo f in new DirectoryInfo(mStoragePath).GetFiles())
            {
                f.Delete();
            }
            mAllocationTable.FileItemNames.Clear();
            mAllocationTable.FileUid = 0;
            SaveAllocationTable();
            IncVersion();
        }

        /// <summary>
        /// Gets the object at the specified index.
        /// </summary>
        /// <value>
        /// The value
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>Item</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index</exception>
        public override T this[int index]
        {
            get
            {
                DoDisposeCheck();
                if (index > Count - 1)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return LoadObject(mAllocationTable.FileItemNames[index]);
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public override int Count
        {
            get
            {
                DoDisposeCheck();
                return mAllocationTable.FileItemNames.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public override bool IsEmpty
        {
            get
            {
                DoDisposeCheck();
                return mAllocationTable.FileItemNames.Count == 0;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator of generic items</returns>
        public override IEnumeratorSpecialized<T> GetEnumerator()
        {
            DoDisposeCheck();
            return new StorageProviderIterator<T>(this, 0, mAllocationTable.FileItemNames.Count);
        }

        /// <summary>
        /// Gets a value indicating whether [compress content].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [compress content]; otherwise, <c>false</c>.
        /// </value>
        public bool CompressContent { get { return mCompressContent; } }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DirectoryInfo dir = new DirectoryInfo(mStoragePath);
                try
                {
                    foreach (FileInfo f in dir.GetFiles())
                    {
                        try
                        {
                            f.Delete();
                        }
                        catch (Exception ex)
                        {
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format("Failed to delete a persistent file: {0}", f.FullName), ex);
                        }
                    }
                    dir.Delete();
                }
                catch (Exception ex)
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format("Failed to delete a persistent folder: {0}", dir.FullName), ex);
                }
            }
            mAllocationTable = null;
            mTableFormatter = null;
            base.Dispose(disposing);
        }

        #endregion

        #region Private method(s)

        private void Initialize()
        {
            mStoragePath = Path.Combine(mBaseUrl, StorageId);
            mTableFile = Path.Combine(mStoragePath, mTableFile);
            PerformFolderSecurityTest();
            LoadAllocationTable();
        }

        private void CheckAndCreatePath()
        {
            DirectoryInfo dir = new DirectoryInfo(mStoragePath);
            if (!dir.Exists)
            {
                try
                {
                    dir.Create();
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to create path '{0}'.", mStoragePath), ex);
                }
            }
        }

        private void PerformFolderSecurityTest()
        {
            CheckAndCreatePath();
            FileInfo testFile = new FileInfo(Path.Combine(mStoragePath, String.Format("{0}.txt", Guid.NewGuid().ToString())));
            try
            {
                if (testFile.Exists)
                {
                    testFile.Delete();
                }
                testFile.Create().Dispose();
                testFile.Delete();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Security test failed on folder '{0}'. Grant read, write and delete rights to the current user.", mStoragePath), ex);
            }
        }

        private void LoadAllocationTable()
        {
            FileInfo fi = new FileInfo(mTableFile);
            if (fi.Exists)
            {
                try
                {
                    mAllocationTable = SerializationHelper.Read<ItemAllocationTable>(fi, mTableFormatter, false);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to read allocation table. Invalid file.", ex);
                }
            }
            else
            {
                mAllocationTable = new ItemAllocationTable();
            }
        }

        private void SaveAllocationTable()
        {
            try
            {
                SerializationHelper.Write<ItemAllocationTable>(mAllocationTable, new FileInfo(mTableFile), mTableFormatter, false);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to persist allocation table.", ex);
            }
        }

        private T LoadObject(String fileName)
        {
            T result = default(T);

            String itemPath = Path.Combine(mStoragePath, fileName);
            FileInfo file = new FileInfo(itemPath);
            if (file.Length > 0)
            {
                try
                {
                    result = SerializationHelper.Read<T>(file, DataFormatter, CompressContent);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to read object. Invalid file.", ex);
                }
            }
            else if (!file.Exists)
            {
                throw new Exception("Entry file does not exist. Check integrity of your storage device.");
            }

            return result;
        }

        private void SaveObject(String fileName, T o)
        {
            String itemPath = Path.Combine(mStoragePath, fileName);
            FileInfo file = new FileInfo(itemPath);
            if (o == null)
            {
                try
                {
                    file.Create().Dispose();
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to create persistence file '{0}'.", itemPath), ex);
                }
            }
            else
            {
                try
                {
                    SerializationHelper.Write<T>(o, file, DataFormatter, CompressContent);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to persist object.", ex);
                }
            }
        }

        private void RemoveObject(String fileName)
        {
            String itemPath = Path.Combine(mStoragePath, fileName);
            new FileInfo(itemPath).Delete();
        }

        private int GetNextFileUid()
        {
            lock (LOCK_OBJECT_FOR_ID)
            {
                if (Int32.MaxValue == mAllocationTable.FileUid)
                {
                    mAllocationTable.FileUid = 0;
                }
                mAllocationTable.FileUid = mAllocationTable.FileUid + 1;
                return mAllocationTable.FileUid;
            }
        }

        #endregion

    }

}
