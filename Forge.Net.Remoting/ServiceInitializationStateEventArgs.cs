/* *********************************************************************
 * Date: 15 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Remoting
{

    /// <summary>
    /// Service initialization state event args
    /// </summary>
    [Serializable]
    public class ServiceInitializationStateEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInitializationStateEventArgs"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        public ServiceInitializationStateEventArgs(ServiceInitializationStateEnum state)
        {
            this.State = state;
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public ServiceInitializationStateEnum State { get; private set; }

    }

}
