/* *********************************************************************
 * Date: 08 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Diagnostics;

namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>
    /// Rules of the context. Describes how the context communicate with each other
    /// </summary>
    [Serializable]
    public sealed class ContextRule : IEquatable<ContextRule>
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mNetworkContextName = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mRule = string.Empty;

        #endregion

        #region Constructor(s)

        /// <summary>Initializes a new instance of the <see cref="ContextRule" /> class.</summary>
        public ContextRule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextRule"/> class.
        /// </summary>
        /// <param name="networkContextName">Name of the network context.</param>
        /// <param name="rule">The rule.</param>
        public ContextRule(string networkContextName, string rule)
        {
            if (string.IsNullOrEmpty(networkContextName))
            {
                ThrowHelper.ThrowArgumentNullException("networkContextName");
            }
            if (string.IsNullOrEmpty(rule))
            {
                ThrowHelper.ThrowArgumentNullException("rule");
            }
            mNetworkContextName = networkContextName;
            mRule = rule;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the name of the network context.
        /// </summary>
        /// <value>
        /// The name of the network context.
        /// </value>
        [DebuggerHidden]
        public string NetworkContextName
        {
            get { return mNetworkContextName; }
            set 
            { 
                if (string.IsNullOrWhiteSpace(value)) ThrowHelper.ThrowArgumentNullException("value");
                mNetworkContextName = value; 
            }
        }

        /// <summary>
        /// Gets the rule.
        /// </summary>
        /// <value>
        /// The rule.
        /// </value>
        [DebuggerHidden]
        public string Rule
        {
            get { return mRule; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) ThrowHelper.ThrowArgumentNullException("value");
                mRule = value;
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

            ContextRule other = (ContextRule)obj;
            return other.mNetworkContextName == mNetworkContextName && other.mRule == mRule;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True, if the other class is equals with this.</returns>
        public bool Equals(ContextRule other)
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
            return string.Format("Network Context '{0}' rule '{1}'", mNetworkContextName, mRule);
        }

        #endregion

    }

}
