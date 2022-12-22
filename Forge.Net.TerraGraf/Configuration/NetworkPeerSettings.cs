/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;

namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>
    /// Represents the settings of the network peers
    /// </summary>
    [Serializable]
    public sealed class NetworkPeerSettings
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NATGatewaySettings mNATGateways = new NATGatewaySettings();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TCPServerSettings mTCPServerSettings = new TCPServerSettings();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TCPConnectionSettings mTCPConnections = new TCPConnectionSettings();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UDPDetectionSettings mUDPDetection = new UDPDetectionSettings();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NATUPnPSettings mNATUPnPSettings = new NATUPnPSettings();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPeerSettings"/> class.
        /// </summary>
        public NetworkPeerSettings()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the NAT gateways.
        /// </summary>
        /// <value>
        /// The NAT gateways.
        /// </value>
        [DebuggerHidden]
        public NATGatewaySettings NATGateways
        {
            get { return mNATGateways; }
        }

        /// <summary>
        /// Gets the TCP servers.
        /// </summary>
        /// <value>
        /// The TCP servers.
        /// </value>
        [DebuggerHidden]
        public TCPServerSettings TCPServers
        {
            get { return mTCPServerSettings; }
        }

        /// <summary>
        /// Gets the TCP connections.
        /// </summary>
        /// <value>
        /// The TCP connections.
        /// </value>
        [DebuggerHidden]
        public TCPConnectionSettings TCPConnections
        {
            get { return mTCPConnections; }
        }

        /// <summary>
        /// Gets the UDP detection.
        /// </summary>
        /// <value>
        /// The UDP detection.
        /// </value>
        [DebuggerHidden]
        public UDPDetectionSettings UDPDetection
        {
            get { return mUDPDetection; }
        }

        /// <summary>
        /// Gets the NATU pn P settings.
        /// </summary>
        /// <value>
        /// The NATU pn P settings.
        /// </value>
        [DebuggerHidden]
        public NATUPnPSettings NATUPnPSettings
        {
            get { return mNATUPnPSettings; }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Logs the configuration.
        /// </summary>
        internal void LogConfiguration()
        {
            mNATGateways.LogConfiguration();
            mTCPServerSettings.LogConfiguration();
            mTCPConnections.LogConfiguration();
            mUDPDetection.LogConfiguration();
            mNATUPnPSettings.LogConfiguration();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        internal void Initialize()
        {
            mNATGateways.Initialize();
            mTCPServerSettings.Initialize();
            mTCPConnections.Initialize();
            mUDPDetection.Initialize();
            mNATUPnPSettings.Initialize();
        }

        /// <summary>Cleans up.</summary>
        internal void CleanUp()
        {
            mNATGateways.CleanUp();
            mTCPServerSettings.CleanUp();
            mTCPConnections.CleanUp();
            mUDPDetection.CleanUp();
            mNATUPnPSettings.CleanUp();
        }

        #endregion

    }

}
