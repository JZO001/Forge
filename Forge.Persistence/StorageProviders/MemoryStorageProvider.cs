/* *********************************************************************
 * Date: 18 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using Forge.Collections;

namespace Forge.Persistence.StorageProviders
{

    /// <summary>
    /// Memory based provider
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [Serializable]
    public sealed class MemoryStorageProvider<T> : StorageProviderBase<T>
    {

        #region Field(s)

        private readonly ListSpecialized<T> mInnerList = new ListSpecialized<T>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        public MemoryStorageProvider()
            : base(Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        public MemoryStorageProvider(String storageId)
            : base(storageId)
        {
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="MemoryStorageProvider&lt;T&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~MemoryStorageProvider()
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
            mInnerList.Add(o);
            IncVersion();
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="o">The o.</param>
        public override void AddRange(IEnumerable<T> o)
        {
            DoDisposeCheck();
            mInnerList.AddRange(o);
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
            if (index > mInnerList.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            mInnerList.Insert(index, o);
            IncVersion();
        }

        /// <summary>
        /// Removes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>True, if the collection modified, otherwise False.</returns>
        public override bool Remove(T o)
        {
            DoDisposeCheck();
            if (((ICollection<T>)mInnerList).Remove(o))
            {
                IncVersion();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index</exception>
        public override void RemoveAt(int index)
        {
            DoDisposeCheck();
            if (index > mInnerList.Count - 1)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            mInnerList.RemoveAt(index);
            IncVersion();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public override void Clear()
        {
            DoDisposeCheck();
            mInnerList.Clear();
            IncVersion();
        }

        /// <summary>
        /// Gets the object at the specified index.
        /// </summary>
        /// <value>
        /// The value
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>
        /// Item
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index</exception>
        public override T this[int index]
        {
            get
            {
                DoDisposeCheck();
                if (index > mInnerList.Count - 1)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return mInnerList[index];
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
                return mInnerList.Count;
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
                return mInnerList.Count == 0;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator of generic items</returns>
        public override IEnumeratorSpecialized<T> GetEnumerator()
        {
            DoDisposeCheck();
            return (IEnumeratorSpecialized<T>)mInnerList.GetEnumerator();
        }

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
                mInnerList.Clear();
            }
            base.Dispose(disposing);
        }

        #endregion

    }

}
