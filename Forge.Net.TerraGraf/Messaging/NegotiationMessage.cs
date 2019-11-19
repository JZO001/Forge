/* *********************************************************************
 * Date: 11 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Forge.Net.TerraGraf.Messaging
{

    /// <summary>
    /// Negotiation message for peer handshake
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, SenderId = '{SenderId}', MessageId = '{MessageId}', Priority = '{Priority}', MessageType = '{MessageType}', NetworkContext = '{NetworkContextName}']")]
    internal sealed class NegotiationMessage : TerraGrafMessageBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mNetworkContextName = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Version mVersion = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NegotiationMessage"/> class.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <param name="networkContextName">Name of the network context.</param>
        /// <param name="version">The version.</param>
        internal NegotiationMessage(string senderId, string networkContextName, Version version)
            : base(senderId, string.Empty, MessageCodeEnum.Negotiation,
            Interlocked.Increment(ref mGlobalMessageId), MessagePriorityEnum.High, MessageTypeEnum.Udp)
        {
            if (string.IsNullOrEmpty(networkContextName))
            {
                ThrowHelper.ThrowArgumentNullException("networkContextName");
            }
            if (version == null)
            {
                ThrowHelper.ThrowArgumentNullException("version");
            }
            this.mNetworkContextName = networkContextName;
            this.mVersion = version;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the name of the network context.
        /// </summary>
        /// <value>
        /// The name of the network context.
        /// </value>
        [DebuggerHidden]
        internal string NetworkContextName
        {
            get { return mNetworkContextName; }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DebuggerHidden]
        internal Version Version
        {
            get { return mVersion; }
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
                NegotiationMessage other = (NegotiationMessage)obj;
                result = other.mNetworkContextName == mNetworkContextName && other.mVersion == mVersion;
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
            hash = 22 * hash + mNetworkContextName.GetHashCode();
            hash = 22 * hash + mVersion.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append("], NetworkContextName: [");
            sb.Append(NetworkContextName == null ? "(null)" : NetworkContextName);
            sb.Append("], Version: [");
            sb.Append(Version == null ? "(null)" : Version.ToString());
            sb.Append("]");
            return sb.ToString();
        }

        #endregion

    }

}
