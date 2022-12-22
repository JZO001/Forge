/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using Forge.Configuration.Shared;
using Forge.Invoker;
using Forge.RemoteDesktop.ConfigSection;
using Forge.RemoteDesktop.Contracts;
using Forge.Shared;

namespace Forge.RemoteDesktop.Service.Configuration
{

    /// <summary>
    /// Represents the configuration settings for a service
    /// </summary>
    public static class Settings
    {

        #region Field(s)

        private const string CONFIG_ROOT = "Service";

        private const string CONFIG_AUTHENTICATION_MODE = "AuthenticationMode";

        private const string CONFIG_AUTHENTICATION_MODULE_STORE = "AuthenticationModuleStore";

        private const string CONFIG_PROPAGATE_ON_NETWORK = "PropagateServiceOnTheNetwork";

        private const string CONFIG_DESKTOP_SHARE_MODE = "DesktopShareMode";

        private const string CONFIG_ACCEPT_INPUTS_FROM_CLIENTS = "AcceptKeyboardAndMouseInputFromClients";

        private const string CONFIG_DESKTOP_IMAGE_CLIP_WIDTH = "DesktopImageClipWidth";

        private const string CONFIG_DESKTOP_IMAGE_CLIP_HEIGHT = "DesktopImageClipHeight";

        private const string CONFIG_CLIENTS_PER_SERVICE_THREADS = "ClientsPerServiceThreads";

        private const string CONFIG_MAXIMUM_FAILED_LOGIN_ATTEMPT = "MaximumFailedLoginAttempt";

        private const string CONFIG_BLACKLIST_TIMEOUT = "BlackListTimeout";

        private const string CONFIG_IMAGE_CLIP_QUALITY = "DefaultImageClipQuality";

        private const string CONFIG_MOUSE_MOVE_SEND_INTERVAL = "MouseMoveSendInterval";

        private static string mAuthenticationModuleStore = string.Empty;

        private static int mLoginTimeoutInMs = 10000; // default is 10 seconds

        private static int mDesktopImageClipWidth = Consts.DEFAULT_DESKTOP_IMAGE_CLIP_SIZE;

        private static int mDesktopImageClipHeight = Consts.DEFAULT_DESKTOP_IMAGE_CLIP_SIZE;

        private static int mClientsPerServiceThreads = Consts.DEFAULT_CLIENTS_PER_SERVICE_THREADS;

        private static int mMaximumFailedLoginAttempt = Consts.DEFAULT_MAXIMUM_FAILED_LOGIN_ATTEMPT;

        private static int mBlackListTimeout = Consts.DEFAULT_BLACKLIST_TIMEOUT_IN_MINUTES;

        private static int mDefaultImageClipQuality = Consts.DEFAULT_IMAGE_CLIP_QUALITY;

        private static int mMouseMoveSendInterval = Consts.DEFAULT_MOUSE_MOVE_SEND_INTERVAL;

