/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Forge.Configuration.Shared;
using Forge.Logging;
using Forge.Net.Synapse;
using Forge.Net.TerraGraf.ConfigSection;
using Forge.Net.TerraGraf.NetworkPeers;

namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>
    /// Represents the settings of the TCP servers
    /// </summary>
    [Serializable]
    public sealed class TCPServerSettings
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TCPServerSettings));

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mAuto = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<AddressEndPoint> mEndPoints = new List<AddressEndPoint>();

        private bool mInitialized = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPServerSettings"/> class.
        /// </summary>
        internal TCPServerSettings()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="TCPServerSettings"/> is auto.
        /// </summary>
        /// <value>
        ///   <c>true</c> if auto; otherwise, <c>false</c>.
        /// </value>
        public bool Auto
        {
            get { return mAuto; }
        }

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
                            List<TCPServer> serverEps = new List<TCPServer>();
                            foreach (AddressEndPoint aep in mEndPoints)
                            {
                                try
                                {
                                    if (aep.Host.Equals(IPAddress.Any.ToString()) || aep.Host.Equals(IPAddress.IPv6Any.ToString()))
                                    {
                                        // detektálom az interface-eket és mindre ráülök
                                        foreach (IPAddress a in Dns.GetHostAddresses(Dns.GetHostName()))
                                        {
                                            if (a.AddressFamily == AddressFamily.InterNetwork || (a.AddressFamily == AddressFamily.InterNetworkV6 &&
                                                NetworkManager.Instance.InternalConfiguration.Settings.EnableIPV6))
                                            {
                                                try
                                                {
                                                    AddressEndPoint ep = new AddressEndPoint(a.ToString(), aep.Port);
                                                    if (NetworkManager.Instance.InternalNetworkManager.IsServerEndPointExist(ep))
                                                    {
                                                        // már létezik, megtartom az eredetit
                                                        foreach (TCPServer s in NetworkManager.Instance.InternalLocalhost.TCPServerCollection.TCPServers)
                                                        {
                                                            if (s.EndPoint.Equals(ep))
                                                            {
                                                                serverEps.Add(s);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        long serverId = NetworkManager.Instance.InternalNetworkManager.StartServer(ep);
                                                        AddressEndPoint serverEp = NetworkManager.Instance.InternalNetworkManager.GetServerEndPoint(serverId);
                                                        serverEps.Add(new TCPServer(serverId, serverEp, false));
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("TCPServerSettings, failed to start server on interface '{0}' on any port. Reason: {1}", a.ToString(), ex.Message));
                                                }
                                            }
                                        }
                                        foreach (IPAddress ip in Dns.GetHostAddresses("localhost"))
                                        {
                                            if (ip.AddressFamily == AddressFamily.InterNetwork || (ip.AddressFamily == AddressFamily.InterNetworkV6 &&
                                                NetworkManager.Instance.InternalConfiguration.Settings.EnableIPV6))
                                            {
                                                try
                                                {
                                                    NetworkManager.Instance.InternalNetworkManager.StartServer(new AddressEndPoint(ip.ToString(), aep.Port));
                                                }
                                                catch (Exception ex)
                                                {
                                                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("CONNECTION_MANAGER, failed to start server on interface '{0}' on any port. Reason: {1}", ip.ToString(), ex.Message));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        long serverId = NetworkManager.Instance.InternalStartTCPServer(aep, NetworkManager.Instance.InternalNetworkManager.ServerStreamFactory);
                                        AddressEndPoint ep = NetworkManager.Instance.InternalNetworkManager.GetServerEndPoint(serverId);
                                        serverEps.Add(new TCPServer(serverId, ep, false));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // sikertelen a szerver indítása
                                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("TCPServerSettings, failed to start TCP server, {0}. Reason: {1}", aep.ToString(), ex.Message));
                                }
                            }
                            if (serverEps.Count > 0 || mEndPoints.Count == 0)
                            {
                                NetworkManager.Instance.SyncRoot.Lock();
                                try
                                {
                                    foreach (TCPServer server in NetworkManager.Instance.InternalLocalhost.TCPServerCollection.TCPServers)
                                    {
                                        if (server.IsManuallyStarted)
                                        {
                                            if (!serverEps.Contains(server))
                                            {
                                                serverEps.Add(server);
                                            }
                                        }
                                        else if (!server.IsManuallyStarted && !serverEps.Contains(server))
                                        {
                                            // ami már nem kell, leállítom
                                            NetworkManager.Instance.InternalNetworkManager.StopServer(server.ServerId);
                                        }
                                    }
                                    lock (NetworkManager.Instance.InternalLocalhost.TCPServerCollection.TCPServers)
                                    {
                                        NetworkManager.Instance.InternalLocalhost.TCPServerCollection.TCPServers.Clear();
                                        NetworkManager.Instance.InternalLocalhost.TCPServerCollection.TCPServers.AddRange(serverEps);
                                    }
                                    NetworkManager.Instance.InternalSendTCPServers();
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
                LOGGER.Info(string.Format("TERRAGRAF, Auto define and start TCP servers: {0}", this.mAuto));
                StringBuilder sb = new StringBuilder();
                foreach (AddressEndPoint ep in mEndPoints)
                {
                    sb.AppendLine(ep.ToString());
                }
                LOGGER.Info(string.Format("TERRAGRAF, TCP servers: {0}{1}", Environment.NewLine, sb.ToString()));
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
            CategoryPropertyItem piRoot = ConfigurationAccessHelper.GetCategoryPropertyByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkPeering/TCPServers/Auto");
            List<AddressEndPoint> list = new List<AddressEndPoint>();
            if (piRoot != null)
            {
                foreach (CategoryPropertyItem pi in piRoot)
                {
                    list.Add(AddressEndPoint.Parse(pi.EntryValue));
                }
            }
            EndPoints = list;

            string value = ConfigurationAccessHelper.GetValueByPath(TerraGrafConfiguration.Settings.CategoryPropertyItems, "NetworkPeering/TCPServers/Auto");
            bool boolValue = false;
            if (!string.IsNullOrEmpty(value))
            {
                bool.TryParse(value, out boolValue);
            }
            mAuto = boolValue;
        }

        #endregion

    }

}
