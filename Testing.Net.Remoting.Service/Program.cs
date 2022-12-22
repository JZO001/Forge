using Forge.Configuration;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Proxy;
using Forge.Net.Remoting.Service;
using Microsoft.Extensions.Configuration;

namespace Testing.Net.Remoting.Service
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

            // keep the initialization order
            ChannelServices.Initialize(remotingConfig);
            ServiceBaseServices.Initialize(remotingConfig);
            ProxyServices.Initialize(remotingConfig); // only if we have client-side proxies

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();
        }
    }
}