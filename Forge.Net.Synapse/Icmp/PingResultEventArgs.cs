/* *********************************************************************
 * Date: 10 Oct 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Diagnostics;
using System.Net;

namespace Forge.Net.Synapse.Icmp
{

    /// <summary>
    /// Event arguments for ping results
    /// </summary>
    [Serializable]
    public sealed class PingResultEventArgs : EventArgs
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PingResultEnum mPingResultType = PingResultEnum.Result;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IPAddress mIPAddress = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mReceivedBytes = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mResponseTime = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PingResultEventArgs"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        internal PingResultEventArgs(PingResultEnum result)
        {
            mPingResultType = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PingResultEventArgs"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="receivedBytes">The received bytes.</param>
        /// <param name="responseTime">The response time.</param>
        internal PingResultEventArgs(IPAddress address, int receivedBytes, int responseTime)
            : this(PingResultEnum.Result)
        {
            if (address == null)
            {
                ThrowHelper.ThrowArgumentNullException("address");
            }
            mIPAddress = address;
            mReceivedBytes = receivedBytes;
            mResponseTime = responseTime;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the type of the ping result.
        /// </summary>
        /// <value>
        /// The type of the ping result.
        /// </value>
        public PingResultEnum PingResultType
        {
            get { return mPingResultType; }
        }

        /// <summary>
        /// Gets the IP address.
        /// </summary>
        /// <value>
        /// The IP address.
        /// </value>
        public IPAddress IPAddress
        {
            get { return mIPAddress; }
        }

        /// <summary>
        /// Gets the received bytes.
        /// </summary>
        /// <value>
        /// The received bytes.
        /// </value>
        public int ReceivedBytes
        {
            get { return mReceivedBytes; }
        }

        /// <summary>
        /// Gets the response time.
        /// </summary>
        /// <value>
        /// The response time.
        /// </value>
        public int ResponseTime
        {
            get { return mResponseTime; }
        }

        #endregion

    }

}
