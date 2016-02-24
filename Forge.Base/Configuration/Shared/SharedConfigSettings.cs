/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Forge.Configuration.Shared.Interfaces;
using log4net;

namespace Forge.Configuration.Shared
{

    /// <summary>
    /// Shared configuration for local configuration handler classes
    /// <example>
    /// <code>
    /// [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
    /// public class StorageConfiguration : Forge.Base.Configuration.Shared.SharedConfigSettings&lt;StorageSection, StorageConfiguration&gt;
    /// {
    ///
    ///     #region Constructors
    ///
    ///     /// <summary>
    ///     /// Initializes the &lt;see cref="StorageConfiguration"/&gt; class.
    ///     /// </summary>
    ///     static StorageConfiguration()
    ///     {
    ///         LOG_PREFIX = "HIBERNATE_STORAGE_PROVIDER";
    ///     }
    ///
    ///     /// <summary>
    ///     /// Initializes a new instance of the &lt;see cref="StorageConfiguration"/&gt; class.
    ///     /// </summary>
    ///     public StorageConfiguration()
    ///         : base()
    ///     {
    ///     }
    ///
    ///     #endregion
    /// 
    /// }
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TSectionType">The type of the section type.</typeparam>
    /// <typeparam name="TSectionHandlerType">The type of the section handler type.</typeparam>
    public abstract class SharedConfigSettings<TSectionType, TSectionHandlerType> : ConfigSettingsBase, IConfigurationSettingsHandler<TSectionType>
        where TSectionType : ConfigurationSectionBase, new()
        where TSectionHandlerType : SharedConfigSettings<TSectionType, TSectionHandlerType>, new()
    {

        #region Field(s)

        /// <summary>
        /// Logger
        /// </summary>
        protected static readonly ILog LOGGER = null;

        /// <summary>
        /// Configuratino handler for the specified type of section
        /// </summary>
        protected static readonly TSectionHandlerType mConfigHandler = null;

        /// <summary>
        /// Log prefix for logging
        /// </summary>
        protected static string LOG_PREFIX = typeof(TSectionType).Name;

        /// <summary>
        /// Instance of the configuration section
        /// </summary>
        protected TSectionType mSettings = default(TSectionType);

        /// <summary>
        /// The last known good section instance
        /// </summary>
        protected TSectionType mLastKnownGoodSettings = default(TSectionType);

        /// <summary>
        /// The configuration
        /// </summary>
        protected System.Configuration.Configuration mConfig = null;

        private FileSystemWatcher mFSWatcher = null;

        private String mDefaultConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

        private bool mSectionLoaded = false;

        private readonly object LOCK_OBJECT = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="SharedConfigSettings&lt;TSectionType, TSectionHandlerType&gt;"/> class.
        /// </summary>
        static SharedConfigSettings()
        {
            UserLevelModeForLoading = ConfigurationUserLevel.None;
            LOGGER = LogManager.GetLogger(typeof(TSectionHandlerType));
            mConfigHandler = new TSectionHandlerType();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedConfigSettings&lt;TSectionType, TSectionHandlerType&gt;"/> class.
        /// </summary>
        protected SharedConfigSettings()
        {
            if (LOGGER.IsInfoEnabled) LOGGER.Info(String.Format("Type of {0} configuration section instance created.", this.GetType().Name));
            lock (mKnownConfigSettings)
            {
                mKnownConfigSettings.Add(this);
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SharedConfigSettings&lt;TSectionType, TSectionHandlerType&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~SharedConfigSettings()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the user level modefor loading.
        /// </summary>
        /// <value>
        /// The user level modefor loading.
        /// </value>
        public static ConfigurationUserLevel UserLevelModeForLoading { get; set; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public static TSectionType Settings
        {
            get { return mConfigHandler.SettingsLocal; }
        }

        /// <summary>
        /// Gets the last known good settings.
        /// </summary>
        /// <value>
        /// The last known good settings.
        /// </value>
        public static TSectionType LastKnownGoodSettings
        {
            get { return mConfigHandler.LastKnownGoodSettingsLocal; }
        }

        /// <summary>
        /// Refreshes the configuration.
        /// </summary>
        public static void Refresh()
        {
            mConfigHandler.RefreshInstance();
        }

        /// <summary>
        /// Saves the specified mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public static void Save(ConfigurationSaveMode mode)
        {
            mConfigHandler.SaveInstance(mode);
        }

        /// <summary>
        /// Gets the section handler.
        /// </summary>
        /// <value>
        /// The section handler.
        /// </value>
        public static IConfigurationSettingsHandler<TSectionType> SectionHandler
        {
            get { return mConfigHandler; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has configuration file.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has configuration file; otherwise, <c>false</c>.
        /// </value>
        public bool HasConfigurationFile
        {
            get { return mConfig == null ? false : mConfig.HasFile; }
        }

        /// <summary>
        /// Gets or sets the default configuration file.
        /// </summary>
        /// <value>
        /// The default configuration file.
        /// </value>
        /// <exception cref="System.ArgumentNullException">value</exception>
        public override String DefaultConfigurationFile
        {
            get
            {
                return UseGlobalConfigurationFile(
                      mDefaultConfigurationFile,
                      UseLocalConfiguration) ?
                      ConfigurationCenter.ConfigurationFile : mDefaultConfigurationFile;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                this.mDefaultConfigurationFile = value;
            }
        }

        /// <summary>
        /// Gets the settings local.
        /// </summary>
        /// <value>
        /// The settings local.
        /// </value>
        public virtual TSectionType SettingsLocal
        {
            get
            {
                if (!mSectionLoaded)
                {
                    lock (LOCK_OBJECT)
                    {
                        if (!mSectionLoaded)
                        {
                            CreateSection();
                            StartConfigWatcher();
                        }
                    }
                }
                return mSettings;
            }
        }

        /// <summary>
        /// Gets the last known good settings local.
        /// </summary>
        /// <value>
        /// The last known good settings local.
        /// </value>
        public virtual TSectionType LastKnownGoodSettingsLocal
        {
            get { return mLastKnownGoodSettings; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [restart on external changes].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [restart on external changes]; otherwise, <c>false</c>.
        /// </value>
        public override bool RestartOnExternalChanges
        {
            get
            {
                if (!mSectionLoaded)
                {
                    lock (LOCK_OBJECT)
                    {
                        if (!mSectionLoaded)
                        {
                            CreateSection();
                            StartConfigWatcher();
                        }
                    }
                }
                return mSettings.SectionInformation.RestartOnExternalChanges;
            }
            set
            {
                if (!mSectionLoaded)
                {
                    lock (LOCK_OBJECT)
                    {
                        if (!mSectionLoaded)
                        {
                            CreateSection();
                            //StartConfigWatcher();
                        }
                    }
                }
                mSettings.SectionInformation.RestartOnExternalChanges = value;
                StartConfigWatcher();
            }
        }

        /// <summary>
        /// Refreshes the instance.
        /// </summary>
        public override void RefreshInstance()
        {
            lock (LOCK_OBJECT)
            {
                if (mConfig != null)
                {
                    try
                    {
                        if (LOGGER.IsInfoEnabled) LOGGER.Info(String.Format("{0}: refreshing configuration from file '{1}'...", LOG_PREFIX, DefaultConfigurationFile));
                        CreateSection();
                        try
                        {
                            //Validate();
                            StartConfigWatcher();
                            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("{0}: configuration successfully refreshed", LOG_PREFIX));
                        }
                        catch (Exception)
                        {
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("{0}: unable to refresh configuration, because it is invalid.", LOG_PREFIX));
                            LoadFactoryDefaults();
                        }
                        RaiseOnConfigurationChanged(EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format("{0}: unable to refresh configuration. Message: {1}", LOG_PREFIX, ex.Message));
                    }
                }
                else
                {
                    // alap konfig betöltése
                    SettingsLocal.ToString();
                }
            }
        }

        /// <summary>
        /// Saves the instance.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public override void SaveInstance(ConfigurationSaveMode mode)
        {
            if (mConfig != null)
            {
                lock (mKnownConfigSettings)
                {
                    string localConfigFile = DefaultConfigurationFile;
                    bool watchFile = RestartOnExternalChanges;

                    List<ConfigSettingsBase> watchTypeConfigSections = new List<ConfigSettingsBase>();

                    foreach (ConfigSettingsBase d in mKnownConfigSettings)
                    {
                        if (localConfigFile.Equals(d.DefaultConfigurationFile))
                        {
                            if (d.RestartOnExternalChanges)
                            {
                                watchTypeConfigSections.Add(d);
                                d.RestartOnExternalChanges = false;
                            }
                        }
                    }

                    mConfig.Save(mode);

                    if (watchTypeConfigSections.Count > 0)
                    {
                        foreach (ConfigSettingsBase d in watchTypeConfigSections)
                        {
                            d.RestartOnExternalChanges = true;
                        }
                    }

                    if (watchFile)
                    {
                        RaiseOnConfigurationChanged(EventArgs.Empty);
                    }
                }
            }
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Creates the section.
        /// </summary>
        protected override void CreateSection()
        {
            try
            {
                if (LOGGER.IsInfoEnabled) LOGGER.Info(String.Format("{0}: reading configuration from file '{1}'...", LOG_PREFIX, DefaultConfigurationFile));
                ExeConfigurationFileMap fMap = new ExeConfigurationFileMap();
                fMap.ExeConfigFilename = DefaultConfigurationFile;
                mConfig = ConfigurationManager.OpenMappedExeConfiguration(fMap, UserLevelModeForLoading);
                mSettings = (TSectionType)mConfig.Sections[typeof(TSectionType).Name];
                mSectionLoaded = true;
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("{0}: unable to process configuration. Loading factory defaults.", LOG_PREFIX), ex);
            }
            finally
            {
                if (mSettings == null)
                {
                    mSettings = LoadFactoryDefaults();
                    if (mConfig != null)
                    {
                        mConfig.Sections.Remove(typeof(TSectionType).Name);
                        mConfig.Sections.Add(typeof(TSectionType).Name, mSettings);
                        //mConfig.Save(ConfigurationSaveMode.Full);
                    }
                }
            }

            Validate();
        }

        /// <summary>
        /// Loads the factory defaults.
        /// </summary>
        /// <returns>The section</returns>
        protected virtual TSectionType LoadFactoryDefaults()
        {
            mSettings = (TSectionType)typeof(TSectionType).GetConstructor(new Type[] { }).Invoke(new Object[] { });
            mSectionLoaded = true;
            return mSettings;
        }

        /// <summary>
        /// Validates this instance values manually.
        /// </summary>
        protected override void Validate()
        {
        }

        /// <summary>
        /// Starts the config watcher.
        /// </summary>
        protected virtual void StartConfigWatcher()
        {
            if (mFSWatcher != null)
            {
                lock (LOCK_OBJECT)
                {
                    if (mFSWatcher != null)
                    {
                        mFSWatcher.Changed -= new FileSystemEventHandler(FSWatcher_Changed);
                        mFSWatcher.Dispose();
                        if (LOGGER.IsInfoEnabled) LOGGER.Info(String.Format("{0}: listening for configuration file stopped.", LOG_PREFIX));
                    }
                }
            }
            if (mSettings.SectionInformation.RestartOnExternalChanges)
            {
                lock (LOCK_OBJECT)
                {
                    if (LOGGER.IsInfoEnabled) LOGGER.Info(String.Format("{0}: listening configuration file for changes '{1}'...", LOG_PREFIX, DefaultConfigurationFile));
                    mFSWatcher = new FileSystemWatcher(Path.GetDirectoryName(DefaultConfigurationFile));
                    mFSWatcher.Filter = Path.GetFileName(DefaultConfigurationFile);
                    mFSWatcher.IncludeSubdirectories = false;
                    mFSWatcher.NotifyFilter = NotifyFilters.LastWrite;
                    mFSWatcher.Changed += new FileSystemEventHandler(FSWatcher_Changed);
                    mFSWatcher.EnableRaisingEvents = true;
                }
            }
        }

        /// <summary>
        /// Handles the Changed event of the FSWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.IO.FileSystemEventArgs" /> instance containing the event data.</param>
        protected virtual void FSWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed &&
                DefaultConfigurationFile.ToLower().Equals(e.FullPath.ToLower()))
            {
                if (LOGGER.IsInfoEnabled) LOGGER.Info(String.Format("{0}: configuration file for changed '{1}'...", LOG_PREFIX, DefaultConfigurationFile));
                RefreshInstance();
            }
        }

        #endregion

        #region Protected static method(s)

        /// <summary>
        /// Check you can use your local configuration file.
        /// </summary>
        /// <param name="myConfigurationFile">The local configuration file, which you want to use.</param>
        /// <param name="useLocalConfiguration">The flag is true if you want to use your configuration file. Global settings can override this and order you to use the globals.</param>
        /// <returns>True, if use the global config file</returns>
        /// <exception cref="System.ArgumentNullException">myConfigurationFile</exception>
        protected static bool UseGlobalConfigurationFile(String myConfigurationFile, bool useLocalConfiguration)
        {
            if (String.IsNullOrEmpty(myConfigurationFile))
            {
                throw new ArgumentNullException("myConfigurationFile");
            }
            return (ConfigurationCenter.OverrideLocalSettings && ConfigurationCenter.ConfigurationFile.Length > 0 ||
                !ConfigurationCenter.OverrideLocalSettings && !useLocalConfiguration && ConfigurationCenter.ConfigurationFile.Length > 0 ||
                myConfigurationFile.Equals(ConfigurationCenter.ConfigurationFile));
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (mFSWatcher != null)
                {
                    mFSWatcher.Dispose();
                }
            }
        }

        #endregion

    }

}
