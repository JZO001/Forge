/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Forge.Net.Synapse;

namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>
    /// Represents a connection target entry
    /// </summary>
    [Serializable]
    public class ConnectionEntry : IEquatable<ConnectionEntry>
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AddressEndPoint mEndPoint = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mReconnectOnFailure = true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mDelayBetweenAttempsInMS = 1000;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mConnectionTimeout = Timeout.Infinite;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionEntry"/> class.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        public ConnectionEntry(AddressEndPoint endPoint)
        {
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            this.mEndPoint = endPoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionEntry"/> class.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="reconnectOnFailure">if set to <c>true</c> [reconnect on failure].</param>
        /// <param name="delayBetweenAttemptsInMS">The delay between attempts in MS.</param>
        /// <param name="connectionTimeout">The connection timeout.</param>
        public ConnectionEntry(AddressEndPoint endPoint, bool reconnectOnFailure, int delayBetweenAttemptsInMS, int connectionTimeout)
            : this(endPoint)
        {
            if (delayBetweenAttemptsInMS < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("delayBetweenAttemptsInMS");
            }
            this.mReconnectOnFailure = reconnectOnFailure;
            this.mDelayBetweenAttempsInMS = delayBetweenAttemptsInMS;
            this.mConnectionTimeout = connectionTimeout;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>
        /// The end point.
        /// </value>
        [DebuggerHidden]
        public AddressEndPoint EndPoint
        {
            get { return mEndPoint; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }
                mEndPoint = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [reconnect on failure].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [reconnect on failure]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool ReconnectOnFailure
        {
            get { return mReconnectOnFailure; }
            set { mReconnectOnFailure = value; }
        }

        /// <summary>
        /// Gets or sets the delay between attemps in MS.
        /// </summary>
        /// <value>
        /// The delay between attemps in MS.
        /// </value>
        [DebuggerHidden]
        public int DelayBetweenAttempsInMS
        {
            get { return mDelayBetweenAttempsInMS; }
            set
            {
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mDelayBetweenAttempsInMS = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection timeout.
        /// </summary>
        /// <value>
        /// The connection timeout.
        /// </value>
        [DebuggerHidden]
        public int ConnectionTimeout
        {
            get { return mConnectionTimeout; }
            set
            {
                if (value < Timeout.Infinite)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("value");
                }
                mConnectionTimeout = value;
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            ConnectionEntry other = (ConnectionEntry)obj;
            return other.mEndPoint.Equals(mEndPoint) && other.mReconnectOnFailure == mReconnectOnFailure && other.mDelayBetweenAttempsInMS == mDelayBetweenAttempsInMS;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True, if the other class is equals with this.</returns>
        public bool Equals(ConnectionEntry other)
        {
            return Equals((object)other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, Reconnect: {1}, Delay: {2}, Timeout: {3}", mEndPoint, mReconnectOnFailure, mDelayBetweenAttempsInMS, mConnectionTimeout);
        }

        #endregion

    }

}
