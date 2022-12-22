/* *********************************************************************
 * Date: 6 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.RemoteDesktop.Contracts;
using Forge.Shared;

namespace Forge.RemoteDesktop.Service
{

    /// <summary>
    /// Represents an accepted client
    /// </summary>
    [Serializable]
    public class AcceptClientEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AcceptClientEventArgs"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public AcceptClientEventArgs(IRemoteDesktopPeer client)
        {
            if (client == null)
            {
                ThrowHelper.ThrowArgumentNullException("client");
            }
            Client = client;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public IRemoteDesktopPeer Client { get; private set; }

    }

}
