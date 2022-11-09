using Forge.Security.Jwt.Shared.Service;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Forge.Security.Jwt.Service
{

    /// <summary>Maintain the token service and remove the expired a tokens.</summary>
    public class JwtTokenMaintenanceHostedService : IHostedService, IDisposable
    {

        private Timer _timer;
        private readonly IJwtManagementService _jwtAuthManager;

        /// <summary>Initializes a new instance of the <see cref="JwtTokenMaintenanceHostedService" /> class.</summary>
        /// <param name="jwtAuthManager">The JWT authentication manager.</param>
        public JwtTokenMaintenanceHostedService(IJwtManagementService jwtAuthManager)
        {
            if (jwtAuthManager == null) throw new ArgumentNullException(nameof(jwtAuthManager));
            _jwtAuthManager = jwtAuthManager;
        }

        /// <summary>Starts the service</summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>
        ///   Task
        /// </returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // remove expired refresh tokens from cache every minute
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        /// <summary>Stops the service</summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>
        ///   Task
        /// </returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();
            _timer = null;
            return Task.CompletedTask;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }

        private void DoWork(object state)
        {
            _jwtAuthManager.RemoveExpiredRefreshTokens(DateTime.UtcNow);
        }

    }

}
