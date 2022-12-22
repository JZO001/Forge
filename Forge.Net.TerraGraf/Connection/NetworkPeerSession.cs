/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Forge.Legacy;
using Forge.Net.TerraGraf.NetworkPeers;
using Forge.Shared;

namespace Forge.Net.TerraGraf.Connection
{

    /// <summary>
    /// Represents a network peer session
    /// </summary>
    internal class NetworkPeerSession : MBRBase
    {

        #region Field(s)

        /// <summary>
        /// A távoli számítógép, akihez ez a session tartozik
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkPeerRemote mRemotePeer = null;

        /// <summary>
        /// Ez az átjáró a célszámítógéphez. Ez változhat és a túloldalon lehet másvalaki van, aki majd továbbítja az üzenetet.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkConnection mNetworkConnection = null;

        /// <summary>
        /// Tárolja a saját tulajdonú közvetlen hálózati kapcsolatokat
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<NetworkConnection> mActiveConnections = new List<NetworkConnection>();

        /// <summary>
        /// A válaszidő mértéke. Ez az érték nem a fizikai átjáró válaszideje, hanem a grafban való teljes utazás várható ideje.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mReplyTime = 0;

        /// <summary>
        /// Tárolja azokat az üzenetkódokat, amiket ez a peer már ide küldött és hogy hányszor
        /// </summary>
        private readonly Dictionary<long, int> mMessageIDs = new Dictionary<long, int>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPeerSession"/> class.
        /// </summary>
        /// <param name="remotePeer">The remote peer.</param>
        internal NetworkPeerSession(NetworkPeerRemote remotePeer)
        {
            if (remotePeer == null)
            {
                ThrowHelper.ThrowArgumentNullException("remotePeer");
            }
            mRemotePeer = remotePeer;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the remote peer.
        /// </summary>
        /// <value>
        /// The remote peer.
        /// </value>
        [DebuggerHidden]
        internal NetworkPeerRemote RemotePeer
        {
            get { return mRemotePeer; }
        }

        /// <summary>
        /// Gets or sets the network connection.
        /// </summary>
        /// <value>
        /// The network connection.
        /// </value>
        internal NetworkConnection NetworkConnection
        {
            get { return mNetworkConnection; }
            set
            {
                if (value != null)
                {
                    if (mReplyTime < value.ReplyTime)
                    {
                        mReplyTime = value.ReplyTime;
                    }
                }
                mNetworkConnection = value;
            }
        }

        /// <summary>
        /// Gets or sets the reply time.
        /// </summary>
        /// <value>
        /// The reply time.
        /// </value>
        [DebuggerHidden]
        internal long ReplyTime
        {
            get { return mReplyTime; }
            set { mReplyTime = value; }
        }

        /// <summary>
        /// Gets the active connections.
        /// </summary>
        /// <value>
        /// The active connections.
        /// </value>
        [DebuggerHidden]
        internal List<NetworkConnection> ActiveConnections
        {
            get { return mActiveConnections; }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Adds the network connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        internal void AddNetworkConnection(NetworkConnection connection)
        {
            lock (mActiveConnections)
            {
                if (!mActiveConnections.Contains<NetworkConnection>(connection))
                {
                    connection.OwnerSession = this;
                    mActiveConnections.Add(connection);
                }
            }
        }

        /// <summary>
        /// Removes the network connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        internal void RemoveNetworkConnection(NetworkConnection connection)
        {
            lock (mActiveConnections)
            {
                mActiveConnections.Remove(connection);
            }
        }

        /// <summary>
        /// Gets the message ID counter.
        /// </summary>
        /// <param name="messageId">The message id.</param>
        /// <returns></returns>
        internal int GetMessageIDCounter(long messageId)
        {
            int result = 0;
            lock (mMessageIDs)
            {
                if (mMessageIDs.ContainsKey(messageId))
                {
                    result = mMessageIDs[messageId];
                }
            }
            return result;
        }

        /// <summary>
        /// Incs the message ID counter.
        /// </summary>
        /// <param name="messageId">The message id.</param>
        internal void IncMessageIDCounter(long messageId)
        {
            lock (mMessageIDs)
            {
                if (mMessageIDs.ContainsKey(messageId))
                {
                    Int32 result = mMessageIDs[messageId];
                    result++;
                    mMessageIDs[messageId] = result;
                }
                else
                {
                    mMessageIDs[messageId] = 1;
                }
            }
        }

        /// <summary>
        /// Merges the message ID counters.
        /// </summary>
        /// <param name="dictionary">The d.</param>
        internal void MergeMessageIDCounters(Dictionary<long, int> dictionary)
        {
            lock (mMessageIDs)
            {
                foreach (KeyValuePair<long, int> kv in dictionary)
                {
                    mMessageIDs[kv.Key] = kv.Value;
                }
                dictionary.Clear();
            }
        }

        #endregion

    }

}
