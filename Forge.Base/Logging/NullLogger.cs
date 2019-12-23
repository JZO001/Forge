using System;
using System.Reflection;

namespace Forge.Logging
{

    /// <summary>Empty logger. Default implementation, do nothing.</summary>
    /// <seealso cref="Forge.Logging.ILoggerWrapper" />
    public sealed class NullLogger : ILoggerWrapper
    {

        private static readonly NullLog NULL_LOG = new NullLog();

        private NullLogger() { }

        /// <summary>Gets the singleton instance.</summary>
        /// <value>The instance.</value>
        public static NullLogger Instance { get; private set; } = new NullLogger();

#if NETCOREAPP3_1
#else
        public ILog GetLogger(string name)
        {
            return NULL_LOG;
        }
#endif

        public ILog GetLogger(string repository, string name)
        {
            return NULL_LOG;
        }

        public ILog GetLogger(Assembly repositoryAssembly, Type type)
        {
            return NULL_LOG;
        }

        public ILog GetLogger(string repository, Type type)
        {
            return NULL_LOG;
        }

        public ILog GetLogger(Type type)
        {
            return NULL_LOG;
        }

        public ILog GetLogger(Assembly repositoryAssembly, string name)
        {
            return NULL_LOG;
        }

    }

}
