/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Legacy;
using System;
using System.Diagnostics;

namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>
    /// Configuration root
    /// </summary>
    [Serializable]
    public sealed class TerraGrafOptions : MBRBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Settings mSettings = new Settings();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkPeerSettings mNetworkPeering = new NetworkPeerSettings();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkContextSettings mNetworkContext = new NetworkContextSettings();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafOptions"/> class.
        /// </summary>
        public TerraGrafOptions()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        [DebuggerHidden]
        public Settings Settings
        {
            get { return mSettings; }
        }

        /// <summary>
        /// Gets the network peering.
        /// </summary>
        /// <value>
        /// The network peering.
        /// </value>
        [DebuggerHidden]
        public NetworkPeerSettings NetworkPeering
        {
            get { return mNetworkPeering; }
        }

        /// <summary>
        /// Gets the network context.
        /// </summary>
        /// <value>
        /// The network context.
        /// </value>
        [DebuggerHidden]
        public NetworkContextSettings NetworkContext
        {
            get { return mNetworkContext; }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        internal void Initialize()
        {
            mSettings.Initialize();
            mNetworkPeering.Initialize();
            mNetworkContext.Initialize();

            mSettings.LogConfiguration();
            mNetworkPeering.LogConfiguration();
            mNetworkContext.LogConfiguration();
        }

        /// <summary>Cleans up.</summary>
        internal void CleanUp()
        {
            mSettings.CleanUp();
            mNetworkPeering.CleanUp();
            mNetworkContext.CleanUp();
        }

        #endregion

    }

}
