/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Forge.Configuration.Shared;
using Forge.Logging;
using Forge.Net.TerraGraf.ConfigSection;

namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>
    /// General settings
    /// </summary>
    [Serializable]
    public sealed class Settings
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(Settings));

        private static readonly int DEFAULT_MAX_MESSAGE_PASSAGE_NUMBER = 3;

        private static readonly int DEFAULT_CONNECTION_TIMEOUT = 120000;

        private static readonly int DEFAULT_MAX_CONNECTIONS_WITH_NETWORKPEERS = 2;

        private static readonly int DEFAULT_SOCKET_ACCEPT_TIME_WAIT = 60000;

        private static readonly int DEFAULT_SOCKET_BACKLOG_SIZE = 256;

        private static readonly int DEFAULT_CONCURRENT_SOCKET_CONNECTION_ATTEMPTS = 10;

        private static readonly int DEFAULT_RECEIVE_BUFFER_SIZE = Synapse.NetworkManager.DefaultSocketReceiveBufferSize;

        private static readonly int DEFAULT_SEND_BUFFER_SIZE = Synapse.NetworkManager.DefaultSocketSendBufferSize;

        private static readonly int DEFAULT_RECEIVE_TIMEOUT = Timeout.Infinite;

        private static readonly int DEFAULT_SEND_TIMEOUT = Timeout.Infinite;

        private static readonly bool DEFAULT_ADD_WINDOWS_FIREWALL_EXCEPTION = true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mBlackHole = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mInitialized = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDefaultConnectionTimeoutInMS = DEFAULT_CONNECTION_TIMEOUT;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mEnableMultipleConnectionWithNetworkPeers = true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mEnableAgressiveConnectionEstablishment = true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mMaxConnectionsWithNetworkPeers = DEFAULT_MAX_CONNECTIONS_WITH_NETWORKPEERS;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDefaultSocketAcceptTimeWaitInMS = DEFAULT_SOCKET_ACCEPT_TIME_WAIT;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDefaultSocketBacklogSize = DEFAULT_SOCKET_BACKLOG_SIZE;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDefaultConcurrentSocketConnectionAttempts = DEFAULT_CONCURRENT_SOCKET_CONNECTION_ATTEMPTS;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDefaultLowLevelSocketReceiveBufferSize = DEFAULT_RECEIVE_BUFFER_SIZE;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDefaultLowLevelSocketSendBufferSize = DEFAULT_SEND_BUFFER_SIZE;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mDefaultLowLevelNoDelay = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDefaultReceiveBufferSize = DEFAULT_RECEIVE_BUFFER_SIZE;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDefaultSendBufferSize = DEFAULT_SEND_BUFFER_SIZE;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDefaultReceiveTimeoutInMS = DEFAULT_RECEIVE_TIMEOUT;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDefaultSendTimeoutInMS = DEFAULT_SEND_TIMEOUT;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDefaultLowLevelSocketKeepAliveTime = Synapse.NetworkManager.DefaultSocketKeepAliveTime;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDefaultLowLevelSocketKeepAliveTimeInterval = Synapse.NetworkManager.DefaultSocketKeepAliveTimeInterval;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mMaxMessagePassageNumber = DEFAULT_MAX_MESSAGE_PASSAGE_NUMBER;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mEnableIPV6 = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mAddWindowsFirewallException = DEFAULT_ADD_WINDOWS_FIREWALL_EXCEPTION;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        internal Settings()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets a value indicating whether [black hole].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [black hole]; otherwise, <c>false</c>.
        /// </value>
        public bool BlackHole
        {
            get { return mBlackHole; }
            set
            {
                if (value != mBlackHole)
                {
                    mBlackHole = value;
                    if (mInitialized)
                    {
                        NetworkManager.Instance.InternalBlackHole(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the default connection timeout in MS.
        /// </summary>
        /// <value>
        /// The default connection timeout in MS.
        /// </value>
        [DebuggerHidden]
        public int DefaultConnectionTimeoutInMS
        {
            get { return mDefaultConnectionTimeoutInMS; }
            set
            {
                if (value < 500)
                {
                    value = DEFAULT_CONNECTION_TIMEOUT;
                }
                mDefaultConnectionTimeoutInMS = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable multiple connection with network peers].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable multiple connection with network peers]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool EnableMultipleConnectionWithNetworkPeers
        {
            get { return mEnableMultipleConnectionWithNetworkPeers; }
            set { mEnableMultipleConnectionWithNetworkPeers = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable agressive connection establishment].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable agressive connection establishment]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool EnableAgressiveConnectionEstablishment
        {
            get { return mEnableAgressiveConnectionEstablishment; }
            set { mEnableAgressiveConnectionEstablishment = value; }
        }

        /// <summary>
        /// Gets or sets the max connections with network peers.
        /// </summary>
        /// <value>
        /// The max connections with network peers.
        /// </value>
        [DebuggerHidden]
        public int MaxConnectionsWithNetworkPeers
        {
            get { return mMaxConnectionsWithNetworkPeers; }
            set
            {
                if (value > 0)
                {
                    mMaxConnectionsWithNetworkPeers = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the default socket accept time wait in MS.
        /// </summary>
        /// <value>
        /// The default socket accept time wait in MS.
        /// </value>
        [DebuggerHidden]
        public int DefaultSocketAcceptTimeWaitInMS
        {
            get { return mDefaultSocketAcceptTimeWaitInMS; }
            set
            {
                if (value < 0)
                {
                    value = DEFAULT_SOCKET_ACCEPT_TIME_WAIT;
                }
                mDefaultSocketAcceptTimeWaitInMS = value;
            }
        }

        /// <summary>
        /// Gets or sets the default size of the socket backlog.
        /// </summary>
        /// <value>
        /// The default size of the socket backlog.
        /// </value>
        [DebuggerHidden]
        public int DefaultSocketBacklogSize
        {
            get { return mDefaultSocketBacklogSize; }
            set
            {
                if (value < 0)
                {
                    value = DEFAULT_SOCKET_BACKLOG_SIZE;
                }
                mDefaultSocketBacklogSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the default size of the low level socket receive buffer.
        /// </summary>
        /// <value>
        /// The default size of the low level socket receive buffer.
        /// </value>
        [DebuggerHidden]
        public int DefaultLowLevelSocketReceiveBufferSize
        {
            get { return mDefaultLowLevelSocketReceiveBufferSize; }
            set
            {
                if (value < 1024)
                {
                    value = DEFAULT_RECEIVE_BUFFER_SIZE;
                }
                mDefaultLowLevelSocketReceiveBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the default size of the low level socket send buffer.
        /// </summary>
        /// <value>
        /// The default size of the low level socket send buffer.
        /// </value>
        [DebuggerHidden]
        public int DefaultLowLevelSocketSendBufferSize
        {
            get { return mDefaultLowLevelSocketSendBufferSize; }
            set
            {
                if (value < 1024)
                {
                    value = DEFAULT_SEND_BUFFER_SIZE;
                }
                mDefaultLowLevelSocketSendBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [default low level no delay].
        /// </summary>
        /// <value>
        /// <c>true</c> if [default low level no delay]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool DefaultLowLevelNoDelay
        {
            get { return mDefaultLowLevelNoDelay; }
            set { mDefaultLowLevelNoDelay = value; }
        }

        /// <summary>
        /// Gets or sets the default size of the receive buffer.
        /// </summary>
        /// <value>
        /// The default size of the receive buffer.
        /// </value>
        [DebuggerHidden]
        public int DefaultReceiveBufferSize
        {
            get { return mDefaultReceiveBufferSize; }
            set
            {
                if (value < 1024)
                {
                    value = DEFAULT_RECEIVE_BUFFER_SIZE;
                }
                mDefaultReceiveBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the default size of the send buffer.
        /// </summary>
        /// <value>
        /// The default size of the send buffer.
        /// </value>
        [DebuggerHidden]
        public int DefaultSendBufferSize
        {
            get { return mDefaultSendBufferSize; }
            set
            {
                if (value < 1024)
                {
                    value = DEFAULT_SEND_BUFFER_SIZE;
                }
                mDefaultSendBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the default receive timeout in MS.
        /// </summary>
        /// <value>
        /// The default receive timeout in MS.
        /// </value>
        [DebuggerHidden]
        public int DefaultReceiveTimeoutInMS
        {
            get { return mDefaultReceiveTimeoutInMS; }
            set
            {
                if (value < 500)
                {
                    value = 500;
                }
                mDefaultReceiveTimeoutInMS = value;
            }
        }

        /// <summary>
        /// Gets or sets the default send timeout in MS.
        /// </summary>
        /// <value>
        /// The default send timeout in MS.
        /// </value>
        [DebuggerHidden]
        public int DefaultSendTimeoutInMS
        {
            get { return mDefaultSendTimeoutInMS; }
            set
            {
                if (value < 500)
                {
                    value = 500;
                }
                mDefaultSendTimeoutInMS = value;
            }
        }

        /// <summary>
        /// Gets or sets the default keep alive time.
        /// </summary>
        /// <value>
        /// The default keep alive time.
        /// </value>
        [DebuggerHidden]
        public int DefaultLowLevelSocketKeepAliveTime
        {
            get { return mDefaultLowLevelSocketKeepAliveTime; }
            set
            {
                if (value >= 1000)
                {
                    mDefaultLowLevelSocketKeepAliveTime = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the default keep alive time interval.
        /// </summary>
        /// <value>
        /// The default keep alive time interval.
        /// </value>
        [DebuggerHidden]
        public int DefaultLowLevelSocketKeepAliveTimeInterval
        {
            get { return mDefaultLowLevelSocketKeepAliveTimeInterval; }
            set
            {
                if (value >= 1000)
                {
                    mDefaultLowLevelSocketKeepAliveTimeInterval = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the default concurrent socket connection attempts.
        /// </summary>
        /// <value>
        /// The default concurrent socket connection attempts.
        /// </value>
        [DebuggerHidden]
        public int DefaultConcurrentSocketConnectionAttempts
        {
            get { return mDefaultConcurrentSocketConnectionAttempts; }
            set
            {
                if (value < 1)
                {
                    value = DEFAULT_CONCURRENT_SOCKET_CONNECTION_ATTEMPTS;
                }
                mDefaultConcurrentSocketConnectionAttempts = value;
            }
        }

        /// <summary>
        /// Gets or sets the max message passage number.
        /// </summary>
        /// <value>
        /// The max message passage number.
        /// </value>
        [DebuggerHidden]
        public int MaxMessagePassageNumber
        {
            get { return mMaxMessagePassageNumber; }
            set
            {
                if (value < 1)
                {
                    value = DEFAULT_MAX_MESSAGE_PASSAGE_NUMBER;
                }
                mMaxMessagePassageNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable IP v6].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable IP v6]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool EnableIPV6
        {
            get { return mEnableIPV6; }
            set { mEnableIPV6 = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [add windows firewall exception].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [add windows firewall exception]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool AddWindowsFirewallException
        {
            get { return mAddWindowsFirewallException; }
            set { mAddWindowsFirewallException = value; }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Logs the configuration.
        /// </summary>
        internal void LogConfiguration()
        {
            if (LOGGER.IsInfoEnabled)
            {
                LOGGER.Info(string.Format("TERRAGRAF, Blackhole: {0}", this.mBlackHole));
                LOGGER.Info(string.Format("TERRAGRAF, DefaultConnectionTimeoutInMS: {0}", this.mDefaultConnectionTimeoutInMS));
                LOGGER.Info(string.Format("TERRAGRAF, EnableMultipleConnectionWithNetworkPeers: {0}", this.mEnableMultipleConnectionWithNetworkPeers));
                LOGGER.Info(string.Format("TERRAGRAF, EnableAgressiveConnectionEstablishment: {0}", this.mEnableAgressiveConnectionEstablishment));
                LOGGER.Info(string.Format("TERRAGRAF, MaxConnectionsWithNetworkPeers: {0}", this.mMaxConnectionsWithNetworkPeers));
                LOGGER.Info(string.Format("TERRAGRAF, DefaultSocketAcceptTimeWaitInMS: {0}", this.mDefaultSocketAcceptTimeWaitInMS));
                LOGGER.Info(string.Format("TERRAGRAF, DefaultSocketBacklogSize: {0}", this.mDefaultSocketBacklogSize));
                LOGGER.Info(string.Format("TERRAGRAF, DefaultConcurrentSocketConnectionAttempts: {0}", this.mDefaultConcurrentSocketConnectionAttempts));
                LOGGER.Info(string.Format("TERRAGRAF, DefaultReceiveBufferSize: {0}", this.mDefaultReceiveBufferSize));
                LOGGER.Info(string.Format("TERRAGRAF, DefaultSendBufferSize: {0}", this.mDefaultSendBufferSize));
                LOGGER.Info(string.Format("TERRAGRAF, DefaultReceiveTimeoutInMS: {0}", this.mDefaultReceiveTimeoutInMS));
                LOGGER.Info(string.Format("TERRAGRAF, DefaultSendTimeoutInMS: {0}", this.mDefaultSendTimeoutInMS));
                LOGGER.Info(string.Format("TERRAGRAF, DefaultLowLevelSocketKeepAliveTime: {0}", this.mDefaultLowLevelSocketKeepAliveTime));
                LOGGER.Info(string.Format("TERRAGRAF, DefaultLowLevelSocketKeepAliveTimeInterval: {0}", this.mDefaultLowLevelSocketKeepAliveTimeInterval));
                LOGGER.Info(string.Format("TERRAGRAF, DefaultLowLevelNoDelay: {0}", this.mDefaultLowLevelNoDelay.ToString()));
                LOGGER.Info(string.Format("TERRAGRAF, MaxMessagePassageNumber: {0}", this.mMaxMessagePassageNumber));
                LOGGER.Info(string.Format("TERRAGRAF, EnableIPV6: {0}", this.mEnableIPV6.ToString()));
                LOGGER.Info(string.Format("TERRAGRAF, AddWindowsFirewallException: {0}", this.mAddWindowsFirewallException.ToString()));
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        internal void Initialize()
        {
            TerraGrafConfiguration.SectionHandler.OnConfigurationChanged += new EventHandler<EventArgs>(SectionHandler_OnConfigurationChanged);
            SectionHandler_OnConfigurationChanged(null, null);
            this.mInitialized = true;
        }

        #endregion

        #region Private method(s)

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)")]
        private void SectionHandler_OnConfigurationChanged(object sender, EventArgs e)
        {
            string value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/BlackHole");
            bool boolValue = false;
            if (!string.IsNullOrEmpty(value))
            {
                bool.TryParse(value, out boolValue);
            }
            BlackHole = boolValue;

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/EnableIPV6");
            boolValue = false;
            if (!string.IsNullOrEmpty(value))
            {
                bool.TryParse(value, out boolValue);
            }
            EnableIPV6 = boolValue;

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultConnectionTimeoutInMS");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue >= 0)
                    {
                        DefaultConnectionTimeoutInMS = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/EnableMultipleConnectionWithNetworkPeers");
            boolValue = false;
            if (!string.IsNullOrEmpty(value))
            {
                if (bool.TryParse(value, out boolValue))
                {
                    EnableMultipleConnectionWithNetworkPeers = boolValue;
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultLowLevelNoDelay");
            boolValue = false;
            if (!string.IsNullOrEmpty(value))
            {
                if (bool.TryParse(value, out boolValue))
                {
                    DefaultLowLevelNoDelay = boolValue;
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/EnableAgressiveConnectionEstablishment");
            boolValue = false;
            if (!string.IsNullOrEmpty(value))
            {
                if (bool.TryParse(value, out boolValue))
                {
                    EnableAgressiveConnectionEstablishment = boolValue;
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/MaxConnectionsWithNetworkPeers");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue > 0)
                    {
                        MaxConnectionsWithNetworkPeers = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/MaxMessagePassageNumber");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (mMaxMessagePassageNumber > 0)
                    {
                        MaxMessagePassageNumber = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultSocketAcceptTimeWaitInMS");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue > 0)
                    {
                        mDefaultSocketAcceptTimeWaitInMS = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultSocketBacklogSize");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue > 0)
                    {
                        mDefaultSocketBacklogSize = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultLowLevelSocketReceiveBufferSize");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue > 0)
                    {
                        DefaultLowLevelSocketReceiveBufferSize = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultLowLevelSocketSendBufferSize");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue > 0)
                    {
                        DefaultLowLevelSocketSendBufferSize = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultReceiveBufferSize");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue > 0)
                    {
                        DefaultReceiveBufferSize = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultSendBufferSize");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue > 0)
                    {
                        DefaultSendBufferSize = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultLowLevelSocketKeepAliveTime");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue >= 1000)
                    {
                        DefaultLowLevelSocketKeepAliveTime = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultLowLevelSocketKeepAliveTimeInterval");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue >= 1000)
                    {
                        DefaultLowLevelSocketKeepAliveTimeInterval = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultConcurrentSocketConnectionAttempts");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue > 0)
                    {
                        mDefaultConcurrentSocketConnectionAttempts = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultReceiveTimeoutInMS");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue > 0)
                    {
                        DefaultReceiveTimeoutInMS = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/DefaultSendTimeoutInMS");
            if (!string.IsNullOrEmpty(value))
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    if (intValue > 0)
                    {
                        DefaultSendTimeoutInMS = intValue;
                    }
                }
            }

            value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "Settings/AddWindowsFirewallException");
            boolValue = DEFAULT_ADD_WINDOWS_FIREWALL_EXCEPTION;
            if (!string.IsNullOrEmpty(value))
            {
                if (bool.TryParse(value, out boolValue))
                {
                    AddWindowsFirewallException = boolValue;
                }
            }
        }

        #endregion

    }

}
