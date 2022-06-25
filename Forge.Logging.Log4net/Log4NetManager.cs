#if NETCOREAPP3_1_OR_GREATER
using Forge.IO;
#endif
using System;
using System.IO;
using System.Reflection;

namespace Forge.Logging.Log4net
{

    /// <summary>Log4Net wrapper implementation</summary>
    /// <seealso cref="Forge.Logging.ILoggerWrapper" />
    public class Log4NetManager : ILoggerWrapper
    {

        private Log4NetManager()
        {
        }

        /// <summary>Gets the singleton instance.</summary>
        /// <value>The instance.</value>
        public static Log4NetManager Instance { get; private set; } = new Log4NetManager();

        /// <summary>Initializes from application configuration.</summary>
        public static void InitializeFromAppConfig()
        {
#if NETCOREAPP3_1_OR_GREATER
            System.Xml.XmlDocument log4netConfig = new System.Xml.XmlDocument();
            log4netConfig.Load(System.IO.File.OpenRead(System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None).FilePath));
            var repo = log4net. LogManager.CreateRepository(Assembly.GetEntryAssembly(),
               typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, (System.Xml.XmlElement)log4netConfig.GetElementsByTagName("log4net")[0]);
#else
            log4net.Config.XmlConfigurator.Configure();
#endif
            SetLogger();
        }

#if NETCOREAPP3_1_OR_GREATER
        /// <summary>Initializes from configuration file.</summary>
        public static void InitializeFromConfigFile()
        {
            InitializeFromConfigFile(new FileInfo(Path.Combine(
                PathHelper.CutoffLastEntryFromPath(System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None).FilePath), 
                "log4net.config")));
        }
#endif

        /// <summary>Initializes from configuration file.</summary>
        /// <param name="configFileFullPath">The configuration file full path.</param>
        public static void InitializeFromConfigFile(string configFileFullPath)
        {
            InitializeFromConfigFile(new FileInfo(configFileFullPath));
        }

        /// <summary>Initializes from configuration file.</summary>
        /// <param name="configFileInfo">The configuration file information.</param>
        public static void InitializeFromConfigFile(FileInfo configFileInfo)
        {
            System.Xml.XmlDocument log4netConfig = new System.Xml.XmlDocument();
            log4netConfig.Load(System.IO.File.OpenRead(configFileInfo.FullName));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
               typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
            SetLogger();
        }

        private static void SetLogger()
        {
            if (LogManager.LOGGER.GetType() == typeof(NullLogger))
            {
                LogManager.LOGGER = Instance;
            }
        }

#if NETCOREAPP3_1_OR_GREATER
#else
        /// <summary>Gets the logger.</summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ILog GetLogger(string name)
        {
            return new Log4NetLog(log4net.LogManager.GetLogger(name));
        }
#endif

        /// <summary>Gets the logger.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ILog GetLogger(string repository, string name)
        {
            return new Log4NetLog(log4net.LogManager.GetLogger(repository, name));
        }

        /// <summary>Gets the logger.</summary>
        /// <param name="repositoryAssembly">The repository assembly.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ILog GetLogger(Assembly repositoryAssembly, Type type)
        {
            return new Log4NetLog(log4net.LogManager.GetLogger(repositoryAssembly, type));
        }

        /// <summary>Gets the logger.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ILog GetLogger(string repository, Type type)
        {
            return new Log4NetLog(log4net.LogManager.GetLogger(repository, type));
        }

        /// <summary>Gets the logger.</summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ILog GetLogger(Type type)
        {
            return new Log4NetLog(log4net.LogManager.GetLogger(type));
        }

        /// <summary>Gets the logger.</summary>
        /// <param name="repositoryAssembly">The repository assembly.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ILog GetLogger(Assembly repositoryAssembly, string name)
        {
            return new Log4NetLog(log4net.LogManager.GetLogger(repositoryAssembly, name));
        }

    }

}
