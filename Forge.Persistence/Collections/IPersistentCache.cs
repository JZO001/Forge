/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Collections;

namespace Forge.Persistence.Collections
{

    /// <summary>
    /// Persistent cache interface
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public interface IPersistentCache<T> : IEnumerableSpecialized<T>, IDisposable
    {

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        string Id { get; }

        /// <summary>
        /// Gets or sets the size of the cache.
        /// </summary>
        /// <value>
        /// The size of the cache.
        /// </value>
        int CacheSize { get; set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether [contains] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified o]; otherwise, <c>false</c>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "o")]
        bool Contains(T o);

        /// <summary>
        /// Toes the array.
        /// </summary>
        /// <returns>Array of items</returns>
        T[] ToArray();

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets the cache strategy.
        /// </summary>
        /// <value>
        /// The cache strategy.
        /// </value>
        CacheStrategyEnum CacheStrategy { get; }

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <param name="action">The action.</param>
        void ForEach(Action<T> action);

    }

}
