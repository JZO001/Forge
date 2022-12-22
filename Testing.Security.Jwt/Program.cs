using Forge.Security.Jwt.Client;
using Forge.Security.Jwt.Client.Services;
using Forge.Security.Jwt.Service;
using Forge.Security.Jwt.Shared;
using Forge.Security.Jwt.Shared.Client.Api;
using Forge.Security.Jwt.Shared.Client.Models;
using Forge.Security.Jwt.Shared.Client.Services;
using Forge.Security.Jwt.Shared.Serialization;
using Forge.Security.Jwt.Shared.Service.Models;
using Forge.Security.Jwt.Shared.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace Forge.Testing.Security.Jwt
{
    internal class Program
    {

        static void Main(string[] args)
        {
            //EventTest test = new EventTest();
            //test.OnEvent += Test_OnEvent;
            //test.Raise();

            Task.Run(() => TestRefreshService(args)).Wait();
            //Console.ReadKey();
        }

        static void TestService()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            var jwtTokenConfig = config.GetSection("JwtTokenConfig").Get<JwtTokenConfiguration>();


            JwtManagementService manager = new JwtManagementService(jwtTokenConfig, new MemoryStorage<JwtRefreshToken>());

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

            var jwtResult = manager.GenerateTokens(userName, claims, DateTime.UtcNow, keys);

            Console.WriteLine(jwtResult.AccessToken);
            Console.WriteLine(jwtResult.RefreshToken);

            if (manager.Validate(jwtResult.RefreshToken, jwtResult.AccessToken, DateTime.UtcNow, keys) != JwtTokenValidationResultEnum.Valid)
            {
                Console.WriteLine("Validation failed.");
            }

            jwtResult = manager.Refresh(jwtResult.RefreshToken, jwtResult.AccessToken, DateTime.UtcNow, keys);

            // logout
            manager.RemoveRefreshTokenByUserNameAndKeys(userName, keys);

            List<Claim> claimsFromToken = JwtParserHelper.ParseClaimsFromJwt(jwtResult.AccessToken);
            Console.WriteLine("Number of claims: " + claimsFromToken.Count.ToString());

        }

        static async Task TestRefreshService(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                await host.StartAsync();
                var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

                ILoggerFactory loggerFactory = host.Services.GetService<ILoggerFactory>()!;
                loggerFactory.AddLog4Net();

                IAuthenticationService authService = host.Services.GetService<IAuthenticationService>()!;
                string devPass = "Passw0rd12345";
                JwtTokenResult loginResult = await authService.AuthenticateUserAsync<Credentials, JwtTokenResult>(new Credentials() { Username = "Admin", Password = devPass });

                string loginResultStr = JsonSerializer.Serialize(loginResult);

                Console.WriteLine("Press a key...");
                Console.ReadKey();

                lifetime.StopApplication();
                await host.WaitForShutdownAsync();
            }
        }

        private static string DEV_URL = "https://localhost:7253";

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                .ConfigureServices((hostContext, services) =>
                {

                    services.AddOptions();

                    services.AddAuthorizationCore();

                    services.AddForgeJwtClientAuthenticationCore((JwtClientAuthenticationCoreOptions options) => {
                        options.BaseAddress = DEV_URL;
                        options.RefreshTokenBeforeExpirationInMilliseconds = 1190000;
                    });

                    /*
                    //services.AddSingleton<IJSInProcessRuntime>(services => (IJSInProcessRuntime)services.GetRequiredService<IJSRuntime>());

                    // add LocalStorage storage provider
                    services.AddScoped<IStorage<ParsedTokenData>, MemoryStorage<ParsedTokenData>>();
                    //services.AddScoped<IStorage<ParsedTokenData>, SessionStorage<ParsedTokenData>>();

                    // add JSON serialization provider for network communication
                    services.AddScoped<ISerializationProvider, SystemTextJsonSerializer>();

                    // optionally configure the default serialization configuration behaviors
                    //services.Configure<SystemTextJsonSerializerOptions>(configureOptions =>
                    //{
                    //});

                    // add communication layer for authentication service
                    services.AddScoped<IApiCommunicationHttpClientFactory, ApiCommunicationHttpClientFactory>(serviceProvider =>
                        new ApiCommunicationHttpClientFactory(new HttpClient { BaseAddress = new Uri(LIVE_URL) })
                    );

                    // optionally configure the default Http request configuration
                    //services.Configure<TokenizedApiCommunicationServiceOptions>(configureOptions =>
                    //{
                    //});

                    services.AddScoped<ITokenizedApiCommunicationService, TokenizedApiCommunicationService>();

                    //services.AddSingleton<JwtTokenAuthenticationStateProvider>();
                    services.AddScoped<AuthenticationStateProvider, JwtTokenAuthenticationStateProvider>();
                    //services.AddScoped<IJwtTokenAuthenticationStateProvider, JwtTokenAuthenticationStateProvider>();

                    // LogoutData is optional, depends on the Authentication Service.
                    // INFO: the guid is not a good solution here, because it always changing at app startup
                    // Try to use a contant value
                    services.AddScoped<IAdditionalData, AdditionalData>(serviceProvider =>
                    {
                        AdditionalData logoutData = new AdditionalData();
                        logoutData.SecondaryKeys.Add(new Forge.Security.Jwt.Shared.Service.Models.JwtKeyValuePair("DeviceId", "7010c030-6a2c-4dc5-86a3-2a9702baa7b3"));
                        return logoutData;
                    });

                    // Info: optionally, I can add data which will be sent at logout to the server
                    // This info maybe the same as at login, for example DeviceId or something,
                    // which helps to make access token more user dependent
                    services.AddScoped<IAuthenticationService, AuthenticationService>(); // this will be injected into the login page

                    // optionally, I can set the relative uri for the Api calls, if different than the default
                    //AuthenticationService.AuthenticationUri = "?";
                    //AuthenticationService.UserInfoUri = "?";
                    //AuthenticationService.LogoutUri = "?";

                    //services.AddScoped<UserContext>();

                    // refresh-token maintenance service
                    services.AddScoped<IJwtTokenRefreshServiceConfiguration, JwtTokenRefreshServiceConfiguration>(factory => new JwtTokenRefreshServiceConfiguration() { RefreshTokenBeforeExpirationInMilliseconds = 1190000 });
                    services.AddHostedService<JwtTokenRefreshHostedService>();
                    */

                });
    }
}

internal class Credentials : IAdditionalData
{

    public string Username { get; set; }

    public string Password { get; set; }

    public List<JwtKeyValuePair> SecondaryKeys { get; set; } = new List<JwtKeyValuePair>();

}
