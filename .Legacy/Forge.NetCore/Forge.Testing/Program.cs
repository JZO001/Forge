using Forge.EventRaiser;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using Forge.Security;
using Forge.Logging;
using Forge.Logging.Log4net;

namespace Forge.Testing
{

    class Program
    {

        static void Main(string[] args)
        {
            Log4NetManager.InitializeFromAppConfig();
            //LogManager.LOGGER = Log4NetManager.Instance;

            //XmlDocument log4netConfig = new XmlDocument();
            //log4netConfig.Load(File.OpenRead(System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath));
            //var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
            //   typeof(log4net.Repository.Hierarchy.Hierarchy));
            //log4net.Config.XmlConfigurator.Configure(repo, (XmlElement)log4netConfig.GetElementsByTagName("log4net")[0]);

            //FireEvent fe = new FireEvent();
            //fe.OnTest += Fe_OnTest;
            //fe.Fire();

            BinarySerializerTest.Test();


            Console.ReadLine();

        }

        private static void Fe_OnTest(object sender, EventArgs e)
        {
            Console.WriteLine("Event arrived");
            X509Certificate2 cert = CertCreator.CreateCert("CN=subjectName", DateTime.UtcNow.AddDays(-1), DateTime.MaxValue, null);
            Console.WriteLine(cert.Subject);
        }
    }

    class FireEvent
    {

        public event EventHandler<EventArgs> OnTest;

        public FireEvent()
        {

        }

        public void Fire()
        {
            Raiser.CallDelegatorByAsync(OnTest, new object[] { this, EventArgs.Empty });
        }

    }

    class CertCreator
    {

        public static X509Certificate2 CreateCert(string subject, DateTime startTime, DateTime endTime, string insecurePassword)
        {
            byte[] data = CertificateFactory.CreateSelfSignCertificatePfx(subject, startTime, endTime, insecurePassword);
            return new X509Certificate2(data, insecurePassword);
        }

    }

}
