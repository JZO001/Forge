/* *********************************************************************
 * Date: 07 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Invoker;

namespace Forge.RemoteDesktop.Service
{

    /// <summary>
    /// Represents the service side proxy
    /// </summary>
    public abstract class RemoteDesktopAbstractServiceProxy : Forge.Net.Remoting.Proxy.ProxyBase, Forge.RemoteDesktop.Contracts.IRemoteDesktop
    {

        /// <summary>
        /// Occurs when the remote peer disconnects this session
        /// </summary>
        public event System.EventHandler<Forge.RemoteDesktop.Contracts.DisconnectEventArgs> Disconnected;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDesktopAbstractServiceProxy"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sessionId">The session id.</param>
        protected RemoteDesktopAbstractServiceProxy(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) 
        {
            channel.SessionStateChange += new System.EventHandler<Net.Remoting.Channels.SessionStateEventArgs>(Channel_SessionStateChange);
            Forge.Net.Remoting.Channels.ISessionInfo info = channel.GetSessionInfo(sessionId);
            IsConnected = info == null ? false : true;
            if (info != null)
            {
                RemoteHost = info.RemoteEndPoint.Host;
            }
        }

        private void Channel_SessionStateChange(object sender, Net.Remoting.Channels.SessionStateEventArgs e)
        {
            if (e.SessionId.Equals(SessionId) && !e.IsConnected)
            {
                OnDisconnected(new Contracts.DisconnectEventArgs(SessionId));
            }
        }

        #region Protected method(s)

        /// <summary>
        /// Raises the <see cref="E:Disconnected" /> event.
        /// </summary>
        /// <param name="e">The <see cref="Forge.RemoteDesktop.Contracts.DisconnectEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDisconnected(Forge.RemoteDesktop.Contracts.DisconnectEventArgs e)
        {
            IsActive = false;
            IsConnected = false;
            Executor.Invoke(Disconnected, this, e);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                mChannel.SessionStateChange -= new System.EventHandler<Net.Remoting.Channels.SessionStateEventArgs>(Channel_SessionStateChange);
                if (mChannel.IsSessionReusable)
                {
                    // release the connection anytime
                    mChannel.Disconnect(mSessionId);
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the remote host.
        /// </summary>
        /// <value>
        /// The remote host.
        /// </value>
        public string RemoteHost
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public System.Boolean IsConnected
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
        /// </value>
        public System.Boolean IsAuthenticated
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public abstract System.Boolean IsActive
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the authentication information.
        /// </summary>
        /// <returns></returns>
        public abstract Forge.RemoteDesktop.Contracts.AuthModeResponseArgs GetAuthenticationInfo();

        /// <summary>
        /// Logins the specified _P0.
        /// </summary>
        /// <param name="_p0">The _P0.</param>
        /// <returns></returns>
        public abstract Forge.RemoteDesktop.Contracts.LoginResponseArgs Login(Forge.RemoteDesktop.Contracts.LoginRequestArgs _p0);

        /// <summary>
        /// Gets the configuration of the service.
        /// </summary>
        /// <returns></returns>
        public abstract Forge.RemoteDesktop.Contracts.DescriptionResponseArgs ClientGetConfiguration();

        /// <summary>
        /// Start the event pump.
        /// </summary>
        public abstract Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs ClientStartEventPump();

        /// <summary>
        /// Stops the event pump.
        /// </summary>
        public abstract void ClientStopEventPump();

        /// <summary>
        /// Clients the set image clip quality.
        /// </summary>
        /// <param name="_p0">The _P0.</param>
        public abstract void ClientSetImageClipQuality(Forge.RemoteDesktop.Contracts.ImageClipQualityArgs _p0);

        /// <summary>
        /// Clients the subscribe for desktop.
        /// </summary>
        /// <param name="_p0">The _P0.</param>
        public abstract void ClientSubscribeForDesktop(Forge.RemoteDesktop.Area _p0);

        /// <summary>
        /// Requests a desktop refresh from the service.
        /// </summary>
        public abstract void ClientRefreshDesktop();

        /// <summary>
        /// Clients the send key event.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.KeyboardEventArgs"/> instance containing the event data.</param>
        public abstract void ClientSendKeyEvent(Forge.RemoteDesktop.Contracts.KeyboardEventArgs _p0);

        /// <summary>
        /// Clients the send mouse button event.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.MouseButtonEventArgs"/> instance containing the event data.</param>
        public abstract void ClientSendMouseButtonEvent(Forge.RemoteDesktop.Contracts.MouseButtonEventArgs _p0);

        /// <summary>
        /// Clients the send mouse wheel event.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.MouseWheelEventArgs"/> instance containing the event data.</param>
        public abstract void ClientSendMouseWheelEvent(Forge.RemoteDesktop.Contracts.MouseWheelEventArgs _p0);

        /// <summary>
        /// Clients the send mouse move event.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.MouseMoveEventArgs"/> instance containing the event data.</param>
        public abstract void ClientSendMouseMoveEvent(Forge.RemoteDesktop.Contracts.MouseMoveEventArgs _p0);

        /// <summary>
        /// Clients the content of the send clipboard.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs"/> instance containing the event data.</param>
        public abstract void ClientSendClipboardContent(Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs _p0);

        /// <summary>
        /// Sends a file to the service.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="file">The file.</param>
        public abstract void ClientSendFile(string fileName, System.IO.Stream file);

        /// <summary>
        /// Services the send mouse move event.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs"/> instance containing the event data.</param>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">Unable to call remote method. See inner exception for details.</exception>
        //[System.Diagnostics.DebuggerStepThroughAttribute]
        public void ServiceSendMouseMoveEvent(Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs _p0)
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.ServiceBase.CheckProxyRegistered(this);

                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs).Assembly.FullName).Name, _p0);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.DatagramOneway, Forge.Net.Remoting.MessageInvokeModeEnum.RequestCallback, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ServiceSendMouseMoveEvent", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, Forge.Net.Remoting.ServiceBase.GetPeerProxyId(this));

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ServiceSendMouseMoveEvent", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Services the send desktop image clip.
        /// </summary>
        /// <param name="_p0">The _P0.</param>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">
        /// Unable to call remote method. See inner exception for details.
        /// or
        /// An exception arrived from the remote side. See inner exception for details.
        /// </exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void ServiceSendDesktopImageClip(Forge.RemoteDesktop.Contracts.DesktopImageClipArgs _p0)
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.ServiceBase.CheckProxyRegistered(this);

                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.RemoteDesktop.Contracts.DesktopImageClipArgs).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.RemoteDesktop.Contracts.DesktopImageClipArgs).Assembly.FullName).Name, _p0);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.DatagramOneway, Forge.Net.Remoting.MessageInvokeModeEnum.RequestCallback, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ServiceSendDesktopImageClip", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, Forge.Net.Remoting.ServiceBase.GetPeerProxyId(this));

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ServiceSendDesktopImageClip", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Services the content of the send clipboard.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">Unable to call remote method. See inner exception for details.</exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void ServiceSendClipboardContent(Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs _p0)
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.ServiceBase.CheckProxyRegistered(this);

                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs).Assembly.FullName).Name, _p0);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.DatagramOneway, Forge.Net.Remoting.MessageInvokeModeEnum.RequestCallback, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ServiceSendClipboardContent", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, Forge.Net.Remoting.ServiceBase.GetPeerProxyId(this));

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ServiceSendClipboardContent", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

    }

}
