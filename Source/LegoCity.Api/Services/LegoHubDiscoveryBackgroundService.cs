// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Services
{
    using SharpBrick.PoweredUp;

    /// <summary>A background service that discovers and connects Lego Hubs automatically.</summary>
    internal sealed class LegoHubDiscoveryBackgroundService : BackgroundService
    {
        private readonly ILogger<LegoHubDiscoveryBackgroundService> logger;
        private readonly PoweredUpHost poweredUpHost;

        public LegoHubDiscoveryBackgroundService(ILogger<LegoHubDiscoveryBackgroundService> logger, PoweredUpHost poweredUpHost)
        {
            this.logger = logger;
            this.poweredUpHost = poweredUpHost;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) 
        {
            this.poweredUpHost.Discover(async hub => await this.HandleDiscoveredHubAsync(hub), stoppingToken);
            return Task.CompletedTask;
        }

        private async Task HandleDiscoveredHubAsync(SharpBrick.PoweredUp.Hub hub)
        {
            try { 
                await hub.ConnectAsync();
                this.logger.LogInformation($"Connected to hub: {hub.AdvertisingName} ({hub.SystemType})");
            }
            catch { this.logger.LogWarning($"Failed to connect to hub: {hub.AdvertisingName} ({hub.SystemType})"); }
        }
    }
}
