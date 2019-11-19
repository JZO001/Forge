using log4net;
namespace Testing.TerraGraf.RemotingService
{

    public class TestSingletonServiceImpl : Forge.MBRBase, Testing.TerraGraf.Contracts.ITestSingleton
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TestSingletonServiceImpl));

        public TestSingletonServiceImpl()
        {
        }

        public System.String GetName()
        {
            LOGGER.Info("TestSingletonServiceImpl.GetName");
            return "JZO + TestSingletonServiceImpl";

            //throw new System.NotImplementedException();
        }

        public void SayHello()
        {
            LOGGER.Info("TestSingletonServiceImpl.SayHello");
            //throw new System.NotImplementedException();
        }

        public void SendImage(System.IO.Stream _p0)
        {
            // NOTICE: your responsibility to call Dispose() method on the given Stream(s) before you return from this statement or later on an other thread.
            try
            {
                // you may use this try-finally pattern and take your code here
                LOGGER.Info("TestSingletonServiceImpl.SendImage, size: " + _p0 == null ? "<null>" : _p0.Length.ToString());
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

        public void Dispose()
        {
            LOGGER.Info("TestSingletonServiceImpl.Dispose");
        }

    }

}
