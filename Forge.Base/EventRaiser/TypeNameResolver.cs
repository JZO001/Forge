/* *********************************************************************
 * Date: 23 May 2007
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;

namespace Forge.EventRaiser
{

    [Serializable]
    internal sealed class TypeNameResolver
    {

        #region Field(s)

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private String mCallerTypeName = String.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeNameResolver"/> class.
        /// </summary>
        internal TypeNameResolver( )
        {
            StackTrace st = new StackTrace( System.Threading.Thread.CurrentThread, false );
            for( int i = 1; i < st.FrameCount; i++ )
            {
                if ( !st.GetFrame( i ).GetMethod( ).Name.Contains( "CallDelegatorByAsync" ) )
                {
                    mCallerTypeName = st.GetFrame( i ).GetMethod( ).ReflectedType.FullName;
                    break;
                }
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the name of the caller type.
        /// </summary>
        /// <value>
        /// The name of the caller type.
        /// </value>
        [DebuggerHidden]
        internal String CallerTypeName
        {
            get { return mCallerTypeName; }
        }

        #endregion

    }

}
