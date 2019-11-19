/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.IO;

namespace Forge.Net.Remoting.Messaging
{

    /// <summary>
    /// Represents a formal parameter item of a method
    /// </summary>
    [Serializable]
    public class MethodParameter
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mId = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mClassName = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object mValue = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mSize = -1;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodParameter"/> class.
        /// </summary>
        protected MethodParameter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodParameter"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="value">The value.</param>
        public MethodParameter(int id, String className, Object value)
        {
            if (string.IsNullOrEmpty(className))
            {
                ThrowHelper.ThrowArgumentNullException("className");
            }
            this.mId = id;
            this.mClassName = className;
            this.mValue = value;
            if (value != null && (value as Stream) != null)
            {
                try
                {
                    Stream stream = (Stream)value;
                    this.mSize = Convert.ToInt32(stream.Length - stream.Position);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Unable to detect available data of the Stream.", "value", ex);
                }
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [DebuggerHidden]
        public int Id
        {
            get { return mId; }
        }

        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        /// <value>
        /// The name of the class.
        /// </value>
        [DebuggerHidden]
        public string ClassName
        {
            get { return mClassName; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DebuggerHidden]
        public object Value
        {
            get { return mValue; }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        [DebuggerHidden]
        public int Size
        {
            get { return mSize; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Sets the value to null.
        /// </summary>
        public void SetValueToNull()
        {
            this.mValue = null;
        }

        /// <summary>
        /// Sets the value to stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.ArgumentException">Stream parameter not allowed. Original value was not a stream.;stream</exception>
        public void SetValueToStream(Stream stream)
        {
            if (this.mSize < 0)
            {
                throw new ArgumentException("Stream parameter not allowed. Original value was not a stream.", "stream");
            }
            this.mValue = stream;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            MethodParameter other = (MethodParameter)obj;
            return other.mId == mId &&
                other.mClassName == mClassName &&
                other.mValue == mValue &&
                other.mSize == mSize;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

    }

}
