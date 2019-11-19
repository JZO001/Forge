using log4net;
namespace Testing.TerraGraf.RemotingClient
{

    public class TestContractStreamClientImpl : Testing.TerraGraf.RemotingClient.TestContractStreamAbstractClientProxy
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TestContractStreamClientImpl));

        public TestContractStreamClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public override void SendImage(System.IO.Stream _p0)
        {
            // NOTICE: your responsibility to call Dispose() method on the given Stream(s) before you return from this statement or later on an other thread.
            try
            {
                // you may use this try-finally pattern and take your code here
                LOGGER.Info("TestContractStreamClientImpl.SendImage, size: " + _p0.Length);
            }
            finally
            {
                if (_p0 != null)
                {
                    _p0.Dispose();
                }
            }
        }

    }

}
