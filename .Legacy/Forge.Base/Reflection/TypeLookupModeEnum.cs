/* *********************************************************************
 * Date: 26 Mar 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Reflection
{

    /// <summary>
    /// Represents the type resolve mode for TypeHelper
    /// </summary>
    [Serializable]
    [Flags]
    public enum TypeLookupModeEnum
    {
        /// <summary>
        /// Allow older versions
        /// </summary>
        AllowOlderVersions = 1,

        /// <summary>
        /// Allow exact versions
        /// </summary>
        AllowExactVersions = 2,

        /// <summary>
        /// Allow newer versions
        /// </summary>
        AllowNewerVersions = 4,

        /// <summary>
        /// Allow all
        /// </summary>
        AllowAll = AllowOlderVersions | AllowExactVersions | AllowNewerVersions
    }

}
