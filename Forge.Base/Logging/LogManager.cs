using System;
using System.Reflection;

namespace Forge.Logging
{

    /// <summary>This class is used by client applications to request logger instances.</summary>
    public class LogManager
    {

        private static ILoggerWrapper mLogger = NullLogger.Instance;

        /// <summary>Initializes a new instance of the <see cref="LogManager"/> class.</summary>
        protected LogManager() { }

        /// <summary>Gets or sets the logger wrapper.</summary>
        /// <value>The logger wrapper.</value>
        public static ILoggerWrapper LOGGER
        {
            get { return mLogger; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                mLogger = value;
            }
        }

#if NETCOREAPP3_1
#else
        public static ILog GetLogger(string name)
        {
            return LOGGER.GetLogger(name);
        }
#endif

        public static ILog GetLogger(string repository, string name)
        {
            return LOGGER.GetLogger(repository, name);
        }

        public static ILog GetLogger(Assembly repositoryAssembly, Type type)
        {
            return LOGGER.GetLogger(repositoryAssembly, type);
        }

        public static ILog GetLogger(string repository, Type type)
        {
            return LOGGER.GetLogger(repository, type);
        }

        public static ILog GetLogger(Type type)
        {
            return LOGGER.GetLogger(type);
        }

        public static ILog GetLogger(Assembly repositoryAssembly, string name)
        {
            return LOGGER.GetLogger(repositoryAssembly, name);
        }

    }

}
