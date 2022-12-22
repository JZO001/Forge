/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Forge.Net.Remoting.Messaging
{

    /// <summary>
    /// Request message
    /// </summary>
    [Serializable]
    public sealed class RequestMessage : Message
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mContractName = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mMethodName = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MethodParameter[] mMethodParameters = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MessageInvokeModeEnum mMessageInvokeMode = MessageInvokeModeEnum.RequestService;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage"/> class.
        /// </summary>
        private RequestMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage"/> class.
        /// </summary>
        /// <param name="correlationId">The correlation id.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageInvokeMode">The message invoke mode.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="methodName">Name of the method.</param>
        public RequestMessage(String correlationId, MessageTypeEnum messageType, MessageInvokeModeEnum messageInvokeMode, Type contract, String methodName)
            : this(correlationId, messageType, messageInvokeMode, contract, methodName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage"/> class.
        /// </summary>
        /// <param name="correlationId">The correlation id.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageInvokeMode">The message invoke mode.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="methodParameters">The method parameters.</param>
        public RequestMessage(String correlationId, MessageTypeEnum messageType, MessageInvokeModeEnum messageInvokeMode, Type contract, String methodName, MethodParameter[] methodParameters)
            : base(correlationId, messageType)
        {
            if (string.IsNullOrEmpty(correlationId))
            {
                ThrowHelper.ThrowArgumentNullException("correlationId");
            }
            if (!(messageType == MessageTypeEnum.Request || messageType == MessageTypeEnum.Datagram || messageType == MessageTypeEnum.DatagramOneway))
            {
                ThrowHelper.ThrowArgumentException(String.Format("Invalid message type parameter: {0}", messageType));
            }
            if (contract == null)
            {
                ThrowHelper.ThrowArgumentNullException("contract");
            }
            if (string.IsNullOrEmpty(methodName))
            {
                ThrowHelper.ThrowArgumentNullException("methodName");
            }
            mContractName = string.Format("{0}, {1}", contract.FullName, new AssemblyName(contract.Assembly.FullName).Name);
            mMethodName = methodName;
            mMethodParameters = methodParameters;
            mMessageInvokeMode = messageInvokeMode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage"/> class.
        /// </summary>
        /// <param name="correlationId">The correlation unique identifier.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageInvokeMode">The message invoke mode.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="methodParameters">The method parameters.</param>
        /// <param name="allowParallelExecution">Allow parallel execution on the remote side</param>
        public RequestMessage(String correlationId, MessageTypeEnum messageType, MessageInvokeModeEnum messageInvokeMode, Type contract, String methodName, MethodParameter[] methodParameters, bool allowParallelExecution)
            : this(correlationId, messageType, messageInvokeMode, contract, methodName, methodParameters)
        {
            AllowParallelExecution = allowParallelExecution;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the name of the contract.
        /// </summary>
        /// <value>
        /// The name of the contract.
        /// </value>
        [DebuggerHidden]
        public string ContractName
        {
            get { return mContractName; }
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>
        /// The name of the method.
        /// </value>
        [DebuggerHidden]
        public string MethodName
        {
            get { return mMethodName; }
        }

        /// <summary>
        /// Gets the method parameters.
        /// </summary>
        /// <value>
        /// The method parameters.
        /// </value>
        [DebuggerHidden]
        public MethodParameter[] MethodParameters
        {
            get { return mMethodParameters; }
        }

        /// <summary>
        /// Gets the message invoke mode.
        /// </summary>
        /// <value>
        /// The message invoke mode.
        /// </value>
        [DebuggerHidden]
        public MessageInvokeModeEnum MessageInvokeMode
        {
            get { return mMessageInvokeMode; }
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

            RequestMessage other = (RequestMessage)obj;
            return base.Equals(obj) && other.mContractName.Equals(mContractName) && other.mMethodName.Equals(mMethodName) &&
                other.mMessageInvokeMode == mMessageInvokeMode && Arrays.DeepEquals(other.mMethodParameters, mMethodParameters);
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
            return string.Format("{0}, Contract name: {1}, Method name: {2}, Invoke mode: {3}", base.ToString(), mContractName, mMethodName, mMessageInvokeMode.ToString());
        }

        #endregion

    }

}
