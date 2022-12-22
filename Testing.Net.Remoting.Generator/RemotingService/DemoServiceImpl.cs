namespace Testing.Net.Remoting.Generator.RemotingService
{

    public class DemoServiceImpl : Testing.Net.Remoting.Generator.RemotingService.DemoAbstractServiceProxy
    {

        public DemoServiceImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public override void SendMessageToService(System.String message)
        {
            Console.WriteLine(message);
            ThreadPool.QueueUserWorkItem((object? state) => 
            { 
                SendMessageToClient($"Echo: {message}");
            });
        }

    }

}
