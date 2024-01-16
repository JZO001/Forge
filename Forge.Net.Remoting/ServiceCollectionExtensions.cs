#if NET40
#else

using Forge.Configuration;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Options;
using Forge.Net.Remoting.Proxy;
using Forge.Net.Remoting.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace Forge.Net.Remoting
{

    /// <summary>Add external client as transient</summary>
    public static class ServiceCollectionExtensions
    {

#nullable enable

        /// <summary>Configure and registers external client proxy</summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configure">The options</param>
        /// <returns>IServiceCollection</returns>
        public static IHostBuilder AddRemoting(this IHostBuilder hostBuilder, Action<RemotingOptions>? configure)
        {
            return hostBuilder.ConfigureServices((context, services) =>
                services.Configure<RemotingOptions>(configureOptions =>
                {
                    var remotingConfig = new PropertyItem("Remoting", context.Configuration.GetSection("Remoting").Get<List<PropertyItem>>());
                    configureOptions.RemotingConfiguration = remotingConfig;
                    configure?.Invoke(configureOptions);
                    ChannelServices.Initialize(configureOptions.RemotingConfiguration);
                    ServiceBaseServices.Initialize(configureOptions.RemotingConfiguration);
                    ProxyServices.Initialize(configureOptions.RemotingConfiguration);
                }));
        }

#nullable disable

    }

}

#endif
