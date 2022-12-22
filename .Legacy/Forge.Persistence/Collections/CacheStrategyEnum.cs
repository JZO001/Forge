/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Persistence.Collections
{

    /// <summary>
    /// Cache strategy mode
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix"), Serializable]
    public enum CacheStrategyEnum : int
    {
        /// <summary>
        /// Cache the most used items into the memory
        /// </summary>
        RecentlyUsed,

        /// <summary>
        /// Cache the forepart of the collection
        /// </summary>
        CacheForStack,

        /// <summary>
        /// Cache the endpart of the collection
        /// </summary>
        CacheForQueue

    }

}
