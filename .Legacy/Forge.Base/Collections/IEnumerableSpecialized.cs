/* *********************************************************************
 * Date: 18 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;

namespace Forge.Collections
{

    /// <summary>
    /// IEnumerable specialized (mutable enumerable) interface
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public interface IEnumerableSpecialized<T> : IEnumerable<T>
    {

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// Enumerator of generic items
        /// </returns>
        new IEnumeratorSpecialized<T> GetEnumerator();

    }

}
