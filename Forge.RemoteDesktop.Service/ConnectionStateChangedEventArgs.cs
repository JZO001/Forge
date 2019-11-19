/* *********************************************************************
 * Date: 13 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.RemoteDesktop.Contracts;

namespace Forge.RemoteDesktop.Service
{

    /// <summary>
    /// Represents a client connection state change
    /// </summary>
    [Serializable]
    public class ConnectionStateChangedEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStateChangedEventArgs" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="connected">if set to <c>true</c> [connected].</param>
        public ConnectionStateChangedEventArgs(IRemoteDesktopPeer client, bool connected)
        {
            if (client == null)
            {
                ThrowHelper.ThrowArgumentNullException("client");
            }
            this.Client = client;
            this.IsConnected = connected;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public IRemoteDesktopPeer Client { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; private set; }

    }

}
