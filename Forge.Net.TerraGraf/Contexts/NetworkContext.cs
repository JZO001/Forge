/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.Legacy;
using Forge.Net.TerraGraf.NetworkPeers;
using Forge.Shared;

namespace Forge.Net.TerraGraf.Contexts
{

    /// <summary>
    /// Represent a network context
    /// </summary>
    [DebuggerDisplay("[{GetType().Name}, Name = '{Name}']")]
    public sealed class NetworkContext : MBRBase, IComparable<NetworkContext>, IComparable
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mName = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<INetworkPeerRemote> mNetworkPeers = new List<INetworkPeerRemote>(); // lockolni kell

        #endregion

        #region Constructor(s)

        internal NetworkContext()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DebuggerHidden]
        public string Name
        {
            get { return mName; }
            internal set { mName = value; }
        }

        /// <summary>
        /// Gets the known network contexts.
        /// </summary>
        /// <value>
        /// The known network contexts.
        /// </value>
        public static ICollection<NetworkContext> KnownNetworkContexts
        {
            get
            {
                List<NetworkContext> list = NetworkManager.Instance.InternalKnownNetworkContexts;
                lock (list)
                {
                    return new List<NetworkContext>(NetworkManager.Instance.InternalKnownNetworkContexts);
                }
            }
        }

        /// <summary>
        /// Gets the network peers which known in this network context.
        /// </summary>
        /// <value>
        /// The known network peers.
        /// </value>
        public ICollection<INetworkPeerRemote> KnownNetworkPeers
        {
            get
            {
                lock (mNetworkPeers)
                {
                    return new List<INetworkPeerRemote>(mNetworkPeers);
                }
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets the name of the network context by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The Network Context. Null if the name does not exist.</returns>
        public static NetworkContext GetNetworkContextByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                ThrowHelper.ThrowArgumentNullException("name");
            }

            NetworkContext result = null;
            foreach (NetworkContext nc in KnownNetworkContexts)
            {
                if (nc.Name.Equals(name))
                {
                    result = nc;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the network peer by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The value or null</returns>
        public INetworkPeerRemote GetNetworkPeerById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ThrowHelper.ThrowArgumentNullException("id");
            }

            id = id.ToLower().Trim();

            INetworkPeerRemote result = null;

            lock (mNetworkPeers)
            {
                foreach (INetworkPeerRemote peer in mNetworkPeers)
                {
                    if (peer.Id.ToLower().Equals(id))
                    {
                        result = peer;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the name of the network peer by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Network Peer implementation or null</returns>
        public INetworkPeerRemote GetNetworkPeerByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                ThrowHelper.ThrowArgumentNullException("name");
            }

            name = name.ToLower().Trim();

            INetworkPeerRemote result = null;

            lock (mNetworkPeers)
            {
                foreach (INetworkPeerRemote peer in mNetworkPeers)
                {
                    if (peer.HostName.ToLower().Equals(name))
                    {
                        result = peer;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>0 if equals, 1 - greater, -1 - lower</returns>
        public int CompareTo(NetworkContext other)
        {
            if (other == null)
            {
                ThrowHelper.ThrowArgumentNullException("other");
            }
            return mName.CompareTo(other.mName);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="obj"/> is not the same type as this instance. </exception>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                ThrowHelper.ThrowArgumentNullException("obj");
            }
            if (!(obj is NetworkContext))
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(obj, typeof(NetworkContext));
            }
            return CompareTo((NetworkContext)obj);
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Creates the network context.
        /// </summary>
        /// <param name="name">The name.</param>
        internal static NetworkContext CreateNetworkContext(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                ThrowHelper.ThrowArgumentNullException("name");
            }
            NetworkContext result = new NetworkContext() { mName = name };
            List<NetworkContext> list = NetworkManager.Instance.InternalKnownNetworkContexts;
            lock (list)
            {
                list.Add(result);
                list.Sort();
            }
            return result;
        }

        /// <summary>
        /// Aki elviszi, lockolja!
        /// </summary>
        internal List<INetworkPeerRemote> InternalNetworkPeers
        {
            get { return mNetworkPeers; }
        }

        #endregion

    }

}