        /// <summary>
        /// Occurs when [event configuration changed].
        /// </summary>
        public static event EventHandler<EventArgs> EventConfigurationChanged;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="Settings"/> class.
        /// </summary>
        static Settings()
        {
            PropagateServiceOnTheNetwork = true;
            AcceptKeyboardAndMouseInputFromClients = true;
            SectionHandler_OnConfigurationChanged(null, EventArgs.Empty);
            RemoteDesktopConfiguration.SectionHandler.OnConfigurationChanged += new EventHandler<EventArgs>(SectionHandler_OnConfigurationChanged);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the authentication mode.
        /// </summary>
        /// <value>
        /// The authentication mode.
        /// </value>
        public static AuthenticationModeEnum AuthenticationMode { get; set; }

        /// <summary>
        /// Gets or sets the authentication module store.
        /// </summary>
        /// <value>
        /// The authentication module store.
        /// </value>
        public static string AuthenticationModuleStore
        {
            get { return mAuthenticationModuleStore; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                mAuthenticationModuleStore = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [propagate service on the network].
        /// </summary>
        /// <value>
        /// <c>true</c> if [propagate service on the network]; otherwise, <c>false</c>.
        /// </value>
        public static bool PropagateServiceOnTheNetwork { get; set; }

        /// <summary>
        /// Gets or sets the desktop share mode.
        /// </summary>
        /// <value>
        /// The desktop share mode.
        /// </value>
        public static DesktopShareModeEnum DesktopShareMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [accept keyboard and mouse input from clients].
        /// </summary>
        /// <value>
        /// <c>true</c> if [accept keyboard and mouse input from clients]; otherwise, <c>false</c>.
        /// </value>
        public static bool AcceptKeyboardAndMouseInputFromClients { get; set; }

        /// <summary>
        /// Gets or sets the login timeout in ms.
        /// </summary>
        /// <value>
        /// The login timeout in ms.
        /// </value>
        public static int LoginTimeoutInMs
        {
            get { return mLoginTimeoutInMs; }
            set
            {
                if (value < 1)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mLoginTimeoutInMs = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the desktop image clip.
        /// </summary>
        /// <value>
        /// The width of the desktop image clip.
        /// </value>
        public static int DesktopImageClipWidth
        {
            get { return mDesktopImageClipWidth; }
            set
            {
                if (value < Consts.MINIMAL_CLIP_SIZE)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mDesktopImageClipWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the desktop image clip.
        /// </summary>
        /// <value>
        /// The height of the desktop image clip.
        /// </value>
        public static int DesktopImageClipHeight
        {
            get { return mDesktopImageClipHeight; }
            set
            {
                if (value < Consts.MINIMAL_CLIP_SIZE)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mDesktopImageClipHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the clients per service threads.
        /// </summary>
        /// <value>
        /// The clients per service threads.
        /// </value>
        public static int ClientsPerServiceThreads
        {
            get { return mClientsPerServiceThreads; }
            set
            {
                if (value < Consts.MINIMAL_CLIENTS_PER_SERVICE_THREADS)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }

                mClientsPerServiceThreads = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum failed login attempt.
        /// </summary>
        /// <value>
        /// The maximum failed login attempt.
        /// </value>
        public static int MaximumFailedLoginAttempt
        {
            get { return mMaximumFailedLoginAttempt; }
            set
            {
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }

                mMaximumFailedLoginAttempt = value;
            }
        }

        /// <summary>
        /// Gets or sets the black list timeout.
        /// </summary>
        /// <value>
        /// The black list timeout.
        /// </value>
        public static int BlackListTimeout
        {
            get { return mBlackListTimeout; }
            set
            {
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }

                mBlackListTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the default image clip quality.
        /// </summary>
        /// <value>
        /// The default image clip quality.
        /// </value>
        public static int DefaultImageClipQuality
        {
            get { return mDefaultImageClipQuality; }
            set
            {
                if (value < 10 || value > 100)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mDefaultImageClipQuality = value;
            }
        }

        /// <summary>
        /// Gets or sets the mouse move send interval.
        /// </summary>
        /// <value>
        /// The mouse move send interval.
        /// </value>
        public static int MouseMoveSendInterval
        {
            get { return mMouseMoveSendInterval; }
            set
            {
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }

                mMouseMoveSendInterval = value;
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <param name="saveMode">The save mode.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Save(ConfigurationSaveMode saveMode)
        {
            CategoryPropertyItem rootItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(RemoteDesktopConfiguration.Settings.CategoryPropertyItems, CONFIG_ROOT);
            if (rootItem == null)
            {
                rootItem = new CategoryPropertyItem();
                rootItem.Id = CONFIG_ROOT;
                RemoteDesktopConfiguration.Settings.CategoryPropertyItems.Add(rootItem);
            }

            CategoryPropertyItem piAuthMode = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_AUTHENTICATION_MODE);
            if (piAuthMode == null)
            {
                piAuthMode = new CategoryPropertyItem();
                piAuthMode.Id = CONFIG_AUTHENTICATION_MODE;
                rootItem.PropertyItems.Add(piAuthMode);
            }
            piAuthMode.EntryValue = AuthenticationMode.ToString();

            CategoryPropertyItem piModuleStore = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_AUTHENTICATION_MODULE_STORE);
            if (piModuleStore == null)
            {
                piModuleStore = new CategoryPropertyItem();
                piModuleStore.Id = CONFIG_AUTHENTICATION_MODULE_STORE;
                rootItem.PropertyItems.Add(piModuleStore);
            }
            piModuleStore.EntryValue = AuthenticationModuleStore;

            CategoryPropertyItem piPropagation = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_PROPAGATE_ON_NETWORK);
            if (piPropagation == null)
            {
                piPropagation = new CategoryPropertyItem();
                piPropagation.Id = CONFIG_PROPAGATE_ON_NETWORK;
                rootItem.PropertyItems.Add(piPropagation);
            }
            piPropagation.EntryValue = PropagateServiceOnTheNetwork.ToString();

            CategoryPropertyItem piDesktopShareMode = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_DESKTOP_SHARE_MODE);
            if (piDesktopShareMode == null)
            {
                piDesktopShareMode = new CategoryPropertyItem();
                piDesktopShareMode.Id = CONFIG_DESKTOP_SHARE_MODE;
                rootItem.PropertyItems.Add(piDesktopShareMode);
            }
            piDesktopShareMode.EntryValue = DesktopShareMode.ToString();

            CategoryPropertyItem piAcceptInputEvents = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_ACCEPT_INPUTS_FROM_CLIENTS);
            if (piAcceptInputEvents == null)
            {
                piAcceptInputEvents = new CategoryPropertyItem();
                piAcceptInputEvents.Id = CONFIG_ACCEPT_INPUTS_FROM_CLIENTS;
                rootItem.PropertyItems.Add(piAcceptInputEvents);
            }
            piAcceptInputEvents.EntryValue = AcceptKeyboardAndMouseInputFromClients.ToString();

            CategoryPropertyItem piDesktopImageClipWidth = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_DESKTOP_IMAGE_CLIP_WIDTH);
            if (piDesktopImageClipWidth == null)
            {
                piDesktopImageClipWidth = new CategoryPropertyItem();
                piDesktopImageClipWidth.Id = CONFIG_DESKTOP_IMAGE_CLIP_WIDTH;
                rootItem.PropertyItems.Add(piDesktopImageClipWidth);
            }
            piDesktopImageClipWidth.EntryValue = DesktopImageClipWidth.ToString();

            CategoryPropertyItem piDesktopImageClipHeight = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_DESKTOP_IMAGE_CLIP_HEIGHT);
            if (piDesktopImageClipHeight == null)
            {
                piDesktopImageClipHeight = new CategoryPropertyItem();
                piDesktopImageClipHeight.Id = CONFIG_DESKTOP_IMAGE_CLIP_HEIGHT;
                rootItem.PropertyItems.Add(piDesktopImageClipHeight);
            }
            piDesktopImageClipHeight.EntryValue = DesktopImageClipHeight.ToString();

            CategoryPropertyItem piClientsPerServiceThreads = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_CLIENTS_PER_SERVICE_THREADS);
            if (piClientsPerServiceThreads == null)
            {
                piClientsPerServiceThreads = new CategoryPropertyItem();
                piClientsPerServiceThreads.Id = CONFIG_CLIENTS_PER_SERVICE_THREADS;
                rootItem.PropertyItems.Add(piClientsPerServiceThreads);
            }
            piClientsPerServiceThreads.EntryValue = ClientsPerServiceThreads.ToString();

            CategoryPropertyItem piMaximumFailedLoginAttempt = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_MAXIMUM_FAILED_LOGIN_ATTEMPT);
            if (piMaximumFailedLoginAttempt == null)
            {
                piMaximumFailedLoginAttempt = new CategoryPropertyItem();
                piMaximumFailedLoginAttempt.Id = CONFIG_MAXIMUM_FAILED_LOGIN_ATTEMPT;
                rootItem.PropertyItems.Add(piMaximumFailedLoginAttempt);
            }
            piMaximumFailedLoginAttempt.EntryValue = MaximumFailedLoginAttempt.ToString();

            CategoryPropertyItem piBlackListTimeout = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_BLACKLIST_TIMEOUT);
            if (piBlackListTimeout == null)
            {
                piBlackListTimeout = new CategoryPropertyItem();
                piBlackListTimeout.Id = CONFIG_BLACKLIST_TIMEOUT;
                rootItem.PropertyItems.Add(piBlackListTimeout);
            }
            piBlackListTimeout.EntryValue = BlackListTimeout.ToString();

            CategoryPropertyItem piImageClipQuality = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_IMAGE_CLIP_QUALITY);
            if (piImageClipQuality == null)
            {
                piImageClipQuality = new CategoryPropertyItem();
                piImageClipQuality.Id = CONFIG_IMAGE_CLIP_QUALITY;
                rootItem.PropertyItems.Add(piImageClipQuality);
            }
            piImageClipQuality.EntryValue = DefaultImageClipQuality.ToString();

            CategoryPropertyItem piMouseMoveSendingInterval = ConfigurationAccessHelper.GetCategoryPropertyByPath(rootItem.PropertyItems, CONFIG_MOUSE_MOVE_SEND_INTERVAL);
            if (piMouseMoveSendingInterval == null)
            {
                piMouseMoveSendingInterval = new CategoryPropertyItem();
                piMouseMoveSendingInterval.Id = CONFIG_MOUSE_MOVE_SEND_INTERVAL;
                rootItem.PropertyItems.Add(piMouseMoveSendingInterval);
            }
            piMouseMoveSendingInterval.EntryValue = MouseMoveSendInterval.ToString();

            RemoteDesktopConfiguration.Save(saveMode);
        }

