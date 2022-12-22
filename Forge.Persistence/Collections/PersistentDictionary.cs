/* *********************************************************************
 * Date: 03 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.Collections;
using Forge.Persistence.StorageProviders;
using Forge.Shared;

namespace Forge.Persistence.Collections
{

    /// <summary>
    /// Persistent dictionary
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly"), Serializable, DebuggerDisplay("Count = {Count}")]
    public class PersistentDictionary<TKey, TValue> : PersistentCache<KeyValuePair<TKey, TValue>>, IPersistentDictionary<TKey, TValue>
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="mapId">The map id.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public PersistentDictionary(string mapId) :
            base(mapId, CacheStrategyEnum.RecentlyUsed, int.MaxValue)
        {
            SetStorageProvider(new FileStorageProvider<KeyValuePair<TKey, TValue>>(mapId), true);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="mapId">The map id.</param>
        /// <param name="provider">The provider.</param>
        public PersistentDictionary(string mapId, IStorageProvider<KeyValuePair<TKey, TValue>> provider) :
            base(mapId, CacheStrategyEnum.RecentlyUsed, int.MaxValue)
        {
            SetStorageProvider(provider, false);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="mapId">The map id.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        public PersistentDictionary(string mapId, IStorageProvider<KeyValuePair<TKey, TValue>> provider, int cacheSize) :
            base(mapId, CacheStrategyEnum.RecentlyUsed, cacheSize)
        {
            SetStorageProvider(provider, false);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="mapId">The map id.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public PersistentDictionary(string mapId, int cacheSize) :
            base(mapId, CacheStrategyEnum.RecentlyUsed, cacheSize)
        {
            SetStorageProvider(new FileStorageProvider<KeyValuePair<TKey, TValue>>(mapId), true);
            FillCache();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="mapId">The map id.</param>
        /// <param name="configurationName">Name of the configuration.</param>
        public PersistentDictionary(string mapId, string configurationName) :
            base(mapId, CacheStrategyEnum.RecentlyUsed, configurationName)
        {
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            if (ReferenceEquals(key, null))
            {
                ThrowHelper.ThrowArgumentNullException("key");
            }
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            DoDisposeCheck();
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException("key");
            }

            bool result = false;

            lock (mLockObject)
            {
                for (int i = 0; i < Count; i++)
                {
                    KeyValuePair<TKey, TValue> item = GetItem(i);
                    if (key.Equals(item.Key))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>
        /// The keys.
        /// </value>
        public IListSpecialized<TKey> Keys
        {
            get
            {
                DoDisposeCheck();

                ListSpecialized<TKey> resultSet = new ListSpecialized<TKey>();

                lock (mLockObject)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        TKey key = GetItem(i).Key;
                        if (!resultSet.Contains(key))
                        {
                            resultSet.Add(key);
                        }
                    }
                }

                return resultSet;
            }
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True, if the collection modified, otherwise False.</returns>
        public bool Remove(TKey key)
        {
            DoDisposeCheck();
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException("key");
            }

            bool result = false;

            lock (mLockObject)
            {
                for (int i = 0; i < Count; i++)
                {
                    KeyValuePair<TKey, TValue> item = GetItem(i);
                    if (key.Equals(item.Key))
                    {
                        RemoveItem(item);
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>True, if the key found, otherwise False.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            DoDisposeCheck();

            if (ReferenceEquals(key, null))
            {
                ThrowHelper.ThrowArgumentNullException("key");
            }

            bool result = false;
            value = default(TValue);

            lock (mLockObject)
            {
                for (int i = 0; i < Count; i++)
                {
                    KeyValuePair<TKey, TValue> item = GetItem(i);
                    if (key.Equals(item.Key))
                    {
                        value = item.Value;
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IListSpecialized<TValue> Values
        {
            get
            {
                DoDisposeCheck();

                ListSpecialized<TValue> resultList = new ListSpecialized<TValue>();

                lock (mLockObject)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        resultList.Add(GetItem(i).Value);
                    }
                }

                return resultList;
            }
        }

        /// <summary>
        /// Gets or sets the object with the specified key.
        /// </summary>
        /// <value>
        /// The value
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The value</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public TValue this[TKey key]
        {
            get
            {
                DoDisposeCheck();
                if (ReferenceEquals(key, null))
                {
                    ThrowHelper.ThrowArgumentNullException("key");
                }

                TValue result = default(TValue);

                lock (mLockObject)
                {
                    bool found = false;
                    for (int i = 0; i < Count; i++)
                    {
                        KeyValuePair<TKey, TValue> item = GetItem(i);
                        if (key.Equals(item.Key))
                        {
                            result = item.Value;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        throw new KeyNotFoundException(string.Format("Key not found: {0}", key));
                    }
                }

                return result;
            }
            set
            {
                DoDisposeCheck();
                if (ReferenceEquals(key, null))
                {
                    ThrowHelper.ThrowArgumentNullException("key");
                }

                lock (mLockObject)
                {
                    bool found = false;
                    for (int i = 0; i < Count; i++)
                    {
                        KeyValuePair<TKey, TValue> item = GetItem(i);
                        if (key.Equals(item.Key))
                        {
                            KeyValuePair<TKey, TValue> newItem = new KeyValuePair<TKey, TValue>(key, value);
                            RemoveItem(i);
                            AddItem(i, newItem);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        AddItem(new KeyValuePair<TKey, TValue>(key, value));
                    }
                }
            }
        }

        /// <summary>
        /// Adds the specified kv.
        /// </summary>
        /// <param name="kv">The kv.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Forge.ThrowHelper.ThrowArgumentException(System.String,System.String)")]
        public void Add(KeyValuePair<TKey, TValue> kv)
        {
            DoDisposeCheck();
            if (ReferenceEquals(kv.Key, null))
            {
                ThrowHelper.ThrowArgumentNullException("kv.Key");
            }

            lock (mLockObject)
            {
                for (int i = 0; i < Count; i++)
                {
                    KeyValuePair<TKey, TValue> item = GetItem(i);
                    if (kv.Key.Equals(item.Key))
                    {
                        ThrowHelper.ThrowArgumentException(string.Format("Key already exist: {0}", kv.Key), "kv.Key");
                    }
                }
                AddItem(kv);
            }
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException("array");
            }
            if ((index < 0) || (index > array.Length))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("index");
            }
            if ((array.Length - index) < Count)
            {
                ThrowHelper.ThrowArgumentException("array");
            }
            for (int i = 0; i < Count; i++)
            {
                KeyValuePair<TKey, TValue> kv = GetItem(i);
                array[index++] = new KeyValuePair<TKey, TValue>(kv.Key, kv.Value);
            }
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
        /// Removes the specified kv.
        /// </summary>
        /// <param name="kv">The kv.</param>
        /// <returns>True, if the collection modified, otherwise False.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> kv)
        {
            DoDisposeCheck();
            bool result = false;

            lock (mLockObject)
            {
                for (int i = 0; i < Count; i++)
                {
                    KeyValuePair<TKey, TValue> item = GetItem(i);
                    if (kv.Equals(item))
                    {
                        RemoveItem(item);
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>
        /// The keys.
        /// </value>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return Keys; }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return Values; }
        }

        #endregion

    }

}
