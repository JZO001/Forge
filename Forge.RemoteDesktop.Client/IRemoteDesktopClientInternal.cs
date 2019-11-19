/* *********************************************************************
 * Date: 4 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.RemoteDesktop.Contracts;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Represents the client proxy which connects to a remote service with extended internal features
    /// </summary>
    internal interface IRemoteDesktopClientInternal : IRemoteDesktop
    {

        /// <summary>
        /// Gets or sets a value indicating whether [is authenticated].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is authenticated]; otherwise, <c>false</c>.
        /// </value>
        new bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is active].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is active]; otherwise, <c>false</c>.
        /// </value>
        new bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the last known mouse X position.
        /// </summary>
        /// <value>
        /// The last known mouse X position.
        /// </value>
        int LastMousePosX { get; set; }

        /// <summary>
        /// Gets or sets the last known mouse Y position.
        /// </summary>
        /// <value>
        /// The last known mouse Y position.
        /// </value>
        int LastMousePosY { get; set; }

        /// <summary>
        /// Gets or sets the last cursor unique identifier.
        /// </summary>
        /// <value>
        /// The last cursor unique identifier.
        /// </value>
        string LastCursorId { get; set; }

        /// <summary>
        /// Gets or sets the content of the clipboard.
        /// </summary>
        /// <value>
        /// The content of the clipboard.
        /// </value>
        string ClipboardContent { get; set; }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>
        /// The owner.
        /// </value>
        RemoteDesktopWinFormsControl Owner { get; set; }

    }

}
