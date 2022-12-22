namespace Testing.Net.Remoting.Generator.RemotingClient
{

    public class TestContractStreamClientImpl : Testing.Net.Remoting.Generator.RemotingClient.TestContractStreamAbstractClientProxy
    {

        public TestContractStreamClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public override void SendImage(System.IO.Stream stream)
        {
            // NOTICE: your responsibility to call Dispose() method on the given Stream(s) before you return from this statement or later on an other thread.
            try
            {
                // you may use this try-finally pattern and take your code here

            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
            throw new System.NotImplementedException();
        }

    }

}
