using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Forge.Security.Jwt.Shared
{

    /// <summary>Help parse an access token</summary>
    public static class JwtParserHelper
    {

        /// <summary>Parses JWT access token to get the claims.</summary>
        /// <param name="jwtAccessToken">The JWT access token.</param>
        /// <returns>List of extracted Claim(s)</returns>
        public static List<Claim> ParseClaimsFromJwt(string jwtAccessToken)
        {
            List<Claim> claims = new List<Claim>();
            if (!string.IsNullOrEmpty(jwtAccessToken) && jwtAccessToken.Length > 0)
            {
                var payload = jwtAccessToken.Split('.')[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

                if (roles != null)
                {
                    if (roles.ToString().Trim().StartsWith("["))
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                        foreach (var parsedRole in parsedRoles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                    }

                    keyValuePairs.Remove(ClaimTypes.Role);
                }

                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            }
            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

    }

}
