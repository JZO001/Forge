/* *********************************************************************
 * Date: 09 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    // először megnézzük, melyik servert nem próbáltuk még
                    result = GetNeverTriedServer();
                    if (result == null)
                    {
                        // megkeressük azt a szervert, amin már volt sikeres kapcsolódás
                        result = GetGoodServer();
                        if (result == null)
                        {
                            // visszaadja azt a szervert, amit a legkevesebbszer próbáltunk eddig
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
        /// Visszaadja az első olyan gateway-t, amit még sosem próbáltunk
        /// </summary>
        /// <returns>Kiválasztott szerver vagy null, ha már mindent próbáltunk legalább 1x</returns>
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
        /// Visszaad egy gateway-t, amin már volt sikeres csatlakoztatás
        /// </summary>
        /// <returns>Kiválasztott gateway vagy null, ha nem talált sikeresen csatlakozott gateway-t</returns>
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
        /// Visszaadja azt a gateway-t, amit eddig a legkevesebbet próbáltunk.
        /// Ha mindegyiket egyforma mennyiségben próbáltuk, akkor null-t ad vissza.
        /// </summary>
        /// <returns>Kiválasztott gateway vagy null</returns>
        private TCPServer GetLeastTriedServer()
        {
            TCPServer result = null;
            // először megnézzük, hogy mindegyik gateway-t azonos mennyiségben próbáltuk e már
            bool allEqv = true;
            int tried = mTCPServers[0].Attempts;
            foreach (TCPServer sc in mTCPServers)
            {
                if (tried != sc.Attempts)
                {
                    // nem egyformán próbálkoztunk eddig
                    allEqv = false;
                    break;
                }
            }
            if (!allEqv)
            {
                // ha nem, akkor kikeressük azt, amelyiket a legkevesebbszer próbáltuk
                tried = mTCPServers[0].Attempts;
                result = mTCPServers[0];
                mTCPServers.RemoveAt(0);
                foreach (TCPServer sc in mTCPServers)
                {
                    if (tried < sc.Attempts)
                    {
                        // ezt kevesebbszer próbáltuk
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
