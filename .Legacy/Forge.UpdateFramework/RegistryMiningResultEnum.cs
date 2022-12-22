/* *********************************************************************
 * Date: 15 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Represents the result of a registry key mining
    /// </summary>
    [Serializable]
    public enum RegistryMiningResultEnum
    {
        /// <summary>
        /// Key successfully read
        /// </summary>
        Success = 0,

        /// <summary>
        /// Key not found
        /// </summary>
        NotFound,

        /// <summary>
        /// Unable to access the specified registry key
        /// </summary>
        AccessDenied

    }

}
