/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;
using Forge.Net.Synapse;
using Forge.Net.TerraGraf.NetworkInfo;
using Forge.Net.TerraGraf.NetworkPeers;
using Forge.Shared;

namespace Forge.Net.TerraGraf.Contexts
{

    /// <summary>
    /// Represents the information about network peers
    /// </summary>
    public static class NetworkPeerContext
    {

        #region Public method(s)

        /// <summary>
        /// Gets the known network peers EXCEPT ME. Localhost not listed in this collection.
        /// </summary>
        /// <value>
        /// The known network peers.
        /// </value>
        public static ICollection<INetworkPeerRemote> KnownNetworkPeers
        {
            get
            {
                NetworkManager.Instance.DoInitializationCheck();
                return InternalKnownNetworkPeers;
            }
        }

        /// <summary>
        /// Gets the network peer by id. Search in global list.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The Network Peer Remote</returns>
        public static INetworkPeerRemote GetNetworkPeerById(string id)
        {
            NetworkManager.Instance.DoInitializationCheck();
            if (string.IsNullOrEmpty(id))
            {
                ThrowHelper.ThrowArgumentNullException("id");
            }

            INetworkPeerRemote result = null;

            foreach (INetworkPeerRemote peer in KnownNetworkPeers)
            {
                if (peer.Id.Equals(id))
                {
                    if (result == null)
                    {
                        result = peer;
                        if (peer.NetworkContext.Name.Equals(NetworkManager.Instance.InternalCurrentNetworkContext.Name))
                        {
                            break;
                        }
                    }
                    else if (peer.NetworkContext.Name.Equals(NetworkManager.Instance.InternalCurrentNetworkContext.Name))
                    {
                        result = peer;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the name of the network peer by. Search in global list.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The Network Peer Remote</returns>
        public static INetworkPeerRemote GetNetworkPeerByName(string name)
        {
            NetworkManager.Instance.DoInitializationCheck();
            if (string.IsNullOrEmpty(name))
            {
                ThrowHelper.ThrowArgumentNullException("name");
            }

            INetworkPeerRemote result = null;

            foreach (INetworkPeerRemote peer in KnownNetworkPeers)
            {
                if (peer.HostName.Equals(name))
                {
                    if (result == null)
                    {
                        result = peer;
                        if (peer.NetworkContext.Name.Equals(NetworkManager.Instance.InternalCurrentNetworkContext.Name))
                        {
                            break;
                        }
                    }
                    else if (peer.NetworkContext.Name.Equals(NetworkManager.Instance.InternalCurrentNetworkContext.Name))
                    {
                        result = peer;
                        break;
                    }
                }
            }

            return result;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the known network peers EXCEPT ME. Localhost not listed in this collection.
        /// </summary>
        /// <value>
        /// The known network peers.
        /// </value>
        internal static ICollection<INetworkPeerRemote> InternalKnownNetworkPeers
        {
            get
            {
                List<INetworkPeerRemote> result = new List<INetworkPeerRemote>();
                foreach (NetworkContext nc in NetworkContext.KnownNetworkContexts)
                {
                    result.AddRange(nc.KnownNetworkPeers);
                }
                result.Sort();
                return result;
            }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Creates the network peer.
        /// </summary>
        /// <param name="peer">The peer.</param>
        /// <returns></returns>
        internal static NetworkPeerRemote CreateNetworkPeer(NetworkPeer peer)
        {
            if (peer == null)
            {
                ThrowHelper.ThrowArgumentNullException("peer");
            }

            NetworkContext networkContext = NetworkContext.GetNetworkContextByName(peer.NetworkContext);
            if (networkContext == null)
            {
                networkContext = NetworkContext.CreateNetworkContext(peer.NetworkContext);
            }
            NetworkPeerRemote remotePeer = new NetworkPeerRemote();
            remotePeer.HostName = peer.HostName;
            remotePeer.Id = peer.Id;
            remotePeer.NetworkContext = networkContext;
            remotePeer.PeerType = PeerTypeEnum.Remote;
            RefreshNetworkPeerWithoutRelations(peer, remotePeer);
            List<INetworkPeerRemote> list = networkContext.InternalNetworkPeers;
            lock (list)
            {
                list.Add(remotePeer);
                list.Sort();
            }
            return remotePeer;
        }

        /// <summary>
        /// Refreshes the network peer.
        /// </summary>
        /// <param name="sourcePeer">The source peer.</param>
        /// <param name="targetPeer">The target peer.</param>
        internal static void RefreshNetworkPeerWithoutRelations(NetworkPeer sourcePeer, NetworkPeerRemote targetPeer)
        {
            if (sourcePeer == null)
            {
                ThrowHelper.ThrowArgumentNullException("sourcePeer");
            }
            if (targetPeer == null)
            {
                ThrowHelper.ThrowArgumentNullException("targetPeer");
            }

            if (sourcePeer.BlackHoleContainer.StateId > targetPeer.BlackHoleContainer.StateId)
            {
                targetPeer.BlackHoleContainer.StateId = targetPeer.BlackHoleContainer.StateId;
                targetPeer.BlackHoleContainer.IsBlackHole = targetPeer.BlackHoleContainer.IsBlackHole;
            }
            if (sourcePeer.NATGateways.StateId > targetPeer.NATGatewayCollection.StateId)
            {
                targetPeer.NATGatewayCollection.StateId = sourcePeer.NATGateways.StateId;
                targetPeer.NATGatewayCollection.NATGateways.Clear();
                if (sourcePeer.NATGateways.Gateways != null)
                {
                    List<NATGateway> backup = new List<NATGateway>(targetPeer.NATGatewayCollection.NATGateways);
                    targetPeer.NATGatewayCollection.NATGateways.Clear();
                    foreach (AddressEndPoint ep in sourcePeer.NATGateways.Gateways)
                    {
                        NATGateway gw = new NATGateway(ep);
                        bool found = false;
                        foreach (NATGateway g in backup)
                        {
                            if (g.Equals(gw))
                            {
                                targetPeer.NATGatewayCollection.NATGateways.Add(g);
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            targetPeer.NATGatewayCollection.NATGateways.Add(gw);
                        }
                    }
                }
                else
                {
                    targetPeer.NATGatewayCollection.NATGateways.Clear();
                }
            }
            if (sourcePeer.PeerContext.StateId > targetPeer.InternalPeerContext.StateId)
            {
                targetPeer.InternalPeerContext.StateId = sourcePeer.PeerContext.StateId;
                targetPeer.InternalPeerContext.PeerContext = sourcePeer.PeerContext.PeerContext;
            }
            if (sourcePeer.TCPServers.StateId > targetPeer.TCPServerCollection.StateId)
            {
                targetPeer.TCPServerCollection.StateId = sourcePeer.TCPServers.StateId;
                if (sourcePeer.TCPServers.Servers != null)
                {
                    List<TCPServer> backup = new List<TCPServer>(targetPeer.TCPServerCollection.TCPServers);
                    targetPeer.TCPServerCollection.TCPServers.Clear();
                    foreach (AddressEndPoint ep in sourcePeer.TCPServers.Servers)
                    {
                        TCPServer sw = new TCPServer(ep);
                        bool found = false;
                        foreach (TCPServer s in backup)
                        {
                            if (s.Equals(sw))
                            {
                                targetPeer.TCPServerCollection.TCPServers.Add(s);
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            targetPeer.TCPServerCollection.TCPServers.Add(sw);
                        }
                    }
                }
                else
                {
                    targetPeer.TCPServerCollection.TCPServers.Clear();
                }
            }
            targetPeer.Version = sourcePeer.Version;
        }

        /// <summary>
        /// Refreshes the network peer without relations.
        /// </summary>
        /// <param name="sourcePeer">The source peer.</param>
        /// <param name="targetPeer">The target peer.</param>
        internal static void RefreshNetworkPeerOnlyRelations(NetworkPeer sourcePeer, NetworkPeerRemote targetPeer)
        {
            if (sourcePeer == null)
            {
                ThrowHelper.ThrowArgumentNullException("sourcePeer");
            }
            if (targetPeer == null)
            {
                ThrowHelper.ThrowArgumentNullException("targetPeer");
            }

            //if (sourcePeer.PeerRelations.StateId > targetPeer.PeerRelationPairs.StateId)
            {
                //targetPeer.PeerRelationPairs.StateId = sourcePeer.PeerRelations.StateId;
                //targetPeer.PeerRelationPairs.PeerRelationPairs.Clear();
                if (sourcePeer.PeerRelations.PeerRelations != null)
                {
                    foreach (PeerRelation realation in sourcePeer.PeerRelations.PeerRelations)
                    {
                        targetPeer.PeerRelationPairs.AddOrUpdatePeerRelationForce(
                            targetPeer,
                            NetworkManager.Instance.InternalLocalhost.Id.Equals(realation.PeerB) ? NetworkManager.Instance.InternalLocalhost : (NetworkPeerRemote)NetworkPeerContext.GetNetworkPeerById(realation.PeerB), realation.Connected, realation.StateId);
                    }
                }
            }
        }

        #endregion

    }

}
