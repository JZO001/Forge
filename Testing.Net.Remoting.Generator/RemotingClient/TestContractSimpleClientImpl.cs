namespace Testing.Net.Remoting.Generator.RemotingClient
{

    public class TestContractSimpleClientImpl : Testing.Net.Remoting.Generator.RemotingClient.TestContractSimpleAbstractClientProxy
    {

        public TestContractSimpleClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public override System.Object Clone()
        {
            throw new System.NotImplementedException();
        }

        public override System.Boolean Equals(Testing.Net.Remoting.Generator.Contracts.ITestContractSimple other)
        {
            throw new System.NotImplementedException();
        }

    }

}
