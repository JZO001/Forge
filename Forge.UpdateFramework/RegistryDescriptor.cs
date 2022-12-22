/* *********************************************************************
 * Date: 15 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

#if NETCOREAPP3_1_OR_GREATER
#else

using System;
using System.Collections.Generic;
using Forge.Shared;
using Microsoft.Win32;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Represents a registry key
    /// </summary>
    [Serializable]
    public class RegistryDescriptor : DescriptorBase
    {

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="RegistryDescriptor"/> class from being created.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="hive">The hive.</param>
        /// <param name="view">The view.</param>
        private RegistryDescriptor(string id, RegistryHive hive, RegistryView view)
            : base(id, DescriptorTypeEnum.Registry)
        {
            this.Hive = hive;
            this.View = view;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryDescriptor" /> class.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="hive">The hive.</param>
        /// <param name="view">The view.</param>
        /// <param name="miningResult">The mining result.</param>
        public RegistryDescriptor(string id, RegistryHive hive, RegistryView view, RegistryMiningResultEnum miningResult)
            : this(id, hive, view)
        {
            this.MiningResult = miningResult;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryDescriptor"/> class.
        /// </summary>
        /// <param name="hive">The hive.</param>
        /// <param name="view">The view.</param>
        /// <param name="key">The key.</param>
        public RegistryDescriptor(RegistryHive hive, RegistryView view, RegistryKey key)
            : this(key.Name, hive, view, RegistryMiningResultEnum.Success)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException("key");
            }

            this.Name = key.Name;

            if (key.SubKeyCount > 0)
            {
                this.SubKeys = new Dictionary<string, RegistryDescriptor>();
                foreach (string subKey in key.GetSubKeyNames())
                {
                    try
                    {
                        using (RegistryKey subRegistryKey = key.OpenSubKey(subKey, false))
                        {
                            this.SubKeys.Add(subKey, new RegistryDescriptor(hive, view, subRegistryKey));
                        }
                    }
                    catch (Exception)
                    {
                        this.SubKeys.Add(subKey, new RegistryDescriptor(string.Format("{0}\\{1}", key.Name, subKey), hive, view, RegistryMiningResultEnum.AccessDenied));
                    }
                }
            }

            if (key.GetValueNames().Length > 0)
            {
                this.Values = new Dictionary<string, RegistryItemDescriptor>();
                foreach (string valueKey in key.GetValueNames())
                {
                    this.Values.Add(valueKey, new RegistryItemDescriptor(this, key, valueKey));
                }
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the mining result.
        /// </summary>
        public RegistryMiningResultEnum MiningResult { get; private set; }

        /// <summary>
        /// Gets the hive.
        /// </summary>
        public RegistryHive Hive { get; private set; }

        /// <summary>
        /// Gets the view.
        /// </summary>
        public RegistryView View { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the sub keys.
        /// </summary>
        public Dictionary<string, RegistryDescriptor> SubKeys { get; private set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public Dictionary<string, RegistryItemDescriptor> Values { get; private set; }

        #endregion

    }

}

#endif