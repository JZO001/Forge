/* *********************************************************************
 * Date: 13 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;
using Forge.RemoteDesktop.Contracts;

namespace Forge.RemoteDesktop.Service
{

    /// <summary>
    /// Represents the internal service methods of a client
    /// </summary>
    internal interface IRemoteDesktopInternalClient : IRemoteDesktop
    {

        /// <summary>
        /// Gets or sets a value indicating whether [is active].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is active]; otherwise, <c>false</c>.
        /// </value>
        new bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is accepted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is accepted]; otherwise, <c>false</c>.
        /// </value>
        bool IsAccepted { get; set; }

        /// <summary>
        /// Gets the image quality.
        /// </summary>
        /// <value>
        /// The image quality.
        /// </value>
        int ImageQuality { get; set; }

        /// <summary>
        /// Gets or sets the subscribed clips.
        /// </summary>
        /// <value>
        /// The subscribed clips.
        /// </value>
        HashSet<DesktopClip> SubscribedClips { get; }

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

    }

}
