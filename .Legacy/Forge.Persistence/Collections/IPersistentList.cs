/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;
using Forge.Collections;

namespace Forge.Persistence.Collections
{

    /// <summary>
    /// Persistent list
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public interface IPersistentList<T> : IPersistentCollection<T>, IList<T>, ISubList<T>
    {
        
        /// <summary>
        /// Determines whether the specified c contains all.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>
        ///   <c>true</c> if the specified c contains all; otherwise, <c>false</c>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
        bool ContainsAll(ICollection<T> c);

        /// <summary>
        /// Inserts the range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="c">The c.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
        void InsertRange(int index, ICollection<T> c);

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="c">The c.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
        void RemoveAll(ICollection<T> c);

        /// <summary>
        /// Retains all.
        /// </summary>
        /// <param name="c">The c.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
        void RetainAll(ICollection<T> c);

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "o")]
        int LastIndexOf(T o);

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        void CopyTo(T[] array);

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// <param name="count">The count.</param>
        void CopyTo(int index, T[] array, int arrayIndex, int count);

    }

}
