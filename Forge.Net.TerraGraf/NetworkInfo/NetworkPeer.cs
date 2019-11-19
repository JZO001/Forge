/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;

namespace Forge.Net.TerraGraf.NetworkInfo
{

    /// <summary>
    /// Represent a network peer which sending on the network
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, Id = '{Id}', HostName = '{HostName}', NetworkContext = '{NetworkContext}']")]
    internal sealed class NetworkPeer
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPeer"/> class.
        /// </summary>
        internal NetworkPeer()
        {
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Unique identifier: AppId + ContextName + Hostname
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [DebuggerHidden]
        internal string Id { get; set; }

        /// <summary>
        /// Real host name of the computer
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        [DebuggerHidden]
        internal string HostName { get; set; }

        /// <summary>
        /// Network context name
        /// </summary>
        /// <value>
        /// The network context.
        /// </value>
        [DebuggerHidden]
        internal string NetworkContext { get; set; }

        /// <summary>
        /// Gets or sets the black hole container.
        /// </summary>
        /// <value>
        /// The black hole container.
        /// </value>
        [DebuggerHidden]
        internal BlackHoleContainer BlackHoleContainer { get; set; }

        /// <summary>
        /// Peer context
        /// </summary>
        /// <value>
        /// The peer context.
        /// </value>
        [DebuggerHidden]
        internal PeerContextContainer PeerContext { get; set; }

        /// <summary>
        /// Version of the TerraGraf
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DebuggerHidden]
        internal Version Version { get; set; }

        /// <summary>
        /// TCP szervereim
        /// </summary>
        /// <value>
        /// The TCP servers.
        /// </value>
        [DebuggerHidden]
        internal ServerContainer TCPServers { get; set; }

        /// <summary>
        /// NAT gateways
        /// </summary>
        /// <value>
        /// The NAT gateways.
        /// </value>
        [DebuggerHidden]
        internal NATGatewayContainer NATGateways { get; set; }

        /// <summary>
        /// List of direct connections
        /// </summary>
        /// <value>
        /// The peer relations.
        /// </value>
        [DebuggerHidden]
        internal PeerRelationContainer PeerRelations { get; set; }

        #endregion

    }

}
