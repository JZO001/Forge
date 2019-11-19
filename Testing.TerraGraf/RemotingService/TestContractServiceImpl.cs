using System;
using System.IO;
using log4net;
namespace Testing.TerraGraf.RemotingService
{

    public class TestContractServiceImpl : Testing.TerraGraf.RemotingService.TestContractAbstractServiceProxy
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TestContractServiceImpl));

        private string mName = "JZO";

        private int mAge = 32;

        public TestContractServiceImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

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

        public override void SendNonImportantMessage(System.String _p0)
        {
            LOGGER.Info("TestContractServiceImpl.SendNonImportantMessage: " + _p0);
        }

        public override System.IO.Stream GetImage()
        {
            LOGGER.Info("TestContractServiceImpl.GetImage");
            using (FileStream fs = new FileStream("Forge.Net.TerraGraf.pdb", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                SendResponseManually(fs); // van returnTimeout-os override is
            }
            return null;
        }

        public override void SetImage(System.IO.Stream _p0)
        {
            // NOTICE: your responsibility to call Dispose() method on the given Stream(s) before you return from this statement or later on an other thread.
            try
            {
                // you may use this try-finally pattern and take your code here
                LOGGER.Info("TestContractServiceImpl.SetImage, size: " + (_p0 == null ? "<null>" : _p0.Length.ToString()));
            }
            finally
            {
                if (_p0 != null)
                {
                    _p0.Dispose();
                }
            }
            if (_p0 == null)
            {
                throw new Exception("Test exception.");
            }
            //throw new System.NotImplementedException();
        }

        public override System.String GetName()
        {
            LOGGER.Info("TestContractServiceImpl.GetName");
            return mName;
        }

        public override void SetName(System.String _p0)
        {
            LOGGER.Info("TestContractServiceImpl.SetName: " + _p0);
            this.mName = _p0;
        }

        public override System.Int32 GetAge()
        {
            LOGGER.Info("TestContractServiceImpl.GetAge");
            return mAge;
        }

        public override void SetAge(System.Int32 _p0)
        {
            LOGGER.Info("TestContractServiceImpl.SetAge: " + _p0);
            this.mAge = _p0;
        }

        public override void SayHello()
        {
            LOGGER.Info("TestContractServiceImpl.SayHello BEGIN");
            SendMessage("HELLO - CALLBACK");
            LOGGER.Info("TestContractServiceImpl.SayHello END");
        }

        public override void DoNothing()
        {
            LOGGER.Info("TestContractServiceImpl.DoNothing");
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
