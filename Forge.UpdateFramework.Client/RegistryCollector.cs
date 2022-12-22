/* *********************************************************************
 * Date: 16 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

#if NETCOREAPP3_1_OR_GREATER
#else

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using Forge.Configuration.Shared;
using Forge.UpdateFramework.Client.Configuration;
using Microsoft.Win32;
using Forge.Legacy;
using Forge.Shared;
using Forge.Configuration;

namespace Forge.UpdateFramework.Client
{

    /// <summary>
    /// Collects registry information from the specified location
    /// </summary>
    public class RegistryCollector : MBRBase, IDataCollector
    {

#region Field(s)

        private static readonly string REGISTRY_HIVE = "Hive";
        private static readonly string REGISTRY_VIEW = "View";

        private readonly Dictionary<string, RegistryEntry> mRegistryEntries = new Dictionary<string, RegistryEntry>();

#endregion

#region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryCollector"/> class.
        /// </summary>
        public RegistryCollector()
        {
        }

#endregion

#region Public properties

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; private set; }

#endregion

#region Public method(s)

        /// <summary>
        /// Initializes the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="propertyItem">The configuration.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize(IPropertyItem configuration)
        {
            if (configuration == null)
            {
                ThrowHelper.ThrowArgumentNullException("configuration");
            }

            if (!IsInitialized)
            {
                foreach (IPropertyItem item in configuration.Items.Values)
                {
                    RegistryHive hive = RegistryHive.CurrentUser;
                    RegistryView view = RegistryView.Default;

                    ConfigurationAccessHelper.ParseEnumValue<RegistryHive>(item, REGISTRY_HIVE, ref hive);
                    ConfigurationAccessHelper.ParseEnumValue<RegistryView>(item, REGISTRY_VIEW, ref view);

                    mRegistryEntries[item.Id] = new RegistryEntry() { Path = item.Id, Hive = hive, View = view };
                }

                IsInitialized = true;
            }
        }

        /// <summary>
        /// Collects the specified update data.
        /// </summary>
        /// <param name="updateData">The update data.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Collect(Dictionary<string, DescriptorBase> updateData)
        {
            if (updateData == null)
            {
                ThrowHelper.ThrowArgumentNullException("updateData");
            }
            if (!IsInitialized)
            {
                throw new InitializationException("Collector has not been initialized.");
            }

            foreach (KeyValuePair<string, RegistryEntry> kv in mRegistryEntries)
            {
                RegistryKey tempKey = null;
                try
                {
                    tempKey = RegistryKey.OpenBaseKey(kv.Value.Hive, kv.Value.View);
                    string[] tree = kv.Key.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in tree)
                    {
                        try
                        {
                            tempKey = tempKey.OpenSubKey(item, false);
                            if (tempKey == null)
                            {
                                updateData[kv.Key] = new RegistryDescriptor(kv.Key, kv.Value.Hive, kv.Value.View, RegistryMiningResultEnum.NotFound);
                                break;
                            }
                        }
                        catch (SecurityException)
                        {
                            updateData[kv.Key] = new RegistryDescriptor(kv.Key, kv.Value.Hive, kv.Value.View, RegistryMiningResultEnum.AccessDenied);
                        }
                    }
                    if (tempKey != null)
                    {
                        updateData[kv.Key] = new RegistryDescriptor(kv.Value.Hive, kv.Value.View, tempKey);
                    }
                }
                catch (Exception)
                {
                    updateData[kv.Key] = new RegistryDescriptor(kv.Key, kv.Value.Hive, kv.Value.View, RegistryMiningResultEnum.AccessDenied);
                }
            }
        }

#endregion

    }

}

#endif
