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
    /// Persistent dictionary interface
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IPersistentDictionary<TKey, TValue> : IPersistentCache<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {

        /// <summary>
        /// Gets an IPersistentDictionary&lt;TKey,TValue&gt; containing the keys of
        /// the IPersistentDictionary&lt;TKey,TValue&gt;.
        /// </summary>
        /// <value>
        /// An IPersistentDictionary&lt;TKey,TValue&gt; containing the keys of the object
        /// that implements IPersistentDictionary&lt;TKey,TValue&gt;.
        /// </value>
        new IListSpecialized<TKey> Keys { get; }

        /// <summary>
        /// Gets an IPersistentDictionary&lt;TKey,TValue&gt; containing the values in
        /// object that implements IPersistentDictionary&lt;TKey,TValue&gt;.
        /// </summary>
        /// <value>
        /// An IPersistentDictionary&lt;TKey,TValue&gt; containing the values in the
        /// object that implements IPersistentDictionary&lt;TKey,TValue&gt;.
        /// </value>
        new IListSpecialized<TValue> Values { get; }

    }

}
