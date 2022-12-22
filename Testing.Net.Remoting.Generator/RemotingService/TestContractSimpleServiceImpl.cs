namespace Testing.Net.Remoting.Generator.RemotingService
{

    public class TestContractSimpleServiceImpl : Forge.Net.Remoting.Proxy.ProxyBase, Testing.Net.Remoting.Generator.Contracts.ITestContractSimple
    {

        public TestContractSimpleServiceImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public System.String GetName()
        {
            throw new System.NotImplementedException();
        }

        public void SetName(System.String name)
        {
            throw new System.NotImplementedException();
        }

        public System.Int32 GetAge()
        {
            throw new System.NotImplementedException();
        }

        public void SetAge(System.Int32 age)
        {
            throw new System.NotImplementedException();
        }

        public System.Object Clone()
        {
            throw new System.NotImplementedException();
        }

        public System.Boolean Equals(Testing.Net.Remoting.Generator.Contracts.ITestContractSimple other)
        {
            throw new System.NotImplementedException();
        }

    }

}
