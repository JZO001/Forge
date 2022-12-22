/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Forge.Configuration.Shared;
using Forge.Logging.Abstraction;
using Forge.Net.TerraGraf.ConfigSection;
using Forge.Shared;

namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>
    /// UDP detection settings
    /// </summary>
    [Serializable]
    public sealed class UDPDetectionSettings
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<UDPDetectionSettings>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mEnabled = true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<int> mBroadcastListeningPorts = new List<int>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<int> mBroadcastTargetPorts = new List<int>();

        private bool mInitialized = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="UDPDetectionSettings"/> class.
        /// </summary>
        public UDPDetectionSettings()
        {
            IPv4MulticastAddress = string.Empty;
            IPv6MulticastAddress = string.Empty;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UDPDetectionSettings"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get { return mEnabled; }
            set
            {
                if (!mInitialized) mEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the broadcast listening ports.
        /// </summary>
        /// <value>
        /// The broadcast listening ports.
        /// </value>
        public List<int> BroadcastListeningPorts
        {
            get { return mInitialized ? new List<int>(mBroadcastListeningPorts) : mBroadcastListeningPorts; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                // are they different?
                bool dif = value.Count != mBroadcastListeningPorts.Count;
                if (!dif)
                {
                    foreach (int e in mBroadcastListeningPorts)
                    {
                        if (!value.Contains<int>(e))
                        {
                            dif = true;
                            break;
                        }
                    }
                }

                if (dif)
                {
                    mBroadcastListeningPorts.Clear();
                    mBroadcastListeningPorts.AddRange(value);
                    if (mInitialized)
                    {
                        // propagate changes

                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the broadcast target ports.
        /// </summary>
        /// <value>
        /// The broadcast target ports.
        /// </value>
        public List<int> BroadcastTargetPorts
        {
            get { return mInitialized ? new List<int>(mBroadcastTargetPorts) : mBroadcastTargetPorts; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                // are they different?
                bool dif = value.Count != mBroadcastTargetPorts.Count;
                if (!dif)
                {
                    foreach (int e in mBroadcastTargetPorts)
                    {
                        if (!value.Contains<int>(e))
                        {
                            dif = true;
                            break;
                        }
                    }
                }

                if (dif)
                {
                    mBroadcastTargetPorts.Clear();
                    mBroadcastTargetPorts.AddRange(value);
                    if (mInitialized)
                    {
                        // propagate changes

                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the detection mode.
        /// </summary>
        /// <value>
        /// The detection mode.
        /// </value>
        public UDPDetectionModeEnum DetectionMode { get; set; }

        /// <summary>
        /// Gets or sets the IPV4 multicast address.
        /// </summary>
        /// <value>
        /// The i PV4 multicast address.
        /// </value>
        public string IPv4MulticastAddress { get; set; }

        /// <summary>
        /// Gets or sets the IPV6 multicast address.
        /// </summary>
        /// <value>
        /// The i PV6 multicast address.
        /// </value>
        public string IPv6MulticastAddress { get; set; }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Logs the configuration.
        /// </summary>
        internal void LogConfiguration()
        {
            if (LOGGER.IsInfoEnabled)
            {
                LOGGER.Info(string.Format("TERRAGRAF, Broadcast detection enabled: {0}", mEnabled));
                LOGGER.Info(string.Format("TERRAGRAF, Broadcast detection mode: {0}", DetectionMode.ToString()));
                LOGGER.Info(string.Format("TERRAGRAF, IPv4MulticastAddress: {0}", IPv4MulticastAddress));
                LOGGER.Info(string.Format("TERRAGRAF, IPv6MulticastAddress: {0}", IPv6MulticastAddress));

                StringBuilder sb = new StringBuilder();
                foreach (int i in mBroadcastListeningPorts)
                {
                    sb.Append("Port: ");
                    sb.AppendLine(i.ToString());
                }
                LOGGER.Info(string.Format("TERRAGRAF, Broadcast listening ports: {0}{1}", Environment.NewLine, sb.ToString()));

                sb.Clear();
                foreach (int i in mBroadcastTargetPorts)
                {
                    sb.Append("Port: ");
                    sb.AppendLine(i.ToString());
                }
                LOGGER.Info(string.Format("TERRAGRAF, Broadcast target ports: {0}{1}", Environment.NewLine, sb.ToString()));
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        internal void Initialize()
        {
            if (NetworkManager.ConfigurationSource == ConfigurationSourceEnum.ConfigurationManager)
            {
                TerraGrafConfiguration.SectionHandler.OnConfigurationChanged += new EventHandler<EventArgs>(SectionHandler_OnConfigurationChanged);
                SectionHandler_OnConfigurationChanged(null, null);
            }
            mInitialized = true;
        }

        /// <summary>Cleans up.</summary>
        internal void CleanUp()
        {
            if (mInitialized && NetworkManager.ConfigurationSource == ConfigurationSourceEnum.ConfigurationManager)
            {
                TerraGrafConfiguration.SectionHandler.OnConfigurationChanged -= new EventHandler<EventArgs>(SectionHandler_OnConfigurationChanged);
            }
            mInitialized = false;
        }

        #endregion

        #region Private method(s)

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)")]
        private void SectionHandler_OnConfigurationChanged(object sender, EventArgs e)
        {
            CategoryPropertyItem piRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkPeering/UDP_Detection/Broadcast_Listening_Ports");
            List<int> list = new List<int>();
            if (piRoot != null)
            {
                foreach (CategoryPropertyItem pi in piRoot)
                {
                    list.Add(int.Parse(pi.EntryValue));
                }
            }
            BroadcastListeningPorts = list;

            piRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkPeering/UDP_Detection/Broadcast_Target_Ports");
            list = new List<int>();
            if (piRoot != null)
            {
                foreach (CategoryPropertyItem pi in piRoot)
                {
                    list.Add(int.Parse(pi.EntryValue));
                }
            }
            mBroadcastTargetPorts = list;

            string value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkPeering/UDP_Detection");
            bool boolValue = false;
            if (!string.IsNullOrEmpty(value))
            {
                bool.TryParse(value, out boolValue);
            }
            Enabled = boolValue;

            IPv4MulticastAddress = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkPeering/UDP_Detection/IPv4MulticastAddress");
            IPv6MulticastAddress = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkPeering/UDP_Detection/IPv6MulticastAddress");

            UDPDetectionModeEnum mode = UDPDetectionModeEnum.Broadcast;
            if (ConfigurationAccessHelper.ParseEnumValue<UDPDetectionModeEnum>(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkPeering/UDP_Detection/DetectionMode", ref mode))
            {
                DetectionMode = mode;
            }

        }

        #endregion

    }

}
