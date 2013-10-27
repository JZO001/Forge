/* *********************************************************************
 * Date: 8 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Security;
using System.Threading;

namespace Forge
{

    /// <summary>
    /// Common client helper methods
    /// </summary>
    public static class ClientUtils
    {

        /// <summary>
        /// Gets the bit count.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static int GetBitCount(uint x)
        {
            int num = 0;
            while (x > 0)
            {
                x &= x - 1;
                num++;
            }
            return num;
        }

        /// <summary>
        /// Determines whether [is critical exception] [the specified ex].
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>
        ///   <c>true</c> if [is critical exception] [the specified ex]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCriticalException(Exception ex)
        {
            return (ex is NullReferenceException) || (ex is StackOverflowException) || (ex is OutOfMemoryException) || (ex is ThreadAbortException) || (ex is IndexOutOfRangeException) || (ex is AccessViolationException);
        }

        /// <summary>
        /// Determines whether [is enum valid] [the specified enum value].
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="value">The value.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <returns>
        ///   <c>true</c> if [is enum valid] [the specified enum value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue)
        {
            return ((value >= minValue) && (value <= maxValue));
        }

        /// <summary>
        /// Determines whether [is enum valid] [the specified enum value].
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="value">The value.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="maxNumberOfBitsOn">The max number of bits on.</param>
        /// <returns>
        ///   <c>true</c> if [is enum valid] [the specified enum value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue, int maxNumberOfBitsOn)
        {
            return (((value >= minValue) && (value <= maxValue)) && (GetBitCount((uint)value) <= maxNumberOfBitsOn));
        }

        /// <summary>
        /// Determines whether [is enum valid_ masked] [the specified enum value].
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="value">The value.</param>
        /// <param name="mask">The mask.</param>
        /// <returns>
        ///   <c>true</c> if [is enum valid_ masked] [the specified enum value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEnumValid_Masked(Enum enumValue, int value, uint mask)
        {
            return ((value & mask) == value);
        }

        /// <summary>
        /// Determines whether [is enum valid_ not sequential] [the specified enum value].
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="value">The value.</param>
        /// <param name="enumValues">The enum values.</param>
        /// <returns>
        ///   <c>true</c> if [is enum valid_ not sequential] [the specified enum value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEnumValid_NotSequential(Enum enumValue, int value, params int[] enumValues)
        {
            for (int i = 0; i < enumValues.Length; i++)
            {
                if (enumValues[i] == value)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is security or critical exception] [the specified ex].
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>
        ///   <c>true</c> if [is security or critical exception] [the specified ex]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSecurityOrCriticalException(Exception ex)
        {
            return ((ex is SecurityException) || IsCriticalException(ex));
        }

    }

}
