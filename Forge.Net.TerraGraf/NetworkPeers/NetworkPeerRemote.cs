/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Forge.Net.TerraGraf.Connection;
using Forge.Net.TerraGraf.Contexts;
using Forge.Net.TerraGraf.NetworkInfo;

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represents a common remote peer
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, Id = '{Id}', HostName = '{HostName}', Distance = '{Distance}']")]
    internal class NetworkPeerRemote : MBRBase, INetworkPeerRemote, IComparable<NetworkPeerRemote>, IComparable, IEquatable<NetworkPeerRemote>
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BlackHoleContainer mBlackHoleContainer = new BlackHoleContainer();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected NetworkPeerDataContextContainer mPeerContext = new NetworkPeerDataContextContainer();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TCPServerCollection mTCPServers = new TCPServerCollection();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NATGatewayCollection mNATGateways = new NATGatewayCollection();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PeerRelationPairCollection mPeerRelationPairs = new PeerRelationPairCollection();

        #endregion

        #region Constructor(s)

        internal NetworkPeerRemote()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [DebuggerHidden]
        public string Id { get; set; }

        /// <summary>
        /// Gets the name of the host.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        [DebuggerHidden]
        public string HostName { get; set; }

        /// <summary>
        /// Gets the network context.
        /// </summary>
        [DebuggerHidden]
        public NetworkContext NetworkContext { get; set; }

        /// <summary>
        /// Gets the distance.
        /// </summary>
        /// <value>
        /// The distance.
        /// </value>
        [DebuggerHidden]
        public int Distance { get; set; }

        /// <summary>
        /// Gets the peer context.
        /// </summary>
        /// <value>
        /// The peer context.
        /// </value>
        /// <exception cref="System.NotSupportedException"></exception>
        public virtual NetworkPeerDataContext PeerContext
        {
            get
            {
                NetworkPeerDataContext context = mPeerContext.PeerContext;
                return context == null ? null : (NetworkPeerDataContext)context.Clone();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the type of the peer.
        /// </summary>
        /// <value>
        /// The type of the peer.
        /// </value>
        [DebuggerHidden]
        public PeerTypeEnum PeerType { get; set; }

        /// <summary>
        /// Gets the reply time.
        /// </summary>
        [DebuggerHidden]
        public long ReplyTime
        {
            get { return this.Session == null ? (long)Timeout.Infinite : this.Session.ReplyTime; }
        }

        /// <summary>
        /// Gets the active network connection.
        /// </summary>
        /// <value>
        /// The active network connection.
        /// </value>
        public INetworkConnection ActiveNetworkConnection
        {
            get
            {
                INetworkConnection result = null;
                if (Session != null)
                {
                    NetworkConnection c = Session.NetworkConnection;
                    if (c != null && c.IsConnected)
                    {
                        // csakis a saját tulajdonú, aktív hálózati kapcsolatot adom vissza
                        if (c.OwnerSession.RemotePeer.Id.Equals(this.Id))
                        {
                            result = c;
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the network connections.
        /// </summary>
        /// <value>
        /// The network connections.
        /// </value>
        public ICollection<INetworkConnection> NetworkConnections
        {
            get
            {
                List<INetworkConnection> result = new List<INetworkConnection>();

                if (Session != null)
                {
                    result.AddRange(Session.ActiveConnections);
                }

                return result;
            }
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DebuggerHidden]
        internal Version Version { get; set; }

        /// <summary>
        /// Gets the black hole container.
        /// </summary>
        [DebuggerHidden]
        internal BlackHoleContainer BlackHoleContainer
        {
            get { return mBlackHoleContainer; }
        }

        /// <summary>
        /// Gets the TCP servers.
        /// </summary>
        [DebuggerHidden]
        internal TCPServerCollection TCPServerCollection
        {
            get { return mTCPServers; }
        }

        /// <summary>
        /// Gets the NAT gateways.
        /// </summary>
        [DebuggerHidden]
        internal NATGatewayCollection NATGatewayCollection
        {
            get { return mNATGateways; }
        }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        [DebuggerHidden]
        internal NetworkPeerSession Session { get; set; }

        /// <summary>
        /// Gets the peer relation pairs.
        /// </summary>
        [DebuggerHidden]
        internal PeerRelationPairCollection PeerRelationPairs
        {
            get { return mPeerRelationPairs; }
        }

        /// <summary>
        /// Gets the peer context.
        /// </summary>
        [DebuggerHidden]
        internal NetworkPeerDataContextContainer InternalPeerContext
        {
            get { return mPeerContext; }
            set { mPeerContext = value; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public int CompareTo(NetworkPeerRemote other)
        {
            if (other == null)
            {
                ThrowHelper.ThrowArgumentNullException("other");
            }
            return this.Id.CompareTo(other.Id);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj" />. Zero This instance is equal to <paramref name="obj" />. Greater than zero This instance is greater than <paramref name="obj" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="obj" /> is not the same type as this instance.</exception>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                ThrowHelper.ThrowArgumentNullException("obj");
            }
            if (!(obj is NetworkPeerRemote))
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(obj, typeof(NetworkPeerRemote));
            }
            return CompareTo((NetworkPeerRemote)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            NetworkPeerRemote other = (NetworkPeerRemote)obj;
            return other.Id == Id;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True, if the other class is equals with this.</returns>
        public bool Equals(NetworkPeerRemote other)
        {
            return Equals((object)other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Builds the network peer.
        /// </summary>
        /// <returns>NetworkPeer</returns>
        internal NetworkPeer BuildNetworkPeer(bool blackHole, string targetNetworkContext)
        {
            NetworkPeer peer = new NetworkPeer();

            peer.HostName = this.HostName;
            peer.Id = this.Id;
            peer.BlackHoleContainer = this.BlackHoleContainer.BuildBlackHoleContainer();
            peer.NATGateways = this.NATGatewayCollection.BuildNATGatewayContainer();
            peer.NetworkContext = this.NetworkContext.Name;
            peer.PeerContext = this.InternalPeerContext.BuildPeerContextContainer();
            peer.PeerRelations = this.PeerRelationPairs.BuildPeerRelationContainer(blackHole, targetNetworkContext);
            peer.TCPServers = this.TCPServerCollection.BuildServerContainer();
            peer.Version = this.Version;

            return peer;
        }

        #endregion

    }

}
