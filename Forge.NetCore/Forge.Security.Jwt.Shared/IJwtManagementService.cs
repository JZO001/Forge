using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Forge.Security.Jwt.Shared
{

    /// <summary>Jwt Management Service declaration</summary>
    public interface IJwtManagementService
    {

        /// <summary>Generates a new token.</summary>
        /// <param name="username">The username.
        /// Mandatory.</param>
        /// <param name="claims">The claims.</param>
        /// <param name="now">Indicates, when the token will be activated.</param>
        /// <param name="secondaryKeys">The secondary keys to identify a token.</param>
        /// <returns>Jwt access and refresh token</returns>
        JwtTokenResult GenerateTokens(string username, Claim[] claims, DateTime now, IEnumerable<JwtKeyValuePair> secondaryKeys);

        /// <summary>Generates new access and refresh tokens</summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="now">The time when the refresh token will be active</param>
        /// <param name="secondaryKeys">The secondary keys to identify the refresh token</param>
        /// <returns>Jwt access and refresh token</returns>
        JwtTokenResult Refresh(string refreshToken, string accessToken, DateTime now, IEnumerable<JwtKeyValuePair> secondaryKeys);

        /// <summary>Removes the expired refresh tokens.</summary>
        /// <param name="now">The time before the tokens are expired</param>
        void RemoveExpiredRefreshTokens(DateTime now);

        /// <summary>Removes the refresh token by user name and keys.</summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="secondaryKey">The secondary key.</param>
        void RemoveRefreshTokenByUserNameAndKeys(string userName, IEnumerable<JwtKeyValuePair> secondaryKey);

        /// <summary>Decodes the JWT token and get back the stored information</summary>
        /// <param name="token">The token.</param>
        /// <returns>ClaimsPrincipal and JwtSecurityToken</returns>
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);

    }

}
