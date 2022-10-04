using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.Security.Jwt.Shared
{

    /// <summary>
    /// Default memory token storage
    /// </summary>
    /// <seealso cref="Forge.Security.Jwt.Shared.IJwtTokenStorage" />
    public class JwtTokenMemoryStorage : IJwtTokenStorage
    {

        private readonly ConcurrentDictionary<string, JwtRefreshToken> _refreshTokens = new ConcurrentDictionary<string, JwtRefreshToken>();

        /// <summary>Initializes a new instance of the <see cref="JwtTokenMemoryStorage" /> class.</summary>
        public JwtTokenMemoryStorage()
        {
        }

        /// <summary>Add a token.</summary>
        /// <param name="token">The token.</param>
        public void AddOrUpdate(JwtRefreshToken token)
        {
            _refreshTokens.AddOrUpdate(token.TokenString, token, (s, t) => token);
        }

        /// <summary>Gets stored token</summary>
        /// <returns>List of tokens</returns>
        public IEnumerable<JwtRefreshToken> Get()
        {
            return _refreshTokens.Values;
        }

        /// <summary>Removes a token.</summary>
        /// <param name="key">The token key.</param>
        /// <returns>True, if the token successfully removed, otherwise False.</returns>
        public bool Remove(string key)
        {
            return _refreshTokens.TryRemove(key, out _);
        }

    }

}
