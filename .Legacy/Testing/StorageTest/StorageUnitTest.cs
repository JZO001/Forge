using System;
using System.Configuration;
using Forge.Configuration.Shared;
using Forge.Persistence.StorageProviders.ConfigSection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.StorageTest
{
    [TestClass]
    public class StorageUnitTest
    {
        #region Fields

        string name = "Storage_Network_send";

        string id = string.Empty;

        string entryValue = string.Empty;

        #endregion

        #region Constructor

        public StorageUnitTest()
        {
            Forge.Logging.Log4net.Log4NetManager.InitializeFromAppConfig();
        }

        #endregion

        #region Test Method

        [TestMethod]
        public void StorageTestMethod()
        {
            foreach (CategoryPropertyItem item in StorageConfiguration.Settings.CategoryPropertyItems)
            {
                if (name == item.EntryName)
                {
                    item.Id = Guid.NewGuid().ToString();
                    item.EntryValue = Guid.NewGuid().ToString();

                    id = item.Id;
                    entryValue = item.EntryValue;
                    break;
                }
            }
            StorageConfiguration.Save(ConfigurationSaveMode.Modified);

            AppDomainSetup domainInfo = new AppDomainSetup();
            domainInfo.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            domainInfo.ConfigurationFile = StorageConfiguration.SectionHandler.DefaultConfigurationFile;
            domainInfo.ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;

            AppDomain domain = AppDomain.CreateDomain("StorageUnitTest", AppDomain.CurrentDomain.Evidence, domainInfo);

            StorageProxy storageProxy = (StorageProxy)domain.CreateInstanceAndUnwrap(typeof(StorageProxy).Assembly.FullName, typeof(StorageProxy).FullName);
            storageProxy.ProxyTest(id, entryValue);
        }

        [TestMethod]
        public void StorageTestMethod2()
        {
            AppDomainSetup domainInfo = new AppDomainSetup();
            domainInfo.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            domainInfo.ConfigurationFile = StorageConfiguration.SectionHandler.DefaultConfigurationFile;
            domainInfo.ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;

            AppDomain domain = AppDomain.CreateDomain("StorageUnitTest", AppDomain.CurrentDomain.Evidence, domainInfo);

            StorageProxy storageProxy = (StorageProxy)domain.CreateInstanceAndUnwrap(typeof(StorageProxy).Assembly.FullName, typeof(StorageProxy).FullName);

            storageProxy.Initialize();

            foreach (CategoryPropertyItem item in StorageConfiguration.Settings.CategoryPropertyItems)
            {
                if (name == item.EntryName)
                {
                    item.Id = Guid.NewGuid().ToString();
                    item.EntryValue = Guid.NewGuid().ToString();

                    id = item.Id;
                    entryValue = item.EntryValue;
                    storageProxy.SetupVlaue(item.Id, item.EntryValue);
                    break;
                }
            }
            StorageConfiguration.Save(ConfigurationSaveMode.Modified);
        }

        [TestMethod]
        public void StorageTestMethod3()
        {
            string id = string.Empty;
            string otherID = string.Empty;

            foreach (CategoryPropertyItem item in StorageConfiguration.Settings.CategoryPropertyItems)
            {
                id = item.Id;
            }

            foreach (CategoryPropertyItem item in StorageConfiguration.Settings.CategoryPropertyItems)
            {
                otherID = item.EntryName;
            }
            
            Assert.AreNotEqual(id, otherID);
        }

        [TestMethod]
        public void StorageTestMethod4()
        {
            string id = string.Empty;
            CategoryPropertyItem entryNameCategoryPropertyItem;
            string configPath = "HibernateProvider/HibernateProperties/Default/cache.use_second_level_cache";
            string entryName = string.Empty;

            entryNameCategoryPropertyItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);
            entryName = entryNameCategoryPropertyItem.EntryName;
            id = ConfigurationAccessHelper.GetValueByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);


            Assert.IsNotNull(entryName);
            Assert.AreNotEqual(entryName, id);
        }

        [TestMethod]
        public void StorageTestMethod5()
        {
            string id = string.Empty;
            string otherID = string.Empty;
            string configPath = "HibernateProvider/HibernateProperties/Default/dialect";
            string configPath2 = "HibernateProvider/HibernateProperties/Default/connection.provider";

            CategoryPropertyItem categoryPropertyItem = new CategoryPropertyItem();
            
            id = ConfigurationAccessHelper.GetValueByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);
            otherID = ConfigurationAccessHelper.GetValueByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath2);

            Assert.IsNotNull(id);
            Assert.IsNotNull(otherID);
            Assert.AreNotEqual(id, otherID);
        }

        [TestMethod]
        public void StorageTestMethod6()
        {
            string id = string.Empty;
            CategoryPropertyItem entryNameCategoryPropertyItem;
            string entryName = string.Empty;
            string configPath = "HibernateProvider/HibernateProperties/Default/dialect";
            
            
            id = ConfigurationAccessHelper.GetValueByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);
            entryNameCategoryPropertyItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);
            entryName = entryNameCategoryPropertyItem.EntryName;

            Assert.IsNotNull(id);
            Assert.IsNotNull(entryName);
            Assert.AreNotEqual(entryName, id);          
        }

        [TestMethod]
        public void StorageTestMethod7()
        {
            CategoryPropertyItem entryNameCategoryPropertyItem;
            CategoryPropertyItem entryNameCategoryPropertyItem2;
            string entryName = string.Empty;
            string entryName2 = string.Empty;
            string configPath = "HibernateProvider/HibernateProperties/Default/show_sql";
            string configPath2 = "HibernateProvider/HibernateProperties/Default/cache.use_query_cache";

            entryNameCategoryPropertyItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);
            entryName = entryNameCategoryPropertyItem.EntryName;
            entryNameCategoryPropertyItem2 = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath2);
            entryName2 = entryNameCategoryPropertyItem2.EntryName;
            
            Assert.IsNotNull(entryNameCategoryPropertyItem);
            Assert.IsNotNull(entryNameCategoryPropertyItem2);
            Assert.IsNotNull(entryName);
            Assert.IsNotNull(entryName2);
            Assert.AreNotEqual(entryName, entryName2);
        }

        [TestMethod]
        public void StorageTestMethod8()
        {
            string configPath = "HibernateProvider/HibernateProperties/Default/dialect";
            string configPath2 = "HibernateProvider/HibernateProperties/Default/connection.provider";

            CategoryPropertyItem id = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);
            CategoryPropertyItem otherId = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath2);

            Assert.IsNotNull(id);
            Assert.IsNotNull(otherId);
            Assert.AreNotEqual(id, otherId);
        }

        [TestMethod]
        public void StorageTestMethod9()
        {
            string configPath = "HibernateProvider/HibernateProperties/Default/dialect";

            CategoryPropertyItem id = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);
            CategoryPropertyItem otherId = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);

            Assert.IsNotNull(id);
            Assert.IsNotNull(otherId);
            Assert.AreEqual(id, otherId);
        }

        [TestMethod]
        public void StorageTestMethod10()
        {
            string id = string.Empty;
            string id2 = string.Empty;
            string configPath = "HibernateProvider/HibernateProperties/Default/dialect";
            string configPath2 = "HibernateProvider/HibernateProperties/Default/max_fetch_depth";
            
            CategoryPropertyItem idCategoryPropertyItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);
            id = idCategoryPropertyItem.Id;
            CategoryPropertyItem idCategoryPropertyItem2 = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath2);
            id2 = idCategoryPropertyItem2.Id;

            Assert.IsNotNull(id);
            Assert.IsNotNull(id2);
            Assert.IsNotNull(idCategoryPropertyItem);
            Assert.IsNotNull(idCategoryPropertyItem2);
            Assert.AreNotEqual(id, id2);
        }

        [TestMethod]
        public void StorageTestMethod11()
        {
            string id = string.Empty;
            CategoryPropertyItem idCategoryPropertyItem;
            string entryName;
            CategoryPropertyItem entryNameCategoryPropertyItem;

            string configPath = "HibernateProvider/HibernateProperties/Default/max_fetch_depth";

            idCategoryPropertyItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);
            id = idCategoryPropertyItem.Id;

            entryNameCategoryPropertyItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);
            entryName = entryNameCategoryPropertyItem.EntryName;

            Assert.IsNotNull(entryName);
            Assert.IsNotNull(id);
            Assert.AreNotEqual(id, entryName);

        }

        [TestMethod]
        public void StorageTestMethod12()
        {
            string id = string.Empty;
            CategoryPropertyItem idCategoryPropertyItem;
            string entryName = string.Empty;
            CategoryPropertyItem entryNameCategoryPropertyItem;

            string configPath = "HibernateProvider/HibernateProperties/Default/connection.isolation";

            idCategoryPropertyItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);
            id = idCategoryPropertyItem.Id;

            entryNameCategoryPropertyItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, configPath);
            entryName = entryNameCategoryPropertyItem.EntryName;

            Assert.IsNotNull(entryName);
            Assert.IsNotNull(id);
            Assert.AreEqual(id, entryName);
        }

        #endregion
    }
}
