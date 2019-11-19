/* *********************************************************************
 * Date: 10 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Forge.Net.TerraGraf.NetworkInfo;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Represent a TerraGraf network structure message
    /// </summary>
    [Serializable]
    internal sealed class TerraGrafInformationMessage : TerraGrafMessageBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TerraGrafNetworkInformation mNetworkInfo = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PeerRelation mTargetHostRelation = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BlackHoleContainer mBlackHoleContainer = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Prevents a default instance of the <see cref="TerraGrafInformationMessage"/> class from being created.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        private TerraGrafInformationMessage(string senderId)
            : base(senderId, string.Empty, MessageCodeEnum.TerraGrafInformation,
            Interlocked.Increment(ref mGlobalMessageId), MessagePriorityEnum.Normal, MessageTypeEnum.Udp)
        {
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="TerraGrafInformationMessage"/> class from being created.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="targetId">The target id.</param>
        private TerraGrafInformationMessage(string senderId, string targetId)
            : base(senderId, targetId, MessageCodeEnum.TerraGrafInformation,
            Interlocked.Increment(ref mGlobalMessageId), MessagePriorityEnum.Normal, MessageTypeEnum.Udp)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                ThrowHelper.ThrowArgumentNullException("targetId");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafInformationMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="targetId">The target id.</param>
        /// <param name="networkInfo">The network info.</param>
        internal TerraGrafInformationMessage(string senderId, string targetId, TerraGrafNetworkInformation networkInfo)
            : this(senderId, targetId)
        {
            if (networkInfo == null)
            {
                ThrowHelper.ThrowArgumentNullException("networkInfo");
            }
            this.mNetworkInfo = networkInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafInformationMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="networkInfo">The network info.</param>
        /// <param name="targetHostRelation">The target host relation.</param>
        internal TerraGrafInformationMessage(string senderId, TerraGrafNetworkInformation networkInfo, PeerRelation targetHostRelation) :
            this(senderId)
        {
            if (networkInfo == null)
            {
                ThrowHelper.ThrowArgumentNullException("networkInfo");
            }
            if (targetHostRelation == null)
            {
                ThrowHelper.ThrowArgumentNullException("targetHostRelation");
            }
            this.mNetworkInfo = networkInfo;
            this.mTargetHostRelation = targetHostRelation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafInformationMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="targetHostRelation">The target host relation.</param>
        internal TerraGrafInformationMessage(string senderId, PeerRelation targetHostRelation)
            : this(senderId)
        {
            if (targetHostRelation == null)
            {
                ThrowHelper.ThrowArgumentNullException("targetHostRelation");
            }
            this.mTargetHostRelation = targetHostRelation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafInformationMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="blackHoleContainer">The black hole container.</param>
        internal TerraGrafInformationMessage(string senderId, BlackHoleContainer blackHoleContainer)
            : this(senderId)
        {
            if (blackHoleContainer == null)
            {
                ThrowHelper.ThrowArgumentNullException("blackHoleContainer");
            }
            this.mBlackHoleContainer = blackHoleContainer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafInformationMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="networkInfo">The network info.</param>
        /// <param name="blackHoleContainer">The black hole container.</param>
        internal TerraGrafInformationMessage(string senderId, TerraGrafNetworkInformation networkInfo, BlackHoleContainer blackHoleContainer)
            : this(senderId, blackHoleContainer)
        {
            if (networkInfo == null)
            {
                ThrowHelper.ThrowArgumentNullException("networkInfo");
            }
            this.mNetworkInfo = networkInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerraGrafInformationMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="messageId">The message id.</param>
        /// <param name="networkInfo">The network info.</param>
        /// <param name="targetHostRelation">The target host relation.</param>
        /// <param name="blackHoleContainer">The black hole container.</param>
        internal TerraGrafInformationMessage(string senderId, long messageId, TerraGrafNetworkInformation networkInfo, PeerRelation targetHostRelation, BlackHoleContainer blackHoleContainer)
            : base(senderId, string.Empty, MessageCodeEnum.TerraGrafInformation, messageId, MessagePriorityEnum.Normal, MessageTypeEnum.Udp)
        {
            if (networkInfo == null)
            {
                ThrowHelper.ThrowArgumentNullException("networkInfo");
            }
            this.mNetworkInfo = networkInfo;
            this.mTargetHostRelation = targetHostRelation;
            this.mBlackHoleContainer = blackHoleContainer;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the network info.
        /// </summary>
        /// <value>
        /// The network info.
        /// </value>
        [DebuggerHidden]
        internal TerraGrafNetworkInformation NetworkInfo
        {
            get { return mNetworkInfo; }
        }

        /// <summary>
        /// Gets the target host relation.
        /// </summary>
        /// <value>
        /// The target host relation.
        /// </value>
        [DebuggerHidden]
        internal PeerRelation TargetHostRelation
        {
            get { return mTargetHostRelation; }
        }

        /// <summary>
        /// Gets the black hole container.
        /// </summary>
        /// <value>
        /// The black hole container.
        /// </value>
        [DebuggerHidden]
        internal BlackHoleContainer BlackHoleContainer
        {
            get { return mBlackHoleContainer; }
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
            bool result = false;
            if (base.Equals(obj))
            {
                TerraGrafInformationMessage other = (TerraGrafInformationMessage)obj;
                result = other.mNetworkInfo == mNetworkInfo;
            }
            return result;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash = 21 * hash + mNetworkInfo.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            if (mNetworkInfo != null && mTargetHostRelation != null)
            {
                sb.Append(", [propagation]");
            }
            else if (mBlackHoleContainer != null && mNetworkInfo != null)
            {
                sb.Append(", [blackhole OFF]");
            }
            else if (mBlackHoleContainer != null)
            {
                sb.Append(", [blackhole ON]");
            }
            else if (mNetworkInfo == null)
            {
                sb.Append(", [relation update]");
            }
            else
            {
                sb.Append(", [negotiation]");
            }

            return sb.ToString();
        }

        #endregion

    }

}
