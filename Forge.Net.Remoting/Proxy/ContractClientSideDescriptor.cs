/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Legacy;
using Forge.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Forge.Net.Remoting.Proxy
{

    /// <summary>
    /// Represents the client side descriptor of the contract
    /// </summary>
    [Serializable]
    public sealed class ContractClientSideDescriptor : MBRBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Type mContractType = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mDefaultChannelId = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Type mDefaultProxyType = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, Type> mImplementationPerChannel = new Dictionary<string, Type>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractClientSideDescriptor"/> class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="defaultChannelId">The default channel id.</param>
        /// <param name="defaultProxyType">Default type of the proxy.</param>
        internal ContractClientSideDescriptor(Type contractType, String defaultChannelId, Type defaultProxyType)
        {
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }
            if (defaultProxyType != null)
            {
                if (!typeof(ProxyBase).IsAssignableFrom(defaultProxyType))
                {
                    ThrowHelper.ThrowArgumentException("Type of default proxy is not assignable from ProxyBase.");
                }
            }
            mContractType = contractType;
            mDefaultChannelId = defaultChannelId;
            mDefaultProxyType = defaultProxyType;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <value>
        /// The type of the contract.
        /// </value>
        [DebuggerHidden]
        public Type ContractType
        {
            get { return mContractType; }
        }

        /// <summary>
        /// Gets the default channel id.
        /// </summary>
        /// <value>
        /// The default channel id.
        /// </value>
        [DebuggerHidden]
        public string DefaultChannelId
        {
            get { return mDefaultChannelId; }
        }

        /// <summary>
        /// Gets the default type of the proxy.
        /// </summary>
        /// <value>
        /// The default type of the proxy.
        /// </value>
        [DebuggerHidden]
        public Type DefaultProxyType
        {
            get { return mDefaultProxyType; }
        }

        /// <summary>
        /// Gets the implementation per channel.
        /// </summary>
        /// <value>
        /// The implementation per channel.
        /// </value>
        [DebuggerHidden]
        public Dictionary<string, Type> ImplementationPerChannel
        {
            get { return mImplementationPerChannel; }
        }

        #endregion

    }

}
