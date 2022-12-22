/* *********************************************************************
 * Date: 19 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Security;

namespace Forge
{

    /// <summary>
    /// Helper class to throw common exceptions
    /// </summary>
    public static class ThrowHelper
    {

        /// <summary>
        /// If the null and nulls are illegal then throw.
        /// </summary>
        /// <typeparam name="T">Type to exam</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="argName">Name of the arg.</param>
        public static void IfNullAndNullsAreIllegalThenThrow<T>(object value, string argName)
        {
            if ((value == null) && (default(T) != null))
            {
                ThrowArgumentNullException(argName);
            }
        }

        /// <summary>
        /// Throws the argument exception.
        /// </summary>
        /// <param name="argName">Name of the arg.</param>
        [SecuritySafeCritical]
        public static void ThrowArgumentException(string argName)
        {
            throw new ArgumentException(String.Format("Invalid argument: {0}", argName));
        }

        /// <summary>
        /// Throws the argument exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="argName">Name of the arg.</param>
        [SecuritySafeCritical]
        public static void ThrowArgumentException(string message, string argName)
        {
            throw new ArgumentException(message, argName);
        }

        /// <summary>
        /// Throws the argument null exception.
        /// </summary>
        /// <param name="argName">Name of the arg.</param>
        [SecuritySafeCritical]
        public static void ThrowArgumentNullException(string argName)
        {
            throw new ArgumentNullException(argName);
        }

        /// <summary>
        /// Throws the argument out of range exception.
        /// </summary>
        /// <param name="argName">Name of the arg.</param>
        [SecuritySafeCritical]
        public static void ThrowArgumentOutOfRangeException(string argName)
        {
            throw new ArgumentOutOfRangeException(argName);
        }

        /// <summary>
        /// Throws the wrong value type argument exception.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        [SecuritySafeCritical]
        public static void ThrowWrongValueTypeArgumentException(object value, Type targetType)
        {
            throw new ArgumentException(String.Format("Wrong value type '{0}'. Expected type: {1}", value == null ? "null" : value.GetType().FullName, targetType == null ? "null" : targetType.FullName));
        }

    }

}
