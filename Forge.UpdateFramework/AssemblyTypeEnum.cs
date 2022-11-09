/* *********************************************************************
 * Date: 16 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.UpdateFramework
{

    /// <summary>
    /// Represents the type of an assembly
    /// </summary>
    [Serializable]
    public enum AssemblyTypeEnum
    {
        /// <summary>
        /// .NET Assembly
        /// </summary>
        Managed = 0,

        /// <summary>
        /// Unmanaged assembly
        /// </summary>
        Native
    }

}
