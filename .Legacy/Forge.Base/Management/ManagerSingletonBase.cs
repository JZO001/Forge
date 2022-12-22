/* *********************************************************************
 * Date: 22 Jan 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Reflection;

namespace Forge.Management
{

    /// <summary>
    /// Represents the base methods and properties of a singleton manager service
    /// </summary>
    /// <typeparam name="TManager">The type of the manager.</typeparam>
    [Serializable]
    public abstract class ManagerSingletonBase<TManager> : ManagerBase where TManager : ManagerSingletonBase<TManager>
    {

        /// <summary>
        /// The singleton instance
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected static TManager mSingletonInstance = null;

        /// <summary>
        /// Initializes the <see cref="ManagerSingletonBase{TManager}" /> class.
        /// </summary>
        static ManagerSingletonBase()
        {
            mSingletonInstance = (TManager)typeof(TManager).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, new ParameterModifier[] { }).Invoke(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerSingletonBase{TManager}" /> class.
        /// </summary>
        protected ManagerSingletonBase()
            : base()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The singleton instance.
        /// </value>
        [DebuggerHidden]
        public static TManager Instance
        {
            get { return mSingletonInstance; }
        }

    }

}
