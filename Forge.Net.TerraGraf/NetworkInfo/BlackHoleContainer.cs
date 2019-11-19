/* *********************************************************************
 * Date: 8 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;

namespace Forge.Net.TerraGraf.NetworkInfo
{

    /// <summary>
    /// Represents a blackhole state and identifier of the state
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType().Name}, StateId = '{StateId}', IsBlackHole = '{IsBlackHole}']")]
    internal sealed class BlackHoleContainer
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mStateId = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mBlackHole = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="BlackHoleContainer"/> class.
        /// </summary>
        internal BlackHoleContainer()
        {
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets or sets the state id.
        /// </summary>
        /// <value>
        /// The state id.
        /// </value>
        [DebuggerHidden]
        internal long StateId
        {
            get { return mStateId; }
            set { mStateId = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is black hole.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is black hole; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        internal bool IsBlackHole
        {
            get { return mBlackHole; }
            set { mBlackHole = value; }
        }

        #endregion

    }

}
