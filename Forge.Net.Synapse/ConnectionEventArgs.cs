/* *********************************************************************
 * Date: 07 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Diagnostics;
using System.Net;

namespace Forge.Net.Synapse
{

    /// <summary>
    /// Represent the arguments of a new network connection event
    /// </summary>
    public class ConnectionEventArgs : EventArgs
    {

        #region Field(s)

        private long mServerId = 0;

        private EndPoint mLocalEndPoint = null;

        private NetworkStream mNetworkStream = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionEventArgs"/> class.
        /// </summary>
        /// <param name="serverId">The server id.</param>
        /// <param name="endPoint">The end point.</param>
        /// <param name="stream">The stream.</param>
        public ConnectionEventArgs(long serverId, EndPoint endPoint, NetworkStream stream)
        {
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }
            this.mServerId = serverId;
            this.mLocalEndPoint = endPoint;
            this.mNetworkStream = stream;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the server id.
        /// </summary>
        /// <value>
        /// The server id.
        /// </value>
        [DebuggerHidden]
        public long ServerId
        {
            get { return mServerId; }
        }

        /// <summary>Gets the local end point.</summary>
        /// <value>The local end point.</value>
        [DebuggerHidden]
        public EndPoint LocalEndPoint
        {
            get { return mLocalEndPoint; }
        }

        /// <summary>
        /// Gets the network stream.
        /// </summary>
        /// <value>
        /// The network stream.
        /// </value>
        [DebuggerHidden]
        public NetworkStream NetworkStream
        {
            get { return mNetworkStream; }
        }

        #endregion

    }

}
