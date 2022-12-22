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
    /// Persistent stack
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly"), Serializable, DebuggerDisplay("Count = {Count}")]
    public class PersistentStack<T> : PersistentCache<T>, IPersistentStack<T>
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentStack&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="stackId">The stack id.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public PersistentStack(string stackId) :
            base(stackId, CacheStrategyEnum.CacheForStack, int.MaxValue)
        {
            SetStorageProvider(new FileStorageProvider<T>(stackId), true);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentStack&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="stackId">The stack id.</param>
        /// <param name="provider">The provider.</param>
        public PersistentStack(string stackId, IStorageProvider<T> provider) :
            base(stackId, CacheStrategyEnum.CacheForStack, int.MaxValue)
        {
            SetStorageProvider(provider, false);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentStack&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="stackId">The stack id.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        public PersistentStack(string stackId, IStorageProvider<T> provider, int cacheSize) :
            base(stackId, CacheStrategyEnum.CacheForStack, cacheSize)
        {
            SetStorageProvider(provider, false);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentStack&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="stackId">The stack id.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public PersistentStack(string stackId, int cacheSize) :
            base(stackId, CacheStrategyEnum.CacheForStack, cacheSize)
        {
            SetStorageProvider(new FileStorageProvider<T>(stackId), true);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentStack&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="stackId">The stack id.</param>
        /// <param name="configurationName">Name of the configuration.</param>
        public PersistentStack(string stackId, string configurationName) :
            base(stackId, CacheStrategyEnum.CacheForStack, configurationName)
        {
        }

        #endregion

        #region Public method(s)

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

        /// <summary>
        /// Pops this instance.
        /// </summary>
        /// <returns>Item</returns>
        /// <exception cref="System.InvalidOperationException">The collection is empty.</exception>
        public T Pop()
        {
            DoDisposeCheck();
            if (StorageProvider.IsEmpty)
            {
                throw new InvalidOperationException("The collection is empty.");
            }
            lock (mLockObject)
            {
                T result = GetItem(0);
                RemoveItem(0);
                return result;
            }
        }

        /// <summary>
        /// Pushes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        public void Push(T o)
        {
            DoDisposeCheck();
            AddItem(0, o);
        }

        #endregion

    }

}
