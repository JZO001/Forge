#if NETCOREAPP3_1_OR_GREATER

using Forge.Net.TerraGraf.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Forge.Net.TerraGraf
{

    /// <summary>ServiceCollection extension methods</summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Registers the TerraGraf services.
        /// </summary>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddTerraGraf(this IServiceCollection services)
        {
            return AddTerraGraf(services, null);
        }

        /// <summary>
        /// Registers the TerraGraf services.
        /// </summary>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddTerraGraf(this IServiceCollection services, Action<TerraGrafOptions>
#if NET40
#else
#nullable enable
            ?
#nullable disable
#endif
            configure)
        {
            return services
                .AddSingleton<ITerraGrafNetworkFactory, TerraGrafNetworkFactory>()
                .AddSingleton<ITerraGrafNetworkManager, NetworkManager>()
                .AddSingleton<NetworkManager>((serviceProvider) => NetworkManager.Instance)
                .Configure<TerraGrafOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                    if (configure != null)
                    {
                        NetworkManager.ConfigurationSource = ConfigurationSourceEnum.External;
                        NetworkManager.Configuration = configureOptions;
                    }
                });
        }

    }

}

#endif
