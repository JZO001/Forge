/* *********************************************************************
 * Date: 28 Feb 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Threading.Tasking
{

    /// <summary>
    /// Task result without result
    /// </summary>
    [Serializable]
    public class TaskResult
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResult"/> class.
        /// </summary>
        /// <param name="inParameters">The in parameters.</param>
        public TaskResult(object[] inParameters)
        {
            this.InParameters = inParameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResult"/> class.
        /// </summary>
        /// <param name="inParameters">The in parameters.</param>
        /// <param name="exception">The exception.</param>
        public TaskResult(object[] inParameters, Exception exception)
            : this(inParameters)
        {
            this.Exception = exception;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the in parameters.
        /// </summary>
        /// <value>
        /// The in parameters.
        /// </value>
        public object[] InParameters { get; private set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; private set; }

        #endregion

    }

    /// <summary>
    /// Task result with generic result property
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    [Serializable]
    public class TaskResult<TResult> : TaskResult
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResult{TResult}"/> class.
        /// </summary>
        /// <param name="inParameters">The in parameters.</param>
        public TaskResult(object[] inParameters)
            : base(inParameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResult{TResult}"/> class.
        /// </summary>
        /// <param name="inParameters">The in parameters.</param>
        /// <param name="exception">The exception.</param>
        public TaskResult(object[] inParameters, Exception exception)
            : base(inParameters, exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResult{TResult}"/> class.
        /// </summary>
        /// <param name="inParameters">The in parameters.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="result">The result.</param>
        public TaskResult(object[] inParameters, Exception exception, TResult result)
            : base(inParameters, exception)
        {
            this.Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResult{TResult}"/> class.
        /// </summary>
        /// <param name="inParameters">The in parameters.</param>
        /// <param name="result">The result.</param>
        public TaskResult(object[] inParameters, TResult result)
            : this(inParameters, null, result)
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public TResult Result { get; private set; }

        #endregion

    }

}
