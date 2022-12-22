/* *********************************************************************
 * Date: 18 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using Forge.Collections;
using Forge.Formatters;

namespace Forge.Persistence.StorageProviders
{

    /// <summary>
    /// Interface for storage providers
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public interface IStorageProvider<T> : IEnumerableSpecialized<T>, IDisposable
    {

        /// <summary>
        /// Get the unique identifier of the storage provider
        /// </summary>
        /// <value>
        /// The storage id.
        /// </value>
        /// <returns>Unique identifier</returns>
        string StorageId { get; }

        /// <summary>
        /// Get the formatter which used by the provider to persist data
        /// </summary>
        /// <value>
        /// The data formatter.
        /// </value>
        /// <returns>The formatter</returns>
        IDataFormatter<T> DataFormatter { get; }

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        /// <returns>Type of the data</returns>
        Type DataType { get; }

        /// <summary>
        /// Adds an object to the persistent store
        /// </summary>
        /// <param name="o">The o.</param>
        void Add(T o);

        /// <summary>
        /// Adds the range of T.
        /// </summary>
        /// <param name="o">The o.</param>
        void AddRange(IEnumerable<T> o);

        /// <summary>
        /// Insert the item into the specified position
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="o">The o.</param>
        void Insert(int index, T o);

        /// <summary>
        /// Removes an object from the persistent store
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>True, if the collection modified, otherwise False.</returns>
        bool Remove(T o);

        /// <summary>
        /// Remove item from the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        void RemoveAt(int index);

        /// <summary>
        /// Clears the persistent store
        /// </summary>
        void Clear();

        /// <summary>
        /// Get item from the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        T this[int index] { get; }

        /// <summary>
        /// Gets the size of the persistent store.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        /// <returns>The number of items in the collection</returns>
        int Count { get; }

        /// <summary>
        /// Determines whether this persistent store is empty.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this persistent store is empty; otherwise, <c>false</c>.
        /// </returns>
        bool IsEmpty { get; }

    }

}
