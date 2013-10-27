/* *********************************************************************
 * Date: 19 Jul 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Forge.Threading
{

    /// <summary>
    /// A long value that may be updated atomically.  See the
    /// An AtomicLong is used in applications such as atomically
    /// incremented sequence numbers, and cannot be used as a replacement
    /// for a long.
    /// </summary>
    [Serializable]
    public sealed class AtomicLong : MBRBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long mValue = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicLong"/> class.
        /// </summary>
        public AtomicLong()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicLong"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public AtomicLong(long value)
        {
            this.mValue = value;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DebuggerHidden]
        public long LongValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return mValue; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = value; }
        }

        /// <summary>
        /// Gets or sets the int value.
        /// </summary>
        /// <value>
        /// The int value.
        /// </value>
        [DebuggerHidden]
        public int IntValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToInt32(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt64(value); }
        }

        /// <summary>
        /// Gets or sets the short value.
        /// </summary>
        /// <value>
        /// The short value.
        /// </value>
        [DebuggerHidden]
        public short ShortValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToInt16(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt64(value); }
        }

        /// <summary>
        /// Gets or sets the float value.
        /// </summary>
        /// <value>
        /// The float value.
        /// </value>
        [DebuggerHidden]
        public float FloatValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToSingle(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt64(value); }
        }

        /// <summary>
        /// Gets or sets the double value.
        /// </summary>
        /// <value>
        /// The double value.
        /// </value>
        [DebuggerHidden]
        public double DoubleValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToDouble(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt64(value); }
        }

        /// <summary>
        /// Gets or sets the decimal value.
        /// </summary>
        /// <value>
        /// The decimal value.
        /// </value>
        [DebuggerHidden]
        public decimal DecimalValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToDecimal(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt64(value); }
        }

        /// <summary>
        /// Gets or sets the byte value.
        /// </summary>
        /// <value>
        /// The sbyte value.
        /// </value>
        [DebuggerHidden]
        public byte ByteValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToByte(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt64(value); }
        }

        /// <summary>
        /// Gets or sets the ushort value.
        /// </summary>
        /// <value>
        /// The ushort value.
        /// </value>
        [DebuggerHidden]
        public ushort UShortValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToUInt16(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt64(value); }
        }

        /// <summary>
        /// Gets or sets the uint value.
        /// </summary>
        /// <value>
        /// The uint value.
        /// </value>
        [DebuggerHidden]
        public uint UIntValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToUInt32(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt64(value); }
        }

        /// <summary>
        /// Gets or sets the ulong value.
        /// </summary>
        /// <value>
        /// The ulong value.
        /// </value>
        /// <exception cref="System.OverflowException">value is greater than long.MaxValue ot less than long.MinValue</exception>
        [DebuggerHidden]
        public ulong ULongValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToUInt64(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt64(value); }
        }

        /// <summary>
        /// Gets or sets the sbyte value.
        /// </summary>
        /// <value>
        /// The sbyte value.
        /// </value>
        [DebuggerHidden]
        public sbyte SByteValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToSByte(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt64(value); }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Gets the and set.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        /// The original value
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerStepThrough]
        public long GetAndSet(long newValue)
        {
            return Interlocked.Exchange(ref mValue, newValue);
        }

        /// <summary>
        /// Gets the and increment.
        /// </summary>
        /// <returns>
        /// The original value
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerStepThrough]
        public long GetAndIncrement()
        {
            return Interlocked.Exchange(ref mValue, mValue + 1);
        }

        /// <summary>
        /// Gets the and decrement.
        /// </summary>
        /// <returns>
        /// The original value
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerStepThrough]
        public long GetAndDecrement()
        {
            return Interlocked.Exchange(ref mValue, mValue - 1);
        }

        /// <summary>
        /// Gets the and add.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns>
        /// The original value
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerStepThrough]
        public long GetAndAdd(long delta)
        {
            return Interlocked.Exchange(ref mValue, mValue + delta);
        }

        /// <summary>
        /// Increments the and get.
        /// </summary>
        /// <returns>
        /// The incremented value.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerStepThrough]
        public long IncrementAndGet()
        {
            return Interlocked.Increment(ref mValue);
        }

        /// <summary>
        /// Decrements the and get.
        /// </summary>
        /// <returns>
        /// The decremented value.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerStepThrough]
        public long DecrementAndGet()
        {
            return Interlocked.Decrement(ref mValue);
        }

        /// <summary>
        /// Adds the and get.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns>The new value</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerStepThrough]
        public long AddAndGet(long delta)
        {
            mValue = mValue + delta;
            return mValue;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return mValue.ToString();
        }

        #endregion

    }

}
