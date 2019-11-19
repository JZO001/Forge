/* *********************************************************************
 * Date: 25 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Diagnostics;
using System.Reflection;

namespace Forge.Net.Remoting.Validators
{

    /// <summary>
    /// Represents a property info
    /// </summary>
    public sealed class PropertyComparator : MBRBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly PropertyInfo mPropertyInfo = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly MethodComparator mGetMethod = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly MethodComparator mSetMethod = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyComparator"/> class.
        /// </summary>
        /// <param name="pi">The pi.</param>
        public PropertyComparator(PropertyInfo pi)
        {
            if (pi == null)
            {
                ThrowHelper.ThrowArgumentNullException("pi");
            }
            this.mPropertyInfo = pi;
            if (pi.GetGetMethod() != null)
            {
                this.mGetMethod = new MethodComparator(pi.GetGetMethod());
            }
            if (pi.GetSetMethod() != null)
            {
                this.mSetMethod = new MethodComparator(pi.GetSetMethod());
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the property info.
        /// </summary>
        /// <value>
        /// The property info.
        /// </value>
        [DebuggerHidden]
        public PropertyInfo PropertyInfo
        {
            get { return mPropertyInfo; }
        }

        /// <summary>
        /// Gets the get method.
        /// </summary>
        /// <value>
        /// The get method.
        /// </value>
        [DebuggerHidden]
        public MethodComparator GetMethod
        {
            get { return mGetMethod; }
        }

        /// <summary>
        /// Gets the set method.
        /// </summary>
        /// <value>
        /// The set method.
        /// </value>
        [DebuggerHidden]
        public MethodComparator SetMethod
        {
            get { return mSetMethod; }
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
            if (obj != null && obj is PropertyComparator)
            {
                PropertyComparator pc = (PropertyComparator)obj;
                return mPropertyInfo.Name.Equals(pc.mPropertyInfo.Name) && pc.GetMethod == GetMethod && pc.SetMethod == SetMethod;
            }
            return false;
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

        #endregion

    }

}
