namespace Testing.Net.Remoting.Generator.RemotingClient
{

    public abstract class TestNullableAbstractClientProxy : Forge.Net.Remoting.Proxy.ProxyBase, Testing.Net.Remoting.Generator.Contracts.ITestNullable
    {

        protected TestNullableAbstractClientProxy(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public abstract System.Boolean SendValue(System.Boolean? isCheck);

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public System.Boolean GetValue(System.Boolean? isCheck)
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(System.Boolean?).FullName + ", " + new System.Reflection.AssemblyName(typeof(System.Boolean?).Assembly.FullName).Name, isCheck);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Testing.Net.Remoting.Generator.Contracts.ITestNullable), "GetValue", _mps, false);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, this.ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Testing.Net.Remoting.Generator.Contracts.ITestNullable), "GetValue", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                _response = (Forge.Net.Remoting.Messaging.ResponseMessage)this.mChannel.SendMessage(this.mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }

            if (_response.MethodInvocationException == null)
            {
                return (System.Boolean)_response.ReturnValue.Value;
            }
            else
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("An exception arrived from the remote side. See inner exception for details.", _response.MethodInvocationException);
            }
        }

    }

}
