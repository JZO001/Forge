/* *********************************************************************
 * Date: 15 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Represents the type of a descriptor
    /// </summary>
    [Serializable]
    public enum DescriptorTypeEnum
    {
        /// <summary>
        /// Represents an executable or a DLL descriptor
        /// </summary>
        Assembly = 0,

        /// <summary>
        /// Represents a native DLL
        /// </summary>
        NonManagedAssembly,

        /// <summary>
        /// Represents a certificate file
        /// </summary>
        Certificate,

        /// <summary>
        /// Represents a registry entry
        /// </summary>
        Registry,

        /// <summary>
        /// Represents a configuration
        /// </summary>
        Configuration,

        /// <summary>
        /// Represents a custom definied user descriptor
        /// </summary>
        Custom
    }

}
