/* *********************************************************************
 * Date: 11 Jun 2009
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Forge.Persistence.Formatters
{

    /// <summary>
    /// Serialize object into XML format
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [Serializable]
    public sealed class XmlDataFormatter<T> : IDataFormatter<T>
    {

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="XmlDataFormatter{T}"/> class.
        /// </summary>
        static XmlDataFormatter()
        {
            DefaultEncoding = Encoding.UTF8;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataFormatter&lt;T&gt;"/> class.
        /// </summary>
        public XmlDataFormatter()
        {
            Encoding = DefaultEncoding;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the default encoding for XmlDataFormatter instances.
        /// </summary>
        /// <value>
        /// The default encoding.
        /// </value>
        public static Encoding DefaultEncoding { get; set; }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>
        /// The encoding.
        /// </value>
        public Encoding Encoding { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance can read the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>
        ///   <c>true</c> if this instance can read the specified stream; otherwise, <c>false</c>.
        /// </returns>
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
                    XmlTextReader xmlReader = new XmlTextReader(stream);
                    xmlReader.WhitespaceHandling = WhitespaceHandling.Significant;
                    result = new XmlSerializer(typeof(T)).CanDeserialize(xmlReader);
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
                using (StreamWriter sw = new StreamWriter(ms, Encoding))
                {
                    try
                    {
                        new XmlSerializer(typeof(T)).Serialize(sw, item);
                        sw.Flush();
                        result = true;
                    }
                    catch (Exception)
                    {
                    }
                    ms.SetLength(0);
                }
            }
            return result;
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public T Read(Stream stream)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            StreamReader sr = new StreamReader(stream, Encoding);
            try
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(sr);
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

            StreamWriter sw = new StreamWriter(stream, Encoding);
            try
            {
                new XmlSerializer(typeof(T)).Serialize(sw, data);
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
            return new XmlDataFormatter<T>() { Encoding = this.Encoding };
        }

        #endregion

    }

}
