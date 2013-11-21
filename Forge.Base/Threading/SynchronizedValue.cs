/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Forge.Threading
{

    /// <summary>
    /// Provide syncronized access to a value
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [Serializable]
    public class SynchronizedValue<TValue> : IDisposable
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NonSerialized]
        private DeadlockSafeLock mValueLock = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TValue mValue = default(TValue);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedValue&lt;TValue&gt;"/> class.
        /// </summary>
        public SynchronizedValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedValue&lt;TValue&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SynchronizedValue(TValue value)
        {
            this.mValue = value;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SynchronizedValue&lt;TValue&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~SynchronizedValue()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DebuggerHidden]
        public TValue Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Gets the value lock.
        /// </summary>
        /// <value>
        /// The value lock.
        /// </value>
        public DeadlockSafeLock ValueLock
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (this.mValueLock == null)
                {
                    this.mValueLock = new DeadlockSafeLock(ToString());
                }
                return mValueLock;
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0}@{1}", base.ToString(), GetHashCode());
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Does the dispose check.
        /// </summary>
        protected virtual void DoDisposeCheck()
        {
            if (mValueLock != null && mValueLock.IsDisposed)
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && mValueLock != null)
            {
                mValueLock.Dispose();
            }
        }
        #endregion

    }

}
