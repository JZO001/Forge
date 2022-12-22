/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.Net.TerraGraf.Contexts;
using Forge.Threading;

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represents the localhost
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    [DebuggerDisplay("[{GetType().Name}, Id = '{Id}', HostName = '{HostName}']")]
    internal sealed class NetworkPeerLocal : NetworkPeerRemote, INetworkPeerLocal
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly PeerContextLock mPeerContextLock = new PeerContextLock("TerraGraf_Localhost_PeerContextLock");

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPeerLocal"/> class.
        /// </summary>
        internal NetworkPeerLocal()
        {
            PeerType = PeerTypeEnum.Local;
        }

        #endregion

        #region Public properties(s)

        /// <summary>
        /// Gets the peer context lock.
        /// </summary>
        /// <value>
        /// The peer context lock.
        /// </value>
        public ILock PeerContextLock
        {
            get { return mPeerContextLock; }
        }

        /// <summary>
        /// Gets the peer context.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Peer context data must have been locked before you get, modify and set it.</exception>
        public override NetworkPeerDataContext PeerContext
        {
            get
            {
                NetworkPeerDataContext result = null;

                if (mPeerContextLock.IsHeldByCurrentThread)
                {
                    result = mPeerContext.PeerContext;
                }
                else
                {
                    result = base.PeerContext;
                }

                return result;
            }
            set
            {
                if (mPeerContextLock.IsHeldByCurrentThread)
                {
                    NetworkManager.Instance.InternalSendLocalhostPeerContext(value);
                    NetworkManager.Instance.OnNetworkPeerContextChanged(this, true);
                }
                else
                {
                    throw new InvalidOperationException("Peer context data must have been locked before you get, modify and set it.");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is black hole.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is black hole; otherwise, <c>false</c>.
        /// </value>
        public bool IsBlackHole
        {
            get { return NetworkManager.Instance.InternalConfiguration.Settings.BlackHole; }
            set { NetworkManager.Instance.InternalConfiguration.Settings.BlackHole = value; }
        }

        /// <summary>
        /// Gets the TCP servers.
        /// </summary>
        /// <value>
        /// The TCP servers.
        /// </value>
        public ICollection<TCPServer> TCPServers
        {
            get
            {
                lock (NetworkManager.Instance.InternalLocalhost.TCPServerCollection.TCPServers)
                {
                    return new List<TCPServer>(NetworkManager.Instance.InternalLocalhost.TCPServerCollection.TCPServers);
                }
            }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        internal void Initialize()
        {
            mPeerContext.StateId = DateTime.UtcNow.Ticks;
        }

        #endregion

    }

}
