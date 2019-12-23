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
using Forge.Logging;
using Forge.Net.Synapse;
using Forge.Net.TerraGraf.ConfigSection;
using Forge.Net.TerraGraf.NetworkPeers;

namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>
    /// Describe NAT gateways
    /// </summary>
    [Serializable]
    public sealed class NATGatewaySettings
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(NATGatewaySettings));

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<AddressEndPoint> mEndPoints = new List<AddressEndPoint>();

        private bool mInitialized = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NATGatewaySettings"/> class.
        /// </summary>
        internal NATGatewaySettings()
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
        public List<AddressEndPoint> EndPoints
        {
            get { return new List<AddressEndPoint>(mEndPoints); }
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
                    foreach (AddressEndPoint e in mEndPoints)
                    {
                        if (!value.Contains<AddressEndPoint>(e))
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
                        if (NetworkManager.Instance.ManagerState == Management.ManagerStateEnum.Started)
                        {
                            NetworkManager.Instance.SyncRoot.Lock();
                            try
                            {
                                bool changes = mEndPoints.Count == 0;
                                mEndPoints.ForEach(i =>
                                {
                                    NATGateway gw = new NATGateway(i);
                                    if (!NetworkManager.Instance.InternalLocalhost.NATGatewayCollection.NATGateways.Contains(gw))
                                    {
                                        NetworkManager.Instance.InternalLocalhost.NATGatewayCollection.NATGateways.Add(gw);
                                        changes = true;
                                    }
                                }
                                );
                                if (changes)
                                {
                                    NetworkManager.Instance.InternalSendNATGateways();
                                }
                            }
                            finally
                            {
                                NetworkManager.Instance.SyncRoot.Unlock();
                            }
                        }
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
            foreach (AddressEndPoint ep in mEndPoints)
            {
                sb.AppendLine(ep.ToString());
            }
            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("TERRAGRAF, NAT Gateways: {0}{1}", Environment.NewLine, sb.ToString()));
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

        private void SectionHandler_OnConfigurationChanged(object sender, EventArgs e)
        {
            CategoryPropertyItem piRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkPeering/NAT_Gateways");
            List<AddressEndPoint> list = new List<AddressEndPoint>();
            if (piRoot != null)
            {
                foreach (CategoryPropertyItem pi in piRoot)
                {
                    list.Add(AddressEndPoint.Parse(pi.EntryValue));
                }
            }
            EndPoints = list;
        }

        #endregion

    }

}
