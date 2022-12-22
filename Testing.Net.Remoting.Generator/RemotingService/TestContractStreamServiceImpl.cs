namespace Testing.Net.Remoting.Generator.RemotingService
{

    public class TestContractStreamServiceImpl : Testing.Net.Remoting.Generator.RemotingService.TestContractStreamAbstractServiceProxy
    {

        public TestContractStreamServiceImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public override System.String GetName()
        {
            throw new System.NotImplementedException();
        }

        public override void SayHello()
        {
            throw new System.NotImplementedException();
        }

    }

}
