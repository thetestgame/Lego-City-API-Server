﻿// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Utils
{
    using global::Discord;
    using global::Discord.Interactions;
    using global::Discord.WebSocket;
    using LegoCity.Api.Models.Options;
    using LegoCity.Api.Services.Discord;
    using LegoCity.Api.Services.Lego;
    using LegoCity.Api.Utils.Errors;
    using SharpBrick.PoweredUp;

    /// <summary>Static extension methods for the .NET <see cref="IServiceCollection"/> object.</summary>
    public static class ServiceCollectionExtensions
    {
        private static readonly DiscordSocketConfig socketConfig = new()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true,
        };

        /// <summary>Configures an <see cref="IServiceCollection"/> instane to support restful api interactions</summary>
        /// <param name="services"><see cref="IServiceCollection"/> to configure the restful api controllers for.</param>
        public static void AddRestApiControllers(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<HttpResponseExceptionFilter>();
            });

            services.AddHealthChecks();
            services.AddHttpContextAccessor();
        }

        /// <summary>Configures an <see cref="IServiceCollection"/> instane to support Discord bot interactions</summary>
        /// <param name="services"><see cref="IServiceCollection"/> to configure Discord for.</param>
        /// <param name="configuration">Root <see cref="IConfiguration"/> to pull the Discord config from.</param>
        public static void AddDiscordBotSupport(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DiscordOptions>(configuration.GetSection("Discord"));
            services.AddSingleton(socketConfig);
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
            services.AddSingleton<DiscordInteractionHandler>();
            services.AddHostedService<DiscordBotBackgroundService>();
        }

        /// <summary>Configures an <see cref="IServiceCollection"/> instance to support the Lego PoweredUP protocol and its supported services.</summary>
        /// <param name="services"><see cref="IServiceCollection"/> to configure PoweredUp for.</param>
        public static void AddLegoPoweredUpServices(this IServiceCollection services)
        {
            services.AddPoweredUp();
            services.AddWinRTBluetooth();            
            services.AddSingleton<LegoHubService>();
            services.AddSingleton<LegoTrainService>();
            services.AddHostedService<LegoHubDiscoveryBackgroundService>();
        }
    }
}
