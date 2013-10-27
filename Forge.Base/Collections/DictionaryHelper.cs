/* *********************************************************************
 * Date: 28 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;
using log4net;

namespace Forge.Collections
{

    /// <summary>
    /// Contains the helper methods for a dictionary
    /// </summary>
    public static class DictionaryHelper
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(DictionaryHelper));

        /// <summary>
        /// Gets the dictionary value.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value</returns>
        public static TValue GetDictionaryValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return GetDictionaryValue(dictionary, key, true);
        }

        /// <summary>
        /// Gets the dictionary value.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="throwOnMissingKey">if set to <c>true</c> [throw on missing key].</param>
        /// <returns>
        /// The value
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Occurs, if the throwOnMissingKey is true and the key no found.</exception>
        public static TValue GetDictionaryValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, bool throwOnMissingKey)
        {
            TValue value = default(TValue);
            if (dictionary.ContainsKey(key))
            {
                value = dictionary[key];
            }
            else
            {
                string errorMessage = string.Format("Dictionary of type {0} did not contain the following key: {1}", dictionary.GetType(), key);
                if (throwOnMissingKey)
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(errorMessage);
                    throw new KeyNotFoundException(errorMessage);
                }
                else
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(errorMessage);
                }
            }
            return value;
        }

    }

}
