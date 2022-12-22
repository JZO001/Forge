namespace Testing.Net.Remoting.Generator.RemotingService
{

    public class TestNullableServiceImpl : Testing.Net.Remoting.Generator.RemotingService.TestNullableAbstractServiceProxy
    {

        public TestNullableServiceImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public override System.Boolean GetValue(System.Boolean? isCheck)
        {
            throw new System.NotImplementedException();
        }

    }

}
