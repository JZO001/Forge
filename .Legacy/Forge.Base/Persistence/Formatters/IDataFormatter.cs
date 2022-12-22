/* *********************************************************************
 * Date: 11 Jun 2009
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;

namespace Forge.Persistence.Formatters
{

    /// <summary>
    /// Service interface for formatters
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public interface IDataFormatter<T> : ICloneable
    {

        /// <summary>
        /// Indicate the content of the stream is deserializable
        /// </summary>
        /// <param name="stream">Content stream</param>
        /// <returns>
        /// true if the content is deserializable
        /// </returns>
        bool CanRead(Stream stream);

        /// <summary>
        /// Indicate that the item is serializable with the current formatter
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if this instance can write the specified item; otherwise, <c>false</c>.
        /// </returns>
        bool CanWrite(T item);

        /// <summary>
        /// Deserialize the content of the stream
        /// </summary>
        /// <param name="stream">Content stream</param>
        /// <returns>
        /// T object
        /// </returns>
        T Read(Stream stream);

        /// <summary>
        /// Serializable the provided object into the supplied stream
        /// </summary>
        /// <param name="stream">Stream that the serialized data has been written</param>
        /// <param name="data">Object that will be serialized</param>
        void Write(Stream stream, T data);

    }

}
