/* *********************************************************************
 * Date: 12 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Legacy;
using Forge.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Forge.Net.Remoting.Service
{

    /// <summary>
    /// Represents the service side description of the contract
    /// </summary>
    [Serializable]
    public sealed class ContractServiceSideDescriptor : MBRBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Type mContractType = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Type mDefaultImplementationType = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, Type> mImplementationPerChannel = new Dictionary<string, Type>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractServiceSideDescriptor"/> class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="defaultImplementationType">Default type of the implementation.</param>
        internal ContractServiceSideDescriptor(Type contractType, Type defaultImplementationType)
        {
            if (contractType == null)
            {
                ThrowHelper.ThrowArgumentNullException("contractType");
            }
            mContractType = contractType;
            mDefaultImplementationType = defaultImplementationType;
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
        /// Gets the default type of the implementation.
        /// </summary>
        /// <value>
        /// The default type of the implementation.
        /// </value>
        [DebuggerHidden]
        public Type DefaultImplementationType
        {
            get { return mDefaultImplementationType; }
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
