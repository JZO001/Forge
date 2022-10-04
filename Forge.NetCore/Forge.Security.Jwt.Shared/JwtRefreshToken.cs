using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Forge.Security.Jwt.Shared
{

    /// <summary>Represents the refresh token and its details</summary>
    [Serializable]
    public class JwtRefreshToken
    {

        /// <summary>Gets or sets the name of the user.</summary>
        /// <value>The name of the user.</value>
        [JsonPropertyName("username")]
        public string Username { get; set; }

        /// <summary>Gets or sets the secondary keys.
        /// Optionally include other metadata, such as user agent, ip address, device name, and so on</summary>
        /// <value>The secondary keys.</value>
        [JsonPropertyName("secondaryKeys")]
        public List<JwtKeyValuePair> SecondaryKeys { get; set; } = new List<JwtKeyValuePair>();

        /// <summary>Gets or sets the token string.</summary>
        /// <value>The token string.</value>
        [JsonPropertyName("tokenString")]
        public string TokenString { get; set; }

        /// <summary>Gets or sets the expiration time</summary>
        /// <value>The expiration time.</value>
        [JsonPropertyName("expireAt")]
        public DateTime ExpireAt { get; set; }

        /// <summary>Compares the secondary keys to an othet set.</summary>
        /// <param name="keys">The keys.</param>
        /// <returns>True if the sets are the same, otherwise False.</returns>
        public bool CompareSecondaryKeys(IEnumerable<JwtKeyValuePair> keys)
        {
            bool result = (keys == null || keys.ToList().Count == 0) && (SecondaryKeys == null || SecondaryKeys.Count == 0);

            if (!result && keys != null && SecondaryKeys != null && keys.ToList().Count == SecondaryKeys.Count)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                SecondaryKeys.ForEach(k => dict[k.Key] = k.Value);
                result = true;
                foreach (JwtKeyValuePair key in keys)
                {
                    if (dict.ContainsKey(key.Key))
                    {
                        if (dict[key.Key] != key.Value)
                        {
                            result = false;
                            break;
                        }
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

    }

}
