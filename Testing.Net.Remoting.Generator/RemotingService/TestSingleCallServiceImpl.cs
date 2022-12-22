namespace Testing.Net.Remoting.Generator.RemotingService
{

    public class TestSingleCallServiceImpl : Forge.Legacy.MBRBase, Testing.Net.Remoting.Generator.Contracts.ITestSingleCall
    {

        public TestSingleCallServiceImpl()
        {
        }

        public System.String GetName()
        {
            throw new System.NotImplementedException();
        }

        public void SayHello()
        {
            throw new System.NotImplementedException();
        }

        public void SendImage(System.IO.Stream stream)
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

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

    }

}
