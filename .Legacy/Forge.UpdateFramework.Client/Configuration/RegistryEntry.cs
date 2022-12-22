/* *********************************************************************
 * Date: 16 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

#if NETCOREAPP3_1_OR_GREATER
#else

using System;
using Microsoft.Win32;

namespace Forge.UpdateFramework.Client.Configuration
{

    /// <summary>
    /// Represents a registry path
    /// </summary>
    [Serializable]
    internal class RegistryEntry
    {

        /// <summary>
        /// Gets or sets the hive.
        /// </summary>
        /// <value>
        /// The hive.
        /// </value>
        internal RegistryHive Hive { get; set; }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        internal RegistryView View { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        internal string Path { get; set; }

    }

}

#endif
