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
    /// An sbyte value that may be updated atomically.  See the
    /// An AtomicSByte is used in applications such as atomically
    /// incremented sequence numbers, and cannot be used as a replacement
    /// for an sbyte.
    /// </summary>
    [Serializable]
    public sealed class AtomicSByte : MBRBase
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private sbyte mValue = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicSByte"/> class.
        /// </summary>
        public AtomicSByte()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicSByte"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public AtomicSByte(sbyte value)
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
        /// <exception cref="System.OverflowException">value is greater than SByte.MaxValue ot less than SByte.MinValue</exception>
        [DebuggerHidden]
        public long LongValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return mValue; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToSByte(value); }
        }

        /// <summary>
        /// Gets or sets the int value.
        /// </summary>
        /// <value>
        /// The int value.
        /// </value>
        /// <exception cref="System.OverflowException">value is greater than SByte.MaxValue ot less than SByte.MinValue</exception>
        [DebuggerHidden]
        public int IntValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return mValue; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToSByte(value); }
        }

        /// <summary>
        /// Gets or sets the short value.
        /// </summary>
        /// <value>
        /// The short value.
        /// </value>
        /// <exception cref="System.OverflowException">value is greater than SByte.MaxValue ot less than SByte.MinValue</exception>
        [DebuggerHidden]
        public short ShortValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToInt16(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToSByte(value); }
        }

        /// <summary>
        /// Gets or sets the float value.
        /// </summary>
        /// <value>
        /// The float value.
        /// </value>
        /// <exception cref="System.OverflowException">value is greater than SByte.MaxValue ot less than SByte.MinValue</exception>
        [DebuggerHidden]
        public float FloatValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToSingle(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToSByte(value); }
        }

        /// <summary>
        /// Gets or sets the double value.
        /// </summary>
        /// <value>
        /// The double value.
        /// </value>
        /// <exception cref="System.OverflowException">value is greater than SByte.MaxValue ot less than SByte.MinValue</exception>
        [DebuggerHidden]
        public double DoubleValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToDouble(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToSByte(value); }
        }

        /// <summary>
        /// Gets or sets the decimal value.
        /// </summary>
        /// <value>
        /// The decimal value.
        /// </value>
        /// <exception cref="System.OverflowException">value is greater than SByte.MaxValue ot less than SByte.MinValue</exception>
        [DebuggerHidden]
        public decimal DecimalValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return Convert.ToDecimal(mValue); }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { mValue = Convert.ToSByte(value); }
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
            set { mValue = Convert.ToSByte(value); }
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
            set { mValue = Convert.ToSByte(value); }
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
            set { mValue = Convert.ToSByte(value); }
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
            set { mValue = Convert.ToSByte(value); }
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
            set { mValue = Convert.ToSByte(value); }
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
        public sbyte GetAndSet(sbyte newValue)
        {
            sbyte result = mValue;
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
        public sbyte GetAndIncrement()
        {
            sbyte result = mValue;
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
        public sbyte GetAndDecrement()
        {
            sbyte result = mValue;
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
        public sbyte GetAndAdd(sbyte delta)
        {
            sbyte result = mValue;
            mValue = Convert.ToSByte(mValue + delta);
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
        public sbyte IncrementAndGet()
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
        public sbyte DecrementAndGet()
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
        public sbyte AddAndGet(sbyte delta)
        {
            mValue = Convert.ToSByte(mValue + delta);
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
