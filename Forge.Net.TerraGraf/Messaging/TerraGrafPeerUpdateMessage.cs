/* *********************************************************************
 * Date: 21 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Forge.Net.TerraGraf.NetworkInfo;
using Forge.Shared;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Represents an unpdate information message of a network peer
    /// </summary>
    [Serializable]
    internal sealed class TerraGrafPeerUpdateMessage : TerraGrafMessageBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NATGatewayContainer mNATGatewayContainer = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PeerContextContainer mPeerContextContainer = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ServerContainer mServerContainer = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafPeerUpdateMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="natGatewayContainer">The nat gateway container.</param>
        internal TerraGrafPeerUpdateMessage(string senderId, NATGatewayContainer natGatewayContainer)
            : base(senderId, string.Empty, MessageCodeEnum.TerraGrafPeerUpdate, Interlocked.Increment(ref mGlobalMessageId),
            MessagePriorityEnum.Normal, MessageTypeEnum.Udp)
        {
            if (natGatewayContainer == null)
            {
                ThrowHelper.ThrowArgumentNullException("natGatewayContainer");
            }
            mNATGatewayContainer = natGatewayContainer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafPeerUpdateMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="peerContextContainer">The peer context container.</param>
        internal TerraGrafPeerUpdateMessage(string senderId, PeerContextContainer peerContextContainer)
            : base(senderId, string.Empty, MessageCodeEnum.TerraGrafPeerUpdate, Interlocked.Increment(ref mGlobalMessageId),
            MessagePriorityEnum.Normal, MessageTypeEnum.Udp)
        {
            if (peerContextContainer == null)
            {
                ThrowHelper.ThrowArgumentNullException("peerContextContainer");
            }
            mPeerContextContainer = peerContextContainer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafPeerUpdateMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="serverContainer">The server container.</param>
        internal TerraGrafPeerUpdateMessage(string senderId, ServerContainer serverContainer)
            : base(senderId, string.Empty, MessageCodeEnum.TerraGrafPeerUpdate, Interlocked.Increment(ref mGlobalMessageId),
            MessagePriorityEnum.Normal, MessageTypeEnum.Udp)
        {
            if (serverContainer == null)
            {
                ThrowHelper.ThrowArgumentNullException("serverContainer");
            }
            mServerContainer = serverContainer;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the NAT gateway container.
        /// </summary>
        /// <value>
        /// The NAT gateway container.
        /// </value>
        [DebuggerHidden]
        internal NATGatewayContainer NATGatewayContainer
        {
            get { return mNATGatewayContainer; }
        }

        /// <summary>
        /// Gets the peer context container.
        /// </summary>
        /// <value>
        /// The peer context container.
        /// </value>
        [DebuggerHidden]
        internal PeerContextContainer PeerContextContainer
        {
            get { return mPeerContextContainer; }
        }

        /// <summary>
        /// Gets the server container.
        /// </summary>
        /// <value>
        /// The server container.
        /// </value>
        [DebuggerHidden]
        internal ServerContainer ServerContainer
        {
            get { return mServerContainer; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append(", Contains: ");
            if (mNATGatewayContainer != null)
            {
                sb.Append("NATGateway;");
            }
            if (mPeerContextContainer != null)
            {
                sb.Append("PeerContext;");
            }
            if (mServerContainer != null)
            {
                sb.Append("Server;");
            }

            return sb.ToString();
        }

        #endregion

    }

}
