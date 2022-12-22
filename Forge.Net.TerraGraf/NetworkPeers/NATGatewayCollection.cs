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
                    // first I check, which server has not been tried
                    result = GetNeverTriedServer();
                    if (result == null)
                    {
                        // search the server, which has already a successfully connection to
                        result = GetGoodServer();
                        if (result == null)
                        {
                            // return the server which has been tried the less number
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
        /// Return the first gateway from the list, which has been never been tried
        /// </summary>
        /// <returns>
        /// The selected gateway, or null, in the case, if I has already tried all of them at least once
        /// </returns>
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
        /// Returns a getaway, which already has a successfully connection to
        /// </summary>
        /// <returns>
        /// The selected gateway, or null, if it does not find an instance with a previously successful connection
        /// </returns>
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
        /// Returns the gateway which has been tried less times.
        /// If each one has been tried on the same numbers/times, the result will be null.
        /// </summary>
        /// <returns>
        /// Selected gateway or null
        /// </returns>
        private NATGateway GetLeastTriedServer()
        {
            NATGateway result = null;
            // first check, if every gateway has been checked at the same numbers/times
            bool allEqv = true;
            int tried = mNATGateways[0].Attempts;
            foreach (NATGateway sc in mNATGateways)
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
                tried = mNATGateways[0].Attempts;
                result = mNATGateways[0];
                mNATGateways.RemoveAt(0);
                foreach (NATGateway sc in mNATGateways)
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
