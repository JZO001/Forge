/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Legacy;
using Forge.Shared;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Forge.Net.Remoting.Validators
{

    /// <summary>
    /// Helpers compare two methods
    /// </summary>
    public sealed class MethodComparator : MBRBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly MethodInfo mMethod = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodComparator"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        public MethodComparator(MethodInfo method)
        {
            if (method == null)
            {
                ThrowHelper.ThrowArgumentNullException("method");
            }
            mMethod = method;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        [DebuggerHidden]
        public MethodInfo Method
        {
            get { return mMethod; }
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
            // Ez a kódrészlet a Java Method implementációjából való.
            // Fontos különbség, hogy itt nem vizsgálom a DeclaringClass-t, azaz nem számít melyik interface-ből való.
            // Nem vizsgálom a visszatérési típust sem, mert irreveláns azokban az esetekben, ahol ezt az osztályt használom.
            // Ami számít, hogy a neve és a paraméterlistája pontosan megegyezzen.
            if (obj != null && obj is MethodComparator)
            {
                MethodComparator other = (MethodComparator)obj;
                if (Method.Name.Equals(other.Method.Name))
                {
                    /* Avoid unnecessary cloning */
                    ParameterInfo[] params1 = mMethod.GetParameters();
                    ParameterInfo[] params2 = other.Method.GetParameters();
                    if (params1.Length == params2.Length)
                    {
                        for (int i = 0; i < params1.Length; i++)
                        {
                            if (params1[i].ParameterType != params2[i].ParameterType)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
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

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}", mMethod.Name);
        }

        #endregion

    }

}
