using Forge.Security.Jwt.Core;
using Forge.Security.Jwt.Shared;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Claims;

namespace Forge.Testing.Security.Jwt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            var jwtTokenConfig = config.GetSection("JwtTokenConfig").Get<JwtTokenConfiguration>();


            JwtManagementService manager = new JwtManagementService(jwtTokenConfig);

            string userId = "database_id";
            string userName = "zoltan";
            string role = "admin";
            IPAddress remoteIpAddress = IPAddress.Parse("192.168.0.1");
            string userAgent = "Chrome useragent stuff";

            List<JwtKeyValuePair> keys = new List<JwtKeyValuePair>();
            keys.Add(new JwtKeyValuePair(nameof(remoteIpAddress), remoteIpAddress.ToString()));
            keys.Add(new JwtKeyValuePair(nameof(userAgent), userAgent));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, userId),
            };

            var jwtResult = manager.GenerateTokens(userName, claims, DateTime.Now, keys);

            Console.WriteLine(jwtResult.AccessToken);
            Console.WriteLine(jwtResult.RefreshToken.TokenString);

            jwtResult = manager.Refresh(jwtResult.RefreshToken.TokenString, jwtResult.AccessToken, DateTime.Now, keys);

            // logout
            manager.RemoveRefreshTokenByUserNameAndKeys(userName, keys);

            List<Claim> claimsFromToken = JwtParserHelper.ParseClaimsFromJwt(jwtResult.AccessToken);
            Console.WriteLine("Number of claims: " + claimsFromToken.Count.ToString());

        }
    }
}