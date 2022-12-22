/* *********************************************************************
 * Date: 15 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

#if NETCOREAPP3_1_OR_GREATER
#else

using System;
using Microsoft.Win32;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Represents a registry item
    /// </summary>
    [Serializable]
    public class RegistryItemDescriptor : DescriptorBase
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryItemDescriptor"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="key">The key.</param>
        /// <param name="valueKey">The value key.</param>
        public RegistryItemDescriptor(RegistryDescriptor parent, RegistryKey key, string valueKey)
            : base(string.Format("{0}\\{1}", key.Name, valueKey), DescriptorTypeEnum.Registry)
        {
            this.Parent = parent;
            this.ValueKind = key.GetValueKind(valueKey);
            this.Value = key.GetValue(valueKey);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the parent.
        /// </summary>
        public RegistryDescriptor Parent { get; private set; }

        /// <summary>
        /// Gets the kind of the value.
        /// </summary>
        /// <value>
        /// The kind of the value.
        /// </value>
        public RegistryValueKind ValueKind { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; private set; }

        #endregion

    }

}

#endif
