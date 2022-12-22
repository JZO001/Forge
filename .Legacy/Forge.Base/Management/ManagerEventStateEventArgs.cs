/* *********************************************************************
 * Date: 22 Jan 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Management
{

    /// <summary>
    /// Represents the state of an event
    /// </summary>
    [Serializable]
    public class ManagerEventStateEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerEventStateEventArgs" /> class.
        /// </summary>
        /// <param name="eventState">State of the event.</param>
        public ManagerEventStateEventArgs(ManagerEventStateEnum eventState)
        {
            this.EventState = eventState;
        }

        /// <summary>
        /// Gets the state of the event.
        /// </summary>
        /// <value>
        /// The state of the event.
        /// </value>
        public ManagerEventStateEnum EventState { get; private set; }

    }

}
