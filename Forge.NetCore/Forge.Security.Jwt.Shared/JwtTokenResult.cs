using System;
using System.Text.Json.Serialization;

namespace Forge.Security.Jwt.Shared
{

    /// <summary>The result of the token generator</summary>
    [Serializable]
    public class JwtTokenResult
    {
        /// <summary>Gets or sets the access token.</summary>
        /// <value>The access token.</value>
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        /// <summary>Gets or sets the refresh token.</summary>
        /// <value>The refresh token.</value>
        [JsonPropertyName("refreshToken")]
        public JwtRefreshToken RefreshToken { get; set; }
    }

}
