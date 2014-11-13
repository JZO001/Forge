using System;
using Forge.Collections;
using Forge.Configuration.Shared;
using Forge.Persistence.Formatters;
using Forge.Persistence.Serialization;
using Forge.Persistence.StorageProviders.ConfigSection;
using Forge.Reflection;
using log4net;

namespace Forge.Persistence.StorageProviders.HibernateStorageProvider
{

    /// <summary>
    /// Helps to create storage provider in safe mode
    /// </summary>
    public static class FailSafeFactory
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(FailSafeFactory));

        /// <summary>
        /// Fails the safe start storage provider.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storageId">The storage id.</param>
        /// <param name="enableReset">if set to <c>true</c> [enable reset].</param>
        /// <param name="hasError">if set to <c>true</c> [has error].</param>
        /// <returns></returns>
        public static HibernateStorageProvider<T> FailSafeStartStorageProvider<T>(string storageId, bool enableReset, out bool hasError)
        {
            return FailSafeStartStorageProvider(storageId, new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true), false, out hasError);
        }

        /// <summary>
        /// Fails the safe start storage provider.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storageId">The storage id.</param>
        /// <param name="compressContent">if set to <c>true</c> [compress content].</param>
        /// <param name="enableReset">if set to <c>true</c> [enable reset].</param>
        /// <param name="hasError">if set to <c>true</c> [has error].</param>
        /// <returns></returns>
        public static HibernateStorageProvider<T> FailSafeStartStorageProvider<T>(string storageId, bool compressContent, bool enableReset, out bool hasError)
        {
            return FailSafeStartStorageProvider(storageId, new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true), compressContent, enableReset, out hasError);
        }

        /// <summary>
        /// Fails the safe start storage provider.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storageId">The storage id.</param>
        /// <param name="dataFormatter">The data formatter.</param>
        /// <param name="enableReset">if set to <c>true</c> [enable reset].</param>
        /// <param name="hasError">if set to <c>true</c> [has error].</param>
        /// <returns></returns>
        public static HibernateStorageProvider<T> FailSafeStartStorageProvider<T>(string storageId, IDataFormatter<T> dataFormatter, bool enableReset, out bool hasError)
        {
            return FailSafeStartStorageProvider(storageId, dataFormatter, false, enableReset, out hasError);
        }

        /// <summary>
        /// Fails the safe start storage provider.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storageId">The storage id.</param>
        /// <param name="dataFormatter">The data formatter.</param>
        /// <param name="compressContent">if set to <c>true</c> [compress content].</param>
        /// <param name="enableReset">if set to <c>true</c> [enable reset].</param>
        /// <param name="hasError">if set to <c>true</c> [has error].</param>
        /// <returns></returns>
        public static HibernateStorageProvider<T> FailSafeStartStorageProvider<T>(string storageId, IDataFormatter<T> dataFormatter, bool compressContent, bool enableReset, out bool hasError)
        {
            HibernateStorageProvider<T> result = null;
            hasError = false;

            try
            {
                result = new HibernateStorageProvider<T>(storageId, dataFormatter, compressContent);
                IEnumeratorSpecialized<T> iterator = result.GetEnumerator();
                while (iterator.MoveNext())
                {
                    // check data consistency
                }
            }
            catch (Exception)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NHIBERNATE_STORAGE_PROVIDER, failed to initialize storage provider: '{0}'. Reset allowed: {1}", storageId, enableReset.ToString()));
                hasError = true;
                if (enableReset)
                {
                    // reset the content of the storage provider
                    CategoryPropertyItem root = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, "NHibernateProvider");
                    CategoryPropertyItem removeItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(root.PropertyItems, "KnownStorageIdsToReset");
                    if (removeItem == null)
                    {
                        removeItem = new CategoryPropertyItem();
                        removeItem.Id = "KnownStorageIdsToReset";
                        root.PropertyItems.Add(removeItem);
                    }

                    CategoryPropertyItem storeItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(removeItem.PropertyItems, storageId);
                    if (storeItem == null)
                    {
                        storeItem = new CategoryPropertyItem();
                        storeItem.Id = storageId;
                        removeItem.PropertyItems.Add(storeItem);
                        try
                        {
                            result = new HibernateStorageProvider<T>(storageId, dataFormatter, compressContent);
                        }
                        finally
                        {
                            removeItem.PropertyItems.Remove(storeItem);
                        }
                    }
                }
                else
                {
                    throw;
                }
            }

            return result;
        }

        /// <summary>
        /// Fails the safe start storage provider.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storageId">The storage identifier.</param>
        /// <param name="configItem">The configuration item.</param>
        /// <param name="enableReset">if set to <c>true</c> [enable reset].</param>
        /// <param name="hasError">if set to <c>true</c> [has error].</param>
        /// <returns></returns>
        public static HibernateStorageProvider<T> FailSafeStartStorageProvider<T>(string storageId, CategoryPropertyItem configItem, bool enableReset, out bool hasError)
        {
            HibernateStorageProvider<T> result = null;
            hasError = false;

            try
            {
                result = new HibernateStorageProvider<T>(storageId, configItem);
                IEnumeratorSpecialized<T> iterator = result.GetEnumerator();
                while (iterator.MoveNext())
                {
                    // check data consistency
                }
            }
            catch (Exception)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("NHIBERNATE_STORAGE_PROVIDER, failed to initialize storage provider: '{0}'. Reset allowed: {1}", storageId, enableReset.ToString()));
                hasError = true;
                if (enableReset)
                {
                    // reset the content of the storage provider
                    CategoryPropertyItem root = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, "NHibernateProvider");
                    CategoryPropertyItem removeItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(root.PropertyItems, "KnownStorageIdsToReset");
                    if (removeItem == null)
                    {
                        removeItem = new CategoryPropertyItem();
                        removeItem.Id = "KnownStorageIdsToReset";
                        root.PropertyItems.Add(removeItem);
                    }

                    CategoryPropertyItem storeItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(removeItem.PropertyItems, storageId);
                    if (storeItem == null)
                    {
                        storeItem = new CategoryPropertyItem();
                        storeItem.Id = storageId;
                        removeItem.PropertyItems.Add(storeItem);
                        try
                        {
                            result = new HibernateStorageProvider<T>(storageId, configItem);
                        }
                        finally
                        {
                            removeItem.PropertyItems.Remove(storeItem);
                        }
                    }
                }
                else
                {
                    throw;
                }
            }

            return result;
        }

    }

}
