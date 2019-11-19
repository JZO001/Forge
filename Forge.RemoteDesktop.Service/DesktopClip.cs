/* *********************************************************************
 * Date: 13 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace Forge.RemoteDesktop.Service
{

    /// <summary>
    /// Represents the state of a desktop clip
    /// </summary>
    [DebuggerDisplay("[{GetType()}, X = {Location.X}, Y = {Location.Y}, Width = {Size.Width}, Height = {Size.Height}]")]
    public sealed class DesktopClip
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopClip" /> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="size">The size.</param>
        internal DesktopClip(Point location, Size size)
        {
            if (location == null)
            {
                ThrowHelper.ThrowArgumentNullException("location");
            }
            this.Location = location;
            this.Size = size;
            this.SubscribedClients = new Dictionary<IRemoteDesktopInternalClient, ClientContext>();
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        internal Point Location { get; private set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        internal Size Size { get; private set; }

        /// <summary>
        /// Gets or sets the maximum quality.
        /// </summary>
        /// <value>
        /// The maximum quality.
        /// </value>
        internal byte[] MaxQuality { get; set; }

        /// <summary>
        /// Gets or sets the last content.
        /// </summary>
        /// <value>
        /// The last content.
        /// </value>
        internal byte[] LastContent { get; set; }

        /// <summary>
        /// Gets or sets the CRC of the last content.
        /// </summary>
        /// <value>
        /// The CRC.
        /// </value>
        internal byte[] CRC { get; set; }

        /// <summary>
        /// Gets the subscribed clients.
        /// </summary>
        /// <value>
        /// The subscribed clients.
        /// </value>
        internal Dictionary<IRemoteDesktopInternalClient, ClientContext> SubscribedClients { get; private set; }

        /// <summary>
        /// Determines whether [is information area] [the specified area].
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        internal bool IsInArea(Area area)
        {
            return area.StartX <= (Location.X + Size.Width) && area.EndX >= Location.X &&
                area.StartY <= (Location.Y + Size.Height) && area.EndY >= Location.Y;
        }

    }

    internal sealed class ClientContext
    {

        internal ClientContext(IRemoteDesktopInternalClient client, byte[] contentToSend, AutoResetEvent workerThreadEvent)
        {
            this.Client = client;
            this.IsChanged = true;
            this.ContentToSend = contentToSend;
            this.WorkerThreadEvent = workerThreadEvent;
        }

        internal IRemoteDesktopInternalClient Client { get; private set; }

        internal bool IsChanged { get; set; }

        internal bool IsRequestToResend { get; set; }

        internal byte[] ContentToSend { get; set; }

        internal AutoResetEvent WorkerThreadEvent { get; private set; }

    }

}
