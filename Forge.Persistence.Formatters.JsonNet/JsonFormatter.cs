/* *********************************************************************
 * Date: 18 Feb 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using Forge.Formatters;
using Forge.Shared;
using Newtonsoft.Json;

namespace Forge.Persistence.Formatters.JsonNet
{

    /// <summary>
    /// Json.NET formatter
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public sealed class JsonFormatter<T> : IDataFormatter<T>
    {

        #region Field(s)

        private JsonSerializerSettings mSerializerSettings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DefaultValueHandling = DefaultValueHandling.Populate,
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Include,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };

        private JsonSerializer mSerializer = null;

        private readonly object mLockObject = new object();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFormatter{T}"/> class.
        /// </summary>
        public JsonFormatter()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the serializer settings.
        /// </summary>
        /// <value>
        /// The serializer settings.
        /// </value>
        public JsonSerializerSettings SerializerSettings
        {
            get { return mSerializerSettings; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowArgumentNullException("value");
                }

                mSerializerSettings = value;
                lock (mLockObject)
                {
                    mSerializer = null;
                }
            }
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
        /// <exception cref="System.ArgumentNullException">stream</exception>
        public bool CanRead(Stream stream)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            bool result = false;
            try
            {
                long pos = stream.Position;
                try
                {
                    StreamReader sr = new StreamReader(stream);
                    JsonTextReader jsonReader = new JsonTextReader(sr);
                    GetSerializer().Deserialize<T>(jsonReader);
                    result = true;
                }
                catch (Exception)
                {
                }
                finally
                {
                    stream.Position = pos;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// Determines whether this instance can write the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if this instance can write the specified item; otherwise, <c>false</c>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public bool CanWrite(T item)
        {
            if (item == null)
            {
                ThrowHelper.ThrowArgumentNullException("item");
            }

            bool result = false;
            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(ms))
                    {
                        using (JsonTextWriter writer = new JsonTextWriter(sw))
                        {
                            GetSerializer().Serialize(writer, item);
                            writer.Flush();
                            result = true;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Data</returns>
        /// <exception cref="System.FormatException"></exception>
        public T Read(Stream stream)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            try
            {
                StreamReader sr = new StreamReader(stream);
                JsonTextReader jsonReader = new JsonTextReader(sr);
                return GetSerializer().Deserialize<T>(jsonReader);
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }
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
        /// <exception cref="System.FormatException"></exception>
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

            try
            {
                StreamWriter sw = new StreamWriter(stream);
                JsonTextWriter writer = new JsonTextWriter(sw);
                GetSerializer().Serialize(writer, data);
                writer.Flush();
                sw.Flush();
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new JsonFormatter<T>();
        }

        #endregion

        #region Private method(s)

        private JsonSerializer GetSerializer()
        {
            lock (mLockObject)
            {
                if (mSerializer == null)
                {
                    mSerializer = JsonSerializer.Create(mSerializerSettings);
                }
                return mSerializer;
            }
        }

        #endregion

    }

}
