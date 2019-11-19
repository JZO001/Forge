using log4net;
namespace Testing.TerraGraf.RemotingService
{

    public class TestContractSimpleServiceImpl : Forge.Net.Remoting.Proxy.ProxyBase, Testing.TerraGraf.Contracts.ITestContractSimple
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TestContractSimpleServiceImpl));

        private int mAge = 25;

        private string mName = "JZO + TestContractSimpleServiceImpl";

        public TestContractSimpleServiceImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public System.String GetName()
        {
            LOGGER.Info("TestContractSimpleServiceImpl.GetName");
            return this.mName;
        }

        public void SetName(System.String _p0)
        {
            LOGGER.Info("TestContractSimpleServiceImpl.SetName, " + _p0);
            this.mName = _p0;
        }

        public System.Int32 GetAge()
        {
            LOGGER.Info("TestContractSimpleServiceImpl.GetAge");
            return this.mAge;
        }

        public void SetAge(System.Int32 _p0)
        {
            LOGGER.Info("TestContractSimpleServiceImpl.SetAge, " + _p0.ToString());
            this.mAge = _p0;
        }

        public System.Object Clone()
        {
            return this;
        }

        public System.Boolean Equals(Testing.TerraGraf.Contracts.ITestContractSimple _p0)
        {
            return true;
        }

    }

}
