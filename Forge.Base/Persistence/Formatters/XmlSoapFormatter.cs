/* *********************************************************************
 * Date: 11 Jun 2009
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters;

namespace Forge.Persistence.Formatters
{

    /// <summary>
    /// Soap Formatter
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public sealed class XmlSoapFormatter<T> : IDataFormatter<T>
    {

        #region Field(s)

        private readonly SoapFormatter mFormatter = new SoapFormatter();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSoapFormatter&lt;T&gt;"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods")]
        public XmlSoapFormatter()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the top object format.
        /// </summary>
        /// <value>
        /// The top object format.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods"), DefaultValue(0)]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods"), DefaultValue(0)]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods")]
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
                    mFormatter.Deserialize(stream);
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods")]
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
                    this.mFormatter.Serialize(ms, item);
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods")]
        public T Read(Stream stream)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            try
            {
                return (T)mFormatter.Deserialize(stream);
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods")]
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
                mFormatter.Serialize(stream, data);
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
            return new XmlSoapFormatter<T>();
        }

        #endregion

    }

}
