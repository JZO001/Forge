/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.Configuration;

namespace Forge.Net.TerraGraf.Contexts
{

    /// <summary>
    /// Representation the context of a peer
    /// </summary>
    [Serializable]
    public sealed class NetworkPeerDataContext : IPropertyItem, ICloneable
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, PropertyItem> mPropertyItems = new Dictionary<string, PropertyItem>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPeerDataContext"/> class.
        /// </summary>
        public NetworkPeerDataContext()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the property items.
        /// </summary>
        [DebuggerHidden]
        public Dictionary<string, PropertyItem> PropertyItems
        {
            get { return mPropertyItems; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets the value by path.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <param name="configPath">The config path.</param>
        /// <returns>The value</returns>
        public static String GetValueByPath(IPropertyItem pi, String configPath)
        {
            if (pi == null)
            {
                throw new ArgumentNullException("pi");
            }
            if (string.IsNullOrEmpty("configPath"))
            {
                throw new ArgumentNullException("configPath");
            }

            List<String> keys = new List<String>();
            keys.AddRange(configPath.Split(new string[] { "/" }, StringSplitOptions.None));
            String result = FindValueByKey(pi, keys);
            keys.Clear();
            return result;
        }

        /// <summary>
        /// Gets the property item by path.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <param name="configPath">The config path.</param>
        /// <returns>The property item</returns>
        public static PropertyItem GetPropertyItemByPath(IPropertyItem pi, String configPath)
        {
            if (pi == null)
            {
                throw new ArgumentNullException("pi");
            }
            if (string.IsNullOrEmpty(configPath))
            {
                throw new ArgumentNullException("configPath");
            }

            List<String> keys = new List<String>();
            keys.AddRange(configPath.Split(new string[] { "/" }, StringSplitOptions.None));
            PropertyItem result = FindPropertyItemByKey(pi, keys);
            keys.Clear();
            return result;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            NetworkPeerDataContext cloned = new NetworkPeerDataContext();

            if (mPropertyItems.Count > 0)
            {
                foreach (KeyValuePair<string, PropertyItem> kv in mPropertyItems)
                {
                    cloned.mPropertyItems.Add(kv.Key, kv.Value == null ? null : (PropertyItem)kv.Value.Clone());
                }
            }

            return cloned;
        }

        #endregion

        #region Private method(s)

        private static String FindValueByKey(IPropertyItem pi, List<String> keys)
        {
            String result = null;
            if (pi.PropertyItems.ContainsKey(keys[0]))
            {
                PropertyItem item = pi.PropertyItems[keys[0]];
                if (keys.Count == 1)
                {
                    result = item.Value;
                }
                else
                {
                    keys.RemoveAt(0);
                    result = FindValueByKey(item, keys);
                }
            }
            return result;
        }

        private static PropertyItem FindPropertyItemByKey(IPropertyItem pi, List<String> keys)
        {
            PropertyItem result = null;
            if (pi.PropertyItems.ContainsKey(keys[0]))
            {
                PropertyItem item = pi.PropertyItems[keys[0]];
                if (keys.Count == 1)
                {
                    result = item;
                }
                else
                {
                    keys.RemoveAt(0);
                    result = FindPropertyItemByKey(item, keys);
                }
            }
            return result;
        }

        #endregion

    }

}
