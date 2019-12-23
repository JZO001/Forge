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
using System.Threading;
using Forge.Configuration.Shared;
using Forge.Logging;
using Forge.Net.Synapse;
using Forge.Net.TerraGraf.ConfigSection;

namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>
    /// Represents the settings of the remote targets
    /// </summary>
    [Serializable]
    public sealed class TCPConnectionSettings
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TCPConnectionSettings));

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<ConnectionEntry> mEndPoints = new List<ConnectionEntry>();

        private bool mInitialized = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPConnectionSettings"/> class.
        /// </summary>
        internal TCPConnectionSettings()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the end points.
        /// </summary>
        /// <value>
        /// The end points.
        /// </value>
        public List<ConnectionEntry> EndPoints
        {
            get { return new List<ConnectionEntry>(mEndPoints); }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                // különböznek?
                bool dif = value.Count != mEndPoints.Count;
                if (!dif)
                {
                    foreach (ConnectionEntry e in mEndPoints)
                    {
                        if (!value.Contains<ConnectionEntry>(e))
                        {
                            dif = true;
                            break;
                        }
                    }
                }

                if (dif)
                {
                    mEndPoints.Clear();
                    mEndPoints.AddRange(value);
                    if (mInitialized)
                    {
                        // változtatások propagálása

                    }
                }
            }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Logs the configuration.
        /// </summary>
        internal void LogConfiguration()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ConnectionEntry ep in mEndPoints)
            {
                sb.AppendLine(ep.ToString());
            }
            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("TERRAGRAF, TCP connection(s): {0}{1}", Environment.NewLine, sb.ToString()));
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Int32.TryParse(System.String,System.Int32@)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)")]
        private void SectionHandler_OnConfigurationChanged(object sender, EventArgs e)
        {
            CategoryPropertyItem piRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkPeering/TCP_Connections");
            List<ConnectionEntry> list = new List<ConnectionEntry>();
            if (piRoot != null)
            {
                foreach (CategoryPropertyItem pi in piRoot)
                {
                    AddressEndPoint address = AddressEndPoint.Parse(pi.EntryValue);
                    bool reConnect = true;
                    int delayValue = 1000;
                    int conTimeout = Timeout.Infinite;
                    string item = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, string.Format("NetworkPeering/TCP_Connections/{0}/ReconnectOnFailure", pi.Id));
                    if (!string.IsNullOrEmpty(item))
                    {
                        bool.TryParse(item, out reConnect);
                    }
                    item = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, string.Format("NetworkPeering/TCP_Connections/{0}/DelayBetweenAttempsInMS", pi.Id));
                    if (!string.IsNullOrEmpty(item))
                    {
                        int.TryParse(item, out delayValue);
                        if (delayValue < 0)
                        {
                            delayValue = 1000;
                        }
                    }
                    item = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, string.Format("NetworkPeering/TCP_Connections/{0}/ConnectionTimeout", pi.Id));
                    if (!string.IsNullOrEmpty(item))
                    {
                        int.TryParse(item, out conTimeout);
                        if (conTimeout < Timeout.Infinite)
                        {
                            conTimeout = Timeout.Infinite;
                        }
                    }
                    list.Add(new ConnectionEntry(address, reConnect, delayValue, conTimeout));
                }
            }
            EndPoints = list;
        }

        #endregion

    }

}
