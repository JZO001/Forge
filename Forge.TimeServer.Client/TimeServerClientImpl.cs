/* *********************************************************************
 * Date: 21 Nov 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.TimeServer.Client
{

    /// <summary>
    /// Client proxy for time server call
    /// </summary>
    public class TimeServerClientImpl : Forge.Net.Remoting.Proxy.ProxyBase, Forge.TimeServer.Contracts.ITimeServer
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeServerClientImpl"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sessionId">The session id.</param>
        public TimeServerClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        /// <summary>
        /// Gets the UTC date time.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">
        /// Unable to call remote method. See inner exception for details.
        /// or
        /// An exception arrived from the remote side. See inner exception for details.
        /// </exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public System.Int64 GetUTCDateTime()
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.TimeServer.Contracts.ITimeServer), "GetUTCDateTime", _mps, false);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, this.ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.TimeServer.Contracts.ITimeServer), "GetUTCDateTime", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                _response = (Forge.Net.Remoting.Messaging.ResponseMessage)this.mChannel.SendMessage(this.mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }

            if (_response.MethodInvocationException == null)
            {
                return (System.Int64)_response.ReturnValue.Value;
            }
            else
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("An exception arrived from the remote side. See inner exception for details.", _response.MethodInvocationException);
            }
        }

    }

}
