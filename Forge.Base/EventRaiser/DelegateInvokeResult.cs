/* *********************************************************************
 * Date: 23 May 2007
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Forge.EventRaiser
{

    /// <summary>
    /// Result of invocation
    /// </summary>
    [Serializable]
    public class DelegateInvokeResult
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Delegate mDelegate = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Object[] mParameters = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mControlInvoke = true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mParallelInvocation = true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Object> mResultList = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Object mState = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateInvokeResult"/> class.
        /// </summary>
        /// <param name="del">The del.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="results">The results.</param>
        /// <param name="controlInvoke">if set to <c>true</c> [control invoke].</param>
        /// <param name="parallelInvocation">if set to <c>true</c> [parallel invocation].</param>
        /// <param name="state">The state.</param>
        internal DelegateInvokeResult(Delegate del, Object[] parameters,
            List<Object> results,
            bool controlInvoke, bool parallelInvocation, Object state)
        {
            mDelegate = del;
            mParameters = parameters;
            mControlInvoke = controlInvoke;
            mParallelInvocation = parallelInvocation;
            mResultList = results;
            mState = state;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        [DebuggerHidden]
        public Delegate Delegate
        {
            get { return mDelegate; }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        [DebuggerHidden]
        public Object[] Parameters
        {
            get { return mParameters; }
        }

        /// <summary>
        /// Gets a value indicating whether [control invoke].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [control invoke]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool ControlInvoke
        {
            get { return mControlInvoke; }
        }

        /// <summary>
        /// Gets a value indicating whether [parallel invocation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [parallel invocation]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool ParallelInvocation
        {
            get { return mParallelInvocation; }
        }

        /// <summary>
        /// Gets the result list.
        /// </summary>
        /// <value>
        /// The result list.
        /// </value>
        [DebuggerHidden]
        public List<Object> ResultList
        {
            get { return mResultList; }
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        [DebuggerHidden]
        public Object State
        {
            get { return mState; }
        }

        #endregion

    }

}
