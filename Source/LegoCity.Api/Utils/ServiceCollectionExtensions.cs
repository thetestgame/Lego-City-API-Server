// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Utils
{
    using global::Discord;
    using global::Discord.Interactions;
    using global::Discord.WebSocket;
    using LegoCity.Api.Hubs;
    using LegoCity.Api.Models.Options;
    using LegoCity.Api.Services.Discord;
    using LegoCity.Api.Services.Environment;
    using LegoCity.Api.Services.Lego;
    using LegoCity.Api.Utils.Errors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.OpenApi.Models;
    using SharpBrick.PoweredUp;
    using System.Runtime.InteropServices;

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

            // Add the matching bluetooth handler based on operating system
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                services.AddWinRTBluetooth();
            else
                services.AddBlueGigaBLEBluetooth();

            services.AddSingleton<LegoHubService>();
            services.AddSingleton<LegoTrainService>();
            services.AddHostedService<LegoHubDiscoveryBackgroundService>();
        }

        /// <summary>Configures an <see cref="IServiceCollection"/> instance to support time of day management services.</summary>
        /// <param name="services"><see cref="IServiceCollection"/> to configure.</param>
        /// <param name="section">Section to configure time of day options using.</param>
        public static void AddTimeOfDayServices(this IServiceCollection services, IConfiguration section)
        {
            services.Configure<TimeOfDayOptions>(section);
            services.AddSingleton<TimeOfDayManager>();
            services.AddHostedService<TimeOfDayBackgroundService>();
        }

        /// <summary>Configures an <see cref="IServiceCollection"/> instance to support SignalR hub services.</summary>
        /// <param name="services"><see cref="IServiceCollection"/> to configure.</param>
        public static void AddSignalRHubs(this IServiceCollection services) =>
            services.AddSignalR(o =>
            {                                                                       // Enables our SignalR implementation for communicating with game servers.
                o.EnableDetailedErrors = true;                                      // Enable detailed error reporting.
                o.AddFilter<SignalRHubLoggingFilter>();                             // Adds our custom filter for logging game server hub interaction and exceptions.
            });

        /// <summary>Configures restful api versioning and documentation generation</summary>
        /// <param name="versionDescriptions">Dictionary [version, description] of all available apis to document.</param>
        public static void ConfigureApiVersioning(this IServiceCollection services, Dictionary<string, string>? versionDescriptions = default)
        {
            services.AddApiVersioning(options =>                                    // Enables api versioning across our MVC controllers.
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);                   // Define our default api version
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(setup =>                               // Enables the versioned api explorer for viewing several versions of the EBS api in Swagger
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            if (versionDescriptions != null && versionDescriptions.Count > 0)      // Enables our Swagger documentation generator is requested.
            {
                services.AddSwaggerGen(setup =>
                {                        
                    foreach(var version in versionDescriptions)
                        setup.SwaggerDoc(version.Key, new OpenApiInfo
                        {
                            Version = version.Key,
                            Title = $"Lego City Api {version.Key}",
                            Description = version.Value
                        });
                });
            }
        }
    }
}
