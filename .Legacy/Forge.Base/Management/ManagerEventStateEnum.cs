/* *********************************************************************
 * Date: 22 Jan 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Management
{

    /// <summary>
    /// Represents an event state
    /// </summary>
    [Serializable]
    public enum ManagerEventStateEnum
    {
        /// <summary>
        /// Before the event
        /// </summary>
        Before,

        /// <summary>
        /// After the event
        /// </summary>
        After
    }

}
