/* *********************************************************************
 * Date: 11 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Services.Locators
{

    /// <summary>
    /// Represents the arguments of an event
    /// </summary>
    [Serializable]
    public class ServiceStateChangedEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        public ServiceStateChangedEventArgs(ServiceStateEnum state)
        {
            ServiceState = state;
        }

        /// <summary>
        /// Gets the state of the service.
        /// </summary>
        /// <value>
        /// The state of the service.
        /// </value>
        public ServiceStateEnum ServiceState { get; private set; }

    }

}
