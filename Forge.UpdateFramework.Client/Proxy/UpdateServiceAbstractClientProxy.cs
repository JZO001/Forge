/* *********************************************************************
 * Date: 12 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/


namespace Forge.UpdateFramework.Client.Proxy
{

    /// <summary>
    /// Update service client side proxy
    /// </summary>
    public abstract class UpdateServiceAbstractClientProxy : Forge.Net.Remoting.Proxy.ProxyBase, Forge.UpdateFramework.Contracts.IUpdateService
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateServiceAbstractClientProxy"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sessionId">The session id.</param>
        protected UpdateServiceAbstractClientProxy(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        /// <summary>
        /// Service orders the client to execute the update search process
        /// </summary>
        public abstract void CheckUpdatePush();

        /// <summary>
        /// Sends the client content description.
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public Forge.UpdateFramework.Contracts.UpdateResponseArgs SendClientContentDescription(Forge.UpdateFramework.Contracts.UpdateRequestArgs args)
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.UpdateFramework.Contracts.UpdateRequestArgs).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.UpdateFramework.Contracts.UpdateRequestArgs).Assembly.FullName).Name, args);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.UpdateFramework.Contracts.IUpdateService), "SendClientContentDescription", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, this.ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.UpdateFramework.Contracts.IUpdateService), "SendClientContentDescription", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                _response = (Forge.Net.Remoting.Messaging.ResponseMessage)this.mChannel.SendMessage(this.mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }

            if (_response.MethodInvocationException == null)
            {
                return (Forge.UpdateFramework.Contracts.UpdateResponseArgs)_response.ReturnValue.Value;
            }
            else
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("An exception arrived from the remote side. See inner exception for details.", _response.MethodInvocationException);
            }
        }

    }

}
