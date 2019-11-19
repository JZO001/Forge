/* *********************************************************************
 * Date: 06 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Net.Remoting;
using Forge.Net.Remoting.Channels;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the common methods and events for a remote desktop peer (service/client)
    /// </summary>
    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.PerSession)]
    public interface IRemoteDesktopPeer : IRemoteContract
    {

        /// <summary>
        /// Occurs when the remote peer disconnects this session
        /// </summary>
        event EventHandler<DisconnectEventArgs> Disconnected;

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        Channel Channel { get; }

        /// <summary>
        /// Gets the session id.
        /// </summary>
        /// <value>
        /// The session id.
        /// </value>
        string SessionId { get; }

        /// <summary>
        /// Gets the remote host.
        /// </summary>
        /// <value>
        /// The remote host.
        /// </value>
        string RemoteHost { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
        /// </value>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        bool IsActive { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        bool IsDisposed { get; }

    }

}
