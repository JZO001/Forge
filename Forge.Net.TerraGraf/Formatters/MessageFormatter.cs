/* *********************************************************************
 * Date: 10 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using System.Text;
using Forge.Formatters;
using Forge.Net.TerraGraf.Messaging;
using Forge.Persistence.Formatters;
using Forge.Persistence.Serialization;
using Forge.Reflection;
using Forge.Shared;

namespace Forge.Net.TerraGraf.Formatters
{

    /// <summary>
    /// Serialize or deserialize a message
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    internal sealed class MessageFormatter<T> : IDataFormatter<T> where T : MessageBase
    {

        #region Field(s)

        public static readonly int MAX_MESSAGE_HEADER_LENGTH = 5;

        private BinarySerializerFormatter<T> mFormatter = new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFormatter&lt;T&gt;"/> class.
        /// </summary>
        internal MessageFormatter()
        {
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Determines whether this instance can read the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>
        ///   <c>true</c> if this instance can read the specified stream; otherwise, <c>false</c>.
        /// </returns>
        public bool CanRead(System.IO.Stream stream)
        {
            return true;
        }

        /// <summary>
        /// Determines whether this instance can write the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if this instance can write the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool CanWrite(T item)
        {
            return true;
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The data</returns>
        public T Read(Stream stream)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            int b = stream.ReadByte();
            if (b == 0)
            {
                b = stream.ReadByte(); // skip maker byte, if I have one
            }
            if (b == 0)
            {
                throw new FormatException("Invalid message length.");
            }

            T result = default(T);

            using (MemoryStream ms = new MemoryStream())
            {
                int messageLength = 0;
                while (b > 0)
                {
                    ms.WriteByte((byte)b);
                    if (ms.Length > MAX_MESSAGE_HEADER_LENGTH)
                    {
                        throw new FormatException("Message header too long.");
                    }
                    b = stream.ReadByte();
                }
                if (!int.TryParse(Encoding.UTF8.GetString(ms.ToArray()), out messageLength))
                {
                    throw new FormatException("Invalid message header length value.");
                }
                ms.SetLength(0);

                // reading predicted data length
                byte[] buffer = new byte[messageLength];
                int readBytes = 0;
                while (readBytes < buffer.Length)
                {
                    readBytes += stream.Read(buffer, readBytes, buffer.Length - readBytes);
                }

                ms.Write(buffer, 0, buffer.Length);
                ms.Position = 0;
                result = mFormatter.Read(ms);
                ms.SetLength(0);
            }

            return result;
        }

        /// <summary>Restore the content of the stream</summary>
        /// <param name="inputStream">Source stream</param>
        /// <param name="outputStream">Output stream</param>
        /// <exception cref="System.NotImplementedException">In all cases</exception>
        public void Read(Stream inputStream, Stream outputStream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="data">The data.</param>
        public void Write(Stream stream, T data)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }
            if (data == null)
            {
                ThrowHelper.ThrowArgumentNullException("data");
            }
            using (MemoryStream dataStream = new MemoryStream())
            {
                mFormatter.Write(dataStream, data); // it can throw an FormatException
                using (MemoryStream headerStream = new MemoryStream())
                {
                    byte[] lenBytes = Encoding.UTF8.GetBytes(dataStream.Length.ToString());
                    headerStream.WriteByte((byte)0); // maker byte
                    headerStream.Write(lenBytes, 0, lenBytes.Length); // length of the message
                    headerStream.WriteByte((byte)0); // maker / split
                    headerStream.Position = 0;
                    headerStream.CopyTo(stream); // writing header
                    dataStream.Position = 0;
                    dataStream.CopyTo(stream); // writing data
                }
            }
        }

        /// <summary>Format the provided object into the output stream from the input stream</summary>
        /// <param name="outputStream">Stream that the formatted data has been written</param>
        /// <param name="inputStream">Object that will be formatted</param>
        /// <exception cref="System.NotImplementedException">In all cases</exception>
        public void Write(Stream outputStream, Stream inputStream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new MessageFormatter<T>();
        }

        #endregion

    }

}
