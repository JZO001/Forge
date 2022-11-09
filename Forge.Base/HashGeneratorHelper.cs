/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Security.Cryptography;
using System.Text;

namespace Forge
{

    /// <summary>
    /// Helps generate hash codes
    /// </summary>
    public static class HashGeneratorHelper
    {

        /// <summary>
        /// Gets the SHA 256 based value of the input string and convert it to a long representation.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The long value</returns>
        /// <example>
        /// <code>
        /// public EntityVersion(string deviceId)
        /// {
        ///     if (String.IsNullOrEmpty(deviceId))
        ///     {
        ///         ThrowHelper.ThrowArgumentNullException("deviceId");
        ///     }
        ///     this.versionDeviceId = EFUtils.GetSHA256BasedValue(deviceId);
        /// }
        /// </code>
        /// </example>
        public static long GetSHA256BasedValue(string input)
        {
            if (input == null)
            {
                ThrowHelper.ThrowArgumentNullException("input");
            }

            long result = 0;
#if NET6_0_OR_GREATER
            using (SHA256 sha256 = SHA256.Create())
#else
            using (SHA256Managed sha256 = new SHA256Managed())
#endif
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                int hashCode = input.GetHashCode();
                foreach (byte b in hash)
                {
                    result += b.GetHashCode() * hashCode;
                }
            }

            return Math.Abs(result);
        }

    }

}
