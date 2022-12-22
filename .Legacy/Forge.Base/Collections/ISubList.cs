/* *********************************************************************
 * Date: 03 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;

namespace Forge.Collections
{

    /// <summary>
    /// Mutable sub list interface
    /// This class represents a view of a parent list
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public interface ISubList<TItem> : IEnumerableSpecialized<TItem>, IList<TItem>
    {

        /// <summary>
        /// Subs the list.
        /// </summary>
        /// <param name="fromIndex">From index.</param>
        /// <param name="toIndex">To index.</param>
        /// <returns>Sub list with generic items</returns>
        ISubList<TItem> SubList(int fromIndex, int toIndex);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="c">The c.</param>
        void AddRange(IEnumerable<TItem> c);

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        int Version { get; }

    }

}
