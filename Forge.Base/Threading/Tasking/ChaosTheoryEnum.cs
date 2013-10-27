/* *********************************************************************
 * Date: 28 Feb 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Threading.Tasking
{

    /// <summary>
    /// Represents the order mode of the Task Manager
    /// </summary>
    [Serializable]
    public enum ChaosTheoryEnum
    {

        /// <summary>
        /// Preemtive
        /// </summary>
        Chaos = 0,

        /// <summary>
        /// Tasks ordered by task delegate target
        /// </summary>
        OrderByTaskDelegateTarget,

        /// <summary>
        /// Tasks ordered by return delegate target
        /// </summary>
        OrderByReturnDelegateTarget

    }

}
