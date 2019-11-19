/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Forge.Net.Synapse;

namespace Forge.Net.TerraGraf.NetworkPeers
{

    /// <summary>
    /// Represents a TCPServer
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, ServerId = '{ServerId}', EndPoint = '{EndPoint}', Attempts = '{Attempts}', Success = '{Success}', Manual = '{IsManuallyStarted}']")]
    public sealed class TCPServer
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mServerId = -1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AddressEndPoint mEndPoint = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mAttempts = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mSuccess = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mManuallyStarted = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPServer"/> class.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        internal TCPServer(AddressEndPoint endPoint)
        {
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }
            this.mEndPoint = endPoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPServer"/> class.
        /// </summary>
        /// <param name="serverId">The server id.</param>
        /// <param name="endPoint">The end point.</param>
        /// <param name="manuallyStarted">if set to <c>true</c> [manually started].</param>
        internal TCPServer(long serverId, AddressEndPoint endPoint, bool manuallyStarted)
            : this(endPoint)
        {
            this.mServerId = serverId;
            this.mManuallyStarted = manuallyStarted;
        }

        #endregion

        #region Internal properties

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

        /// <summary>
        /// Gets the IP end point.
        /// </summary>
        /// <value>
        /// The end point.
        /// </value>
        [DebuggerHidden]
        public AddressEndPoint EndPoint
        {
            get { return this.mEndPoint; }
        }

        /// <summary>
        /// Gets the attempts.
        /// </summary>
        /// <value>
        /// The attempts.
        /// </value>
        [DebuggerHidden]
        internal int Attempts
        {
            get { return mAttempts; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TCPServer"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        internal bool Success
        {
            get { return mSuccess; }
            set { mSuccess = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is manually started.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is manually started; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        internal bool IsManuallyStarted
        {
            get { return mManuallyStarted; }
            set { mManuallyStarted = value; }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Increment the attempt number.
        /// </summary>
        internal void IncAttempts()
        {
            Interlocked.Increment(ref mAttempts);
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

            TCPServer other = (TCPServer)obj;
            return other.mEndPoint.Equals(mEndPoint) && other.mServerId == mServerId;
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

        #endregion

    }

}
