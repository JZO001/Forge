/* *********************************************************************
 * Date: 03 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using Forge.Persistence.StorageProviders;

namespace Forge.Persistence.Collections
{

    /// <summary>
    /// Persistent queue
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly"), Serializable, DebuggerDisplay("Count = {Count}")]
    public class PersistentQueue<T> : PersistentCache<T>, IPersistentQueue<T>
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentQueue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="queueId">The queue id.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public PersistentQueue(string queueId) :
            base(queueId, CacheStrategyEnum.CacheForQueue, int.MaxValue)
        {
            SetStorageProvider(new FileStorageProvider<T>(queueId), true);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentQueue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="queueId">The queue id.</param>
        /// <param name="provider">The provider.</param>
        public PersistentQueue(string queueId, IStorageProvider<T> provider) :
            base(queueId, CacheStrategyEnum.CacheForQueue, int.MaxValue)
        {
            SetStorageProvider(provider, false);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentQueue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="queueId">The queue id.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        public PersistentQueue(string queueId, IStorageProvider<T> provider, int cacheSize) :
            base(queueId, CacheStrategyEnum.CacheForQueue, cacheSize)
        {
            SetStorageProvider(provider, false);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentQueue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="queueId">The queue id.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public PersistentQueue(string queueId, int cacheSize) :
            base(queueId, CacheStrategyEnum.CacheForQueue, cacheSize)
        {
            SetStorageProvider(new FileStorageProvider<T>(queueId), true);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentQueue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="queueId">The queue id.</param>
        /// <param name="configurationName">Name of the configuration.</param>
        public PersistentQueue(string queueId, string configurationName) :
            base(queueId, CacheStrategyEnum.CacheForQueue, configurationName)
        {
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Dequeues this instance.
        /// </summary>
        /// <returns>Item</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public T Dequeue()
        {
            DoDisposeCheck();
            if (StorageProvider.IsEmpty)
            {
                throw new InvalidOperationException();
            }
            lock (mLockObject)
            {
                T result = GetItem(0);
                RemoveItem(0);
                return result;
            }
        }

        /// <summary>
        /// Enqueues the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        public void Enqueue(T o)
        {
            DoDisposeCheck();
            AddItem(Count, o);
        }

        /// <summary>
        /// Peeks this instance.
        /// </summary>
        /// <returns>Item</returns>
        /// <exception cref="System.InvalidOperationException">The collection is empty.</exception>
        public T Peek()
        {
            DoDisposeCheck();
            if (StorageProvider.IsEmpty)
            {
                throw new InvalidOperationException("The collection is empty.");
            }
            return GetItem(0);
        }

        #endregion

    }

}
