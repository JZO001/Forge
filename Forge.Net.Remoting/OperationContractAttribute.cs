/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Net.Remoting
{

    /// <summary>
    /// Indicates the method is a remote procedure or function and the level of the reliability
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OperationContractAttribute : Attribute
    {

        #region Field(s)

        internal static readonly int DEFAULT_METHOD_TIMEOUT = 120000; // 2 perc

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationContractAttribute"/> class.
        /// </summary>
        public OperationContractAttribute()
        {
            IsReliable = true;
            CallTimeout = DEFAULT_METHOD_TIMEOUT;
            ReturnTimeout = DEFAULT_METHOD_TIMEOUT;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationContractAttribute"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="isOneWay">if set to <c>true</c> [is one way].</param>
        /// <param name="isReliable">if set to <c>true</c> [is reliable].</param>
        /// <param name="callTimeout">The call timeout.</param>
        /// <param name="returnTimeout">The return timeout.</param>
        public OperationContractAttribute(OperationDirectionEnum direction, bool isOneWay, bool isReliable, long callTimeout, long returnTimeout)
        {
            Direction = direction;
            IsOneWay = isOneWay;
            IsReliable = isReliable;
            CallTimeout = callTimeout < System.Threading.Timeout.Infinite ? System.Threading.Timeout.Infinite : callTimeout;
            ReturnTimeout = returnTimeout < System.Threading.Timeout.Infinite ? System.Threading.Timeout.Infinite : returnTimeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationContractAttribute"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="isOneWay">if set to <c>true</c> [is one way].</param>
        /// <param name="isReliable">if set to <c>true</c> [is reliable].</param>
        /// <param name="callTimeout">The call timeout.</param>
        /// <param name="returnTimeout">The return timeout.</param>
        /// <param name="allowParallelExecution">if set to <c>true</c> [allow parallel execution].</param>
        public OperationContractAttribute(OperationDirectionEnum direction, bool isOneWay, bool isReliable, long callTimeout, long returnTimeout, bool allowParallelExecution)
            : this(direction, isOneWay, isReliable, callTimeout, returnTimeout)
        {
            AllowParallelExecution = allowParallelExecution;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public OperationDirectionEnum Direction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is one way.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is one way; otherwise, <c>false</c>.
        /// </value>
        public bool IsOneWay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is reliable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is reliable; otherwise, <c>false</c>.
        /// </value>
        public bool IsReliable { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public long CallTimeout { get; set; }

        /// <summary>
        /// Gets or sets the return timeout.
        /// </summary>
        /// <value>
        /// The return timeout.
        /// </value>
        public long ReturnTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow parallel execution].
        /// </summary>
        /// <value>
        /// <c>true</c> if [allow parallel execution]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowParallelExecution { get; set; }

        #endregion

    }

}
