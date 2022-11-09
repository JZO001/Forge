/* *********************************************************************
 * Date: 12 Oct 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.UpdateFramework.Contracts;
namespace Forge.UpdateFramework.Service
{

    /// <summary>
    /// Update service implementation proxy
    /// </summary>
    public abstract class UpdateServiceAbstractServiceProxy : Forge.Net.Remoting.Proxy.ProxyBase, Forge.UpdateFramework.Contracts.IUpdateService
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateServiceAbstractServiceProxy"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sessionId">The session id.</param>
        protected UpdateServiceAbstractServiceProxy(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        /// <summary>
        /// Sends the client content description.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public abstract UpdateResponseArgs SendClientContentDescription(UpdateRequestArgs args);

        /// <summary>
        /// Service orders the client to execute the update search process
        /// </summary>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void CheckUpdatePush()
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.ServiceBase.CheckProxyRegistered(this);

                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestCallback, typeof(Forge.UpdateFramework.Contracts.IUpdateService), "CheckUpdatePush", _mps);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, Forge.Net.Remoting.ServiceBase.GetPeerProxyId(this));

                long _timeout = GetTimeoutByMethod(typeof(Forge.UpdateFramework.Contracts.IUpdateService), "CheckUpdatePush", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                _response = (Forge.Net.Remoting.Messaging.ResponseMessage)this.mChannel.SendMessage(this.mSessionId, _message, _timeout);
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

    }

}
