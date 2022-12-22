/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.IO;
using Forge.Persistence.Formatters;
using Forge.Reflection;

namespace Forge.Persistence.Serialization
{

    /// <summary>
    /// Serialization/deserialization helper class
    /// </summary>
    public static class SerializationHelper
    {

        private const bool DEFAULT_ENABLE_COMPRESS = true;

        private static readonly GZipFormatter COMPRESSION_FORMATTER = new GZipFormatter();

        /// <summary>
        /// Reads the specified file.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="file">The file.</param>
        /// <returns>Data</returns>
        public static T Read<T>(FileInfo file)
        {
            return Read<T>(file, new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true), DEFAULT_ENABLE_COMPRESS);
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns>Data</returns>
        public static T Read<T>(Stream stream)
        {
            return Read<T>(stream, new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true), DEFAULT_ENABLE_COMPRESS);
        }

        /// <summary>
        /// Reads the specified file.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="file">The file.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns>Data</returns>
        public static T Read<T>(FileInfo file, IDataFormatter<T> formatter)
        {
            return Read<T>(file, formatter, DEFAULT_ENABLE_COMPRESS);
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns>Data</returns>
        public static T Read<T>(Stream stream, IDataFormatter<T> formatter)
        {
            return Read<T>(stream, formatter, DEFAULT_ENABLE_COMPRESS);
        }

        /// <summary>
        /// Reads the specified file.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="file">The file.</param>
        /// <param name="formatter">The formatter.</param>
        /// <param name="decompress">if set to <c>true</c> [decompress].</param>
        /// <returns>Data</returns>
        /// <exception cref="System.ArgumentNullException">
        /// file
        /// or
        /// formatter
        /// </exception>
        public static T Read<T>(FileInfo file, IDataFormatter<T> formatter, bool decompress)
        {
            if (file == null)
            {
                ThrowHelper.ThrowArgumentNullException("file");
            }
            if (formatter == null)
            {
                ThrowHelper.ThrowArgumentNullException("formatter");
            }

            using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Read<T>(fs, formatter, decompress);
            }
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="formatter">The formatter.</param>
        /// <param name="decompress">if set to <c>true</c> [decompress].</param>
        /// <returns>Data</returns>
        /// <exception cref="System.ArgumentNullException">
        /// stream
        /// or
        /// formatter
        /// </exception>
        public static T Read<T>(Stream stream, IDataFormatter<T> formatter, bool decompress)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }
            if (formatter == null)
            {
                ThrowHelper.ThrowArgumentNullException("formatter");
            }

            T result = default(T);

            if (decompress)
            {
                using (MemoryStream ms = new MemoryStream(COMPRESSION_FORMATTER.Read(stream)))
                {
                    result = formatter.Read(ms);
                }
            }
            else
            {
                result = formatter.Read(stream);
            }

            return result;
        }

        /// <summary>
        /// Writes the specified obj.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="file">The file.</param>
        public static void Write<T>(T obj, FileInfo file)
        {
            Write<T>(obj, file, new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true), DEFAULT_ENABLE_COMPRESS);
        }

        /// <summary>
        /// Writes the specified obj.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="stream">The stream.</param>
        public static void Write<T>(T obj, Stream stream)
        {
            Write<T>(obj, stream, new BinarySerializerFormatter<T>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true), DEFAULT_ENABLE_COMPRESS);
        }

        /// <summary>
        /// Writes the specified obj.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="file">The file.</param>
        /// <param name="formatter">The formatter.</param>
        public static void Write<T>(T obj, FileInfo file, IDataFormatter<T> formatter)
        {
            Write<T>(obj, file, formatter, DEFAULT_ENABLE_COMPRESS);
        }

        /// <summary>
        /// Writes the specified obj.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="formatter">The formatter.</param>
        public static void Write<T>(T obj, Stream stream, IDataFormatter<T> formatter)
        {
            Write<T>(obj, stream, formatter, DEFAULT_ENABLE_COMPRESS);
        }

        /// <summary>
        /// Writes the specified obj.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="file">The file.</param>
        /// <param name="formatter">The formatter.</param>
        /// <param name="compress">if set to <c>true</c> [compress].</param>
        /// <exception cref="System.ArgumentNullException">
        /// file
        /// or
        /// formatter
        /// </exception>
        public static void Write<T>(T obj, FileInfo file, IDataFormatter<T> formatter, bool compress)
        {
            if (file == null)
            {
                ThrowHelper.ThrowArgumentNullException("file");
            }
            if (formatter == null)
            {
                ThrowHelper.ThrowArgumentNullException("formatter");
            }

            using (FileStream fs = new FileStream(file.FullName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                Write<T>(obj, fs, formatter, compress);
            }
        }

        /// <summary>
        /// Writes the specified obj.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="formatter">The formatter.</param>
        /// <param name="compress">if set to <c>true</c> [compress].</param>
        /// <exception cref="System.ArgumentNullException">
        /// stream
        /// or
        /// formatter
        /// </exception>
        public static void Write<T>(T obj, Stream stream, IDataFormatter<T> formatter, bool compress)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }
            if (formatter == null)
            {
                ThrowHelper.ThrowArgumentNullException("formatter");
            }

            if (compress)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Write(ms, obj);
                    COMPRESSION_FORMATTER.Write(stream, ms.ToArray());
                }
            }
            else
            {
                formatter.Write(stream, obj);
            }
        }

    }

}
