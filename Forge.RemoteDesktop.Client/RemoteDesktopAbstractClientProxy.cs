/* *********************************************************************
 * Date: 07 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Invoker;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Represents a client side proxy
    /// </summary>
    public abstract class RemoteDesktopAbstractClientProxy : Forge.Net.Remoting.Proxy.ProxyBase, Forge.RemoteDesktop.Contracts.IRemoteDesktop
    {

        /// <summary>
        /// Occurs when the remote peer disconnects this session
        /// </summary>
        public event System.EventHandler<Forge.RemoteDesktop.Contracts.DisconnectEventArgs> Disconnected;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDesktopAbstractClientProxy"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sessionId">The session id.</param>
        protected RemoteDesktopAbstractClientProxy(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) 
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
            IsAuthenticated = false;
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
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public System.Boolean IsActive
        {
            get;
            set;
        }

        /// <summary>
        /// Services the send mouse move event.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs"/> instance containing the event data.</param>
        public abstract void ServiceSendMouseMoveEvent(Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs _p0);

        /// <summary>
        /// Services the send desktop image clip.
        /// </summary>
        /// <param name="_p0">The _P0.</param>
        public abstract void ServiceSendDesktopImageClip(Forge.RemoteDesktop.Contracts.DesktopImageClipArgs _p0);

        /// <summary>
        /// Services the content of the send clipboard.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs"/> instance containing the event data.</param>
        public abstract void ServiceSendClipboardContent(Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs _p0);

        /// <summary>
        /// Gets the authentication information.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">
        /// Unable to call remote method. See inner exception for details.
        /// or
        /// An exception arrived from the remote side. See inner exception for details.
        /// </exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public Forge.RemoteDesktop.Contracts.AuthModeResponseArgs GetAuthenticationInfo()
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "GetAuthenticationInfo", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "GetAuthenticationInfo", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                _response = (Forge.Net.Remoting.Messaging.ResponseMessage)mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }

            if (_response.MethodInvocationException == null)
            {
                return (Forge.RemoteDesktop.Contracts.AuthModeResponseArgs)_response.ReturnValue.Value;
            }
            else
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("An exception arrived from the remote side. See inner exception for details.", _response.MethodInvocationException);
            }
        }

        /// <summary>
        /// Logins the specified _P0.
        /// </summary>
        /// <param name="_p0">The _P0.</param>
        /// <returns></returns>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">
        /// Unable to call remote method. See inner exception for details.
        /// or
        /// An exception arrived from the remote side. See inner exception for details.
        /// </exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public Forge.RemoteDesktop.Contracts.LoginResponseArgs Login(Forge.RemoteDesktop.Contracts.LoginRequestArgs _p0)
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.RemoteDesktop.Contracts.LoginRequestArgs).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.RemoteDesktop.Contracts.LoginRequestArgs).Assembly.FullName).Name, _p0);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "Login", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "Login", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                _response = (Forge.Net.Remoting.Messaging.ResponseMessage)mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }

            if (_response.MethodInvocationException == null)
            {
                return (Forge.RemoteDesktop.Contracts.LoginResponseArgs)_response.ReturnValue.Value;
            }
            else
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("An exception arrived from the remote side. See inner exception for details.", _response.MethodInvocationException);
            }
        }

        /// <summary>
        /// Gets the configuration of the service.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">
        /// Unable to call remote method. See inner exception for details.
        /// or
        /// An exception arrived from the remote side. See inner exception for details.
        /// </exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public Forge.RemoteDesktop.Contracts.DescriptionResponseArgs ClientGetConfiguration()
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientGetConfiguration", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientGetConfiguration", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                _response = (Forge.Net.Remoting.Messaging.ResponseMessage)mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }

            if (_response.MethodInvocationException == null)
            {
                return (Forge.RemoteDesktop.Contracts.DescriptionResponseArgs)_response.ReturnValue.Value;
            }
            else
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("An exception arrived from the remote side. See inner exception for details.", _response.MethodInvocationException);
            }
        }

        /// <summary>
        /// Clients the set image clip quality.
        /// </summary>
        /// <param name="_p0">The _P0.</param>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">
        /// Unable to call remote method. See inner exception for details.
        /// or
        /// An exception arrived from the remote side. See inner exception for details.
        /// </exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void ClientSetImageClipQuality(Forge.RemoteDesktop.Contracts.ImageClipQualityArgs _p0)
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.RemoteDesktop.Contracts.ImageClipQualityArgs).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.RemoteDesktop.Contracts.ImageClipQualityArgs).Assembly.FullName).Name, _p0);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSetImageClipQuality", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSetImageClipQuality", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                _response = (Forge.Net.Remoting.Messaging.ResponseMessage)mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }

            if (_response.MethodInvocationException != null)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("An exception arrived from the remote side. See inner exception for details.", _response.MethodInvocationException);
            }
        }

        /// <summary>
        /// Clients the subscribe for desktop.
        /// </summary>
        /// <param name="_p0">The _P0.</param>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">Unable to call remote method. See inner exception for details.</exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void ClientSubscribeForDesktop(Forge.RemoteDesktop.Area _p0)
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.RemoteDesktop.Area).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.RemoteDesktop.Area).Assembly.FullName).Name, _p0);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Datagram, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSubscribeForDesktop", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSubscribeForDesktop", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Start the event pump.
        /// </summary>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">
        /// Unable to call remote method. See inner exception for details.
        /// or
        /// An exception arrived from the remote side. See inner exception for details.
        /// </exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public virtual Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs ClientStartEventPump()
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientStartEventPump", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientStartEventPump", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                _response = (Forge.Net.Remoting.Messaging.ResponseMessage)mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }

            if (_response.MethodInvocationException == null)
            {
                return (Forge.RemoteDesktop.Contracts.MouseMoveServiceEventArgs)_response.ReturnValue.Value;
            }
            else
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("An exception arrived from the remote side. See inner exception for details.", _response.MethodInvocationException);
            }
        }

        /// <summary>
        /// Stops the event pump.
        /// </summary>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">
        /// Unable to call remote method. See inner exception for details.
        /// or
        /// An exception arrived from the remote side. See inner exception for details.
        /// </exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public virtual void ClientStopEventPump()
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientStopEventPump", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientStopEventPump", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                _response = (Forge.Net.Remoting.Messaging.ResponseMessage)mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }

            if (_response.MethodInvocationException != null)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("An exception arrived from the remote side. See inner exception for details.", _response.MethodInvocationException);
            }
        }

        /// <summary>
        /// Requests a desktop refresh from the service.
        /// </summary>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">Unable to call remote method. See inner exception for details.</exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void ClientRefreshDesktop()
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.DatagramOneway, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientRefreshDesktop", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientRefreshDesktop", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Clients the send key event.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.KeyboardEventArgs"/> instance containing the event data.</param>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">Unable to call remote method. See inner exception for details.</exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void ClientSendKeyEvent(Forge.RemoteDesktop.Contracts.KeyboardEventArgs _p0)
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.RemoteDesktop.Contracts.KeyboardEventArgs).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.RemoteDesktop.Contracts.KeyboardEventArgs).Assembly.FullName).Name, _p0);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.DatagramOneway, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSendKeyEvent", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSendKeyEvent", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Clients the send mouse button event.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.MouseButtonEventArgs"/> instance containing the event data.</param>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">Unable to call remote method. See inner exception for details.</exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void ClientSendMouseButtonEvent(Forge.RemoteDesktop.Contracts.MouseButtonEventArgs _p0)
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.RemoteDesktop.Contracts.MouseButtonEventArgs).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.RemoteDesktop.Contracts.MouseButtonEventArgs).Assembly.FullName).Name, _p0);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.DatagramOneway, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSendMouseButtonEvent", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSendMouseButtonEvent", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Clients the send mouse wheel event.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.MouseWheelEventArgs"/> instance containing the event data.</param>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">Unable to call remote method. See inner exception for details.</exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void ClientSendMouseWheelEvent(Forge.RemoteDesktop.Contracts.MouseWheelEventArgs _p0)
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.RemoteDesktop.Contracts.MouseWheelEventArgs).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.RemoteDesktop.Contracts.MouseWheelEventArgs).Assembly.FullName).Name, _p0);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.DatagramOneway, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSendMouseWheelEvent", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSendMouseWheelEvent", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Clients the send mouse move event.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.MouseMoveEventArgs"/> instance containing the event data.</param>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">Unable to call remote method. See inner exception for details.</exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void ClientSendMouseMoveEvent(Forge.RemoteDesktop.Contracts.MouseMoveEventArgs _p0)
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.RemoteDesktop.Contracts.MouseMoveEventArgs).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.RemoteDesktop.Contracts.MouseMoveEventArgs).Assembly.FullName).Name, _p0);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.DatagramOneway, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSendMouseMoveEvent", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSendMouseMoveEvent", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Clients the content of the send clipboard.
        /// </summary>
        /// <param name="_p0">The <see cref="Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">Unable to call remote method. See inner exception for details.</exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void ClientSendClipboardContent(Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs _p0)
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs).Assembly.FullName).Name, _p0);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.DatagramOneway, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSendClipboardContent", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSendClipboardContent", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Clients the send file.
        /// </summary>
        /// <param name="_p0">The _P0.</param>
        /// <param name="_p1">The _P1.</param>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">
        /// Unable to call remote method. See inner exception for details.
        /// or
        /// An exception arrived from the remote side. See inner exception for details.
        /// </exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void ClientSendFile(System.String _p0, System.IO.Stream _p1)
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(System.String).FullName + ", " + new System.Reflection.AssemblyName(typeof(System.String).Assembly.FullName).Name, _p0);
                Forge.Net.Remoting.Messaging.MethodParameter _mp1 = new Forge.Net.Remoting.Messaging.MethodParameter(1, typeof(System.IO.Stream).FullName + ", " + new System.Reflection.AssemblyName(typeof(System.IO.Stream).Assembly.FullName).Name, _p1);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0, _mp1 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Datagram, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSendFile", _mps, true);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.RemoteDesktop.Contracts.IRemoteDesktop), "ClientSendFile", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                mChannel.SendMessage(mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

    }

}
