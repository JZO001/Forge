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
    public sealed class NetworkPeerDataContext : ICloneable
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

    }

}
