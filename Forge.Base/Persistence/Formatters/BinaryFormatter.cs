/* *********************************************************************
 * Date: 11 Jun 2009
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace Forge.Persistence.Formatters
{

    /// <summary>
    /// Serialize data into binary format
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public sealed class BinaryFormatter<T> : IDataFormatter<T>
    {

        #region Field(s)

        private BinaryFormatter mFormatter = new BinaryFormatter();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormatter&lt;T&gt;"/> class.
        /// </summary>
        public BinaryFormatter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormatter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="topObjectFormat">The top object format.</param>
        /// <param name="typeFormat">The type format.</param>
        public BinaryFormatter(FormatterAssemblyStyle topObjectFormat, FormatterTypeStyle typeFormat)
        {
            this.mFormatter.AssemblyFormat = topObjectFormat;
            this.mFormatter.TypeFormat = typeFormat;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the top object format.
        /// </summary>
        /// <value>
        /// The top object format.
        /// </value>
        [DefaultValue(0)]
        public FormatterAssemblyStyle TopObjectFormat
        {
            get { return this.mFormatter.AssemblyFormat; }
            set { this.mFormatter.AssemblyFormat = value; }
        }

        /// <summary>
        /// Gets or sets the type format.
        /// </summary>
        /// <value>
        /// The type format.
        /// </value>
        [DefaultValue(0)]
        public FormatterTypeStyle TypeFormat
        {
            get { return this.mFormatter.TypeFormat; }
            set { this.mFormatter.TypeFormat = value; }
        }

        #endregion

        #region Public methods

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

            bool result = true;
            try
            {
                long pos = stream.Position;
                try
                {
                    this.mFormatter.Deserialize(stream);
                }
                catch (Exception)
                {
                    result = false;
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
        /// <exception cref="System.ArgumentNullException">item</exception>
        public bool CanWrite(T item)
        {
            if (item == null)
            {
                ThrowHelper.ThrowArgumentNullException("item");
            }

            bool result = false;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    this.mFormatter.Serialize(ms, item);
                    result = true;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        /// <exception cref="System.FormatException"></exception>
        public T Read(Stream stream)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            try
            {
                return (T)this.mFormatter.Deserialize(stream);
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

        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="data">The data.</param>
        /// <exception cref="System.ArgumentNullException">
        /// stream
        /// or
        /// data
        /// </exception>
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
                this.mFormatter.Serialize(stream, data);
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
            return new BinaryFormatter<T>(this.TopObjectFormat, this.TypeFormat);
        }

        #endregion

    }

}
