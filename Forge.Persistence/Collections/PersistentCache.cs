/* *********************************************************************
 * Date: 03 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Forge.Collections;
using Forge.Configuration.Shared;
using Forge.Logging.Abstraction;
using Forge.Persistence.StorageProviders;
using Forge.Persistence.StorageProviders.ConfigSection;
using Forge.Shared;

namespace Forge.Persistence.Collections
{

    /// <summary>
    /// Persistent cache base
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), Serializable, DebuggerDisplay("Count = {Count}")]
    public abstract class PersistentCache<T> : IPersistentCache<T>
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<PersistentCache<T>>();

        private static readonly HashSet<string> GLOBAL_IDENTIFIERS = new HashSet<string>(); // ezzel segítek elkerülni, hogy ugyanazzal az ID-val két collection példányosodjon

        private string mId = string.Empty;

        private int mCacheSize = int.MaxValue;

        private CacheStrategyEnum mCacheStrategy = CacheStrategyEnum.RecentlyUsed;

        private IStorageProvider<T> mStorageProvider = null;

        private ListSpecialized<CacheItem<T>> mCache = new ListSpecialized<CacheItem<T>>();

        /// <summary>
        /// Object for locking
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        protected readonly Object mLockObject = new Object();

        private int mVersion = 0;

        private bool mStorageOwner = false;

        private bool mDisposed = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="PersistentCache&lt;T&gt;"/> class from being created.
        /// </summary>
        /// <param name="cacheId">The cache id.</param>
        /// <param name="cacheStrategy">The cache strategy.</param>
        private PersistentCache(string cacheId, CacheStrategyEnum cacheStrategy)
        {
            if (string.IsNullOrEmpty(cacheId))
            {
                ThrowHelper.ThrowArgumentNullException("cacheId");
            }
            this.mId = cacheId;
            this.mCacheStrategy = cacheStrategy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentCache&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="cacheId">The cache id.</param>
        /// <param name="cacheStrategy">The cache strategy.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        protected PersistentCache(string cacheId, CacheStrategyEnum cacheStrategy, int cacheSize)
            : this(cacheId, cacheStrategy)
        {
            if (cacheSize < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("cacheSize");
            }
            this.mCacheSize = cacheSize == int.MaxValue ? cacheSize - 1 : cacheSize; // max integer méretnél egy helyet hagyni kell a végére

            RegisterCacheId(cacheId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentCache&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="cacheId">The cache id.</param>
        /// <param name="cacheStrategy">The cache strategy.</param>
        /// <param name="configurationName">Name of the configuration.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        protected PersistentCache(string cacheId, CacheStrategyEnum cacheStrategy, string configurationName)
            : this(cacheId, cacheStrategy)
        {
            if (string.IsNullOrEmpty(configurationName))
            {
                ThrowHelper.ThrowArgumentNullException("configurationName");
            }

            string configPath = "Collections/" + configurationName;
            {
                string cacheSizeStr = ConfigurationAccessHelper.GetValueByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath + "/CacheSize");
                if (!string.IsNullOrEmpty(cacheSizeStr))
                {
                    try
                    {
                        this.mCacheSize = int.Parse(cacheSizeStr);
                        if (this.mCacheSize < 0)
                        {
                            throw new InvalidConfigurationValueException("Cache size is lower than zero.");
                        }
                        mCacheSize = mCacheSize == int.MaxValue ? mCacheSize - 1 : mCacheSize; // max integer méretnél egy helyet hagyni kell a végére
                    }
                    catch (InvalidConfigurationValueException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidConfigurationValueException("Invalid cache size.", ex);
                    }
                }
            }
            {
                CategoryPropertyItem providerConfigItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath + "/PersistentProviderConfiguration");
                if (providerConfigItem == null)
                {
                    SetStorageProvider(new FileStorageProvider<T>(cacheId), true);
                }
                else
                {
                    if (string.IsNullOrEmpty(providerConfigItem.EntryValue))
                    {
                        SetStorageProvider(new FileStorageProvider<T>(cacheId, providerConfigItem), true);
                    }
                    else
                    {
                        Type type = null;
                        try
                        {
                            type = Type.GetType(providerConfigItem.EntryValue, true, true);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Unable to find storage provider: {0}", providerConfigItem.EntryValue), ex);
                        }
                        try
                        {
                            SetStorageProvider((IStorageProvider<T>)type.GetConstructor(new Type[] { typeof(string), typeof(CategoryPropertyItem) }).Invoke(new object[] { cacheId, providerConfigItem }), true);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Unable to instantiate storage provider: {0}", providerConfigItem.EntryValue), ex);
                        }
                    }
                }
            }

            RegisterCacheId(cacheId);
            FillCache();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="PersistentCache&lt;T&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~PersistentCache()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [DebuggerHidden]
        public string Id
        {
            get { return this.mId; }
        }

        /// <summary>
        /// Gets or sets the size of the cache.
        /// </summary>
        /// <value>
        /// The size of the cache.
        /// </value>
        public int CacheSize
        {
            get
            {
                return this.mCacheSize;
            }
            set
            {
                DoDisposeCheck();
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                lock (mLockObject)
                {
                    this.mCacheSize = value == int.MaxValue ? value - 1 : value; // max integer méretnél egy helyet hagyni kell a végére
                    if (value == 0)
                    {
                        // cache kikapcsolása
                        mCache.Clear();
                    }
                    else
                    {
                        if (mCache.Count > value)
                        {
                            if (mCacheStrategy == CacheStrategyEnum.RecentlyUsed)
                            {
                                // kiszedem a legrégebbieket, ha a cache méretét csökkentem és a cache mérete nagyobb, mint az új méret
                                while (mCache.Count > value)
                                {
                                    mCache.RemoveAt(mCache.Count - 1);
                                }
                            }
                            else if (mCacheStrategy == CacheStrategyEnum.CacheForStack)
                            {
                                // kiszedem a végéről az elemeket, ha a cache méretét csökkentem és a cache mérete nagyobb, mint az új méret
                                while (mCache.Count > value)
                                {
                                    mCache.RemoveAt(mCache.Count - 1);
                                }
                            }
                            else
                            {
                                // kiszedem az eljéről az elemeket, ha a cache méretét csökkentem és a cache mérete nagyobb, mint az új méret
                                while (mCache.Count > value)
                                {
                                    mCache.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get
            {
                DoDisposeCheck();
                return mStorageProvider.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                DoDisposeCheck();
                return mStorageProvider.IsEmpty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsDisposed
        {
            get { return this.mDisposed; }
        }

        /// <summary>
        /// Gets the cache strategy.
        /// </summary>
        /// <value>
        /// The cache strategy.
        /// </value>
        [DebuggerHidden]
        public CacheStrategyEnum CacheStrategy
        {
            get { return this.mCacheStrategy; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            DoDisposeCheck();
            lock (mLockObject)
            {
                mCache.Clear();
                mStorageProvider.Clear();
                this.mVersion = 0;
            }
        }

        /// <summary>
        /// Determines whether [contains] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified o]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T o)
        {
            DoDisposeCheck();
            bool result = false;
            lock (mLockObject)
            {
                if (mCache.Count > 0)
                {
                    foreach (CacheItem<T> item in mCache)
                    {
                        if (o == null)
                        {
                            if (item.Element == null)
                            {
                                result = true;
                                break;
                            }
                        }
                        else
                        {
                            result = o.Equals(item.Element);
                            if (result)
                            {
                                break;
                            }
                        }
                    }
                    // itt azt vizsgálom, hogy a storage-ból bent van-e minden adat a memóriában. Ha igen, akkor nem kell a storage-ot átfésülni.
                    if (!result && !mStorageProvider.IsEmpty && mStorageProvider.Count != mCache.Count)
                    {
                        for (int i = 0; i < mStorageProvider.Count; i++)
                        {
                            T item = mStorageProvider[i];
                            if (o == null)
                            {
                                if (item == null)
                                {
                                    result = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (o.Equals(item))
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Toes the array.
        /// </summary>
        /// <returns>Array of the items</returns>
        public T[] ToArray()
        {
            DoDisposeCheck();
            T[] result = new T[this.Count];
            if (this.Count > 0)
            {
                lock (mLockObject)
                {
                    for (int i = 0; i < this.Count; i++)
                    {
                        result[i] = this.GetItem(i);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator of generic items</returns>
        public virtual IEnumeratorSpecialized<T> GetEnumerator()
        {
            return new CacheIterator<T>(this);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator of generic items</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator of generic items</returns>
        Forge.Collections.IEnumeratorSpecialized<T> Forge.Collections.IEnumerableSpecialized<T>.GetEnumerator()
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

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <param name="action">The action.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void ForEach(Action<T> action)
        {
            if (action == null)
            {
                ThrowHelper.ThrowArgumentNullException("action");
            }
            for (int i = 0; i < this.Count; i++)
            {
                action(GetItem(i));
            }
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="obj">The o.</param>
        protected void AddItem(T obj)
        {
            lock (mLockObject)
            {
                AddItem(mStorageProvider.Count, obj);
            }
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="obj">The o.</param>
        protected void AddItem(int index, T obj)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("index");
            }

            //        if (getId().equals("unitOfWorksHistory2")) {
            //            writeDump(StringFormat.format("ADDITEM({0}) BEGIN", index));
            //        }

            lock (mLockObject)
            {
                mStorageProvider.Insert(index, obj);
                IncVersion();
                if (mCacheStrategy == CacheStrategyEnum.RecentlyUsed)
                {
                    if (mCacheSize > 0)
                    {
                        mCache.Insert(0, new CacheItem<T>(index, obj, mCacheStrategy));
                        // újraindexálok
                        for (int i = 1; i < mCache.Count; i++)
                        {
                            CacheItem<T> item = mCache[i];
                            if (item.Index >= index)
                            {
                                item.Index = item.Index + 1;
                            }
                        }
                        mCache.Sort();
                        if (mCache.Count > mCacheSize)
                        {
                            // a legutolsót ki kell dobni, mert túlméretes a cache
                            mCache.RemoveAt(mCache.Count - 1);
                        }
                    }
                }
                else if (mCacheStrategy == CacheStrategyEnum.CacheForStack)
                {
                    if (mCacheSize > 0 && index < mCacheSize)
                    {
                        // csak akkor rakom a cache-be, ha az index alapján is beférhet
                        mCache.Insert(0, new CacheItem<T>(index, obj, mCacheStrategy)); // berakom az elejére
                        if (mCache.Count > 1)
                        {
                            // újraindexálok
                            for (int i = 1; i < mCache.Count; i++)
                            {
                                CacheItem<T> item = mCache[i];
                                if (item.Index >= index)
                                {
                                    item.Index = item.Index + 1;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            mCache.Sort();
                        }
                        if (mCache.Count > mCacheSize)
                        {
                            // a legutolsót ki kell dobni, mert túlméretes a cache
                            mCache.RemoveAt(mCache.Count - 1);
                        }
                    }
                }
                else
                {
                    // csak akkor rakom a cache-be, ha az index alapján is beférhet
                    if (mCache.Count < mCacheSize)
                    {
                        mCache.Insert(0, new CacheItem<T>(index, obj, mCacheStrategy)); // berakom az elejére
                        if (mCache.Count > 1)
                        {
                            // újraindexálok
                            for (int i = 1; i < mCache.Count; i++)
                            {
                                CacheItem<T> item = mCache[i];
                                if (item.Index >= index)
                                {
                                    item.Index = item.Index + 1;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            mCache.Sort();
                        }
                        //if (mCache.Count > mCacheSize)
                        //{
                        //    // a legutolsót ki kell dobni, mert túlméretes a cache
                        //    mCache.RemoveAt(mCache.Count - 1);
                        //}
                    }
                }
            }

            //        if (getId().equals("unitOfWorksHistory2")) {
            //            writeDump(StringFormat.format("ADDITEM({0}) END", index));
            //        }

        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        protected bool RemoveItem(T o)
        {
            bool result = false;
            lock (mLockObject)
            {
                foreach (CacheItem<T> item in mCache)
                {
                    if (o == null)
                    {
                        if (item.Element == null)
                        {
                            result = true;
                            RemoveItem(item.Index);
                            break;
                        }
                    }
                    else
                    {
                        if ((o == null && item.Element == null) || (o != null && o.Equals(item.Element)))
                        {
                            RemoveItem(item.Index);
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="index">The index.</param>
        protected void RemoveItem(int index)
        {
            if (index < 0 || index >= mStorageProvider.Count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("index");
            }

            //        if (getId().equals("unitOfWorksHistory2")) {
            //            writeDump(StringFormat.format("REMOVEITEM({0}) BEGIN", index));
            //        }

            lock (mLockObject)
            {
                mStorageProvider.RemoveAt(index);
                DecVersion();
                if (mCacheSize > 0 && mCache.Count > 0)
                {
                    if (mCacheStrategy == CacheStrategyEnum.RecentlyUsed)
                    {
                        // első körben keresem a törlendő elemet
                        IEnumeratorSpecialized<CacheItem<T>> iterator = (IEnumeratorSpecialized<CacheItem<T>>)mCache.GetEnumerator();
                        while (iterator.MoveNext())
                        {
                            CacheItem<T> item = iterator.Current;
                            if (item.Index == index)
                            {
                                iterator.Remove();
                                break;
                            }
                        }
                        // index adminisztráció második körben
                        iterator = (IEnumeratorSpecialized<CacheItem<T>>)mCache.GetEnumerator();
                        while (iterator.MoveNext())
                        {
                            CacheItem<T> item = iterator.Current;
                            if (item.Index > index)
                            {
                                item.Index = item.Index - 1;
                            }
                        }
                    }
                    else if (mCacheStrategy == CacheStrategyEnum.CacheForStack)
                    {
                        // benne lehet egyáltalán a cache-ben ?
                        if (mCacheSize > index)
                        {
                            if (mCache.Count > index && mCache[index].Index == index)
                            {
                                // ha pontosan a helyén van a cache-ben, akkor gyorsan ki lehet törölni
                                mCache.RemoveAt(index);
                                if (mCache.Count > index)
                                {
                                    for (int i = index; i < mCache.Count; i++)
                                    {
                                        CacheItem<T> item = mCache[i];
                                        item.Index = item.Index - 1;
                                    }
                                }
                            }
                            else
                            {
                                // egyébként szekvenciálisan keresem
                                IEnumeratorSpecialized<CacheItem<T>> iterator = (IEnumeratorSpecialized<CacheItem<T>>)mCache.GetEnumerator();
                                while (iterator.MoveNext())
                                {
                                    CacheItem<T> item = iterator.Current;
                                    if (item.Index == index)
                                    {
                                        iterator.Remove();
                                    }
                                    else if (item.Index > index)
                                    {
                                        item.Index = item.Index;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // benne lehet egyáltalán a cache-ben ?
                        if (index < mCache.Count)
                        {
                            if (mCache[index].Index == index)
                            {
                                mCache.RemoveAt(index);
                                if (mCache.Count - 1 >= index)
                                {
                                    for (int i = index; i < mCache.Count; i++)
                                    {
                                        mCache[i].Index = mCache[i].Index - 1;
                                    }
                                    index = mCache[mCache.Count - 1].Index;
                                    if (Count - 1 > index)
                                    {
                                        GetItem(index + 1); // cache feltöltés
                                    }
                                }
                            }
                            else
                            {
                                foreach (CacheItem<T> item in mCache)
                                {
                                    if (item.Index == index)
                                    {
                                        mCache.Remove(item);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //        if (getId().equals("unitOfWorksHistory2")) {
            //            writeDump(StringFormat.format("REMOVEITEM({0}) END", index));
            //        }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected T GetItem(int index)
        {
            if (index < 0 || index >= mStorageProvider.Count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("index");
            }

            T result = default(T);
            lock (mLockObject)
            {
                bool found = false;
                if (mCacheStrategy == CacheStrategyEnum.RecentlyUsed)
                {
                    if (mCache.Count > 0)
                    {
                        foreach (CacheItem<T> item in mCache)
                        {
                            if (item.Index == index)
                            {
                                found = true;
                                result = item.Element;
                                item.UpdateLastAccessedTime();
                                mCache.Sort();
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        // gather data from persistent storage
                        result = mStorageProvider[index];
                        // store data into the cache
                        if (mCacheSize > 0)
                        {
                            mCache.Add(new CacheItem<T>(index, result, mCacheStrategy));
                            mCache.Sort();
                            if (mCache.Count > mCacheSize)
                            {
                                // ha túlméretes a cache, kidobom a legrégebbi elemet
                                mCache.RemoveAt(mCache.Count - 1);
                            }
                        }
                    }
                }
                else if (mCacheStrategy == CacheStrategyEnum.CacheForStack)
                {
                    if (mCacheSize > 0 && mCache.Count > index)
                    {
                        // benne van a cache-ben, mert a cache rendezett index szerint növekvő sorrendben
                        result = mCache[index].Element;
                        found = true;
                    }
                    if (!found)
                    {
                        // nem volt a cache-ben
                        result = mStorageProvider[index];
                        if (mCacheSize > 0 && mCache.Count < mCacheSize)
                        {
                            // ha be van kapcsolva a cache és a behelyezés miatt nem lesz túlméretes, akkor tárolom az eredményt
                            mCache.Add(new CacheItem<T>(index, result, mCacheStrategy));
                            mCache.Sort();
                        }
                    }
                }
                else
                {
                    if (mCacheSize > 0)
                    {
                        if (/*storageProvider.size() - */mCache.Count > index && mCache.Count > 0)
                        {
                            // benne van a cache-ben, mert a cache rendezett index szerint csökkenő sorrendben
                            CacheItem<T> item = mCache[index];
                            if (item.Index == index)
                            {
                                result = item.Element;
                                found = true;
                            }
                            else
                            {
                                // nem a helyén van, ezért lineárisan keresek a cache-ben
                                if (item.Index > index)
                                {
                                    // utána kell keresni
                                    for (int i = index; i < mCache.Count; i++)
                                    {
                                        item = mCache[i];
                                        if (item.Index == index)
                                        {
                                            result = item.Element;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // zérótól kell keresni
                                    for (int i = 0; i < index; i++)
                                    {
                                        item = mCache[i];
                                        if (item.Index == index)
                                        {
                                            result = item.Element;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!found)
                    {
                        result = mStorageProvider[index];
                        if (mCacheSize > 0 && mCache.Count < mCacheSize)
                        {
                            // ha be van kapcsolva a cache és a behelyezés miatt nem lesz túlméretes, akkor tárolom az eredményt
                            mCache.Add(new CacheItem<T>(index, result, mCacheStrategy));
                            mCache.Sort();
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the index of item.
        /// </summary>
        /// <param name="obj">The o.</param>
        /// <returns></returns>
        protected int GetIndexOfItem(T obj)
        {
            int result = -1;
            lock (mLockObject)
            {
                if (!mStorageProvider.IsEmpty)
                {
                    bool found = false;
                    if (mCache.Count > 0)
                    {
                        foreach (CacheItem<T> item in mCache)
                        {
                            if ((item.Element == null && obj == null) || (item.Element != null && item.Element.Equals(obj)))
                            {
                                found = true;
                                result = item.Index;
                                if (mCacheStrategy == CacheStrategyEnum.RecentlyUsed)
                                {
                                    item.UpdateLastAccessedTime();
                                    mCache.Sort();
                                }
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        T item = default(T);
                        for (int i = 0; i < mStorageProvider.Count; i++)
                        {
                            item = mStorageProvider[i];
                            if (obj == null)
                            {
                                if (item == null)
                                {
                                    result = i;
                                    break;
                                }
                            }
                            else if ((obj == null && item == null) || (obj != null && obj.Equals(item)))
                            {
                                result = i;
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Does the dispose check.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)"), DebuggerHidden]
        protected void DoDisposeCheck()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(string.Format("{0}, Id: '{1}' already disposed.", GetType().Name, this.mId));
            }
        }

        /// <summary>
        /// Gets the storage provider.
        /// </summary>
        [DebuggerHidden]
        protected IStorageProvider<T> StorageProvider
        {
            get { return mStorageProvider; }
        }

        /// <summary>
        /// Sets the storage provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        protected void SetStorageProvider(IStorageProvider<T> provider, bool owner)
        {
            this.mStorageProvider = provider;
            this.mStorageOwner = owner;
            this.mVersion = provider.Count;
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        [DebuggerHidden]
        protected int Version
        {
            get { return mVersion; }
        }

        /// <summary>
        /// Incs the version.
        /// </summary>
        protected void IncVersion()
        {
            if (int.MaxValue == this.mVersion)
            {
                this.mVersion = 0;
            }
            this.mVersion++;
        }

        /// <summary>
        /// Decs the version.
        /// </summary>
        protected void DecVersion()
        {
            if (int.MinValue == this.mVersion)
            {
                this.mVersion = 0;
            }
            this.mVersion--;
        }

        /// <summary>
        /// Writes the dump.
        /// </summary>
        /// <param name="comment">The comment.</param>
        protected void WriteDump(string comment)
        {
            if (LOGGER.IsDebugEnabled && mCache.Count > 0)
            {
                LOGGER.Debug(string.Format("<----- DUMP BEGIN ({0}), '{1}' ----->", this.mId, comment));
                foreach (CacheItem<T> item in mCache)
                {
                    LOGGER.Debug(string.Format("{0}: {1}", comment, item));
                }
                LOGGER.Debug(string.Format("<-----  DUMP END ({0}), '{1}'  ----->", this.mId, comment));
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.mDisposed = true;
            if (disposing)
            {
                if (this.mStorageProvider != null && this.mStorageOwner)
                {
                    this.mStorageProvider.Dispose();
                }
            }
            lock (GLOBAL_IDENTIFIERS)
            {
                GLOBAL_IDENTIFIERS.Remove(this.mId);
            }
        }

        #endregion

        #region Private method(s)

        private static void RegisterCacheId(string cacheId)
        {
            lock (GLOBAL_IDENTIFIERS)
            {
                if (GLOBAL_IDENTIFIERS.Contains(cacheId))
                {
                    throw new DuplicatedCacheIdentifierException(string.Format("Identifier already exist: {0}", cacheId));
                }
                GLOBAL_IDENTIFIERS.Add(cacheId);
            }
        }

        /// <summary>
        /// Fills the cache.
        /// </summary>
        protected void FillCache()
        {
            if (this.mCacheSize > 0 && mStorageProvider.Count > 0)
            {
                for (int i = 0; mCache.Count < this.mCacheSize && i < mStorageProvider.Count; i++)
                {
                    mCache.Add(new CacheItem<T>(i, mStorageProvider[i], mCacheStrategy));
                }
                //if (this.mCacheStrategy == CacheStrategyEnum.CacheFromEnds)
                //{
                //    for (int i = mStorageProvider.Count - 1; mCache.Count < this.mCacheSize && i >= 0; i--)
                //    {
                //        mCache.Add(new CacheItem<T>(i, mStorageProvider[i], mCacheStrategy));
                //    }
                //}
                //else
                //{
                //    for (int i = 0; mCache.Count < this.mCacheSize && i < mStorageProvider.Count; i++)
                //    {
                //        mCache.Add(new CacheItem<T>(i, mStorageProvider[i], mCacheStrategy));
                //    }
                //}
            }
        }

        #endregion

        #region Nested classes

        [Serializable]
        private class CacheItem<TItem> : IComparable<CacheItem<TItem>>, IComparable
        {

            #region Field(s)

            private int mIndex = 0;

            private DateTime mLastAccessed = DateTime.Now;

            private CacheStrategyEnum mCacheStrategy = CacheStrategyEnum.RecentlyUsed;

            private TItem mElement = default(TItem);

            #endregion

            #region Constructor(s)

            public CacheItem(int index, TItem element, CacheStrategyEnum cacheStrategy)
            {
                this.mIndex = index;
                this.mElement = element;
                this.mCacheStrategy = cacheStrategy;
            }

            #endregion

            #region Public properties

            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            /// <value>
            /// The index.
            /// </value>
            public int Index
            {
                get { return mIndex; }
                set { mIndex = value; }
            }

            /// <summary>
            /// Gets the element.
            /// </summary>
            public TItem Element
            {
                get { return mElement; }
            }

            /// <summary>
            /// Gets the last accessed.
            /// </summary>
            /// <value>
            /// The last accessed.
            /// </value>
            public DateTime LastAccessed
            {
                get { return mLastAccessed; }
            }

            #endregion

            #region Public method(s)

            /// <summary>
            /// Updates the last accessed time.
            /// </summary>
            public void UpdateLastAccessedTime()
            {
                mLastAccessed = DateTime.Now;
            }

            /// <summary>
            /// Compares to.
            /// </summary>
            /// <param name="o">The o.</param>
            /// <returns></returns>
            public int CompareTo(CacheItem<TItem> o)
            {
                int result = 0;
                if (mCacheStrategy == CacheStrategyEnum.RecentlyUsed)
                {
                    result = o.LastAccessed.CompareTo(mLastAccessed); // dátum szerint rendez
                }
                else /*if (mCacheStrategy == CacheStrategyEnum.CacheFromBegins)*/
                {
                    result = mIndex.CompareTo(o.Index); // index szerint növekvő
                }
                //else
                //{
                //    result = o.Index.CompareTo(mIndex); // index szerint csökkenő
                //}
                return result;
            }

            /// <summary>
            /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
            /// </summary>
            /// <param name="obj">An object to compare with this instance.</param>
            /// <returns>
            /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
            /// </returns>
            /// <exception cref="T:System.ArgumentException">
            ///   <paramref name="obj"/> is not the same type as this instance. </exception>
            public int CompareTo(object obj)
            {
                if (obj == null)
                {
                    ThrowHelper.ThrowArgumentNullException("obj");
                }
                if (!(obj is CacheItem<TItem>))
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(obj, typeof(CacheItem<TItem>));
                }
                return CompareTo((CacheItem<TItem>)obj);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(Object obj)
            {
                if (obj == null) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (!obj.GetType().Equals(GetType())) return false;

                CacheItem<T> other = (CacheItem<T>)obj;
                return ((other.mElement == null && this.mElement == null) || (this.mElement != null && this.mElement.Equals(other.mElement)));
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format("{0}, index: {1}, element: {2}", GetType().Name, mIndex, mElement);
            }

            #endregion

        }

        /// <summary>
        /// CacheIterator
        /// </summary>
        /// <typeparam name="TItem">Generic type</typeparam>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        protected struct CacheIterator<TItem> : IEnumeratorSpecialized<TItem>, IDisposable, IEnumerator
        {

            #region Field(s)

            private PersistentCache<TItem> cache;
            private int index;
            private int version;
            private TItem current;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="PersistentCache&lt;T&gt;.CacheIterator&lt;TItem&gt;"/> struct.
            /// </summary>
            /// <param name="cache">The cache.</param>
            public CacheIterator(PersistentCache<TItem> cache)
            {
                this.cache = cache;
                this.index = 0;
                this.version = cache.mVersion;
                this.current = default(TItem);
            }

            #endregion

            #region Public method(s)

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
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
                if ((this.version == cache.mVersion) && (this.index < cache.Count))
                {
                    this.current = cache.GetItem(this.index);
                    this.index++;
                    return true;
                }
                return this.MoveNextRare();
            }

            private bool MoveNextRare()
            {
                if (this.version != this.cache.mVersion)
                {
                    throw new InvalidOperationException("Collection modified while iterating on it.");
                }
                this.index = this.cache.Count + 1;
                this.current = default(TItem);
                return false;
            }

            /// <summary>
            /// Gets the current.
            /// </summary>
            public TItem Current
            {
                get
                {
                    if ((this.index == 0) || (this.index == (this.cache.Count + 1)))
                    {
                        throw new InvalidOperationException();
                    }
                    return this.current;
                }
            }

            /// <summary>
            /// Gets the current.
            /// </summary>
            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            void IEnumerator.Reset()
            {
                if (this.version != this.cache.mVersion)
                {
                    throw new InvalidOperationException("Collection modified while iterating on it.");
                }
                this.index = 0;
                this.current = default(TItem);
            }

            /// <summary>
            /// Removes this instance.
            /// </summary>
            public void Remove()
            {
                if ((this.index == 0) || (this.index == (this.cache.Count + 1)))
                {
                    throw new InvalidOperationException();
                }
                this.index--;
                this.cache.RemoveItem(index);
                this.version = cache.mVersion;
            }

            /// <summary>
            /// Determines whether this instance has next.
            /// </summary>
            /// <returns>
            ///   <c>true</c> if this instance has next; otherwise, <c>false</c>.
            /// </returns>
            public bool HasNext()
            {
                return this.index < cache.Count;
            }

            #endregion

        }

        #endregion

    }

}
