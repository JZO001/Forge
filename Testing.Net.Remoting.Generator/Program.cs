using Forge.Net.Remoting.ProxyGenerator;
using Testing.Net.Remoting.Generator.Contracts;

namespace Testing.Net.Remoting.Generator
{

    internal class Program
    {

        static void Main(string[] args)
        {
            ProxyGenerator<ITestContract> g1 = new ProxyGenerator<ITestContract>();
            g1.Generate("ITestContractDir");
            File.Copy("ITestContractDir\\TestContractAbstractClientProxy.cs", "..\\..\\..\\RemotingClient\\TestContractAbstractClientProxy.cs", true);
            File.Copy("ITestContractDir\\TestContractAbstractServiceProxy.cs", "..\\..\\..\\RemotingService\\TestContractAbstractServiceProxy.cs", true);
            File.Copy("ITestContractDir\\TestContractClientImpl.cs", "..\\..\\..\\RemotingClient\\TestContractClientImpl.cs", true);
            File.Copy("ITestContractDir\\TestContractServiceImpl.cs", "..\\..\\..\\RemotingService\\TestContractServiceImpl.cs", true);

            ProxyGenerator<ITestContractSimple> g2 = new ProxyGenerator<ITestContractSimple>();
            g2.Generate("ITestContractSimpleDir");
            File.Copy("ITestContractSimpleDir\\TestContractSimpleAbstractClientProxy.cs", "..\\..\\..\\RemotingClient\\TestContractSimpleAbstractClientProxy.cs", true);
            //File.Copy("ITestContractSimpleDir\\TestContractSimpleAbstractServiceProxy.cs", "..\\..\\..\\RemotingService\\TestContractSimpleAbstractServiceProxy.cs", true);
            File.Copy("ITestContractSimpleDir\\TestContractSimpleClientImpl.cs", "..\\..\\..\\RemotingClient\\TestContractSimpleClientImpl.cs", true);
            File.Copy("ITestContractSimpleDir\\TestContractSimpleServiceImpl.cs", "..\\..\\..\\RemotingService\\TestContractSimpleServiceImpl.cs", true);

            ProxyGenerator<ITestContractStream> g3 = new ProxyGenerator<ITestContractStream>();
            g3.Generate("ITestContractStreamDir");
            File.Copy("ITestContractStreamDir\\TestContractStreamAbstractClientProxy.cs", "..\\..\\..\\RemotingClient\\TestContractStreamAbstractClientProxy.cs", true);
            File.Copy("ITestContractStreamDir\\TestContractStreamAbstractServiceProxy.cs", "..\\..\\..\\RemotingService\\TestContractStreamAbstractServiceProxy.cs", true);
            File.Copy("ITestContractStreamDir\\TestContractStreamClientImpl.cs", "..\\..\\..\\RemotingClient\\TestContractStreamClientImpl.cs", true);
            File.Copy("ITestContractStreamDir\\TestContractStreamServiceImpl.cs", "..\\..\\..\\RemotingService\\TestContractStreamServiceImpl.cs", true);

            ProxyGenerator<ITestSingleCall> g4 = new ProxyGenerator<ITestSingleCall>();
            g4.Generate("ITestSingleCallDir");
            File.Copy("ITestSingleCallDir\\TestSingleCallClientImpl.cs", "..\\..\\..\\RemotingClient\\TestSingleCallClientImpl.cs", true);
            File.Copy("ITestSingleCallDir\\TestSingleCallServiceImpl.cs", "..\\..\\..\\RemotingService\\TestSingleCallServiceImpl.cs", true);

            ProxyGenerator<ITestSingleton> g5 = new ProxyGenerator<ITestSingleton>();
            g5.Generate("ITestSingletonDir");
            File.Copy("ITestSingletonDir\\TestSingletonClientImpl.cs", "..\\..\\..\\RemotingClient\\TestSingletonClientImpl.cs", true);
            File.Copy("ITestSingletonDir\\TestSingletonServiceImpl.cs", "..\\..\\..\\RemotingService\\TestSingletonServiceImpl.cs", true);

            ProxyGenerator<ITestNullable> g6 = new ProxyGenerator<ITestNullable>();
            g6.Generate("ITestNullable");
            File.Copy("ITestNullable\\TestNullableAbstractClientProxy.cs", "..\\..\\..\\RemotingClient\\TestNullableAbstractClientProxy.cs", true);
            File.Copy("ITestNullable\\TestNullableAbstractServiceProxy.cs", "..\\..\\..\\RemotingService\\TestNullableAbstractServiceProxy.cs", true);
            File.Copy("ITestNullable\\TestNullableClientImpl.cs", "..\\..\\..\\RemotingClient\\TestNullableClientImpl.cs", true);
            File.Copy("ITestNullable\\TestNullableServiceImpl.cs", "..\\..\\..\\RemotingService\\TestNullableServiceImpl.cs", true);

            ProxyGenerator<IDemo> g7 = new ProxyGenerator<IDemo>();
            g7.Generate("IDemo");
            File.Copy("IDemo\\DemoAbstractClientProxy.cs", "..\\..\\..\\RemotingClient\\DemoAbstractClientProxy.cs", true);
            File.Copy("IDemo\\DemoAbstractServiceProxy.cs", "..\\..\\..\\RemotingService\\DemoAbstractServiceProxy.cs", true);
            File.Copy("IDemo\\DemoClientImpl.cs", "..\\..\\..\\RemotingClient\\DemoClientImpl.cs", true);
            File.Copy("IDemo\\DemoServiceImpl.cs", "..\\..\\..\\RemotingService\\DemoServiceImpl.cs", true);

        }

    }

}