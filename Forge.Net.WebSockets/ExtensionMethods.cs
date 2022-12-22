/* *********************************************************************
 * Date: 10 Jun 2019
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

#if NETSTANDARD2_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Forge.Net.WebSockets
{

    /// <summary>Extension methods for ASP.NET Core web service</summary>
    public static class ExtensionMethods
    {

        /// <summary>Uses the web socket manager.</summary>
        /// <param name="app">The application.</param>
        /// <param name="path">The path.</param>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseWebSocketManager(this IApplicationBuilder app, PathString path, WebSocketHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }

        /// <summary>Adds the web socket manager.</summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSocketManager>();

            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (!type.Equals(typeof(WebSocketHandler)) && typeof(WebSocketHandler).IsAssignableFrom(type))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }

    }

}

#endif
