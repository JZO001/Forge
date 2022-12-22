namespace Testing.Net.Remoting.Generator.RemotingClient
{

    public class TestNullableClientImpl : Testing.Net.Remoting.Generator.RemotingClient.TestNullableAbstractClientProxy
    {

        public TestNullableClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public override System.Boolean SendValue(System.Boolean? isCheck)
        {
            throw new System.NotImplementedException();
        }

    }

}
