#if NETCOREAPP3_1_OR_GREATER

using Forge.Legacy;
using Forge.Net.Synapse.NetworkFactory;
using Forge.Net.Synapse.NetworkServices;
using Forge.Net.Synapse.NetworkServices.Defaults;
using Forge.Net.Synapse.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Forge.Net.Synapse
{

    /// <summary>ServiceCollection extension methods</summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Registers Synapse.
        /// </summary>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddSynapse(this IServiceCollection services)
            => services.AddSynapse(null);

#nullable enable
        /// <summary>
        /// Registers Synapse.
        /// </summary>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddSynapse(this IServiceCollection services, Action<SynapseOptions>? configure)
        {
            DefaultServerStreamFactory serverStreamFactory = (DefaultServerStreamFactory)NetworkManager.DefaultServerStreamFactory;
            DefaultClientStreamFactory clientStreamFactory = (DefaultClientStreamFactory)NetworkManager.DefaultClientStreamFactory;
            return services
                .AddSingleton<IClientStreamFactory>((serviceProvider) => clientStreamFactory)
                .AddSingleton<IServerStreamFactory>((serviceProvider) => serverStreamFactory)
                .AddSingleton<ISslClientStreamFactory, SslClientStreamFactory>()
                .AddSingleton<ISslServerStreamFactory, SslServerStreamFactory>()
                .AddSingleton<IDefaultNetworkFactory, DefaultNetworkFactory>()
                .AddSingleton<IDefaultNetworkManager, NetworkManager>()
                .AddSingleton<NetworkManager>((serviceProvider) => new NetworkManager(serviceProvider.GetService<IDefaultNetworkFactory>()))
                .Configure<SynapseOptions>(configureOptions => 
                {
                    configure?.Invoke(configureOptions);
                    NetworkManager.DefaultServerStreamFactory.Initialize(configureOptions.ServerStreamFactoryOptions);
                    NetworkManager.DefaultClientStreamFactory.Initialize(configureOptions.ClientStreamFactoryOptions);
                });
        }
#nullable disable

        /// <summary>
        /// Registers Synapse.
        /// </summary>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddSynapseWithDefaultSSL(this IServiceCollection services)
            => services.AddSynapseWithDefaultSSL(null);

#nullable enable
        /// <summary>
        /// Registers Synapse.
        /// </summary>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddSynapseWithDefaultSSL(this IServiceCollection services, Action<SynapseOptions>? configure)
        {
            SslServerStreamFactory serverStreamFactory = new SslServerStreamFactory();
            NetworkManager.DefaultServerStreamFactory = serverStreamFactory;
            SslClientStreamFactory clientStreamFactory = new SslClientStreamFactory();
            NetworkManager.DefaultClientStreamFactory = clientStreamFactory;
            return services
                .AddSingleton<IClientStreamFactory, DefaultClientStreamFactory>()
                .AddSingleton<IServerStreamFactory, DefaultServerStreamFactory>()
                .AddSingleton<ISslClientStreamFactory>((serviceProvider) => clientStreamFactory)
                .AddSingleton<ISslServerStreamFactory>((serviceProvider) => serverStreamFactory)
                .AddSingleton<IDefaultNetworkFactory, DefaultNetworkFactory>()
                .AddSingleton<IDefaultNetworkManager, NetworkManager>()
                .AddSingleton<NetworkManager>((serviceProvider) => new NetworkManager(serviceProvider.GetService<IDefaultNetworkFactory>()))
                .Configure<SynapseOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                    serverStreamFactory.Initialize(configureOptions.SslServerStreamFactoryOptions);
                    clientStreamFactory.Initialize(configureOptions.SslClientStreamFactoryOptions);
                });
        }
#nullable disable

    }

}

#endif
