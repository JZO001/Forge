/* *********************************************************************
 * Date: 18 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;

namespace Forge.Collections
{

    /// <summary>
    /// IEnumerator specialized (mutable enumerator) interface
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public interface IEnumeratorSpecialized<T> : IEnumerator<T>
    {

        /// <summary>
        /// Removes from the underlying collection the last element returned by the
        /// iterator (optional operation).  This method can be called only once per
        /// call to <tt>next</tt>.  The behavior of an iterator is unspecified if
        /// the underlying collection is modified while the iteration is in
        /// progress in any way other than by calling this method.
        /// </summary>
        void Remove();

        /// <summary>
        /// Determines whether has a next item in the enumerator.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has next; otherwise, <c>false</c>.
        /// </returns>
        bool HasNext();

    }

}
