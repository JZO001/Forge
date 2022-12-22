namespace Testing.Net.Remoting.Generator.RemotingService
{

    public class TestContractServiceImpl : Testing.Net.Remoting.Generator.RemotingService.TestContractAbstractServiceProxy
    {

        public TestContractServiceImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

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

        public override void SendNonImportantMessage(System.String message)
        {
            throw new System.NotImplementedException();
        }

        public override System.IO.Stream GetImage()
        {
            throw new System.NotImplementedException();
        }

        public override void SetImage(System.IO.Stream stream)
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

        public override void IsProductExist(System.Boolean? state)
        {
            throw new System.NotImplementedException();
        }

        public override System.String GetName()
        {
            throw new System.NotImplementedException();
        }

        public override void SetName(System.String name)
        {
            throw new System.NotImplementedException();
        }

        public override System.Int32 GetAge()
        {
            throw new System.NotImplementedException();
        }

        public override void SetAge(System.Int32 age)
        {
            throw new System.NotImplementedException();
        }

        public override void SayHello()
        {
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
