namespace Testing.TerraGraf.RemotingClient
{

    public class TestContractSimpleClientImpl : Testing.TerraGraf.RemotingClient.TestContractSimpleAbstractClientProxy
    {

        public TestContractSimpleClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public override System.Object Clone()
        {
            return this;
        }

        public override System.Boolean Equals(Testing.TerraGraf.Contracts.ITestContractSimple _p0)
        {
            return true;
        }

    }

}
