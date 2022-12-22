namespace Testing.Net.Remoting.Generator.RemotingService
{

    public abstract class TestContractStreamAbstractServiceProxy : Forge.Net.Remoting.Proxy.ProxyBase, Testing.Net.Remoting.Generator.Contracts.ITestContractStream
    {

        protected TestContractStreamAbstractServiceProxy(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public abstract System.String GetName();

        public abstract void SayHello();

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void SendImage(System.IO.Stream stream)
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.ServiceBase.CheckProxyRegistered(this);

                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(System.IO.Stream).FullName + ", " + new System.Reflection.AssemblyName(typeof(System.IO.Stream).Assembly.FullName).Name, stream);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestCallback, typeof(Testing.Net.Remoting.Generator.Contracts.ITestContractStream), "SendImage", _mps, false);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, Forge.Net.Remoting.ServiceBase.GetPeerProxyId(this));

                long _timeout = GetTimeoutByMethod(typeof(Testing.Net.Remoting.Generator.Contracts.ITestContractStream), "SendImage", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

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
