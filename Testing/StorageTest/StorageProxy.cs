using System;
using Forge.Configuration.Shared;
using Forge.Persistence.StorageProviders.ConfigSection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing.StorageTest
{
    class StorageProxy : MBRBase
    {
        #region Fields

        public string id = string.Empty; 

        public string entryValue = string.Empty;

        #endregion

        #region Constructor

        public StorageProxy()
        {

        }

        #endregion

        #region Initialize

        public void Initialize()
        {
            StorageConfiguration.SectionHandler.OnConfigurationChanged += new EventHandler<EventArgs>(SectionHandler_OnConfigurationChanged);
            StorageConfiguration.Refresh();
        }

        #endregion

        #region Event

        void SectionHandler_OnConfigurationChanged(object sender, EventArgs e)
        {
            foreach (CategoryPropertyItem item in StorageConfiguration.Settings.CategoryPropertyItems)
            {
                string name = "Storage_Network_send";
                if (name == item.EntryName)
                {
                    Assert.AreEqual(item.Id, id);
                    Assert.AreEqual(item.EntryValue, entryValue);
                    break;
                }
            }
        }

        #endregion

        #region Public Methods

        public void SetupVlaue(string Id, string EntryValue)
        {
            id = Id;
            entryValue = EntryValue;
        }

        public void ProxyTest(string Id, string EntryValue)
        {
            foreach (CategoryPropertyItem item in StorageConfiguration.Settings.CategoryPropertyItems)
            {
                string name = "Storage_Network_send";
                if (name == item.EntryName)
                {
                    Assert.AreEqual(item.Id, id);
                    Assert.AreEqual(item.EntryValue, entryValue);
                    break;
                }
            }
        }

        #endregion
    }
}
