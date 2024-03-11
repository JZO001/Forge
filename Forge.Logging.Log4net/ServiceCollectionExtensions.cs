#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER

using Forge.Logging.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Forge.Logging.Log4net
{

    /// <summary>ServiceCollection extension methods</summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Registers the Forge Log4Net logging
        /// </summary>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddForgeLog4NetLogging(this IServiceCollection services)
        {
            return services
                .AddSingleton<ILog, Log4NetLog>()
                .AddSingleton<ILoggerWrapper, Log4NetManager>(factory => Log4NetManager.Instance)
                .AddSingleton<log4net.ILog>(log4net.LogManager.GetLogger("[]"));
        }

    }

}

#endif
