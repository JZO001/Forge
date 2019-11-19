/* *********************************************************************
 * Date: 18 Dec 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Forge.Configuration;
using Forge.Net.Synapse;
using Forge.Net.TerraGraf.NetworkPeers;

namespace Forge.Net.Services.Locators
{

    /// <summary>
    /// Represents a service provider descriptor
    /// </summary>
    [Serializable]
    public class ServiceProvider : IComparable<long>, IComparable
    {

        #region Field(s)

        private readonly Dictionary<string, PropertyItem> mProperties = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProvider" /> class.
        /// </summary>
        /// <param name="peer">The peer.</param>
        /// <param name="ep">The ep.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="properties">The properties.</param>
        public ServiceProvider(INetworkPeer peer, AddressEndPoint ep, long priority, Dictionary<string, PropertyItem> properties)
        {
            if (peer == null)
            {
                ThrowHelper.ThrowArgumentNullException("peer");
            }
            if (ep == null)
            {
                ThrowHelper.ThrowArgumentNullException("ep");
            }
            if (priority < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("priority");
            }

            this.NetworkPeer = peer;
            this.RemoteEndPoint = ep;
            this.Priority = priority;
            this.mProperties = properties == null ? new Dictionary<string, PropertyItem>() : new Dictionary<string, PropertyItem>(properties);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the network peer.
        /// </summary>
        /// <value>
        /// The network peer.
        /// </value>
        public INetworkPeer NetworkPeer { get; internal set; }

        /// <summary>
        /// Gets or sets the remote end point.
        /// </summary>
        /// <value>
        /// The remote end point.
        /// </value>
        public AddressEndPoint RemoteEndPoint { get; internal set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public long Priority { get; internal set; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public Dictionary<string, PropertyItem> Properties
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return new Dictionary<string, PropertyItem>(mProperties); }

            [MethodImpl(MethodImplOptions.Synchronized)]
            internal set
            {
                mProperties.Clear();
                if (value != null)
                {
                    foreach (KeyValuePair<string, PropertyItem> kv in value)
                    {
                        mProperties.Add(kv.Key, kv.Value);
                    }
                }
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int CompareTo(long other)
        {
            return this.Priority.CompareTo(other);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj" />. Zero This instance is equal to <paramref name="obj" />. Greater than zero This instance is greater than <paramref name="obj" />.
        /// </returns>
        public int CompareTo(object obj)
        {
            int result = 0;
            if (obj is Int64)
            {
                result = this.Priority.CompareTo((long)obj);
            }
            return result;
        }

        #endregion

    }

}
