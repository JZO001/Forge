/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;

namespace Forge.Persistence.Collections
{

    /// <summary>
    /// Persistent collection interface
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public interface IPersistentCollection<T> : IPersistentCache<T>, ICollection<T>
    {
    }

}
