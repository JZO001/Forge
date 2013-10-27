/* *********************************************************************
 * Date: 11 Jun 2009
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using log4net;

namespace Forge.Persistence.Formatters
{

    /// <summary>
    /// Serialize object into XML format
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [Serializable]
    public sealed class XmlDataFormatter<T> : IDataFormatter<T>
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(XmlDataFormatter<T>));

        private static Dictionary<Type, XmlSerializer> mSerializers = new Dictionary<Type, XmlSerializer>();
        private static ReaderWriterLock mLock = new ReaderWriterLock();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataFormatter&lt;T&gt;"/> class.
        /// </summary>
        public XmlDataFormatter()
        {
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
                    result = GetSerializer(typeof(T)).CanDeserialize(xmlReader);
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
                    GetSerializer(typeof(T)).Serialize(ms, item);
                    result = true;
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
        /// <returns></returns>
        public T Read(Stream stream)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            XmlTextReader xmlReader = new XmlTextReader(stream);
            xmlReader.WhitespaceHandling = WhitespaceHandling.Significant;
            try
            {
                return (T)GetSerializer(typeof(T)).Deserialize(xmlReader);
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }
            finally
            {
                xmlReader.Close();
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
                GetSerializer(typeof(T)).Serialize(stream, data);
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

        #region Protected members

        private static XmlSerializer GetSerializer(Type type)
        {
            XmlSerializer result = null;
            mLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (mSerializers.ContainsKey(type))
                {
                    result = mSerializers[type];
                }
                else
                {
                    result = new XmlSerializer(type);
                    mLock.UpgradeToWriterLock(Timeout.Infinite);
                    mSerializers[type] = result;
                }
            }
            catch (Exception ex)
            {
                if (LOGGER.IsErrorEnabled) LOGGER.Error(String.Format("XmlDataFormatter: unable to create serializer for type '{0}'. Exception: {1}", type.AssemblyQualifiedName, ex.Message));
                throw;
            }
            finally
            {
                if (mLock.IsWriterLockHeld)
                {
                    mLock.ReleaseWriterLock();
                }
                else
                {
                    mLock.ReleaseReaderLock();
                }
            }
            return result;
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
            return new XmlDataFormatter<T>();
        }

        #endregion

    }

}
