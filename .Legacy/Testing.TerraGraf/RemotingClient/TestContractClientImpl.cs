using log4net;
namespace Testing.TerraGraf.RemotingClient
{

    public class TestContractClientImpl : Testing.TerraGraf.RemotingClient.TestContractAbstractClientProxy
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TestContractClientImpl));

        public TestContractClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public override string PropertyTest1
        {
            get { throw new System.NotImplementedException(); }
        }

        public override string PropertyTest2
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public override string PropertyTest3
        {
            set { throw new System.NotImplementedException(); }
        }

        public override void SendMessage(System.String _p0)
        {
            LOGGER.Info("TestContractClientImpl.SendMessage method called. Message: " + _p0);
        }

        public override void SendImage(System.IO.Stream _p0)
        {
            // NOTICE: your responsibility to call Dispose() method on the given Stream(s) before you return from this statement or later on an other thread.
            try
            {
                // you may use this try-finally pattern and take your code here
                LOGGER.Info("TestContractClientImpl.SendImage method called. Size: " + (_p0 == null ? "<null>" : _p0.Length.ToString()));
            }
            finally
            {
                if (_p0 != null)
                {
                    _p0.Dispose();
                }
            }
            //throw new System.NotImplementedException();
        }

        public override void DoNothing()
        {
            LOGGER.Info("TestContractClientImpl.DoNothing method called.");
            //throw new System.NotImplementedException();
        }

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
