/* *********************************************************************
 * Date: 19 Jul 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Forge.Threading
{

    /// <summary>
    /// A short value that may be updated atomically.  See the
    /// An AtomicShort is used in applications such as atomically
    /// incremented sequence numbers, and cannot be used as a replacement
    /// for an short.
    /// </summary>
    [Serializable]
    public sealed class AtomicShort : MBRBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private short mValue = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicShort"/> class.
        /// </summary>
        public AtomicShort()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicShort"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public AtomicShort(short value)
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
        /// <exception cref="System.OverflowException">value is greater than Int16.MaxValue ot less than Int16.MinValue</exception>
        [DebuggerHidden]
        public long LongValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return mValue; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt16(value); }
        }

        /// <summary>
        /// Gets or sets the int value.
        /// </summary>
        /// <value>
        /// The int value.
        /// </value>
        /// <exception cref="System.OverflowException">value is greater than Int16.MaxValue ot less than Int16.MinValue</exception>
        [DebuggerHidden]
        public int IntValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return mValue; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt16(value); }
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
            set { mValue = value; }
        }

        /// <summary>
        /// Gets or sets the float value.
        /// </summary>
        /// <value>
        /// The float value.
        /// </value>
        /// <exception cref="System.OverflowException">value is greater than Int16.MaxValue ot less than Int16.MinValue</exception>
        [DebuggerHidden]
        public float FloatValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToSingle(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt16(value); }
        }

        /// <summary>
        /// Gets or sets the double value.
        /// </summary>
        /// <value>
        /// The double value.
        /// </value>
        /// <exception cref="System.OverflowException">value is greater than Int16.MaxValue ot less than Int16.MinValue</exception>
        [DebuggerHidden]
        public double DoubleValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToDouble(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt16(value); }
        }

        /// <summary>
        /// Gets or sets the decimal value.
        /// </summary>
        /// <value>
        /// The decimal value.
        /// </value>
        /// <exception cref="System.OverflowException">value is greater than Int16.MaxValue ot less than Int16.MinValue</exception>
        [DebuggerHidden]
        public decimal DecimalValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToDecimal(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt16(value); }
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
            set { mValue = Convert.ToInt16(value); }
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
            set { mValue = Convert.ToInt16(value); }
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
            set { mValue = Convert.ToInt16(value); }
        }

        /// <summary>
        /// Gets or sets the ulong value.
        /// </summary>
        /// <value>
        /// The ulong value.
        /// </value>
        [DebuggerHidden]
        public ulong ULongValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToUInt64(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToInt16(value); }
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
            set { mValue = Convert.ToInt16(value); }
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
        public short GetAndSet(short newValue)
        {
            short result = mValue;
            mValue = newValue;
            return result;
        }

        /// <summary>
        /// Gets the and increment.
        /// </summary>
        /// <returns>
        /// The original value
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerStepThrough]
        public short GetAndIncrement()
        {
            short result = mValue;
            mValue++;
            return result;
        }

        /// <summary>
        /// Gets the and decrement.
        /// </summary>
        /// <returns>
        /// The original value
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerStepThrough]
        public short GetAndDecrement()
        {
            short result = mValue;
            mValue--;
            return result;
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
        public short GetAndAdd(short delta)
        {
            short result = mValue;
            mValue = Convert.ToInt16(mValue + delta);
            return result;
        }

        /// <summary>
        /// Increments the and get.
        /// </summary>
        /// <returns>
        /// The incremented value.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerStepThrough]
        public short IncrementAndGet()
        {
            mValue++;
            return mValue;
        }

        /// <summary>
        /// Decrements the and get.
        /// </summary>
        /// <returns>
        /// The decremented value.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerStepThrough]
        public short DecrementAndGet()
        {
            mValue--;
            return mValue;
        }

        /// <summary>
        /// Adds the and get.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns>The new value</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerStepThrough]
        public short AddAndGet(short delta)
        {
            mValue = Convert.ToInt16(mValue + delta);
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
