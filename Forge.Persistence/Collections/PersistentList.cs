/* *********************************************************************
 * Date: 03 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.Collections;
using Forge;
using Forge.Persistence.StorageProviders;

namespace Forge.Persistence.Collections
{

    /// <summary>
    /// Persistent list implementation
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly"), Serializable, DebuggerDisplay("Count = {Count}")]
    public class PersistentList<T> : PersistentCache<T>, IPersistentList<T>
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="listId">The list id.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public PersistentList(String listId)
            : base(listId, CacheStrategyEnum.RecentlyUsed, Int32.MaxValue)
        {
            SetStorageProvider(new FileStorageProvider<T>(listId), true);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="listId">The list id.</param>
        /// <param name="provider">The provider.</param>
        public PersistentList(String listId, IStorageProvider<T> provider)
            : base(listId, CacheStrategyEnum.RecentlyUsed, Int32.MaxValue)
        {
            SetStorageProvider(provider, false);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="listId">The list id.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        public PersistentList(String listId, IStorageProvider<T> provider, int cacheSize)
            : base(listId, CacheStrategyEnum.RecentlyUsed, cacheSize)
        {
            SetStorageProvider(provider, false);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="listId">The list id.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public PersistentList(String listId, int cacheSize)
            : base(listId, CacheStrategyEnum.RecentlyUsed, cacheSize)
        {
            SetStorageProvider(new FileStorageProvider<T>(listId), true);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="listId">The list id.</param>
        /// <param name="configurationName">Name of the configuration.</param>
        public PersistentList(String listId, String configurationName)
            : base(listId, CacheStrategyEnum.RecentlyUsed, configurationName)
        {
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Subs the list.
        /// </summary>
        /// <param name="fromIndex">From index.</param>
        /// <param name="toIndex">To index.</param>
        /// <returns>Sub list of the items</returns>
        public ISubList<T> SubList(int fromIndex, int toIndex)
        {
            return new SubListImpl<T>(this, fromIndex, toIndex);
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public new int Version
        {
            get { return base.Version; }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            DoDisposeCheck();
            AddItem(item);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        public void CopyTo(T[] array)
        {
            this.CopyTo(array, 0);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(this.ToArray(), 0, array, arrayIndex, this.Count);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// <param name="count">The count.</param>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if ((this.Count - index) < count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("index or count");
            }
            Array.Copy(this.ToArray(), index, array, arrayIndex, count);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True, if the collection modified, otherwise False.</returns>
        public bool Remove(T item)
        {
            DoDisposeCheck();
            return RemoveItem(item);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The index of the item, if not found, returns -1.</returns>
        public int IndexOf(T item)
        {
            DoDisposeCheck();
            return GetIndexOfItem(item);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public void Insert(int index, T item)
        {
            DoDisposeCheck();
            AddItem(index, item);
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            DoDisposeCheck();
            RemoveItem(index);
        }

        /// <summary>
        /// Gets or sets the object at the specified index.
        /// </summary>
        /// <value>
        /// The value
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>Item</returns>
        public T this[int index]
        {
            get
            {
                DoDisposeCheck();
                return GetItem(index);
            }
            set
            {
                DoDisposeCheck();
                lock (mLockObject)
                {
                    AddItem(index, value);
                    index++;
                    if (Count > index)
                    {
                        RemoveItem(index);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="c">The collection of items.</param>
        public void AddRange(IEnumerable<T> c)
        {
            DoDisposeCheck();
            if (c == null)
            {
                ThrowHelper.ThrowArgumentNullException("c");
            }

            lock (mLockObject)
            {
                IEnumerator<T> iterator = c.GetEnumerator();
                while (iterator.MoveNext())
                {
                    AddItem(iterator.Current);
                }
            }
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="c">The collection of items.</param>
        public void RemoveAll(ICollection<T> c)
        {
            DoDisposeCheck();
            if (c == null)
            {
                ThrowHelper.ThrowArgumentNullException("c");
            }

            lock (mLockObject)
            {
                IEnumerator<T> iterator = c.GetEnumerator();
                while (iterator.MoveNext())
                {
                    RemoveItem(iterator.Current);
                }
            }
        }

        /// <summary>
        /// Retains all.
        /// </summary>
        /// <param name="c">The collection of items.</param>
        public void RetainAll(ICollection<T> c)
        {
            DoDisposeCheck();
            if (c == null)
            {
                ThrowHelper.ThrowArgumentNullException("c");
            }

            lock (mLockObject)
            {
                IEnumeratorSpecialized<T> iterator = (IEnumeratorSpecialized<T>)this.GetEnumerator();
                while (iterator.MoveNext())
                {
                    if (!c.Contains(iterator.Current))
                    {
                        iterator.Remove();
                    }
                }
            }
        }

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="o">The item</param>
        /// <returns>The index of the last occurence of the item. If not found, returns -1.</returns>
        public int LastIndexOf(T o)
        {
            DoDisposeCheck();
            int result = -1;
            lock (mLockObject)
            {
                if (StorageProvider.Count > 0)
                {
                    T item = default(T);
                    for (int i = StorageProvider.Count - 1; i >= 0; i--)
                    {
                        item = StorageProvider[i];
                        if (o == null)
                        {
                            if (item == null)
                            {
                                result = i;
                                break;
                            }
                        }
                        else if (o.Equals(item))
                        {
                            result = i;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Determines whether the specified c contains all.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>
        ///   <c>true</c> if the specified c contains all; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsAll(ICollection<T> c)
        {
            DoDisposeCheck();
            if (c == null)
            {
                throw new ArgumentNullException("c");
            }

            bool result = true;
            lock (mLockObject)
            {
                IEnumerator<T> iterator = c.GetEnumerator();
                while (iterator.MoveNext())
                {
                    result = Contains(iterator.Current);
                    if (!result)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Inserts the range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="c">The c.</param>
        public void InsertRange(int index, ICollection<T> c)
        {
            DoDisposeCheck();
            if (c == null)
            {
                ThrowHelper.ThrowArgumentNullException("c");
            }

            lock (mLockObject)
            {
                IEnumerator<T> iterator = c.GetEnumerator();
                while (iterator.MoveNext())
                {
                    AddItem(index, iterator.Current);
                    index++;
                }
            }
        }

        #endregion

    }

}
