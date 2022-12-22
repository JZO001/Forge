/* *********************************************************************
 * Date: 14 Aug 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using Forge.Configuration.Shared;
using Forge.Logging.Abstraction;
using Forge.Net.TerraGraf.ConfigSection;
using Forge.Shared;

namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>
    /// Represents the Windows NAT UPnP services settings
    /// </summary>
    [Serializable]
    public sealed class NATUPnPSettings
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<NATUPnPSettings>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mEnabled = true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mProbeAddress = "microsoft.com";

        private bool mInitialized = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NATUPnPSettings"/> class.
        /// </summary>
        public NATUPnPSettings()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsEnabled
        {
            get { return mEnabled; }
            set 
            { 
                if (!mInitialized) mEnabled = value; 
            }
        }

        /// <summary>
        /// Gets or sets the probe address.
        /// </summary>
        /// <value>
        /// The probe address.
        /// </value>
        [DebuggerHidden]
        public string ProbeAddress
        {
            get { return mProbeAddress; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                mProbeAddress = value;
            }
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
                LOGGER.Info(string.Format("TERRAGRAF, NATUPnP enabled: {0}", mEnabled.ToString()));
                LOGGER.Info(string.Format("TERRAGRAF, NATUPnP probe address: {0}", mProbeAddress));
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

        private void SectionHandler_OnConfigurationChanged(object sender, EventArgs e)
        {
            CategoryPropertyItem piRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkPeering/NETUPnP");
            if (piRoot != null)
            {
                bool boolValue = true;
                if (!string.IsNullOrEmpty(piRoot.EntryValue))
                {
                    if (bool.TryParse(piRoot.EntryValue, out boolValue))
                    {
                        mEnabled = boolValue;
                    }
                }

                mProbeAddress = ConfigurationAccessHelper.GetValueByPath(piRoot.PropertyItems, "ProbeAddressToDetectGatewayNetworkInterface");
                if (mProbeAddress == null)
                {
                    mProbeAddress = string.Empty;
                }
            }
        }

        #endregion

    }

}
