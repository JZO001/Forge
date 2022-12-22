using System;
using System.IO;
using Forge.Net.Remoting;
using log4net;
namespace Testing.TerraGraf.RemotingService
{

    public class TestContractStreamServiceImpl : Testing.TerraGraf.RemotingService.TestContractStreamAbstractServiceProxy
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TestContractStreamServiceImpl));

        public TestContractStreamServiceImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        public override System.String GetName()
        {
            LOGGER.Info("TestContractStreamServiceImpl.GetName");
            return "JZO + TestContractStreamServiceImpl";
        }

        public override void SayHello()
        {
            LOGGER.Info("TestContractStreamServiceImpl.SayHello");
            try
            {
                using (FileStream fs = new FileStream("Forge.Net.TerraGraf.pdb", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    SendImage(fs);
                }
            }
            catch (Exception ex)
            {
                throw new RemoteMethodInvocationException(ex.Message, ex);
            }

        }

    }

}
