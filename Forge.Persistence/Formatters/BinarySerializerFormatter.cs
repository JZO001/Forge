/* *********************************************************************
 * Date: 11 Jun 2009
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using Forge.Persistence.Serialization;
using Forge.Reflection;

namespace Forge.Persistence.Formatters
{

    /// <summary>
    /// Binary serializer formatter
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public sealed class BinarySerializerFormatter<T> : IDataFormatter<T>
    {

        #region Field(s)

        private BinarySerializer mFormatter = new BinarySerializer();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySerializerFormatter{T}"/> class.
        /// </summary>
        public BinarySerializerFormatter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySerializerFormatter{T}"/> class.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public BinarySerializerFormatter(BinarySerializerBehaviorEnum behavior)
        {
            this.mFormatter.SerializerBehavior = behavior;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySerializerFormatter{T}"/> class.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <param name="typeLookupMode">The type lookup mode.</param>
        /// <param name="findNewestTypeVersion">if set to <c>true</c> [find newest type version].</param>
        public BinarySerializerFormatter(BinarySerializerBehaviorEnum behavior, TypeLookupModeEnum typeLookupMode, bool findNewestTypeVersion)
        {
            this.mFormatter.SerializerBehavior = behavior;
            this.mFormatter.TypeLookupMode = typeLookupMode;
            this.mFormatter.FindNewestTypeVersion = findNewestTypeVersion;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public StreamingContext Context
        {
            get { return this.mFormatter.Context; }
            set { this.mFormatter.Context = value; }
        }

        /// <summary>
        /// Gets or sets the selector.
        /// </summary>
        /// <value>
        /// The selector.
        /// </value>
        public ISurrogateSelector Selector
        {
            get { return this.mFormatter.Selector; }
            set { this.mFormatter.Selector = value; }
        }

        /// <summary>
        /// Gets or sets the serializer behavior.
        /// </summary>
        /// <value>
        /// The serializer behavior.
        /// </value>
        [DefaultValue(0)]
        public BinarySerializerBehaviorEnum SerializerBehavior
        {
            get { return this.mFormatter.SerializerBehavior; }
            set { this.mFormatter.SerializerBehavior = value; }
        }

        /// <summary>
        /// Gets or sets the type lookup mode.
        /// </summary>
        /// <value>
        /// The type lookup mode.
        /// </value>
        [DefaultValue(2)]
        public TypeLookupModeEnum TypeLookupMode
        {
            get { return this.mFormatter.TypeLookupMode; }
            set { this.mFormatter.TypeLookupMode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [find newest type version].
        /// </summary>
        /// <value>
        /// <c>true</c> if [find newest type version]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool FindNewestTypeVersion
        {
            get { return this.mFormatter.FindNewestTypeVersion; }
            set { this.mFormatter.FindNewestTypeVersion = value; }
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
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
            return new BinarySerializerFormatter<T>(this.SerializerBehavior, this.TypeLookupMode, this.FindNewestTypeVersion) { Context = this.Context, Selector = this.Selector };
        }

        #endregion

    }

}
