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
    /// Represents the collection of NAT gateways
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, StateId = '{StateId}']")]
    internal sealed class NATGatewayCollection : MBRBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<NATGateway> mNATGateways = new List<NATGateway>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mStateId = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NATGatewayCollection"/> class.
        /// </summary>
        internal NATGatewayCollection()
        {
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the NAT gateways. Lock it, if you want to modify the collection.
        /// </summary>
        /// <value>
        /// The NAT gateways.
        /// </value>
        [DebuggerHidden]
        internal List<NATGateway> NATGateways
        {
            get { return mNATGateways; }
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
        /// <returns>NATGatewayContainer</returns>
        internal NATGatewayContainer BuildNATGatewayContainer()
        {
            NATGatewayContainer container = new NATGatewayContainer();

            container.StateId = mStateId;
            container.Gateways = new AddressEndPoint[mNATGateways.Count];
            for (int i = 0; i < mNATGateways.Count; i++)
            {
                container.Gateways[i] = mNATGateways[i].EndPoint;
            }

            return container;
        }

        /// <summary>
        /// Selects the NAT gate way.
        /// </summary>
        /// <returns>NATGateway</returns>
        internal NATGateway SelectNATGateWay()
        {
            NATGateway result = null;
            lock (mNATGateways)
            {
                if (mNATGateways.Count > 0)
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
        private NATGateway GetNeverTriedServer()
        {
            NATGateway result = null;
            foreach (NATGateway sc in mNATGateways)
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
        private NATGateway GetGoodServer()
        {
            NATGateway result = null;
            foreach (NATGateway sc in mNATGateways)
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
        private NATGateway GetLeastTriedServer()
        {
            NATGateway result = null;
            // először megnézzük, hogy mindegyik gateway-t azonos mennyiségben próbáltuk e már
            bool allEqv = true;
            int tried = mNATGateways[0].Attempts;
            foreach (NATGateway sc in mNATGateways)
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
                tried = mNATGateways[0].Attempts;
                result = mNATGateways[0];
                mNATGateways.RemoveAt(0);
                foreach (NATGateway sc in mNATGateways)
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
