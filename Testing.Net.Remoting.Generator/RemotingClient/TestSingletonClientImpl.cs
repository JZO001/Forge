namespace Testing.Net.Remoting.Generator.RemotingClient
{

    public class TestSingletonClientImpl : Forge.Net.Remoting.Proxy.ProxyBase, Testing.Net.Remoting.Generator.Contracts.ITestSingleton
    {

        public TestSingletonClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public System.String GetName()
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Testing.Net.Remoting.Generator.Contracts.ITestSingleton), "GetName", _mps, false);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, this.ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Testing.Net.Remoting.Generator.Contracts.ITestSingleton), "GetName", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                _response = (Forge.Net.Remoting.Messaging.ResponseMessage)this.mChannel.SendMessage(this.mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }

            if (_response.MethodInvocationException == null)
            {
                return (System.String)_response.ReturnValue.Value;
            }
            else
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("An exception arrived from the remote side. See inner exception for details.", _response.MethodInvocationException);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void SayHello()
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Datagram, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Testing.Net.Remoting.Generator.Contracts.ITestSingleton), "SayHello", _mps, false);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, this.ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Testing.Net.Remoting.Generator.Contracts.ITestSingleton), "SayHello", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                this.mChannel.SendMessage(this.mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void SendImage(System.IO.Stream stream)
        {
            DoDisposeCheck();
            Forge.Net.Remoting.Messaging.ResponseMessage _response = null;
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(System.IO.Stream).FullName + ", " + new System.Reflection.AssemblyName(typeof(System.IO.Stream).Assembly.FullName).Name, stream);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Request, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Testing.Net.Remoting.Generator.Contracts.ITestSingleton), "SendImage", _mps, false);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, this.ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Testing.Net.Remoting.Generator.Contracts.ITestSingleton), "SendImage", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

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
