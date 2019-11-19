/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Forge.Net.Synapse;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Represents a message which broadcasted into the network while searching other network peers
    /// </summary>
    [Serializable]
    internal class UdpBroadcastMessage : MessageBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mNetworkContextName = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AddressEndPoint[] mNATGateways = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AddressEndPoint[] mTCPServers = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpBroadcastMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="networkContextName">Name of the network context.</param>
        /// <param name="natGateways">The nat gateways.</param>
        /// <param name="tcpServers">The TCP servers.</param>
        internal UdpBroadcastMessage(string senderId, string networkContextName, AddressEndPoint[] natGateways, AddressEndPoint[] tcpServers)
            : base(senderId, Interlocked.Increment(ref mGlobalMessageId), MessageCodeEnum.UdpBroadcast)
        {
            if (string.IsNullOrEmpty(networkContextName))
            {
                ThrowHelper.ThrowArgumentNullException("networkContextName");
            }
            this.mNetworkContextName = networkContextName;
            this.mNATGateways = natGateways;
            this.mTCPServers = tcpServers;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the name of the network context.
        /// </summary>
        /// <value>
        /// The name of the network context.
        /// </value>
        [DebuggerHidden]
        internal string NetworkContextName
        {
            get { return mNetworkContextName; }
        }

        /// <summary>
        /// Gets the NAT gateways.
        /// </summary>
        /// <value>
        /// The NAT gateways.
        /// </value>
        [DebuggerHidden]
        internal AddressEndPoint[] NATGateways
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
        internal AddressEndPoint[] TCPServers
        {
            get { return mTCPServers; }
        }

        #endregion

    }

}
