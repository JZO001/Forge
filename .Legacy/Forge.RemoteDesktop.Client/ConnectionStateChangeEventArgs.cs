/* *********************************************************************
 * Date: 4 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Represents the connection state
    /// </summary>
    [Serializable]
    public class ConnectionStateChangeEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStateChangeEventArgs"/> class.
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        public ConnectionStateChangeEventArgs(bool state)
        {
            this.IsConnected = state;
        }

        /// <summary>
        /// Gets a value indicating whether [is connected].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is connected]; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; private set; }

    }

}
