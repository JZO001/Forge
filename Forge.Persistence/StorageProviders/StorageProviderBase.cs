/* *********************************************************************
 * Date: 18 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using Forge.Collections;
using Forge.Configuration;
using Forge.Formatters;
using Forge.Persistence.Formatters;
using Forge.Persistence.Serialization;
using Forge.Persistence.StorageProviders.Options;
using Forge.Reflection;
using Forge.Shared;

namespace Forge.Persistence.StorageProviders
{

    /// <summary>
    /// Storage provider base implementation
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public abstract class StorageProviderBase<T> : IStorageProvider<T>
    {

        #region Field(s)

        private static readonly HashSet<string> GLOBAL_STORAGE_ID = new HashSet<string>();

        private string mStorageId = null;

        /// <summary>
        /// The default data formatter
        /// </summary>
        protected IDataFormatter<T> mDataFormatter = null;

        private bool mDisposed = false;

        private int mVersion = 0;

        #endregion

        #region Constructor(s)

        /// <summary>Initializes a new instance of the <see cref="StorageProviderBase{T}" /> class.</summary>
        /// <param name="option">The option.</param>
        /// <exception cref="System.Exception"></exception>
        protected StorageProviderBase(StorageProviderBaseOption option)
        {
            if (option == null) ThrowHelper.ThrowArgumentNullException("option");

            SetId(option.Id);

            Type type = option.DataFormatterType;
            if (type == null && !string.IsNullOrWhiteSpace(option.DataFormatter))
            {
                try
                {
                    type = TypeHelper.GetTypeFromString(option.DataFormatter);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to resolve data formatter type: {0}", option.DataFormatter), ex);
                }
            }

            if (type == null)
            {
                mDataFormatter = new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true);
            }
            else
            {
                try
                {
                    mDataFormatter = (IDataFormatter<T>)type.Assembly.CreateInstance(type.AssemblyQualifiedName);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to instantiate formatter: {0}", type.AssemblyQualifiedName), ex);
                }
            }
        }

        /// <summary>Initializes a new instance of the <see cref="StorageProviderBase{T}" /> class.</summary>
        /// <param name="propertyItem">The property item.</param>
        /// <exception cref="Forge.Persistence.StorageProviders.StorageIdentifierAlreadyExistException"></exception>
        /// <exception cref="System.Exception"></exception>
        protected StorageProviderBase(IPropertyItem propertyItem)
        {
            if (propertyItem == null) ThrowHelper.ThrowArgumentNullException("propertyItem");
            if (string.IsNullOrWhiteSpace(propertyItem.Id))
            {
                ThrowHelper.ThrowArgumentException("PropertyItem does not contain an Id value.", "propertyItem");
            }

            SetId(propertyItem.Id);

            string formatter = string.Empty;
            IPropertyItem item = propertyItem.Items == null ? null : propertyItem.Items["DataFormatter"];
            if (item != null)
            {
                formatter = item.Value;
            }
            if (!string.IsNullOrEmpty(formatter))
            {
                Type type = null;
                try
                {
                    type = TypeHelper.GetTypeFromString(formatter);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to resolve data formatter type: {0}", formatter), ex);
                }
                try
                {
                    mDataFormatter = (IDataFormatter<T>)type.Assembly.CreateInstance(type.AssemblyQualifiedName);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to instantiate formatter: {0}", formatter), ex);
                }
            }
            else
            {
                mDataFormatter = new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageProviderBase&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        protected StorageProviderBase(string storageId)
        {
            if (string.IsNullOrEmpty(storageId))
            {
                throw new ArgumentNullException("storageId");
            }

            lock (GLOBAL_STORAGE_ID)
            {
                if (GLOBAL_STORAGE_ID.Contains(storageId))
                {
                    throw new StorageIdentifierAlreadyExistException(string.Format("Storage identifier already exist: {0}", storageId));
                }
                GLOBAL_STORAGE_ID.Add(storageId);
            }
            mStorageId = storageId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageProviderBase&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="configItem">The config item.</param>
        protected StorageProviderBase(string storageId, IPropertyItem configItem)
            : this(storageId)
        {
            string formatter = string.Empty;
            IPropertyItem item = configItem.Items == null ? null : configItem.Items["DataFormatter"];
            if (item != null)
            {
                formatter = item.Value;
            }
            if (!string.IsNullOrEmpty(formatter))
            {
                Type type = null;
                try
                {
                    type = TypeHelper.GetTypeFromString(formatter);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to resolve data formatter type: {0}", formatter), ex);
                }
                try
                {
                    mDataFormatter = (IDataFormatter<T>)type.Assembly.CreateInstance(type.AssemblyQualifiedName);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to instantiate formatter: {0}", formatter), ex);
                }
            }
            else
            {
                mDataFormatter = new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true);
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="StorageProviderBase&lt;T&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~StorageProviderBase()
        {
            Dispose(false);
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets the storage id.
        /// </summary>
        /// <value>
        /// The storage id.
        /// </value>
        public string StorageId
        {
            get { return mStorageId; }
        }

        /// <summary>
        /// Gets the data formatter.
        /// </summary>
        /// <value>
        /// The data formatter.
        /// </value>
        public IDataFormatter<T> DataFormatter
        {
            get { return mDataFormatter; }
        }

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public Type DataType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Adds the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        public abstract void Add(T o);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="o">The o.</param>
        public abstract void AddRange(IEnumerable<T> o);

        /// <summary>
        /// Inserts the specified o.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="o">The o.</param>
        public abstract void Insert(int index, T o);

        /// <summary>
        /// Removes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>True, if the collection modified, otherwise False.</returns>
        public abstract bool Remove(T o);

        /// <summary>
        /// Removes the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public abstract void RemoveAt(int index);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Gets or sets the object at the specified index.
        /// </summary>
        /// <value>
        /// The value
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>Item</returns>
        public abstract T this[int index]
        {
            get;
        }

        /// <summary>
        /// Sizes this instance.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        /// <returns>Number of the items in the collection</returns>
        public abstract int Count { get; }

        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// Enumerator of generic items
        /// </returns>
        public abstract IEnumeratorSpecialized<T> GetEnumerator();

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// Enumerator of generic items
        /// </returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Gets the version.
        /// </summary>
        protected int Version
        {
            get { return mVersion; }
        }

        /// <summary>
        /// Increments the version.
        /// </summary>
        protected void IncVersion()
        {
            DoDisposeCheck();
            if (Int32.MaxValue == mVersion)
            {
                mVersion = 0;
            }
            mVersion++;
        }

        /// <summary>
        /// Does the dispose check.
        /// </summary>
        protected void DoDisposeCheck()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            mDisposed = true;
            if (mStorageId != null)
            {
                lock (GLOBAL_STORAGE_ID)
                {
                    GLOBAL_STORAGE_ID.Remove(mStorageId);
                }
            }
            mDataFormatter = null;
        }

        #endregion

        #region Private method(s)

        private void SetId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) ThrowHelper.ThrowArgumentNullException("id");
            lock (GLOBAL_STORAGE_ID)
            {
                if (GLOBAL_STORAGE_ID.Contains(id))
                {
                    throw new StorageIdentifierAlreadyExistException(string.Format("Storage identifier already exist: {0}", id));
                }
                GLOBAL_STORAGE_ID.Add(id);
            }
            mStorageId = id;
        }

        #endregion

        #region Nested class(es)

        /// <summary>
        /// Iterator implementation for storage providers
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        protected class StorageProviderIterator<TData> : IEnumeratorSpecialized<TData>
        {

            #region Field(s)

            private TData currentElement = default(TData);

            private int endIndex;

            private int index;

            private StorageProviderBase<TData> list;

            private int startIndex;

            private int version;

            private int originalIndex;

            private int originalCount;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="StorageProviderBase&lt;T&gt;.StorageProviderIterator&lt;T&gt;"/> class.
            /// </summary>
            /// <param name="provider">The provider.</param>
            /// <param name="index">The index.</param>
            /// <param name="count">The count.</param>
            public StorageProviderIterator(StorageProviderBase<TData> provider, int index, int count)
            {
                list = provider;
                startIndex = index;
                index = index - 1;
                endIndex = index + count;
                version = list.Version;
                originalIndex = index;
                originalCount = count;
            }

            /// <summary>
            /// Releases unmanaged resources and performs other cleanup operations before the
            /// <see cref="StorageProviderBase&lt;T&gt;.StorageProviderIterator&lt;TData&gt;"/> is reclaimed by garbage collection.
            /// </summary>
            ~StorageProviderIterator()
            {
                Dispose(false);
            }

            #endregion

            #region Protected method(s)

            /// <summary>
            /// Nexts this instance.
            /// </summary>
            protected void Next()
            {
                if (version != list.Version)
                {
                    throw new InvalidOperationException("Collection modified while iterating on it.");
                }

                if (index < endIndex)
                {
                    index++;
                    currentElement = list[index];
                }
                else
                {
                    index = endIndex + 1;
                }
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources
            /// </summary>
            /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
            protected virtual void Dispose(bool disposing)
            {
                currentElement = default(TData);
                list = null;
            }

            #endregion

            #region Public method(s)

            /// <summary>
            /// Removes this instance.
            /// </summary>
            public void Remove()
            {
                if (index < startIndex)
                {
                    throw new InvalidOperationException("No item selected.");
                }
                else if (index <= endIndex)
                {
                    list.RemoveAt(index);
                    index--;
                    endIndex--;
                    version = list.Version;
                }
                else
                {
                    throw new InvalidOperationException("Iterator reached the end of the collection.");
                }
            }

            /// <summary>
            /// Gets the current.
            /// </summary>
            public TData Current
            {
                get { return currentElement; }
            }

            /// <summary>
            /// Gets the current.
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get { return currentElement; }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                bool result = HasNext();

                if (result)
                {
                    Next();
                }

                return result;
            }

            /// <summary>
            /// Determines whether this instance has next.
            /// </summary>
            /// <returns>
            ///   <c>true</c> if this instance has next; otherwise, <c>false</c>.
            /// </returns>
            public bool HasNext()
            {
                return index < endIndex;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                startIndex = originalIndex;
                index = originalIndex - 1;
                endIndex = index + originalCount;
                if (endIndex > list.Count - 1)
                {
                    endIndex = list.Count - 1;
                }
                currentElement = default(TData);
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

        }

        #endregion

    }

}
