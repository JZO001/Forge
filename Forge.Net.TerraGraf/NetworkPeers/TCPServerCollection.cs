/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.Legacy;
using Forge.Net.Synapse;
using Forge.Net.TerraGraf.NetworkInfo;

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represents a TCPServer collection
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, StateId = '{StateId}']")]
    internal sealed class TCPServerCollection : MBRBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<TCPServer> mTCPServers = new List<TCPServer>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mStateId = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPServerCollection"/> class.
        /// </summary>
        internal TCPServerCollection()
        {
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the TCP servers. Lock it, if you want to modify the collection.
        /// </summary>
        /// <value>
        /// The TCP servers.
        /// </value>
        [DebuggerHidden]
        internal List<TCPServer> TCPServers
        {
            get { return mTCPServers; }
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
        /// Builds the server container.
        /// </summary>
        /// <returns>ServerContainer</returns>
        internal ServerContainer BuildServerContainer()
        {
            ServerContainer container = new ServerContainer();

            container.StateId = mStateId;
            container.Servers = new AddressEndPoint[mTCPServers.Count];
            for (int i = 0; i < mTCPServers.Count; i++)
            {
                container.Servers[i] = mTCPServers[i].EndPoint;
            }

            return container;
        }

        /// <summary>
        /// Selects the NAT gate way.
        /// </summary>
        /// <returns>TCPServer</returns>
        internal TCPServer SelectTCPServer()
        {
            TCPServer result = null;
            lock (mTCPServers)
            {
                if (mTCPServers.Count > 0)
                {
                    // first check, which server has not been tried
                    result = GetNeverTriedServer();
                    if (result == null)
                    {
                        // find the server, where already was a successful connection
                        result = GetGoodServer();
                        if (result == null)
                        {
                            // returns the server, which has been tired less numbers/times
                            result = GetLeastTriedServer();
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Return the first server from the list, which has been never been tried
        /// </summary>
        /// <returns>
        /// The selected server, or null, in the case, if I has already tried all of them at least once
        /// </returns>
        private TCPServer GetNeverTriedServer()
        {
            TCPServer result = null;
            foreach (TCPServer sc in mTCPServers)
            {
                if (sc.Attempts == 0)
                {
                    result = sc;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a server, which already has a successfully connection to
        /// </summary>
        /// <returns>
        /// The selected server, or null, if it does not find an instance with a previously successful connection
        /// </returns>
        private TCPServer GetGoodServer()
        {
            TCPServer result = null;
            foreach (TCPServer sc in mTCPServers)
            {
                if (sc.Success)
                {
                    result = sc;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the server which has been tried less times.
        /// If each one has been tried on the same numbers/times, the result will be null.
        /// </summary>
        /// <returns>
        /// Selected server or null
        /// </returns>
        private TCPServer GetLeastTriedServer()
        {
            TCPServer result = null;
            // first check, if every server has been checked at the same numbers/times
            bool allEqv = true;
            int tried = mTCPServers[0].Attempts;
            foreach (TCPServer sc in mTCPServers)
            {
                if (tried != sc.Attempts)
                {
                    // they have been tried not the same numbers/times
                    allEqv = false;
                    break;
                }
            }
            if (!allEqv)
            {
                // no, so lets find that, which has been tried less numbers/times
                tried = mTCPServers[0].Attempts;
                result = mTCPServers[0];
                mTCPServers.RemoveAt(0);
                foreach (TCPServer sc in mTCPServers)
                {
                    if (tried < sc.Attempts)
                    {
                        // it has been used less numbers/times
                        result = sc;
                        tried = sc.Attempts;
                    }
                }
            }
            return result;
        }

        #endregion

    }

}
