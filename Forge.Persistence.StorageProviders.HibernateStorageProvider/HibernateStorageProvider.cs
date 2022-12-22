﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Forge.Collections;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.DatabaseManagement;
using Forge.Formatters;
using Forge.ORM.NHibernateExtension;
using Forge.ORM.NHibernateExtension.Criterias;
using Forge.ORM.NHibernateExtension.Model.Distributed;
using Forge.Persistence.Formatters;
using Forge.Persistence.Serialization;
using Forge.Persistence.StorageProviders.ConfigSection;
using Forge.Persistence.StorageProviders.HibernateStorageProvider.EntityModel;
using Forge.Reflection;
using Forge.Shared;
using NHibernate;
using NHibernate.Mapping.Attributes;
using NHibernate.Tool.hbm2ddl;

namespace Forge.Persistence.StorageProviders.HibernateStorageProvider
{

    /// <summary>
    /// Hibernate based storage provider
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public sealed class HibernateStorageProvider<T> : StorageProviderBase<T>
    {

        #region Field(s)

        private static readonly bool LOG_QUERY = false;

        private static readonly Dictionary<string, ISessionFactory> mSessionFactoriesForStorages = new Dictionary<string, ISessionFactory>();

        private static long DEFAULT_SYSTEM_ID;

        private static ISessionFactory DEFAULT_SESSION_FACTORY = null;

        private Mutex mAppIdMutex = null;

        private ItemTable mAllocationTable = null;

        private IDataFormatter<ItemTable> mTableFormatter = new BinarySerializerFormatter<ItemTable>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true);

        private long mSystemId;

        private long mDeviceId;

        private bool mCompressContent = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="HibernateStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        static HibernateStorageProvider()
        {
            DEFAULT_SYSTEM_ID = HashGeneratorHelper.GetSHA256BasedValue(ApplicationHelper.ApplicationId);

            CategoryPropertyItem configItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, "NHibernateProvider/NHibernateStorages/Default");
            if (configItem != null)
            {
                DEFAULT_SESSION_FACTORY = CreateEntityManagerFactory(configItem, DEFAULT_SYSTEM_ID);
            }

            configItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, "NHibernateProvider/NHibernateStorages/StorageSpecified");
            if (configItem != null)
            {
                foreach (CategoryPropertyItem pi in configItem)
                {
                    mSessionFactoriesForStorages.Add(configItem.Id, CreateEntityManagerFactory(configItem, DEFAULT_SYSTEM_ID));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HibernateStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        public HibernateStorageProvider(string storageId)
            : this(storageId, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HibernateStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="compressContent">if set to <c>true</c> [compress content].</param>
        public HibernateStorageProvider(string storageId, bool compressContent)
            : this(storageId, new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true), compressContent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HibernateStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="dataFormatter">The data formatter.</param>
        public HibernateStorageProvider(string storageId, IDataFormatter<T> dataFormatter)
            : this(storageId, dataFormatter, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HibernateStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="dataFormatter">The data formatter.</param>
        /// <param name="compressContent">if set to <c>true</c> [compress content].</param>
        public HibernateStorageProvider(string storageId, IDataFormatter<T> dataFormatter, bool compressContent)
            : base(storageId)
        {
            if (dataFormatter == null)
            {
                ThrowHelper.ThrowArgumentNullException("dataFormatter");
            }

            mDataFormatter = dataFormatter;
            mCompressContent = compressContent;

            Initialize(StorageConfiguration.Settings.CategoryPropertyItems);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HibernateStorageProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="configItem">The config item.</param>
        public HibernateStorageProvider(string storageId, IPropertyItem configItem)
            : base(storageId, configItem)
        {
            long systemId = 1;
            if (!ConfigurationAccessHelper.ParseLongValue(configItem, "SystemId", 1, long.MaxValue, ref systemId))
            {
                systemId = DEFAULT_SYSTEM_ID;
            }
            mSystemId = systemId;

            bool compressContent = false;
            ConfigurationAccessHelper.ParseBooleanValue(configItem, "CompressContent", ref compressContent);

            lock (mSessionFactoriesForStorages)
            {
                if (!mSessionFactoriesForStorages.ContainsKey(configItem.Id))
                {
                    mSessionFactoriesForStorages.Add(configItem.Id, CreateEntityManagerFactory(configItem, systemId));
                }
            }

            Initialize(StorageConfiguration.Settings.CategoryPropertyItems);
        }

        /// <summary>Initializes a new instance of the <see cref="HibernateStorageProvider{T}" /> class.</summary>
        /// <param name="propertyItem">The property item.</param>
        public HibernateStorageProvider(IPropertyItem propertyItem) : base(propertyItem)
        {
            long systemId = 1;
            if (!ConfigurationAccessHelper.ParseLongValue(propertyItem, "SystemId", 1, long.MaxValue, ref systemId))
            {
                systemId = DEFAULT_SYSTEM_ID;
            }
            mSystemId = systemId;

            bool compressContent = false;
            ConfigurationAccessHelper.ParseBooleanValue(propertyItem, "CompressContent", ref compressContent);

            lock (mSessionFactoriesForStorages)
            {
                if (!mSessionFactoriesForStorages.ContainsKey(propertyItem.Id))
                {
                    mSessionFactoriesForStorages.Add(propertyItem.Id, CreateEntityManagerFactory(propertyItem, systemId));
                }
            }

            Initialize(propertyItem);
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Adds the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <exception cref="Forge.Persistence.StorageProviders.PersistenceException"></exception>
        public override void Add(T o)
        {
            DoDisposeCheck();
            EntityId entityId = new EntityId(mSystemId, mDeviceId, GetNextId());
            mAllocationTable.ItemIds.Add(entityId);

            using (ISession session = GetSession())
            {
                try
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        SaveObject(entityId, o, session);
                        SaveAllocationTable(session);
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    mAllocationTable.ItemIds.RemoveAt(mAllocationTable.ItemIds.Count - 1);
                    throw new PersistenceException(string.Format("Unable to add object. Id: {0}", entityId), ex);
                }
            }

            IncVersion();
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <exception cref="Forge.Persistence.StorageProviders.PersistenceException">Unable to add object.</exception>
        public override void AddRange(IEnumerable<T> o)
        {
            DoDisposeCheck();
            if (o == null)
            {
                ThrowHelper.ThrowArgumentNullException("o");
            }

            using (ISession session = GetSession())
            {
                List<EntityId> ids = new List<EntityId>();
                try
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        foreach (T item in o)
                        {
                            EntityId entityId = new EntityId(mSystemId, mDeviceId, GetNextId());
                            mAllocationTable.ItemIds.Add(entityId);
                            ids.Add(entityId);
                            SaveObject(entityId, item, session);
                        }

                        SaveAllocationTable(session);
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    if (ids.Count > 0)
                    {
                        foreach (EntityId id in ids)
                        {
                            mAllocationTable.ItemIds.Remove(id);
                        }
                        ids.Clear();
                    }
                    throw new PersistenceException("Unable to add object.", ex);
                }
            }

            IncVersion();
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="o">The o.</param>
        /// <exception cref="Forge.Persistence.StorageProviders.PersistenceException"></exception>
        public override void Insert(int index, T o)
        {
            DoDisposeCheck();
            if (index > Count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("index");
            }
            EntityId entityId = new EntityId(mSystemId, mDeviceId, GetNextId());
            mAllocationTable.ItemIds.Insert(index, entityId);

            using (ISession session = GetSession())
            {
                try
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        SaveObject(entityId, o, session);
                        SaveAllocationTable(session);
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    mAllocationTable.ItemIds.RemoveAt(index);
                    throw new PersistenceException(string.Format("Unable to add object. Id: {0}", entityId), ex);
                }
            }

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
        /// <exception cref="Forge.Persistence.StorageProviders.PersistenceException"></exception>
        public override void RemoveAt(int index)
        {
            DoDisposeCheck();
            if (index > Count - 1)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("index");
            }

            EntityId entityId = mAllocationTable.ItemIds[index];
            mAllocationTable.ItemIds.RemoveAt(index);
            if (mAllocationTable.ItemIds.Count == 0)
            {
                mAllocationTable.ItemUid = 0;
            }

            using (ISession session = GetSession())
            {
                try
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        RemoveObject(entityId, session);
                        SaveAllocationTable(session);
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    throw new PersistenceException(string.Format("Unable to remove object. Id: {0}", entityId), ex);
                }
            }

            IncVersion();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public override void Clear()
        {
            DoDisposeCheck();

            if (mAllocationTable.ItemIds.Count > 0)
            {
                using (ISession session = GetSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        foreach (EntityId entityId in mAllocationTable.ItemIds)
                        {
                            RemoveObject(entityId, session);
                        }
                        mAllocationTable.ItemIds.Clear();
                        mAllocationTable.ItemUid = 0;
                        SaveAllocationTable(session);
                        transaction.Commit();
                    }
                }

                IncVersion();
            }
        }

        /// <summary>
        /// Gets the object at the specified index.
        /// </summary>
        /// <value>
        /// The value
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>Item</returns>
        public override T this[int index]
        {
            get
            {
                DoDisposeCheck();
                if (index > Count - 1)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("index");
                }
                return LoadObject(mAllocationTable.ItemIds[index]);
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
                return mAllocationTable.ItemIds.Count;
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
                return this.Count == 0;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator of generic items</returns>
        public override IEnumeratorSpecialized<T> GetEnumerator()
        {
            DoDisposeCheck();
            return new StorageProviderIterator<T>(this, 0, mAllocationTable.ItemIds.Count);
        }

        #endregion

        #region Public properties

        /// <summary>Gets or sets the default system identifier.</summary>
        /// <value>The default system identifier.</value>
        public static long DefaultSystemId
        {
            get { return DEFAULT_SYSTEM_ID; }
            set { DEFAULT_SYSTEM_ID = value; }
        }

        /// <summary>Gets or sets the default session factory.</summary>
        /// <value>The default session factory.</value>
        public static ISessionFactory DefaultSessionFactory
        {
            get { return DEFAULT_SESSION_FACTORY; }
            set { DEFAULT_SESSION_FACTORY = value; }
        }

        /// <summary>
        /// Gets a value indicating whether [compress content].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [compress content]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool CompressContent
        {
            get { return mCompressContent; }
        }

        #endregion

        #region Private method(s)

        private static ISessionFactory CreateEntityManagerFactory(IPropertyItem configItem, long systemId)
        {
            ISessionFactory sessionFactory = null;
            ISession session = null;
            ITransaction transaction = null;
            try
            {
                sessionFactory = CreateEntityManagerFactory(SchemaFactoryModeEnum.Validate, configItem, systemId);
                session = sessionFactory.OpenSession();
                using (transaction = session.BeginTransaction())
                {
                    session.Flush(); // check if it is working properly
                    transaction.Rollback();
                }
            }
            catch (Exception)
            {
                if (sessionFactory != null)
                {
                    sessionFactory.Dispose();
                    sessionFactory = null;
                }
                if (session != null)
                {
                    session.Dispose();
                    session = null;
                }
                try
                {
                    sessionFactory = CreateEntityManagerFactory(SchemaFactoryModeEnum.Update, configItem, systemId);
                    session = sessionFactory.OpenSession();
                    using (transaction = session.BeginTransaction())
                    {
                        session.Flush(); // check if it is working properly
                        transaction.Rollback();
                    }
                }
                catch (Exception)
                {
                    if (sessionFactory != null)
                    {
                        sessionFactory.Dispose();
                        sessionFactory = null;
                    }
                    if (session != null)
                    {
                        session.Dispose();
                        session = null;
                    }
                    try
                    {
                        sessionFactory = CreateEntityManagerFactory(SchemaFactoryModeEnum.Create, configItem, systemId);
                        session = sessionFactory.OpenSession();
                        using (transaction = session.BeginTransaction())
                        {
                            session.Flush(); // check if it is working properly
                            transaction.Rollback();
                        }
                    }
                    catch (Exception)
                    {
                        if (sessionFactory != null)
                        {
                            sessionFactory.Dispose();
                            sessionFactory = null;
                        }
                        if (session != null)
                        {
                            session.Dispose();
                            session = null;
                        }
                        throw;
                    }
                }
            }
            finally
            {
                if (session != null)
                {
                    session.Dispose();
                }
            }
            return sessionFactory;
        }

        private static ISessionFactory CreateEntityManagerFactory(SchemaFactoryModeEnum mode, IPropertyItem configItem, long systemId)
        {
            string databaseManagerTypeStr = string.Empty;

            IPropertyItem dbItem = ConfigurationAccessHelper.GetPropertyByPath(configItem, "DatabaseManager");
            if (dbItem != null) databaseManagerTypeStr = dbItem.Value;

            if (!string.IsNullOrWhiteSpace(databaseManagerTypeStr))
            {
                Type databaseManagerType = null;
                try
                {
                    databaseManagerType = TypeHelper.GetTypeFromString(databaseManagerTypeStr);
                }
                catch (Exception ex)
                {
                    throw new InvalidConfigurationValueException(ex.Message, ex);
                }

                if (databaseManagerType != null)
                {
                    using (IDatabaseManager manager = (IDatabaseManager)databaseManagerType.GetConstructor(Type.EmptyTypes).Invoke(null))
                    {
                        manager.Initialize(dbItem);
                        Dictionary<string, string> settings = new Dictionary<string, string>();
                        foreach (IPropertyItem pi in ConfigurationAccessHelper.GetPropertyByPath(configItem, "NHibernateSettings").Items.Values)
                        {
                            settings[pi.Id] = pi.Value;
                        }
                        manager.EnsureDatabaseIntegrity(systemId, settings, mode);
                    }
                }

            }

            string hbm2ddl = "hbm2ddl.auto";

            NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();
            cfg.Properties.Clear();

            foreach (IPropertyItem pi in ConfigurationAccessHelper.GetPropertyByPath(configItem, "NHibernateSettings").Items.Values)
            {
                if (!hbm2ddl.Equals(pi.Id.ToLower()))
                {
                    cfg.Properties[pi.Id] = pi.Value;
                }
            }

            if (mode == SchemaFactoryModeEnum.Create)
            {
                cfg.Properties[hbm2ddl] = "create";
            }
            else if (mode == SchemaFactoryModeEnum.Create_And_Drop)
            {
                cfg.Properties[hbm2ddl] = "create-drop";
            }

            HbmSerializer serializer = new HbmSerializer();
            serializer.HbmAssembly = typeof(PersistentStorageItem).Assembly.GetName().FullName;
            serializer.HbmAutoImport = true;
            serializer.Validate = true;
            serializer.WriteDateComment = false;
            serializer.HbmDefaultAccess = "field";

            cfg.AddInputStream(serializer.Serialize(typeof(PersistentStorageItem).Assembly));
            //cfg.Configure();

            if (mode == SchemaFactoryModeEnum.Validate)
            {
                SchemaValidator schemaValidator = new SchemaValidator(cfg);
                schemaValidator.Validate(); // validate the database schema
            }
            else if (mode == SchemaFactoryModeEnum.Update)
            {
                SchemaUpdate schemaUpdater = new SchemaUpdate(cfg); // try to update schema
                schemaUpdater.Execute(false, true);
                if (schemaUpdater.Exceptions.Count > 0)
                {
                    throw new Exception("FAILED TO UPDATE SCHEMA");
                }
            }

            return cfg.BuildSessionFactory();
        }

        private void Initialize(IPropertyItem categoryPropertyItems)
        {
            bool mutexResult = false;

            string typeName = string.Empty;

#if NET40
            string appId = ApplicationHelper.ApplicationId;
            typeName = string.Format("HibernateStorageProvider_{0}_{1}_{2}", appId, StorageId, typeof(T).AssemblyQualifiedName.GetHashCode().ToString());
#else
            typeName = string.Format("HibernateStorageProvider_{0}_{1}", StorageId, typeof(T).AssemblyQualifiedName.GetHashCode().ToString());
#endif

            if (typeName.Length > 255)
            {
                // limit the length of the mutex name to avoid exception
                typeName = typeName.Substring(0, 255);
            }
            mAppIdMutex = new Mutex(true, typeName, out mutexResult);
            if (!mutexResult)
            {
                throw new InitializationException("An other application with the specified application identifier is running.");
            }

            mDeviceId = HashGeneratorHelper.GetSHA256BasedValue(StorageId);

            IPropertyItem pi = ConfigurationAccessHelper.GetPropertyByPath(categoryPropertyItems, string.Format("NHibernateProvider/KnownStorageIdsToReset/{0}", StorageId));
            if (pi != null)
            {
                Reset();
            }

            LoadAllocationTable();
        }

        private ISession GetSession()
        {
            ISession session = null;
            bool hasKey = false;
            lock (mSessionFactoriesForStorages)
            {
                hasKey = mSessionFactoriesForStorages.ContainsKey(StorageId);
            }
            if (hasKey)
            {
                session = mSessionFactoriesForStorages[StorageId].OpenSession();
            }
            else if (DEFAULT_SESSION_FACTORY == null)
            {
                throw new PersistenceException(string.Format("Unable to find hibernate database configuration for this storage: {0}", StorageId));
            }
            else
            {
                session = DEFAULT_SESSION_FACTORY.OpenSession();
            }
            session.FlushMode = NHibernate.FlushMode.Commit;
            return session;
        }

        private void LoadAllocationTable()
        {
            using (ISession session = GetSession())
            {
                try
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        QueryParams<PersistentStorageAllocationTable> query = new QueryParams<PersistentStorageAllocationTable>(
                                GetAllocationTableCriteria(), 1);
                        IList<PersistentStorageAllocationTable> resultList = QueryHelper.Query<PersistentStorageAllocationTable>(session, query, LOG_QUERY);
                        if (resultList.Count == 0)
                        {
                            mAllocationTable = new ItemTable();
                        }
                        else
                        {
                            using (MemoryStream ms = new MemoryStream(resultList[0].ItemAllocationTableData))
                            {
                                ms.Position = 0;
                                mAllocationTable = SerializationHelper.Read<ItemTable>(ms, mTableFormatter, false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PersistenceException("Unable to load allocation table.", ex);
                }
            }
        }

        private void SaveAllocationTable(ISession session)
        {
            try
            {
                QueryParams<PersistentStorageAllocationTable> query = new QueryParams<PersistentStorageAllocationTable>(
                        GetAllocationTableCriteria(), 1);
                IList<PersistentStorageAllocationTable> resultList = QueryHelper.Query<PersistentStorageAllocationTable>(session, query, LOG_QUERY);
                PersistentStorageAllocationTable table = null;
                if (resultList.Count == 0)
                {
                    table = new PersistentStorageAllocationTable();
                    table.Id = new EntityId(mSystemId, mDeviceId, 0L);
                    table.Version = new EntityVersion(mDeviceId);
                }
                else
                {
                    table = resultList[0];
                    table.Version.IncSeqNumber();
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    SerializationHelper.Write<ItemTable>(mAllocationTable, ms, mTableFormatter, false);
                    table.ItemAllocationTableData = ms.ToArray();
                }
                ORMUtils.SaveEntity(table, session);
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Unable to persist allocation table.", ex);
            }
        }

        private Criteria GetAllocationTableCriteria()
        {
            return new GroupCriteria(
                    new ArithmeticCriteria("id.systemId", mSystemId),
                    new ArithmeticCriteria("id.deviceId", mDeviceId),
                    new ArithmeticCriteria("id.id", 0L),
                    new ArithmeticCriteria("deleted", false));
        }

        private T LoadObject(EntityId entityId)
        {
            T result = default(T);

            using (ISession session = GetSession())
            {
                try
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        QueryParams<PersistentStorageItem> query = new QueryParams<PersistentStorageItem>(
                                new GroupCriteria(
                                new ArithmeticCriteria("id.systemId", entityId.SystemId),
                                new ArithmeticCriteria("id.deviceId", entityId.DeviceId),
                                new ArithmeticCriteria("id.id", entityId.Id)), 1);
                        IList<PersistentStorageItem> resultList = QueryHelper.Query<PersistentStorageItem>(session, query, LOG_QUERY);
                        if (resultList.Count == 0)
                        {
                            throw new Exception(string.Format("Unable to read object. Id: {0}", entityId));
                        }
                        else
                        {
                            if (resultList[0].EntryData != null)
                            {
                                using (MemoryStream ms = new MemoryStream(resultList[0].EntryData))
                                {
                                    ms.Position = 0;
                                    result = SerializationHelper.Read<T>(ms, DataFormatter, CompressContent);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PersistenceException(string.Format("Unable to read object. Id: {0}", entityId), ex);
                }
            }

            return result;
        }

        private void SaveObject(EntityId entityId, T o, ISession session)
        {
            try
            {
                QueryParams<PersistentStorageItem> query = new QueryParams<PersistentStorageItem>(
                        new GroupCriteria(
                        new ArithmeticCriteria("id.systemId", entityId.SystemId),
                        new ArithmeticCriteria("id.deviceId", entityId.DeviceId),
                        new ArithmeticCriteria("id.id", entityId.Id)), 1);
                IList<PersistentStorageItem> resultList = QueryHelper.Query<PersistentStorageItem>(session, query, LOG_QUERY);
                PersistentStorageItem item = null;
                if (resultList.Count == 0)
                {
                    item = new PersistentStorageItem();
                    item.Id = entityId;
                    item.Version = new EntityVersion(mDeviceId);
                }
                else
                {
                    item = resultList[0];
                }
                if (o == null)
                {
                    item.EntryData = null;
                }
                else
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        SerializationHelper.Write<T>(o, ms, DataFormatter, CompressContent);
                        item.EntryData = ms.ToArray();
                    }
                }
                ORMUtils.SaveEntity(item, session);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(string.Format("Unable to save object. Id: {0}", entityId), ex);
            }
        }

        private void RemoveObject(EntityId entityId, ISession session)
        {
            QueryParams<PersistentStorageItem> query = new QueryParams<PersistentStorageItem>(
                    new GroupCriteria(
                    new ArithmeticCriteria("id.systemId", entityId.SystemId),
                    new ArithmeticCriteria("id.deviceId", entityId.DeviceId),
                    new ArithmeticCriteria("id.id", entityId.Id)), 1);
            IList<PersistentStorageItem> resultList = QueryHelper.Query<PersistentStorageItem>(session, query, LOG_QUERY);
            PersistentStorageItem item = null;
            if (resultList.Count == 0)
            {
                throw new Exception(string.Format("Unable to remove object. Id: {0}", entityId));
            }
            else
            {
                item = resultList[0];
                ORMUtils.DeleteEntity(item, session);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private long GetNextId()
        {
            if (long.MaxValue == mAllocationTable.ItemUid)
            {
                mAllocationTable.ItemUid = 0;
            }
            mAllocationTable.ItemUid = mAllocationTable.ItemUid + 1;
            return mAllocationTable.ItemUid;
        }

        private void Reset()
        {
            using (ISession session = GetSession())
            {
                try
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        {
                            QueryParams<PersistentStorageAllocationTable> query = new QueryParams<PersistentStorageAllocationTable>(
                                    GetAllocationTableCriteria(), 1);
                            IList<PersistentStorageAllocationTable> resultList = QueryHelper.Query<PersistentStorageAllocationTable>(session, query, LOG_QUERY);
                            if (resultList.Count > 0)
                            {
                                ORMUtils.DeleteEntity(resultList[0], session);
                            }
                        }
                        {
                            QueryParams<PersistentStorageItem> query = new QueryParams<PersistentStorageItem>(
                                    new GroupCriteria(
                                    new ArithmeticCriteria("id.systemId", mSystemId),
                                    new ArithmeticCriteria("id.deviceId", mDeviceId)));
                            IList<PersistentStorageItem> resultList = QueryHelper.Query<PersistentStorageItem>(session, query, LOG_QUERY);
                            if (resultList.Count > 0)
                            {
                                foreach (PersistentStorageItem item in resultList)
                                {
                                    ORMUtils.DeleteEntity(item, session);
                                }
                                resultList.Clear();
                            }
                        }
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    throw new PersistenceException("Unable to reset storage provider.", ex);
                }
            }
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
                if (mAppIdMutex != null)
                {
                    mAppIdMutex.Close();
                    mAppIdMutex = null;
                }

                //if (mAllocationTable != null)
                //{
                //    // remove all elements
                //    try
                //    {
                //        using (ISession session = GetSession())
                //        {
                //            using (ITransaction transaction = session.BeginTransaction())
                //            {
                //                QueryParams<PersistentStorageAllocationTable> query = new QueryParams<PersistentStorageAllocationTable>(
                //                        GetAllocationTableCriteria(), 1);
                //                IList<PersistentStorageAllocationTable> resultList = QueryHelper.Query<PersistentStorageAllocationTable>(session, query, LOG_QUERY);
                //                if (resultList.Count > 0)
                //                {
                //                    ORMUtils.DeleteEntity(resultList[0], session);
                //                }
                //                foreach (EntityId entityId in mAllocationTable.ItemIds)
                //                {
                //                    RemoveObject(entityId, session);
                //                }
                //                transaction.Commit();
                //            }
                //        }
                //    }
                //    catch (Exception)
                //    {
                //    }
                //}

            }
            mAllocationTable = null;
            mTableFormatter = null;
            base.Dispose(disposing);
        }

        #endregion

    }

}
