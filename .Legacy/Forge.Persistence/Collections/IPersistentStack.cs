/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/


namespace Forge.Persistence.Collections
{

    /// <summary>
    /// Persistent stack interface
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public interface IPersistentStack<T> : IPersistentCache<T>
    {

        /// <summary>
        /// Peeks this instance.
        /// </summary>
        /// <returns>Item</returns>
        T Peek();

        /// <summary>
        /// Pops this instance.
        /// </summary>
        /// <returns>Item</returns>
        T Pop();

        /// <summary>
        /// Pushes this instance.
        /// </summary>
        /// <param name="item">The item</param>
        void Push(T item);

    }

}
