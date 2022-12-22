/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.Legacy;
using Forge.Logging.Abstraction;
using Forge.Net.TerraGraf.NetworkInfo;

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represents the network peer pair collection
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, StateId = '{StateId}']")]
    internal sealed class PeerRelationPairCollection : MBRBase
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<PeerRelationPairCollection>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<PeerRelationPair> mPeerRelationPairs = new List<PeerRelationPair>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mStateId = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PeerRelationPairCollection"/> class.
        /// </summary>
        internal PeerRelationPairCollection()
        {
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the peer relation pairs.
        /// </summary>
        /// <value>
        /// The peer relation pairs.
        /// </value>
        [DebuggerHidden]
        internal List<PeerRelationPair> PeerRelationPairs
        {
            get { return mPeerRelationPairs; }
        }

        /// <summary>
        /// Gets or sets the state id.
        /// </summary>
        /// <value>
        /// The state id.
        /// </value>
        [DebuggerHidden]
        internal long StateId
        {
            get { return mStateId; }
            set { mStateId = value; }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Builds the peer relation container.
        /// </summary>
        /// <returns>PeerRelationContainer</returns>
        internal PeerRelationContainer BuildPeerRelationContainer(bool blackHole, string targetNetworkContext)
        {
            PeerRelationContainer container = new PeerRelationContainer();

            container.StateId = mStateId;
            if (!blackHole)
            {
                List<PeerRelationPair> allowedPairs = new List<PeerRelationPair>();
                foreach (PeerRelationPair pair in mPeerRelationPairs)
                {
                    if (NetworkManager.Instance.NetworkContextRuleManager.CheckSeparation(pair.PeerA.NetworkContext.Name, targetNetworkContext) &&
                        NetworkManager.Instance.NetworkContextRuleManager.CheckSeparation(pair.PeerB.NetworkContext.Name, targetNetworkContext))
                    {
                        allowedPairs.Add(pair);
                    }
                }

                if (allowedPairs.Count > 0)
                {
                    container.PeerRelations = new PeerRelation[allowedPairs.Count];
                    for (int i = 0; i < allowedPairs.Count; i++)
                    {
                        PeerRelationPair pair = allowedPairs[i];
                        container.PeerRelations[i] = new PeerRelation(pair.StateId, pair.PeerA.Id, pair.PeerB.Id, pair.Connected);
                    }
                }
            }

            return container;
        }

        /// <summary>
        /// Adds the or update peer relation.
        /// </summary>
        /// <param name="ownerPeer">The owner peer.</param>
        /// <param name="pairPeer">The pair peer.</param>
        /// <param name="connected">if set to <c>true</c> [connected].</param>
        /// <param name="changed">if set to <c>true</c> [changed].</param>
        /// <returns>PeerRelationPair</returns>
        internal PeerRelationPair AddOrUpdatePeerRelation(NetworkPeerRemote ownerPeer, NetworkPeerRemote pairPeer, bool connected, out bool changed)
        {
            changed = false;
            PeerRelationPair result = null;

            foreach (PeerRelationPair pair in mPeerRelationPairs)
            {
                if (pair.PeerA.Equals(ownerPeer) && pair.PeerB.Equals(pairPeer))
                {
                    if (pair.Connected != connected)
                    {
                        pair.Connected = connected;
                        pair.StateId = pair.StateId + 1;
                        changed = true;
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("PEER_RELATION, [{0}] relation changed with [{1}]. StateId: {2}, State: {3}", ownerPeer.Id, pairPeer.Id, pair.StateId, pair.Connected));
                    }
                    else
                    {
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("PEER_RELATION, [{0}] relation NOT changed with [{1}]. StateId: {2}, State: {3}", ownerPeer.Id, pairPeer.Id, pair.StateId, pair.Connected));
                    }
                    result = pair;
                    break;
                }
            }

            if (result == null)
            {
                result = new PeerRelationPair(ownerPeer, pairPeer) { Connected = connected };
                mPeerRelationPairs.Add(result);
                changed = true;
                if (NetworkManager.Instance.InternalLocalhost.Id.Equals(ownerPeer.Id))
                {
                    // modify only my identifier, not others!
                    mStateId++;
                    result.StateId = result.StateId + 1;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds the or update peer relation.
        /// </summary>
        /// <param name="ownerPeer">The owner peer.</param>
        /// <param name="pairPeer">The pair peer.</param>
        /// <param name="connected">if set to <c>true</c> [connected].</param>
        /// <param name="stateId">The state id.</param>
        internal void AddOrUpdatePeerRelationForce(NetworkPeerRemote ownerPeer, NetworkPeerRemote pairPeer, bool connected, long stateId)
        {
            PeerRelationPair _pair = null;

            foreach (PeerRelationPair pair in mPeerRelationPairs)
            {
                if (pair.PeerA.Equals(ownerPeer) && pair.PeerB.Equals(pairPeer))
                {
                    if (pair.StateId < stateId)
                    {
                        pair.Connected = connected;
                        pair.StateId = stateId;
                    }
                    _pair = pair;
                    break;
                }
            }

            if (_pair == null)
            {
                _pair = new PeerRelationPair(ownerPeer, pairPeer) { Connected = connected, StateId = stateId };
                mPeerRelationPairs.Add(_pair);
                //this.mStateId++;
            }
        }

        /// <summary>
        /// Sets the connection offline.
        /// </summary>
        /// <param name="ownerPeer">The owner peer.</param>
        /// <param name="pairPeer">The pair peer.</param>
        internal void SetConnectionOffline(NetworkPeerRemote ownerPeer, NetworkPeerRemote pairPeer)
        {
            foreach (PeerRelationPair pair in mPeerRelationPairs)
            {
                if (pair.PeerA.Equals(ownerPeer) && pair.PeerB.Equals(pairPeer))
                {
                    pair.Connected = false;
                    mPeerRelationPairs.Remove(pair);
                    break;
                }
            }
        }

        /// <summary>
        /// Sets all connection offline.
        /// </summary>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        internal void SetAllConnectionOffline(bool owner)
        {
            if (owner)
            {
                // increase the stateId at myself
                foreach (PeerRelationPair pair in mPeerRelationPairs)
                {
                    pair.Connected = false;
                    if (owner)
                    {
                        pair.StateId = pair.StateId + 1;
                    }
                }
            }
            else
            {
                // foreginers, I clear every connection descriptors, later they can re-set them, if the connection restored.
                mPeerRelationPairs.Clear();
            }
        }

        /// <summary>
        /// Gets the neighborhood.
        /// </summary>
        /// <value>
        /// The neighborhood.
        /// </value>
        internal ICollection<NetworkPeerRemote> Neighborhood
        {
            get
            {
                List<NetworkPeerRemote> result = new List<NetworkPeerRemote>();

                foreach (PeerRelationPair pair in mPeerRelationPairs)
                {
                    if (pair.Connected)
                    {
                        result.Add(pair.PeerB);
                    }
                }

                return result;
            }
        }

        #endregion

    }

}
