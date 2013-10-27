/* *********************************************************************
 * Date: 19 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;

namespace Forge.Collections
{

    /// <summary>
    /// Abstract base list extension
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [Serializable, DebuggerDisplay("Count = {Count}")]
    public abstract class AbstractList<T> : AbstractSubList<T>, IListSpecialized<T>
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractList&lt;T&gt;"/> class.
        /// </summary>
        protected AbstractList()
        {
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator of generic items</returns>
        IEnumeratorSpecialized<T> IEnumerableSpecialized<T>.GetEnumerator()
        {
            return (IEnumeratorSpecialized<T>)GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator of generic items</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }

}
