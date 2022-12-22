using System;
using log4net;
namespace Testing.TerraGraf.RemotingService
{

    public class TestSingleCallServiceImpl : Forge.MBRBase, Testing.TerraGraf.Contracts.ITestSingleCall
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TestSingleCallServiceImpl));

        public TestSingleCallServiceImpl()
        {
        }

        ~TestSingleCallServiceImpl()
        {
            Dispose(false);
        }

        public System.String GetName()
        {
            LOGGER.Info("TestSingleCallServiceImpl.GetName");
            return "JZO + TestSingleCallServiceImpl";
            //throw new System.NotImplementedException();
        }

        public void SayHello()
        {
            LOGGER.Info("TestSingleCallServiceImpl.SayHello");
            //throw new System.NotImplementedException();
        }

        public void SendImage(System.IO.Stream _p0)
        {
            // NOTICE: your responsibility to call Dispose() method on the given Stream(s) before you return from this statement or later on an other thread.
            try
            {
                // you may use this try-finally pattern and take your code here
                LOGGER.Info("TestSingleCallServiceImpl.sendImage, size: " + _p0 == null ? "<null>" : _p0.Length.ToString());
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            LOGGER.Info("TestSingleCallServiceImpl.Dispose, " + disposing);
        }

    }

}
