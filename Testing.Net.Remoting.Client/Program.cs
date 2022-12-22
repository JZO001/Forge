using Forge.Configuration;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Proxy;
using Forge.Net.Remoting.Service;
using Microsoft.Extensions.Configuration;
using Testing.Net.Remoting.Generator.Contracts;

namespace Testing.Net.Remoting.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            var remotingConfig = new PropertyItem("Remoting", config.GetSection("Remoting").Get<List<PropertyItem>>());

            Console.WriteLine("Press a key to start configure client side");
            Console.ReadKey();

            // keep the initialization order
            ChannelServices.Initialize(remotingConfig);
            ServiceBaseServices.Initialize(remotingConfig); // only if we have services
            ProxyServices.Initialize(remotingConfig);

            //Console.WriteLine("Press a key to send a message");
            //Console.ReadKey();

            ProxyFactory<IDemo> demoProxyFactory = new ProxyFactory<IDemo>();
            using (IDemo demo = demoProxyFactory.CreateProxy())
            {
                demo.SendMessageToService("hello");
                Console.WriteLine("Message sent");
                Console.ReadKey();
            }

        }
    }
}