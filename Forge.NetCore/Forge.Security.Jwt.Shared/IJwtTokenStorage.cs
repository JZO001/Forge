using System.Collections.Generic;

namespace Forge.Security.Jwt.Shared
{

    /// <summary>Token persistent storage interface</summary>
    public interface IJwtTokenStorage
    {

        /// <summary>Gets stored token</summary>
        /// <returns>List of tokens</returns>
        IEnumerable<JwtRefreshToken> Get();

        /// <summary>Add a token.</summary>
        /// <param name="token">The token.</param>
        void AddOrUpdate(JwtRefreshToken token);

        /// <summary>Removes a token.</summary>
        /// <param name="key">The token key.</param>
        /// <returns>True, if the token successfully removed, otherwise False.</returns>
        bool Remove(string key);

    }

}
