namespace Testing.Net.Remoting.Generator.RemotingClient
{

    public class DemoClientImpl : Testing.Net.Remoting.Generator.RemotingClient.DemoAbstractClientProxy
    {

        public DemoClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public override void SendMessageToClient(System.String message)
        {
            Console.WriteLine(message);
        }

    }

}
