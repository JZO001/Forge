using Forge.Security.Jwt.Shared;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Forge.Security.Jwt.Core
{

    /// <summary>Jwt Management Service implementation</summary>
    public class JwtManagementService : IJwtManagementService
    {

        private readonly ConcurrentDictionary<string, JwtRefreshToken> _usersRefreshTokens;  // can store in a database or a distributed cache
        private readonly JwtTokenConfiguration _jwtTokenConfig;
        private readonly IJwtTokenStorage _tokenStorage;
        private readonly byte[] _secret;

        /// <summary>Initializes a new instance of the <see cref="JwtManagementService" /> class.</summary>
        /// <param name="jwtTokenConfig">The JWT token configuration.</param>
        /// <param name="jwtTokenStorage">The JWT token persistence storage.</param>
        public JwtManagementService(JwtTokenConfiguration jwtTokenConfig, IJwtTokenStorage jwtTokenStorage)
        {
            if (jwtTokenConfig == null) throw new ArgumentNullException(nameof(jwtTokenConfig));
            if (jwtTokenStorage == null) throw new ArgumentNullException(nameof(jwtTokenStorage));

            _jwtTokenConfig = jwtTokenConfig;
            _tokenStorage = jwtTokenStorage;
            _usersRefreshTokens = new ConcurrentDictionary<string, JwtRefreshToken>();
            jwtTokenStorage.Get().ToList().ForEach(token => _usersRefreshTokens.AddOrUpdate(token.TokenString, token, (k, v) => token));
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }

        /// <summary>Removes the expired refresh tokens.</summary>
        /// <param name="now">The time before the tokens are expired</param>
        public void RemoveExpiredRefreshTokens(DateTime now)
        {
            var expiredTokens = _usersRefreshTokens.Where(x => x.Value.ExpireAt < now).ToList();
            foreach (var expiredToken in expiredTokens)
            {
                _usersRefreshTokens.TryRemove(expiredToken.Key, out _);
                _tokenStorage.Remove(expiredToken.Key);
            }
        }

        /// <summary>Removes the refresh token by user name and keys.</summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="secondaryKey">The secondary key.</param>
        public void RemoveRefreshTokenByUserNameAndKeys(string userName, IEnumerable<JwtKeyValuePair> secondaryKey)
        {
            var refreshTokens = _usersRefreshTokens.Where(x => x.Value.Username == userName && x.Value.CompareSecondaryKeys(secondaryKey)).ToList();
            foreach (var refreshToken in refreshTokens)
            {
                _usersRefreshTokens.TryRemove(refreshToken.Key, out _);
                _tokenStorage.Remove(refreshToken.Key);
            }
        }

        /// <summary>Generates a new token.</summary>
        /// <param name="username">The username.
        /// Mandatory.</param>
        /// <param name="claims">The claims.</param>
        /// <param name="now">Indicates, when the token will be activated.</param>
        /// <param name="secondaryKeys">The secondary keys to identify a token.</param>
        /// <returns>Jwt access and refresh token</returns>
        public JwtTokenResult GenerateTokens(string username, Claim[] claims, DateTime now, IEnumerable<JwtKeyValuePair> secondaryKeys)
        {
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtToken = new JwtSecurityToken(
                _jwtTokenConfig.Issuer,
                shouldAddAudienceClaim ? _jwtTokenConfig.Audience : string.Empty,
                claims,
                expires: now.AddMinutes(_jwtTokenConfig.AccessTokenExpirationInMinutes),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshToken = new JwtRefreshToken
            {
                Username = username,
                TokenString = GenerateRefreshTokenString(),
                ExpireAt = now.AddMinutes(_jwtTokenConfig.RefreshTokenExpirationInMinutes)
            };
            if (secondaryKeys != null) refreshToken.SecondaryKeys.AddRange(secondaryKeys);
            _usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (s, t) => refreshToken);
            _tokenStorage.AddOrUpdate(refreshToken);

            return new JwtTokenResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        /// <summary>Validate the specified access and refresh tokens.</summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="now">The time when the refresh token will be active</param>
        /// <param name="secondaryKeys">The secondary keys.</param>
        /// <returns>True, if the tokens are valid, otherwise False.</returns>
        public bool Validate(string refreshToken, string accessToken, DateTime now, IEnumerable<JwtKeyValuePair> secondaryKeys)
        {
            var (principal, jwtToken) = DecodeJwtToken(accessToken);
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                return false;
            }

            string userName = principal.Identity.Name;
            if (!_usersRefreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
            {
                return false;
            }
            if (existingRefreshToken.Username != userName || existingRefreshToken.ExpireAt < now)
            {
                return false;
            }
            if (!existingRefreshToken.CompareSecondaryKeys(secondaryKeys))
            {
                return false;
            }
            return true;
        }

        /// <summary>Generates new access and refresh tokens</summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="now">The time when the refresh token will be active</param>
        /// <param name="secondaryKeys">The secondary keys to identify the refresh token</param>
        /// <returns>Jwt access and refresh token</returns>
        public JwtTokenResult Refresh(string refreshToken, string accessToken, DateTime now, IEnumerable<JwtKeyValuePair> secondaryKeys)
        {
            if (!Validate(refreshToken, accessToken, now, secondaryKeys))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var (principal, jwtToken) = DecodeJwtToken(accessToken);
            string userName = principal.Identity.Name;

            return GenerateTokens(userName, principal.Claims.ToArray(), now, secondaryKeys); // need to recover the original claims
        }

        /// <summary>Decodes the JWT token and get back the stored information</summary>
        /// <param name="token">The token.</param>
        /// <returns>ClaimsPrincipal and JwtSecurityToken</returns>
        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = _jwtTokenConfig.ValidateIssuer,
                        ValidIssuer = _jwtTokenConfig.Issuer,
                        ValidateIssuerSigningKey = _jwtTokenConfig.ValidateIssuerSigningKey,
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidAudience = _jwtTokenConfig.Audience,
                        ValidateAudience = _jwtTokenConfig.ValidateAudience,
                        ValidateLifetime = _jwtTokenConfig.ValidateLifetime,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }

        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }

}
