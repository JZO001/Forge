/* *********************************************************************
 * Date: 07 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.ComponentModel;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Represents a client side proxy and the client side business logic
    /// </summary>
    public class RemoteDesktopClientImpl : Forge.RemoteDesktop.Client.RemoteDesktopAbstractClientProxy, IRemoteDesktopClientInternal
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDesktopClientImpl"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sessionId">The session id.</param>
        public RemoteDesktopClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) 
        {
            this.LastCursorId = string.Empty;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>
        /// The owner.
        /// </value>
        [Browsable(false)]
        public RemoteDesktopWinFormsControl Owner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last mouse executable.
        /// </summary>
        /// <value>
        /// The last mouse executable.
        /// </value>
        public int LastMousePosX
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last mouse asynchronous.
        /// </summary>
        /// <value>
        /// The last mouse asynchronous.
        /// </value>
        public int LastMousePosY
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last cursor unique identifier.
        /// </summary>
        /// <value>
        /// The last cursor unique identifier.
        /// </value>
        public string LastCursorId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content of the clipboard.
        /// </summary>
        /// <value>
        /// The content of the clipboard.
        /// </value>
        public string ClipboardContent
        {
            get;
            set;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Start the event pump.
        /// </summary>
        /// <returns></returns>
        public override Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs ClientStartEventPump()
        {
            Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs result = base.ClientStartEventPump();
            this.IsActive = true;
            return result;
        }

        /// <summary>
        /// Stops the event pump.
        /// </summary>
        public override void ClientStopEventPump()
        {
            base.ClientStopEventPump();
            this.IsActive = false;
        }

        /// <summary>
        /// Services the send mouse move event.
        /// </summary>
        /// <param name="e">The <see cref="Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs" /> instance containing the event data.</param>
        public override void ServiceSendMouseMoveEvent(Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs e)
        {
            if (this.IsActive)
            {
                Owner.SendMouseMoveEvent(this, e);
            }
        }

        /// <summary>
        /// Services the send desktop image clip.
        /// </summary>
        /// <param name="e">The _P0.</param>
        public override void ServiceSendDesktopImageClip(Forge.RemoteDesktop.Contracts.DesktopImageClipArgs e)
        {
            if (this.IsActive)
            {
                Owner.SendDesktopImageClip(this, e);
            }
        }

        /// <summary>
        /// Services the content of the send clipboard.
        /// </summary>
        /// <param name="e">The <see cref="Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs"/> instance containing the event data.</param>
        public override void ServiceSendClipboardContent(Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs e)
        {
            if (this.IsActive && string.Compare(e.Text, this.ClipboardContent) != 0)
            {
                Owner.SendClipboardContent(this, e);
            }
        }

        #endregion

    }

}
