namespace Testing.Net.Remoting.Generator.RemotingClient
{

    public class TestContractClientImpl : Testing.Net.Remoting.Generator.RemotingClient.TestContractAbstractClientProxy
    {

        public TestContractClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public override System.String PropertyTest1
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public override System.String PropertyTest2
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

        public override System.String PropertyTest3
        {
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public override void SendMessage(System.String message)
        {
            throw new System.NotImplementedException();
        }

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

        public override void DoNothing()
        {
            throw new System.NotImplementedException();
        }

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
