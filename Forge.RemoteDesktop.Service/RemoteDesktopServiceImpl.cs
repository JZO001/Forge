/* *********************************************************************
 * Date: 07 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using Forge.Logging.Abstraction;
using Forge.RemoteDesktop.Contracts;
using Forge.RemoteDesktop.Service.Configuration;
using Forge.Shared;

namespace Forge.RemoteDesktop.Service
{

    /// <summary>
    /// Represents the service side business logic implementation
    /// </summary>
    public class RemoteDesktopServiceImpl : Forge.RemoteDesktop.Service.RemoteDesktopAbstractServiceProxy, IRemoteDesktopInternalClient
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<RemoteDesktopServiceImpl>();

        private int mImageQuality = Settings.DefaultImageClipQuality;

        private int mLoginFailedCounter = 0;

        private static readonly Dictionary<string, DateTime> mBlackList = new Dictionary<string, DateTime>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDesktopServiceImpl"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sessionId">The session id.</param>
        public RemoteDesktopServiceImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId)
            : base(channel, sessionId)
        {
            SubscribedClips = new HashSet<DesktopClip>();
            LastCursorId = string.Empty;
            RemoteDesktopServiceManager.Instance.RegisterNewContract(this);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets a value indicating whether [is accepted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is accepted]; otherwise, <c>false</c>.
        /// </value>
        public bool IsAccepted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public override bool IsActive
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the image quality.
        /// </summary>
        /// <value>
        /// The image quality.
        /// </value>
        public int ImageQuality
        {
            get { return mImageQuality; }
            set
            {
                if (value < 10 || value > 100)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }

                mImageQuality = value;
            }
        }

        /// <summary>
        /// Gets or sets the subscribed clips.
        /// </summary>
        /// <value>
        /// The subscribed clips.
        /// </value>
        public HashSet<DesktopClip> SubscribedClips
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the last known mouse X position.
        /// </summary>
        /// <value>
        /// The last known mouse X position.
        /// </value>
        public int LastMousePosX
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last known mouse Y position.
        /// </summary>
        /// <value>
        /// The last known mouse Y position.
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
        /// Gets the authentication information.
        /// </summary>
        /// <returns></returns>
        public override Forge.RemoteDesktop.Contracts.AuthModeResponseArgs GetAuthenticationInfo()
        {
            return new AuthModeResponseArgs(Settings.AuthenticationMode);
        }

        /// <summary>
        /// Login into the remote service
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public override LoginResponseArgs Login(Forge.RemoteDesktop.Contracts.LoginRequestArgs request)
        {
            if (request == null)
            {
                ThrowHelper.ThrowArgumentNullException("request");
            }

            LoginResponseArgs response = null;

            lock (mBlackList)
            {
                if (mBlackList.ContainsKey(RemoteHost))
                {
                    DateTime blacklistedTime = mBlackList[RemoteHost];
                    if (blacklistedTime.AddMinutes(Settings.BlackListTimeout) < DateTime.Now)
                    {
                        mBlackList.Remove(RemoteHost);
                    }
                    else
                    {
                        if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("REMOTE_DESKTOP_SERVICE_CONTRACT, client blacklisted. SessionId: {0}", SessionId));
                        mChannel.Disconnect(mSessionId);
                        return null;
                    }
                }
            }

            if (RemoteDesktopServiceManager.Instance.ManagerState == Management.ManagerStateEnum.Started)
            {
                if (AuthenticationHandlerModule.CheckAuthenticationInfo(request.UserName, request.Password))
                {
                    // sikeres az auth
                    IsAuthenticated = true;
                    mLoginFailedCounter = 0;
                    if (RemoteDesktopServiceManager.Instance.AcceptUser(this))
                    {
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_SERVICE_CONTRACT, login succeeded. SessionId: {0}", SessionId));
                        response = new LoginResponseArgs(LoginResponseStateEnum.AccessGranted);
                    }
                    else
                    {
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_SERVICE_CONTRACT, login succeeded, but service did not accept new client to serve. Service inactive. SessionId: {0}", SessionId));
                        response = new LoginResponseArgs(LoginResponseStateEnum.ServiceBusy);
                    }
                }
                else
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_SERVICE_CONTRACT, authentication failed. SessionId: {0}", SessionId));
                    response = new LoginResponseArgs(LoginResponseStateEnum.AccessDenied);
                    mLoginFailedCounter++;
                }
            }
            else
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_SERVICE_CONTRACT, login failed. Service inactive. SessionId: {0}", SessionId));
                response = new LoginResponseArgs(LoginResponseStateEnum.ServiceInactive);
                mLoginFailedCounter++;
            }

            if (Settings.MaximumFailedLoginAttempt < mLoginFailedCounter)
            {
                mBlackList[RemoteHost] = DateTime.Now;
                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("REMOTE_DESKTOP_SERVICE_CONTRACT, client blacklisted. SessionId: {0}", SessionId));
                mChannel.Disconnect(mSessionId);
            }

            return response;
        }

        /// <summary>
        /// Gets configuration.
        /// </summary>
        /// <returns></returns>
        public override Forge.RemoteDesktop.Contracts.DescriptionResponseArgs ClientGetConfiguration()
        {
            return RemoteDesktopServiceManager.Instance.GetServiceConfiguration(this);
        }

        /// <summary>
        /// Clients the set image clip quality.
        /// </summary>
        /// <param name="e">The _P0.</param>
        public override void ClientSetImageClipQuality(Forge.RemoteDesktop.Contracts.ImageClipQualityArgs e)
        {
            if (e == null)
            {
                ThrowHelper.ThrowArgumentNullException("e");
            }

            RemoteDesktopServiceManager.Instance.SetImageClipQuality(this, e);
        }

        /// <summary>
        /// Clients the subscribe for desktop.
        /// </summary>
        /// <param name="area">The _P0.</param>
        public override void ClientSubscribeForDesktop(Forge.RemoteDesktop.Area area)
        {
            if (area == null)
            {
                ThrowHelper.ThrowArgumentNullException("area");
            }

            RemoteDesktopServiceManager.Instance.SubscribeForDesktop(this, area);
        }

        /// <summary>
        /// Start the event pump.
        /// </summary>
        public override MouseMoveServiceEventArgs ClientStartEventPump()
        {
            return RemoteDesktopServiceManager.Instance.StartEventPump(this);
        }

        /// <summary>
        /// Stops the event pump.
        /// </summary>
        public override void ClientStopEventPump()
        {
            RemoteDesktopServiceManager.Instance.StopEventPump(this);
        }

        /// <summary>
        /// Clients the refresh desktop.
        /// </summary>
        public override void ClientRefreshDesktop()
        {
            RemoteDesktopServiceManager.Instance.RefreshDesktop(this);
        }

        /// <summary>
        /// Clients the send key event.
        /// </summary>
        /// <param name="e">The <see cref="Forge.RemoteDesktop.Contracts.KeyboardEventArgs"/> instance containing the event data.</param>
        public override void ClientSendKeyEvent(Forge.RemoteDesktop.Contracts.KeyboardEventArgs e)
        {
            RemoteDesktopServiceManager.Instance.SendKeyEvent(this, e);
        }

        /// <summary>
        /// Clients the send mouse button event.
        /// </summary>
        /// <param name="e">The <see cref="Forge.RemoteDesktop.Contracts.MouseButtonEventArgs"/> instance containing the event data.</param>
        public override void ClientSendMouseButtonEvent(Forge.RemoteDesktop.Contracts.MouseButtonEventArgs e)
        {
            RemoteDesktopServiceManager.Instance.SendMouseButtonEvent(this, e);
        }

        /// <summary>
        /// Clients the send mouse wheel event.
        /// </summary>
        /// <param name="e">The <see cref="Forge.RemoteDesktop.Contracts.MouseWheelEventArgs"/> instance containing the event data.</param>
        public override void ClientSendMouseWheelEvent(Forge.RemoteDesktop.Contracts.MouseWheelEventArgs e)
        {
            RemoteDesktopServiceManager.Instance.SendMouseWheelEvent(this, e);
        }

        /// <summary>
        /// Clients the send mouse move event.
        /// </summary>
        /// <param name="e">The <see cref="Forge.RemoteDesktop.Contracts.MouseMoveEventArgs"/> instance containing the event data.</param>
        public override void ClientSendMouseMoveEvent(Forge.RemoteDesktop.Contracts.MouseMoveEventArgs e)
        {
            RemoteDesktopServiceManager.Instance.SendMouseMoveEvent(this, e);
        }

        /// <summary>
        /// Clients the content of the send clipboard.
        /// </summary>
        /// <param name="e">The <see cref="Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs" /> instance containing the event data.</param>
        public override void ClientSendClipboardContent(Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs e)
        {
            RemoteDesktopServiceManager.Instance.SendClipboardContent(this, e);
        }

        /// <summary>
        /// Sends a file to the service.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="file">The file.</param>
        public override void ClientSendFile(string fileName, System.IO.Stream file)
        {
            try
            {
                RemoteDesktopServiceManager.Instance.SendFile(this, fileName, file);
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                }
            }
        }

        #endregion

    }

}
