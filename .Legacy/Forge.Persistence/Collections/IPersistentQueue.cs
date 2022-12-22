/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.Persistence.Collections
{

    /// <summary>
    /// Persistent queue interface
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public interface IPersistentQueue<T> : IPersistentCache<T>
    {

        /// <summary>
        /// Dequeues this instance.
        /// </summary>
        /// <returns>Item</returns>
        T Dequeue();

        /// <summary>
        /// Enqueues this instance.
        /// </summary>
        /// <param name="item">The item.</param>
        void Enqueue(T item);

        /// <summary>
        /// Peeks this instance.
        /// </summary>
        /// <returns>Item</returns>
        T Peek();

    }

}
