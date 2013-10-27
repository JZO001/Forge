/* *********************************************************************
 * Date: 22 Jan 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Management
{

    /// <summary>
    /// Represents the state of a manager
    /// </summary>
    [Serializable]
    public enum ManagerStateEnum
    {
        /// <summary>
        /// The manager is uninitialized
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        /// The manager is started
        /// </summary>
        Started,

        /// <summary>
        /// The manager is stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// The manager is in fault state
        /// </summary>
        Fault
    }

}
