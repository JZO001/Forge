/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.IO;

namespace Forge.Net.Remoting.Messaging
{

    /// <summary>
    /// Response message
    /// </summary>
    [Serializable]
    public class ResponseMessage : Message
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Exception mMethodInvocationException = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MethodParameter mReturnValue = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseMessage"/> class.
        /// </summary>
        protected ResponseMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseMessage"/> class.
        /// </summary>
        /// <param name="correlationId">The correlation id.</param>
        /// <param name="returnValue">The return value.</param>
        public ResponseMessage(String correlationId, MethodParameter returnValue)
            : this(correlationId, returnValue, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseMessage"/> class.
        /// </summary>
        /// <param name="correlationId">The correlation id.</param>
        /// <param name="returnValue">The return value.</param>
        /// <param name="methodInvocationException">The method invocation exception.</param>
        public ResponseMessage(String correlationId, MethodParameter returnValue, Exception methodInvocationException)
            : base(correlationId, MessageTypeEnum.Response)
        {
            if (string.IsNullOrEmpty(correlationId))
            {
                ThrowHelper.ThrowArgumentNullException("correlationId");
            }
            if (returnValue == null)
            {
                ThrowHelper.ThrowArgumentNullException("returnValue");
            }
            this.mReturnValue = returnValue;
            this.mMethodInvocationException = methodInvocationException;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the method invocation exception.
        /// </summary>
        /// <value>
        /// The method invocation exception.
        /// </value>
        [DebuggerHidden]
        public Exception MethodInvocationException
        {
            get { return mMethodInvocationException; }
        }

        /// <summary>
        /// Gets the return value.
        /// </summary>
        /// <value>
        /// The return value.
        /// </value>
        [DebuggerHidden]
        public MethodParameter ReturnValue
        {
            get { return mReturnValue; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Sets the return value to null.
        /// </summary>
        public void SetReturnValueToNull()
        {
            this.mReturnValue.SetValueToNull();
        }

        /// <summary>
        /// Sets the return value to stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void SetReturnValueToStream(Stream stream)
        {
            this.mReturnValue.SetValueToStream(stream);
        }

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

            ResponseMessage other = (ResponseMessage)obj;
            return base.Equals(obj) && other.mMethodInvocationException == mMethodInvocationException &&
                other.mReturnValue.Equals(mReturnValue);
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
            return string.Format("{0}, Has exception: {1}", base.ToString(), (mMethodInvocationException != null).ToString());
        }

        #endregion

    }

}