        #endregion

        #region Private method(s)

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void SectionHandler_OnConfigurationChanged(object sender, EventArgs e)
        {
            CategoryPropertyItem rootItem = ConfigurationAccessHelper.GetCategoryPropertyByPath(RemoteDesktopConfiguration.Settings.CategoryPropertyItems, CONFIG_ROOT);
            if (rootItem != null)
            {
                AuthenticationModeEnum authMode = AuthenticationModeEnum.OnlyPassword;
                if (ConfigurationAccessHelper.ParseEnumValue<AuthenticationModeEnum>(rootItem.PropertyItems, CONFIG_AUTHENTICATION_MODE, ref authMode))
                {
                    AuthenticationMode = authMode;
                }

                string moduleStore = string.Empty;
                if (ConfigurationAccessHelper.ParseStringValue(rootItem.PropertyItems, CONFIG_AUTHENTICATION_MODULE_STORE, ref moduleStore))
                {
                    mAuthenticationModuleStore = moduleStore;
                }

                bool propagateOnNetwork = true;
                if (ConfigurationAccessHelper.ParseBooleanValue(rootItem.PropertyItems, CONFIG_PROPAGATE_ON_NETWORK, ref propagateOnNetwork))
                {
                    PropagateServiceOnTheNetwork = propagateOnNetwork;
                }

                DesktopShareModeEnum desktopShareMode = DesktopShareModeEnum.Shared;
                if (ConfigurationAccessHelper.ParseEnumValue<DesktopShareModeEnum>(rootItem.PropertyItems, CONFIG_DESKTOP_SHARE_MODE, ref desktopShareMode))
                {
                    DesktopShareMode = desktopShareMode;
                }

                bool acceptKeyboardAndMouseInputFromClients = true;
                if (ConfigurationAccessHelper.ParseBooleanValue(rootItem.PropertyItems, CONFIG_ACCEPT_INPUTS_FROM_CLIENTS, ref acceptKeyboardAndMouseInputFromClients))
                {
                    AcceptKeyboardAndMouseInputFromClients = acceptKeyboardAndMouseInputFromClients;
                }

                int desktopImageClipWidth = Consts.DEFAULT_DESKTOP_IMAGE_CLIP_SIZE;
                if (ConfigurationAccessHelper.ParseIntValue(rootItem.PropertyItems, CONFIG_DESKTOP_IMAGE_CLIP_WIDTH, Consts.MINIMAL_CLIP_SIZE, int.MaxValue, ref desktopImageClipWidth))
                {
                    DesktopImageClipWidth = desktopImageClipWidth;
                }

                int desktopImageClipHeight = Consts.DEFAULT_DESKTOP_IMAGE_CLIP_SIZE;
                if (ConfigurationAccessHelper.ParseIntValue(rootItem.PropertyItems, CONFIG_DESKTOP_IMAGE_CLIP_WIDTH, Consts.MINIMAL_CLIP_SIZE, int.MaxValue, ref desktopImageClipHeight))
                {
                    DesktopImageClipHeight = desktopImageClipHeight;
                }

                int clientsPerServiceThreads = Consts.DEFAULT_CLIENTS_PER_SERVICE_THREADS;
                if (ConfigurationAccessHelper.ParseIntValue(rootItem.PropertyItems, CONFIG_DESKTOP_IMAGE_CLIP_WIDTH, Consts.MINIMAL_CLIENTS_PER_SERVICE_THREADS, int.MaxValue, ref clientsPerServiceThreads))
                {
                    ClientsPerServiceThreads = clientsPerServiceThreads;
                }

                int maximumFailedLoginAttempt = Consts.DEFAULT_MAXIMUM_FAILED_LOGIN_ATTEMPT;
                if (ConfigurationAccessHelper.ParseIntValue(rootItem.PropertyItems, CONFIG_MAXIMUM_FAILED_LOGIN_ATTEMPT, 0, int.MaxValue, ref maximumFailedLoginAttempt))
                {
                    MaximumFailedLoginAttempt = maximumFailedLoginAttempt;
                }

                int blackListTimeout = Consts.DEFAULT_BLACKLIST_TIMEOUT_IN_MINUTES;
                if (ConfigurationAccessHelper.ParseIntValue(rootItem.PropertyItems, CONFIG_BLACKLIST_TIMEOUT, 0, int.MaxValue, ref blackListTimeout))
                {
                    BlackListTimeout = blackListTimeout;
                }

                int imageClipQuality = Consts.DEFAULT_IMAGE_CLIP_QUALITY;
                if (ConfigurationAccessHelper.ParseIntValue(rootItem.PropertyItems, CONFIG_IMAGE_CLIP_QUALITY, 10, 100, ref imageClipQuality))
                {
                    DefaultImageClipQuality = imageClipQuality;
                }

                int mouseMoveSendingInterval = Consts.DEFAULT_MOUSE_MOVE_SEND_INTERVAL;
                if (ConfigurationAccessHelper.ParseIntValue(rootItem.PropertyItems, CONFIG_MOUSE_MOVE_SEND_INTERVAL, 0, int.MaxValue, ref mouseMoveSendingInterval))
                {
                    MouseMoveSendInterval = mouseMoveSendingInterval;
                }

            }

            Executor.Invoke(EventConfigurationChanged, null, EventArgs.Empty);
        }

        #endregion

    }

}
