using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Forge.Configuration.Shared;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Proxy;
using Forge.Net.Remoting.Service;
using Forge.Net.Remoting.Sinks;
using Forge.Net.Synapse;
using Forge.Net.Synapse.NetworkFactory;
using Forge.Net.Synapse.NetworkServices;
using Forge.Net.TerraGraf;
using Forge.Net.TerraGraf.Contexts;
using Forge.Net.TerraGraf.NetworkPeers;
using Testing.TerraGraf.Contracts;
using Testing.TerraGraf.RemotingClient;
using Testing.TerraGraf.RemotingService;
using log4net;
using Forge;

namespace Testing.TerraGraf
{

    public class Wrapper : MBRBase
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(Wrapper));

        private Forge.Threading.ThreadPool mThreadPool = new Forge.Threading.ThreadPool(1, 100, 256);

        private MasterForm mOwnerForm = null;

        private Thread mThreadPing = null;

        #region Broadcast field(s)

        private Dictionary<long, bool> mBroadcast_ServerSwitches = new Dictionary<long, bool>();

        private Dictionary<long, Thread> mBroadcast_ServerThreads = new Dictionary<long, Thread>();

        private Dictionary<long, UdpClient> mBroadcast_ServerUdpClients = new Dictionary<long, UdpClient>();

        private Dictionary<long, bool> mBroadcast_ClientSwitches = new Dictionary<long, bool>();

        private Dictionary<long, Thread> mBroadcast_ClientThreads = new Dictionary<long, Thread>();

        #endregion

        #region UDP field(s)

        private Dictionary<long, bool> mUDP_ServerSwitches = new Dictionary<long, bool>();

        private Dictionary<long, Thread> mUDP_ServerThreads = new Dictionary<long, Thread>();

        private Dictionary<long, UdpClient> mUDP_ServerUdpClients = new Dictionary<long, UdpClient>();

        private Dictionary<long, bool> mUDP_ClientSwitches = new Dictionary<long, bool>();

        private Dictionary<long, Thread> mUDP_ClientThreads = new Dictionary<long, Thread>();

        private Dictionary<long, UdpClient> mUDP_ClientUdpClients = new Dictionary<long, UdpClient>();

        #endregion

        #region TCP field(s)

        private Dictionary<long, bool> mTCP_ServerSwitches = new Dictionary<long, bool>();

        private Dictionary<long, Thread> mTCP_ServerThreads = new Dictionary<long, Thread>();

        private Dictionary<long, TcpListener> mTCP_ServerTcpListeners = new Dictionary<long, TcpListener>();

        private Dictionary<long, ITcpClient> mTCP_TcpClients = new Dictionary<long, ITcpClient>();

        private Dictionary<long, bool> mTCP_ClientSwitches = new Dictionary<long, bool>();

        private Dictionary<long, Thread> mTCP_ClientThreads = new Dictionary<long, Thread>();

        #endregion

        ///// <summary>
        ///// Occurs when [network peer discovered].
        ///// </summary>
        //public event EventHandler<NetworkPeerChangedEventArgs> NetworkPeerDiscovered;

        ///// <summary>
        ///// Occurs when [network peer distance changed].
        ///// </summary>
        //public event EventHandler<NetworkPeerChangedEventArgs> NetworkPeerDistanceChanged;

        ///// <summary>
        ///// Occurs when [network peer unaccessible].
        ///// </summary>
        //public event EventHandler<NetworkPeerChangedEventArgs> NetworkPeerUnaccessible;

        ///// <summary>
        ///// Occurs when [network peer context changed].
        ///// </summary>
        //public event EventHandler<NetworkPeerContextEventArgs> NetworkPeerContextChanged;

        #endregion

        public Wrapper()
        {
            Console.WriteLine("Wrapper is executing in domain: " + AppDomain.CurrentDomain.FriendlyName);
        }

        public void Initialize(string id, MasterForm form)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            log4net.Config.XmlConfigurator.Configure();
            this.Id = id;
            this.mOwnerForm = form;
            Forge.Net.TerraGraf.NetworkManager.Instance.NetworkPeerDiscovered += new EventHandler<NetworkPeerChangedEventArgs>(Instance_NetworkPeerDiscovered);
            Forge.Net.TerraGraf.NetworkManager.Instance.NetworkPeerDistanceChanged += new EventHandler<NetworkPeerDistanceChangedEventArgs>(Instance_NetworkPeerDistanceChanged);
            Forge.Net.TerraGraf.NetworkManager.Instance.NetworkPeerUnaccessible += new EventHandler<NetworkPeerChangedEventArgs>(Instance_NetworkPeerUnaccessible);
            Forge.Net.TerraGraf.NetworkManager.Instance.NetworkPeerContextChanged += new EventHandler<NetworkPeerContextEventArgs>(Instance_NetworkPeerContextChanged);
            Forge.Net.TerraGraf.NetworkManager.Instance.Start();
            mThreadPing = new Thread(new ThreadStart(delegate()
                {
                    Thread.Sleep(10000);
                    form.MbrPing();
                }));
            mThreadPing.Name = "Wrapper_Ping_" + id;
            mThreadPing.IsBackground = true;
            mThreadPing.Start();

            ServiceBaseServices.Initialize();
            ProxyServices.Initialize();

            BinaryMessageSink sink = new BinaryMessageSink(true, 1024);
            List<IMessageSink> sinks = new List<IMessageSink>();
            sinks.Add(sink);

            AddressEndPoint serverEndPoint = new AddressEndPoint("127.0.0.1", 57000);
            List<AddressEndPoint> serverData = new List<AddressEndPoint>();
            serverData.Add(serverEndPoint);

            DefaultNetworkFactory networkFactory = new DefaultNetworkFactory();

            DefaultServerStreamFactory serverStreamFactory = new DefaultServerStreamFactory();
            SslClientStreamFactory clientStreamFactory = new SslClientStreamFactory("ForgeNET", true);

            string channelId = "INTNET_CLIENT";
            TCPChannel channelClient = new TCPChannel("INTNET_CLIENT", sinks, sinks, networkFactory, serverStreamFactory, clientStreamFactory);
            ChannelServices.RegisterChannel(channelClient);

            ProxyServices.RegisterImplementationForChannel(typeof(ITestContract), channelId, typeof(TestContractClientImpl));
            ProxyServices.RegisterImplementationForChannel(typeof(ITestContractSimple), channelId, typeof(TestContractSimpleClientImpl));
            ProxyServices.RegisterImplementationForChannel(typeof(ITestContractStream), channelId, typeof(TestContractStreamClientImpl));
            ProxyServices.RegisterImplementationForChannel(typeof(ITestSingleCall), channelId, typeof(TestSingleCallClientImpl));
            ProxyServices.RegisterImplementationForChannel(typeof(ITestSingleton), channelId, typeof(TestSingletonClientImpl));

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LOGGER.Error(e.ExceptionObject.ToString());
        }

        public string Id { get; set; }

        public Forge.Net.TerraGraf.NetworkManager NetworkManager { get { return Forge.Net.TerraGraf.NetworkManager.Instance; } }

        public ICollection<NetworkContext> KnownNetworkContexts
        {
            get { return NetworkContext.KnownNetworkContexts; }
        }

        public ICollection<INetworkPeerRemote> KnownNetworkPeers
        {
            get { return NetworkPeerContext.KnownNetworkPeers; }
        }

        public void Debug()
        {
            Console.WriteLine();
        }

        public bool IsBlackHole
        {
            get { return Forge.Net.TerraGraf.NetworkManager.Instance.Localhost.IsBlackHole; }
            set { Forge.Net.TerraGraf.NetworkManager.Instance.Localhost.IsBlackHole = value; }
        }

        #region Remoting

        private bool mRemoteTestingInternetwork = false;

        private bool mRemoteTestingTerraGraf = false;

        private Thread mThRemotingTerraGraf = null;

        private Thread mThRemotingInterNet = null;

        public bool IsRemoteTestingInterNetwork
        {
            get { return mRemoteTestingInternetwork; }
        }

        public bool IsRemoteTestingTerraGraf
        {
            get { return mRemoteTestingTerraGraf; }
        }

        public void InitializeIntNetServerChannel(string channelId, AddressEndPoint endPoint, NetworkFactoryType factoryType)
        {
            BinaryMessageSink sink = new BinaryMessageSink(true, 1024);
            List<IMessageSink> sinks = new List<IMessageSink>();
            sinks.Add(sink);

            List<AddressEndPoint> serverData = new List<AddressEndPoint>();
            serverData.Add(endPoint);

            INetworkFactory networkFactory = null;
            if (factoryType == NetworkFactoryType.InterNetwork)
            {
                networkFactory = new DefaultNetworkFactory();
            }
            else
            {
                networkFactory = new TerraGrafNetworkFactory();
            }

            StoreName storeName = StoreName.My;
            StoreLocation storeLocation = StoreLocation.CurrentUser;
            X509Certificate certificate = null;

            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                foreach (X509Certificate c in store.Certificates)
                {
                    if (c.Subject.Equals("CN=ForgeNET"))
                    {
                        certificate = c;
                        break;
                    }
                }
                if (certificate == null)
                {
                    throw new InvalidConfigurationException("Failed to find certificate.");
                }
            }
            catch (InvalidConfigurationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidConfigurationException("Failed to find certificate.", ex);
            }
            finally
            {
                store.Close();
            }

            SslServerStreamFactory serverStreamFactory = new SslServerStreamFactory(certificate);
            SslClientStreamFactory clientStreamFactory = new SslClientStreamFactory("ForgeNET", true);

            TCPChannel channelServer = new TCPChannel(channelId, sinks, sinks, serverData, networkFactory, serverStreamFactory, clientStreamFactory);
            channelServer.StartListening();

            ChannelServices.RegisterChannel(channelServer);

            ServiceBaseServices.RegisterImplementationForChannel(typeof(ITestContract), channelId, typeof(TestContractServiceImpl));
            ServiceBaseServices.RegisterImplementationForChannel(typeof(ITestContractSimple), channelId, typeof(TestContractSimpleServiceImpl));
            ServiceBaseServices.RegisterImplementationForChannel(typeof(ITestContractStream), channelId, typeof(TestContractStreamServiceImpl));
            ServiceBaseServices.RegisterImplementationForChannel(typeof(ITestSingleCall), channelId, typeof(TestSingleCallServiceImpl));
            ServiceBaseServices.RegisterImplementationForChannel(typeof(ITestSingleton), channelId, typeof(TestSingletonServiceImpl));
        }

        public ICollection<string> RegisteredChannels
        {
            get
            {
                List<string> channels = new List<string>();
                foreach (Channel c in ChannelServices.RegisteredChannels)
                {
                    channels.Add(c.ChannelId);
                }
                return channels;
            }
        }

        public void StartRemotingTestInterNetwork(string channelId, AddressEndPoint remoteEndPoint)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                ThrowHelper.ThrowArgumentNullException("channelId");
            }
            if (remoteEndPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("remoteEndPoint");
            }

            mRemoteTestingInternetwork = true;
            mThRemotingInterNet = new Thread(new ParameterizedThreadStart(RemotingTestInternetwork));
            mThRemotingInterNet.IsBackground = true;
            mThRemotingInterNet.Name = "Remoting_Test_Internetwork";
            mThRemotingInterNet.Start(new RemotingTestParams() { ChannelId = channelId, RemoteEndPoint = remoteEndPoint });
        }

        public void StartRemotingTestTerraGraf(string channelId, AddressEndPoint remoteEndPoint)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                ThrowHelper.ThrowArgumentNullException("channelId");
            }
            if (remoteEndPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("remoteEndPoint");
            }

            mRemoteTestingTerraGraf = true;
            mThRemotingTerraGraf = new Thread(new ParameterizedThreadStart(RemotingTestTerraGraf));
            mThRemotingTerraGraf.IsBackground = true;
            mThRemotingTerraGraf.Name = "Remoting_Test_TerraGraf";
            mThRemotingTerraGraf.Start(new RemotingTestParams() { ChannelId = channelId, RemoteEndPoint = remoteEndPoint });
        }

        public void StopRemotingTestInterNetwork()
        {
            mRemoteTestingInternetwork = false;
            try
            {
                mThRemotingInterNet.Join();
            }
            catch (Exception) { }
        }

        public void StopRemotingTestTerraGraf()
        {
            mRemoteTestingTerraGraf = false;
            try
            {
                mThRemotingTerraGraf.Join();
            }
            catch (Exception) { }
        }

        private void RemotingTestTerraGraf(object p)
        {
            RemotingTestParams prms = (RemotingTestParams)p;
            try
            {
                ProxyFactory<ITestContract> testContractProxyFactory = new ProxyFactory<ITestContract>(prms.ChannelId, prms.RemoteEndPoint);
                ProxyFactory<ITestContractSimple> testContractSimpleProxyFactory = new ProxyFactory<ITestContractSimple>(prms.ChannelId, prms.RemoteEndPoint);
                ProxyFactory<ITestContractStream> testContractStreamProxyFactory = new ProxyFactory<ITestContractStream>(prms.ChannelId, prms.RemoteEndPoint);
                ProxyFactory<ITestSingleCall> testSingleCallProxyFactory = new ProxyFactory<ITestSingleCall>(prms.ChannelId, prms.RemoteEndPoint);
                ProxyFactory<ITestSingleton> testSingletonProxyFactory = new ProxyFactory<ITestSingleton>(prms.ChannelId, prms.RemoteEndPoint);

                using (ITestContract testContract = testContractProxyFactory.CreateProxy())
                {
                    using (ITestContractSimple testContractSimple = testContractSimpleProxyFactory.CreateProxy())
                    {
                        using (ITestContractStream testContractStream = testContractStreamProxyFactory.CreateProxy())
                        {
                            using (ITestSingleCall testSingleCall = testSingleCallProxyFactory.CreateProxy())
                            {
                                using (ITestSingleton testSingleton = testSingletonProxyFactory.CreateProxy())
                                {
                                    while (IsRemoteTestingTerraGraf)
                                    {
                                        RunRemotingTest(testContract, testContractSimple, testContractStream, testSingleCall, testSingleton);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOGGER.Error("TerraGraf testing. Failed to create proxy.", ex);
            }
        }

        private void RemotingTestInternetwork(object p)
        {
            RemotingTestParams prms = (RemotingTestParams)p;
            try
            {
                ProxyFactory<ITestContract> testContractProxyFactory = new ProxyFactory<ITestContract>(prms.ChannelId, prms.RemoteEndPoint);
                ProxyFactory<ITestContractSimple> testContractSimpleProxyFactory = new ProxyFactory<ITestContractSimple>(prms.ChannelId, prms.RemoteEndPoint);
                ProxyFactory<ITestContractStream> testContractStreamProxyFactory = new ProxyFactory<ITestContractStream>(prms.ChannelId, prms.RemoteEndPoint);
                ProxyFactory<ITestSingleCall> testSingleCallProxyFactory = new ProxyFactory<ITestSingleCall>(prms.ChannelId, prms.RemoteEndPoint);
                ProxyFactory<ITestSingleton> testSingletonProxyFactory = new ProxyFactory<ITestSingleton>(prms.ChannelId, prms.RemoteEndPoint);

                using (ITestContract testContract = testContractProxyFactory.CreateProxy())
                {
                    using (ITestContractSimple testContractSimple = testContractSimpleProxyFactory.CreateProxy())
                    {
                        using (ITestContractStream testContractStream = testContractStreamProxyFactory.CreateProxy())
                        {
                            using (ITestSingleCall testSingleCall = testSingleCallProxyFactory.CreateProxy())
                            {
                                using (ITestSingleton testSingleton = testSingletonProxyFactory.CreateProxy())
                                {
                                    while (IsRemoteTestingInterNetwork)
                                    {
                                        RunRemotingTest(testContract, testContractSimple, testContractStream, testSingleCall, testSingleton);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOGGER.Error("InterNetwork testing. Failed to create proxy.", ex);
            }
        }

        private void RunRemotingTest(ITestContract testContract,
            ITestContractSimple testContractSimple,
            ITestContractStream testContractStream,
            ITestSingleCall testSingleCall,
            ITestSingleton testSingleton)
        {
            LOGGER.Info("ITestContract.GetAge begin");
            int age = testContract.GetAge();
            LOGGER.Info("ITestContract.GetAge end, " + age.ToString());

            LOGGER.Info("ITestContract.SetAge begin, " + age.ToString());
            testContract.SetAge(age + 1);
            LOGGER.Info("ITestContract.GetAge end");

            LOGGER.Info("ITestContract.GetImage begin");
            Stream stream = testContract.GetImage();
            LOGGER.Info("ITestContract.GetImage end");

            LOGGER.Info("ITestContract.SetImage begin");
            testContract.SetImage(stream);
            LOGGER.Info("ITestContract.SetImage end");
            if (stream != null)
            {
                stream.Dispose();
            }

            LOGGER.Info("ITestContract.GetName begin");
            String name = ((ITestContractSimple)testContract).GetName(); // ez érdekes, meg kell adni hogy melyik interface metódusát akarom hívni, pedig ez proxy-ban nem számít
            LOGGER.Info("ITestContract.GetName end, " + name);

            LOGGER.Info("ITestContract.SetName begin, " + name);
            testContract.SetName(name);
            LOGGER.Info("ITestContract.SetName end");

            LOGGER.Info("ITestContract.SayHello begin");
            testContract.SayHello();
            LOGGER.Info("ITestContract.SayHello end");

            LOGGER.Info("ITestContract.SayHello begin");
            testContract.SayHello();
            LOGGER.Info("ITestContract.SayHello end");

            LOGGER.Info("ITestContract.SendNonImportantMessage begin");
            testContract.SendNonImportantMessage("TESTCONTRACT: DatagramOneway message demo");
            LOGGER.Info("ITestContract.SendNonImportantMessage end");

            try
            {
                // exception simulation
                LOGGER.Info("ITestContract.SetImage begin");
                testContract.SetImage(null);
            }
            catch (Exception ex)
            {
                LOGGER.Info("EXC TEST, Details: " + ex.Message);
            }
            finally
            {
                LOGGER.Info("ITestContract.SetImage end");
            }

            // test contract simple
            LOGGER.Info("ITestContractSimple.GetName and SetName begin");
            testContractSimple.SetName(testContractSimple.GetName());
            LOGGER.Info("ITestContractSimple.GetName and SetName end");

            LOGGER.Info("ITestContractSimple.GetAge begin");
            testContractSimple.SetAge(testContractSimple.GetAge() + 1);
            LOGGER.Info("ITestContractSimple.GetAge end");

            // test contract stream
            LOGGER.Info("ITestContractStream.GetName begin");
            LOGGER.Info(testContractStream.GetName());
            LOGGER.Info("ITestContractStream.GetName end");

            LOGGER.Info("ITestContractStream.SayHello begin");
            testContractStream.SayHello();
            LOGGER.Info("ITestContractStream.SayHello end");

            // test single call
            LOGGER.Info("ITestSingleCall.GetName, " + testSingleCall.GetName());

            LOGGER.Info("ITestSingleCall.SayHello begin");
            testSingleCall.SayHello();
            LOGGER.Info("ITestSingleCall.SayHello begin");

            FileStream testStream = null;
            try
            {
                // exception simulation
                testStream = new FileStream("Forge.Net.TerraGraf.pdb", FileMode.Open, FileAccess.Read, FileShare.Read);
                LOGGER.Info("ITestSingleCall.SendImage begin");
                testSingleCall.SendImage(testStream);
            }
            finally
            {
                LOGGER.Info("ITestSingleCall.SendImage end");
                testStream.Close();
            }

            // test singleton
            LOGGER.Info("ITestSingleton.GetName, " + testSingleton.GetName());

            LOGGER.Info("ITestSingleton.SayHello begin");
            testSingleton.SayHello();
            LOGGER.Info("ITestSingleton.SayHello end");

            try
            {
                // exception simulation
                testStream = new FileStream("Forge.Net.TerraGraf.pdb", FileMode.Open, FileAccess.Read, FileShare.Read);
                LOGGER.Info("ITestSingleton.SendImage begin");
                testSingleton.SendImage(testStream);
            }
            finally
            {
                LOGGER.Info("ITestSingleton.SendImage end");
                testStream.Close();
            }

            Thread.Sleep(0);
        }

        private class RemotingTestParams
        {

            public string ChannelId { get; set; }

            public AddressEndPoint RemoteEndPoint { get; set; }

        }

        #endregion

        #region Broadcasting

        public long StartBroadcastUdpServer(int port)
        {
            lock (mBroadcast_ServerSwitches)
            {
                long id = DateTime.Now.Ticks;
                Thread thread = new Thread(new ParameterizedThreadStart(BroadcastUdpServer));
                thread.IsBackground = true;
                thread.Name = "TEST_BROADCAST_UDP_SERVER_" + port.ToString();
                mBroadcast_ServerThreads.Add(id, thread);
                mBroadcast_ServerSwitches.Add(id, true);
                thread.Start(new long[] { id, port });
                return id;
            }
        }

        public void StopBroadcastUdpServer(long id)
        {
            Thread t = null;
            lock (mBroadcast_ServerSwitches)
            {
                if (mBroadcast_ServerSwitches.ContainsKey(id))
                {
                    t = mBroadcast_ServerThreads[id];
                    mBroadcast_ServerSwitches[id] = false;
                }
            }
            lock (mBroadcast_ServerUdpClients)
            {
                if (mBroadcast_ServerUdpClients.ContainsKey(id))
                {
                    mBroadcast_ServerUdpClients[id].Dispose();
                }
            }
            if (t != null)
            {
                t.Join();
            }
        }

        public ICollection<long> GetActiveBroadcastServers()
        {
            lock (mBroadcast_ServerSwitches)
            {
                return new List<long>(mBroadcast_ServerSwitches.Keys);
            }
        }

        public long StartBroadcastUdpClient(int localPort, int targetPort)
        {
            lock (mBroadcast_ClientSwitches)
            {
                long id = DateTime.Now.Ticks;
                Thread thread = new Thread(new ParameterizedThreadStart(BroadcastUdpClient));
                thread.IsBackground = true;
                thread.Name = "TEST_BROADCAST_UDP_CLIENT_" + localPort.ToString() + "_" + targetPort.ToString();
                mBroadcast_ClientThreads.Add(id, thread);
                mBroadcast_ClientSwitches.Add(id, true);
                thread.Start(new long[] { id, localPort, targetPort });
                return id;
            }
        }

        public void StopBroadcastUdpClient(long id)
        {
            Thread t = null;
            lock (mBroadcast_ClientSwitches)
            {
                if (mBroadcast_ClientSwitches.ContainsKey(id))
                {
                    t = mBroadcast_ClientThreads[id];
                    mBroadcast_ClientSwitches[id] = false;
                }
            }
            if (t != null)
            {
                t.Join();
            }
        }

        public ICollection<long> GetActiveBroadcastClients()
        {
            lock (mBroadcast_ClientSwitches)
            {
                return new List<long>(mBroadcast_ClientSwitches.Keys);
            }
        }

        private void BroadcastUdpServer(object port)
        {
            long[] ps = (long[])port;
            long id = ps[0];
            int localPort = (int)ps[1];

            AddressEndPoint broadcastEp = new AddressEndPoint(AddressEndPoint.Any, 0);
            UdpClient udpClient = null;
            while (udpClient == null)
            {
                try
                {
                    udpClient = new UdpClient(localPort); // ez számít
                    lock (mBroadcast_ServerUdpClients)
                    {
                        mBroadcast_ServerUdpClients.Add(id, udpClient);
                    }
                    //udpClient.Connect(IPAddress.Any, 5001); // ezzel elvileg szűrni is fogunk
                    LOGGER.Info(string.Format("Broadcast server is listening on port: {0}", ((AddressEndPoint)udpClient.Client.LocalEndPoint).Port));
                }
                catch (Exception)
                {
                    localPort++;
                }
            }
            udpClient.EnableBroadcast = true;
            while (mBroadcast_ServerSwitches[id])
            {
                try
                {
                    byte[] data = udpClient.Receive(ref broadcastEp); // fogadás, a broadcastEp-ba kerül bele, hogy honnan jött az üzi
                    LOGGER.Info(string.Format("BROADCAST_SERVER({0}), received from: {1}, data: {2}", id, broadcastEp.ToString(), Encoding.UTF8.GetString(data)));
                    Thread.Sleep(0);
                }
                catch (Exception ex)
                {
                    LOGGER.Error(string.Format("BROADCAST_SERVER({0}) threw an exception. Reason: {1}", id, ex.Message), ex);
                }
            }
            udpClient.Dispose();
            LOGGER.Info(string.Format("BROADCAST_SERVER ({0}) stopped.", id));
            lock (mBroadcast_ServerSwitches)
            {
                mBroadcast_ServerSwitches.Remove(id);
                mBroadcast_ServerThreads.Remove(id);
            }
            lock (mBroadcast_ServerUdpClients)
            {
                mBroadcast_ServerUdpClients.Remove(id);
            }
        }

        private void BroadcastUdpClient(object ports)
        {
            long[] ps = (long[])ports;
            long id = ps[0];
            int localPort = (int)ps[1];
            int targetPort = (int)ps[2];

            AddressEndPoint remoteEp = new AddressEndPoint(AddressEndPoint.Broadcast, targetPort);
            UdpClient udpClient = new UdpClient(localPort);
            udpClient.EnableBroadcast = true;

            int index = 0;
            string guid = Guid.NewGuid().ToString();
            while (mBroadcast_ClientSwitches[id])
            {
                Thread.Sleep(1000);
                index++;
                string data = string.Format("HELLO({0}), {1}", index, guid);
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                udpClient.Send(bytes, remoteEp);
                LOGGER.Info(string.Format("BROADCAST_CLIENT({0}), localPort: {1}, targetPort: {2}, data: {3}", id, ((AddressEndPoint)udpClient.Client.LocalEndPoint).Port, targetPort, data));
            }
            udpClient.Dispose();
            LOGGER.Info(string.Format("BROADCAST_CLIENT ({0}) stopped.", id));
            lock (mBroadcast_ClientSwitches)
            {
                mBroadcast_ClientSwitches.Remove(id);
                mBroadcast_ClientThreads.Remove(id);
            }
        }

        #endregion

        #region UDP

        private class UdpClientParams
        {

            public long Id { get; set; }

            public int LocalPort { get; set; }

            public string RemoteHost { get; set; }

            public int TargetPort { get; set; }

            public bool HugeData { get; set; }

        }

        public long StartUdpServer(int port)
        {
            lock (mUDP_ServerSwitches)
            {
                long id = DateTime.Now.Ticks;
                Thread thread = new Thread(new ParameterizedThreadStart(UdpServerMain));
                thread.IsBackground = true;
                thread.Name = "TEST_UDP_SERVER_" + port.ToString();
                mUDP_ServerThreads.Add(id, thread);
                mUDP_ServerSwitches.Add(id, true);
                thread.Start(new long[] { id, port });
                return id;
            }
        }

        public void StopUdpServer(long id)
        {
            Thread t = null;
            lock (mUDP_ServerSwitches)
            {
                if (mUDP_ServerSwitches.ContainsKey(id))
                {
                    t = mUDP_ServerThreads[id];
                    mUDP_ServerSwitches[id] = false;
                }
            }
            lock (mUDP_ServerUdpClients)
            {
                if (mUDP_ServerUdpClients.ContainsKey(id))
                {
                    mUDP_ServerUdpClients[id].Dispose();
                }
            }
            if (t != null)
            {
                t.Join();
            }
        }

        public ICollection<long> GetActiveUDPServers()
        {
            lock (mUDP_ServerSwitches)
            {
                return new List<long>(mUDP_ServerSwitches.Keys);
            }
        }

        public long StartUdpClient(int localPort, string remoteHost, int targetPort, bool hugeData)
        {
            lock (mUDP_ClientSwitches)
            {
                long id = DateTime.Now.Ticks;
                Thread thread = new Thread(new ParameterizedThreadStart(UdpClientMain));
                thread.IsBackground = true;
                thread.Name = "TEST_UDP_CLIENT_" + localPort.ToString() + "_" + remoteHost + "_" + targetPort.ToString();
                mUDP_ClientThreads.Add(id, thread);
                mUDP_ClientSwitches.Add(id, true);
                thread.Start(new UdpClientParams() { Id = id, LocalPort = localPort, RemoteHost = remoteHost, TargetPort = targetPort, HugeData = hugeData });
                return id;
            }
        }

        public void StopUdpClient(long id)
        {
            Thread t = null;
            lock (mUDP_ClientSwitches)
            {
                if (mUDP_ClientSwitches.ContainsKey(id))
                {
                    t = mUDP_ClientThreads[id];
                    mUDP_ClientSwitches[id] = false;
                }
            }
            lock (mUDP_ClientUdpClients)
            {
                if (mUDP_ClientUdpClients.ContainsKey(id))
                {
                    mUDP_ClientUdpClients[id].Dispose();
                }
            }
            if (t != null)
            {
                t.Join();
            }
        }

        public ICollection<long> GetActiveUdpClients()
        {
            lock (mUDP_ClientSwitches)
            {
                return new List<long>(mUDP_ClientSwitches.Keys);
            }
        }

        private void UdpServerMain(object port)
        {
            long[] ps = (long[])port;
            long id = ps[0];
            int localPort = (int)ps[1];

            AddressEndPoint remoteEp = new AddressEndPoint(AddressEndPoint.Any, 0);
            UdpClient udpClient = null;
            while (udpClient == null)
            {
                try
                {
                    udpClient = new UdpClient(localPort); // ez számít
                    lock (mUDP_ServerUdpClients)
                    {
                        mUDP_ServerUdpClients.Add(id, udpClient);
                    }
                    //udpClient.Connect(IPAddress.Any, 5001); // ezzel elvileg szűrni is fogunk
                    LOGGER.Info(string.Format("UDP server is listening on port: {0}", localPort));
                }
                catch (Exception)
                {
                    localPort++;
                }
            }
            while (mUDP_ServerSwitches[id])
            {
                try
                {
                    byte[] data = udpClient.Receive(ref remoteEp); // fogadás, a remoteEp-ba kerül bele, hogy honnan jött az üzi
                    LOGGER.Info(string.Format("UDP_SERVER({0}), received from: {1}, data: {2}", id, remoteEp.ToString(), Encoding.UTF8.GetString(data)));
                    Thread.Sleep(0);
                }
                catch (Exception ex)
                {
                    LOGGER.Error(string.Format("UDP_SERVER({0}) threw an exception. Reason: {1}", id, ex.Message), ex);
                }
            }
            udpClient.Dispose();
            LOGGER.Info(string.Format("UDP_SERVER ({0}) stopped.", id));
            lock (mUDP_ServerSwitches)
            {
                mUDP_ServerSwitches.Remove(id);
                mUDP_ServerThreads.Remove(id);
            }
            lock (mUDP_ServerUdpClients)
            {
                mUDP_ServerUdpClients.Remove(id);
            }
        }

        private void UdpClientMain(object p)
        {
            UdpClientParams prms = (UdpClientParams)p;

            AddressEndPoint remoteEp = new AddressEndPoint(prms.RemoteHost, prms.TargetPort);
            UdpClient udpClient = new UdpClient(prms.LocalPort);

            lock (mUDP_ClientUdpClients)
            {
                mUDP_ClientUdpClients.Add(prms.Id, udpClient);
            }

            int index = 0;
            string guid = Guid.NewGuid().ToString();
            if (prms.HugeData)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream fs = new FileStream("Forge.Net.TerraGraf.pdb", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        ms.SetLength(fs.Length);
                        byte[] buffer = ms.GetBuffer();
                        fs.Read(buffer, 0, buffer.Length);
                    }
                    while (mUDP_ClientSwitches[prms.Id])
                    {
                        Thread.Sleep(1000);
                        ms.Position = 0;
                        try
                        {
                            udpClient.Send(ms.GetBuffer(), remoteEp);
                            LOGGER.Info(string.Format("UDP_CLIENT({0}), localPort: {1}, remoteHost: {2}, targetPort: {3}", prms.Id, ((AddressEndPoint)udpClient.Client.LocalEndPoint).Port, prms.RemoteHost, prms.TargetPort));
                        }
                        catch (Exception ex)
                        {
                            LOGGER.Error(string.Format("UDP_CLIENT({0}) threw an exception. Reason: {1}", prms.Id, ex.Message), ex);
                            break;
                        }
                    }
                }
            }
            else
            {
                while (mUDP_ClientSwitches[prms.Id])
                {
                    Thread.Sleep(1000);
                    index++;
                    string data = string.Format("HELLO({0}), {1}", index, guid);
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    try
                    {
                        udpClient.Send(bytes, remoteEp);
                        LOGGER.Info(string.Format("UDP_CLIENT({0}), localPort: {1}, remoteHost: {2}, targetPort: {3}, data: {4}", prms.Id, ((AddressEndPoint)udpClient.Client.LocalEndPoint).Port, prms.RemoteHost, prms.TargetPort, data));
                    }
                    catch (Exception ex)
                    {
                        LOGGER.Error(string.Format("UDP_CLIENT({0}) threw an exception. Reason: {1}", prms.Id, ex.Message), ex);
                        break;
                    }
                }
            }
            udpClient.Dispose();
            LOGGER.Info(string.Format("UDP_CLIENT ({0}) stopped.", prms.Id));
            lock (mUDP_ClientSwitches)
            {
                mUDP_ClientSwitches.Remove(prms.Id);
                mUDP_ClientThreads.Remove(prms.Id);
            }
            lock (mUDP_ClientUdpClients)
            {
                mUDP_ClientUdpClients.Remove(prms.Id);
            }
        }

        #endregion

        #region TCP

        private class TCPSendParams
        {

            public long Id { get; set; }

            public bool HugeData { get; set; }

            public bool Repeat { get; set; }

        }

        private class TCPReceiver
        {

            private static Forge.Threading.ThreadPool mThreadPool = new Forge.Threading.ThreadPool(1, 4, 256);
            private long mId = -1;
            private byte[] mBuffer = null;
            private ITcpClient mClient = null;
            private NetworkStream mStream = null;
            private FileStream mFileStream = null;
            private string mCurrentFileName = string.Empty;
            private int mPatternFileSize = Convert.ToInt32(new FileInfo("1.bin").Length);
            private int mReceivedBytes = 0;

            public TCPReceiver(long id, ITcpClient client)
            {
                mId = id;
                mClient = client;
                mBuffer = new byte[client.GetStream().ReceiveBufferSize];
                mStream = client.GetStream();
                //mFileStream = new FileStream("C:\\data_" + Guid.NewGuid().ToString() + ".bin", FileMode.Create, FileAccess.Write, FileShare.Read);
                mStream.BeginRead(mBuffer, 0, mBuffer.Length, new AsyncCallback(Receive), null);
            }

            private void Receive(IAsyncResult result)
            {
                try
                {
                    int counter = mStream.EndRead(result);
                    if (counter > 0)
                    {
                        string data = Encoding.UTF8.GetString(mBuffer, 0, counter);
                        OpenFile();
                        if (mReceivedBytes + counter >= mPatternFileSize)
                        {
                            LOGGER.Info(string.Format("TCP_CLIENT({0}), data size: {1}, total: {2}, received: {3}", mId, counter, mPatternFileSize, data));
                            mFileStream.Write(mBuffer, 0, mPatternFileSize - mReceivedBytes);
                            int offset = mPatternFileSize - mReceivedBytes; // ennyit vettem ki a bufferből
                            counter = counter - (mPatternFileSize - mReceivedBytes); // ennyi maradt még
                            mFileStream.Dispose(); // zárás és újranyitás
                            mFileStream = null;
                            mThreadPool.QueueUserWorkItem(new WaitCallback(CompareFiles), mCurrentFileName);
                            if (counter > 0)
                            {
                                // a maradék
                                OpenFile();
                                mFileStream.Write(mBuffer, offset, counter);
                                mReceivedBytes += counter;
                            }
                        }
                        else
                        {
                            // normál beírás
                            mFileStream.Write(mBuffer, 0, counter);
                            mFileStream.Flush();
                            mReceivedBytes += counter;
                            LOGGER.Info(string.Format("TCP_CLIENT({0}), data size: {1}, total: {2}, received: {3}", mId, counter, mReceivedBytes, data));
                        }
                        mStream.BeginRead(mBuffer, 0, mBuffer.Length, new AsyncCallback(Receive), null);
                    }
                    else
                    {
                        LOGGER.Info(string.Format("TCP_CLIENT({0}), connection lost.", mId));
                        mClient.Dispose();
                        if (mFileStream != null)
                        {
                            mFileStream.Dispose();
                        }
                    }
                }
                catch (TimeoutException ex)
                {
                    LOGGER.Error(string.Format("TCP_CLIENT({0}), normal timeout exception: {1}", mId, ex.Message), ex);
                    mStream.BeginRead(mBuffer, 0, mBuffer.Length, new AsyncCallback(Receive), null);
                }
                catch (Exception ex)
                {
                    LOGGER.Error(string.Format("TCP_CLIENT({0}), exception: {1}", mId, ex.Message), ex);
                    mClient.Dispose();
                    //mFileStream.Dispose();
                }
            }

            private void OpenFile()
            {
                if (mFileStream == null)
                {
                    mCurrentFileName = string.Format("data_{0}.bin", Guid.NewGuid().ToString());
                    mFileStream = new FileStream(mCurrentFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                    mReceivedBytes = 0;
                }
            }

            private void CompareFiles(object state)
            {
                string fileName = (string)state;
                if (new FileInfo("1.bin").Length != new FileInfo(fileName).Length)
                {
                    LOGGER.Error(string.Format("FILE_COMPARE, file size 1.bin and {0} does not match!", fileName));
                }
                else
                {
                    bool success = true;
                    using (FileStream patternFile = new FileStream("1.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (FileStream receivedFile = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            byte[] buf1 = new byte[8192];
                            byte[] buf2 = new byte[8192];
                            int count = 0;
                            int pos = 0;
                            while ((count = patternFile.Read(buf1, 0, buf1.Length)) > 0)
                            {
                                receivedFile.Read(buf2, 0, buf2.Length);
                                for (int i = 0; i < count; i++)
                                {
                                    if (!buf1[i].Equals(buf2[i]))
                                    {
                                        LOGGER.Error(string.Format("FILE_COMPARE, file content 1.bin and {0} does not match from position: {1}", fileName, pos));
                                        success = false;
                                        break;
                                    }
                                    pos++;
                                }
                                if (!success)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (success)
                    {
                        LOGGER.Info(string.Format("FILE_COMPARE, file size 1.bin and {0} match.", fileName));
                        File.Delete(fileName);
                    }
                }
            }

        }

        public long StartTcpServer(int port)
        {
            lock (mTCP_ServerSwitches)
            {
                long id = DateTime.Now.Ticks;
                Thread thread = new Thread(new ParameterizedThreadStart(TcpServerMain));
                thread.IsBackground = true;
                thread.Name = "TEST_TCP_SERVER_" + port.ToString();
                mTCP_ServerThreads.Add(id, thread);
                mTCP_ServerSwitches.Add(id, true);
                thread.Start(new long[] { id, port });
                return id;
            }
        }

        public void StopTcpServer(long id)
        {
            Thread t = null;
            lock (mTCP_ServerSwitches)
            {
                if (mTCP_ServerSwitches.ContainsKey(id))
                {
                    t = mTCP_ServerThreads[id];
                    mTCP_ServerSwitches[id] = false;
                }
            }
            lock (mTCP_ServerTcpListeners)
            {
                if (mTCP_ServerTcpListeners.ContainsKey(id))
                {
                    mTCP_ServerTcpListeners[id].Stop();
                }
            }
            if (t != null)
            {
                t.Join();
            }
        }

        public ICollection<long> GetActiveTCPServers()
        {
            lock (mTCP_ServerSwitches)
            {
                return new List<long>(mTCP_ServerSwitches.Keys);
            }
        }

        public long ConnectToOnTcp(string remoteHost, int targetPort)
        {
            long id = -1;

            TcpClient tcpClient = new TcpClient();
            try
            {
                lock (mTCP_TcpClients)
                {
                    Thread.Sleep(8);
                    id = DateTime.Now.Ticks;
                    mTCP_TcpClients.Add(id, tcpClient);
                }
                tcpClient.BeginConnect(remoteHost, targetPort, new AsyncCallback(ConnectCallback), new ConnectParams() { Id = id, Client = tcpClient, RemoteHost = remoteHost, TargetPort = targetPort });
            }
            catch (Exception ex)
            {
                LOGGER.Error(string.Format("Failed to connect to the remote host: {0}, port: {1}", remoteHost, targetPort), ex);
                tcpClient.Dispose();
            }

            return id;
        }

        private class ConnectParams
        {

            public long Id { get; set; }

            public string RemoteHost { get; set; }

            public int TargetPort { get; set; }

            public TcpClient Client { get; set; }

        }

        private void ConnectCallback(IAsyncResult result)
        {
            ConnectParams p = (ConnectParams)result.AsyncState;
            try
            {
                p.Client.EndConnect(result);
                new TCPReceiver(p.Id, p.Client);
            }
            catch (Exception ex)
            {
                LOGGER.Error(string.Format("Failed to connect to the remote host: {0}, port: {1}", p.RemoteHost, p.TargetPort), ex);
                p.Client.Dispose();
            }
        }

        public void DisconnectTcp(long id)
        {
            lock (mTCP_TcpClients)
            {
                if (mTCP_TcpClients.ContainsKey(id))
                {
                    mTCP_TcpClients[id].Dispose();
                    mTCP_TcpClients.Remove(id);
                }
            }
        }

        public long SendOnTcp(long id, bool hugeData, bool repeat)
        {
            long result = -1;

            if (mTCP_TcpClients.ContainsKey(id))
            {
                lock (mTCP_ClientSwitches)
                {
                    result = DateTime.Now.Ticks;
                    Thread thread = new Thread(new ParameterizedThreadStart(TcpClientMain));
                    thread.IsBackground = true;
                    thread.Name = "TEST_TCP_CLIENT_" + id.ToString() + "_" + DateTime.Now.Ticks.ToString();
                    mTCP_ClientThreads.Add(id, thread);
                    mTCP_ClientSwitches.Add(id, true);
                    thread.Start(new TCPSendParams() { Id = id, HugeData = hugeData, Repeat = repeat });
                }
            }

            return result;
        }

        public void StopTcpClient(long id)
        {
            Thread t = null;
            lock (mTCP_ClientSwitches)
            {
                if (mTCP_ClientSwitches.ContainsKey(id))
                {
                    t = mTCP_ClientThreads[id];
                    mTCP_ClientSwitches[id] = false;
                }
            }
            if (t != null)
            {
                t.Join();
            }
        }

        public ICollection<long> GetActiveSendThreads()
        {
            lock (mTCP_ClientSwitches)
            {
                return new List<long>(mTCP_ClientSwitches.Keys);
            }
        }

        public ICollection<long> GetActiveTCPSockets()
        {
            lock (mTCP_TcpClients)
            {
                return new List<long>(mTCP_TcpClients.Keys);
            }
        }

        private void TcpServerMain(object port)
        {
            long[] ps = (long[])port;
            long id = ps[0];
            int localPort = (int)ps[1];

            TcpListener tcpListener = null;
            try
            {
                tcpListener = new TcpListener(localPort); // ez számít
                tcpListener.Start();

                lock (mTCP_ServerTcpListeners)
                {
                    mTCP_ServerTcpListeners.Add(id, tcpListener);
                }

                LOGGER.Info(string.Format("TCP server is listening on port: {0}", localPort));
            }
            catch (Exception ex)
            {
                LOGGER.Error(string.Format("TCP Server failed to start. Reason: {0}", ex.Message), ex);
                lock (mTCP_ServerSwitches)
                {
                    mTCP_ServerSwitches.Remove(id);
                    mTCP_ServerThreads.Remove(id);
                }
                return;
            }
            while (mTCP_ServerSwitches[id])
            {
                try
                {
                    long socketId = -1;
                    ITcpClient tcpClient = tcpListener.AcceptTcpClient();
                    LOGGER.Info(string.Format("TCP_SERVER({0}), connection accepted from: {1}", id, tcpClient.Client.RemoteEndPoint.ToString()));
                    lock (mTCP_TcpClients)
                    {
                        Thread.Sleep(2);
                        socketId = DateTime.Now.Ticks;
                        mTCP_TcpClients.Add(DateTime.Now.Ticks, tcpClient);
                    }
                    new TCPReceiver(socketId, tcpClient);
                }
                catch (Exception ex)
                {
                    LOGGER.Error(string.Format("TCP_SERVER({0}) threw an exception. Reason: {1}", id, ex.Message), ex);
                    break;
                }
            }
            tcpListener.Stop();
            LOGGER.Info(string.Format("TCP_SERVER ({0}) stopped.", id));
            lock (mTCP_ServerSwitches)
            {
                mTCP_ServerSwitches.Remove(id);
                mTCP_ServerThreads.Remove(id);
            }
            lock (mTCP_ServerTcpListeners)
            {
                mTCP_ServerTcpListeners.Remove(id);
            }
        }

        private void TcpClientMain(object p)
        {
            TCPSendParams prms = (TCPSendParams)p;

            ITcpClient tcpClient = mTCP_TcpClients[prms.Id];
            int index = 0;
            string guid = Guid.NewGuid().ToString();
            if (prms.HugeData)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream fs = new FileStream("1.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        ms.SetLength(fs.Length);
                        byte[] buffer = ms.GetBuffer();
                        fs.Read(buffer, 0, buffer.Length);
                    }
                    try
                    {
                        NetworkStream ns = tcpClient.GetStream();
                        while (mTCP_ClientSwitches[prms.Id])
                        {
                            Thread.Sleep(1000);
                            ms.Position = 0;
                            try
                            {
                                ms.WriteTo(ns);
                                LOGGER.Info(string.Format("TCP_SENDER({0}), huge data sent.", prms.Id));
                            }
                            catch (Exception ex)
                            {
                                LOGGER.Error(string.Format("TCP_SENDER({0}) threw an exception. Reason: {1}", prms.Id, ex.Message), ex);
                                break;
                            }
                            if (!prms.Repeat)
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LOGGER.Error(string.Format("TCP_SENDER({0}) trying to send data on a dead connection. Reason: {1}", prms.Id, ex.Message), ex);
                    }
                }
            }
            else
            {
                NetworkStream ns = tcpClient.GetStream();
                while (mTCP_ClientSwitches[prms.Id])
                {
                    index++;
                    string data = string.Format("HELLO({0}), {1}", index, guid);
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    try
                    {
                        ns.Write(bytes, 0, bytes.Length);
                        LOGGER.Info(string.Format("TCP_SENDER({0}), data: {1}", prms.Id, data));
                    }
                    catch (Exception ex)
                    {
                        LOGGER.Error(string.Format("TCP_SENDER({0}) threw an exception. Reason: {1}", prms.Id, ex.Message), ex);
                        break;
                    }
                    if (!prms.Repeat)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            LOGGER.Info(string.Format("TCP_CLIENT_SENDER ({0}) stopped.", prms.Id));

            lock (mTCP_ClientSwitches)
            {
                mTCP_ClientSwitches.Remove(prms.Id);
                mTCP_ClientThreads.Remove(prms.Id);
            }
        }

        #endregion

        public void Connect(string hostAndPort)
        {
            NetworkManager.BeginConnect(AddressEndPoint.Parse(hostAndPort), new AsyncCallback(ConnectAsyncCallback), null);
        }

        private void ConnectAsyncCallback(IAsyncResult asyncResult)
        {
            try
            {
                INetworkPeerRemote peer = NetworkManager.EndConnect(asyncResult);
                if (peer == null)
                {
                    LOGGER.Info("No terragraf client on the other side.");
                }
                else
                {
                    LOGGER.Info(string.Format("Connection established with {0}.", peer.Id));
                }
            }
            catch (Exception ex)
            {
                LOGGER.Error("", ex);
            }
        }

        private void Instance_NetworkPeerUnaccessible(object sender, NetworkPeerChangedEventArgs e)
        {
            mOwnerForm.wrapper_NetworkPeerUnaccessible(this, e);
            //Raiser.CallDelegatorBySync(NetworkPeerUnaccessible, new object[] { this, e }, true, false);
        }

        private void Instance_NetworkPeerDistanceChanged(object sender, NetworkPeerDistanceChangedEventArgs e)
        {
            mOwnerForm.wrapper_NetworkPeerDistanceChanged(this, e);
            //Raiser.CallDelegatorBySync(NetworkPeerDistanceChanged, new object[] { this, e }, true, false);
        }

        private void Instance_NetworkPeerDiscovered(object sender, NetworkPeerChangedEventArgs e)
        {
            mOwnerForm.wrapper_NetworkPeerDiscovered(this, e);
            //Raiser.CallDelegatorBySync(NetworkPeerDiscovered, new object[] { this, e }, true, false);
        }

        private void Instance_NetworkPeerContextChanged(object sender, NetworkPeerContextEventArgs e)
        {
            mOwnerForm.wrapper_NetworkPeerContextChanged(this, e);
            //Raiser.CallDelegatorBySync(NetworkPeerContextChanged, new object[] { this, e }, true, false);
        }

    }

    [Serializable]
    public enum NetworkFactoryType
    {
        InterNetwork = 0,
        TerraGraf
    }

}
