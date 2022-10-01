// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Services
{
    using Discord.WebSocket;
    using Discord;
    using LegoCity.Api.Utils;
    using LegoCity.Api.Models.Options;
    using Microsoft.Extensions.Options;

    /// <summary>Background service to run the configured Discord bot.</summary>
    public class DiscordBotBackgroundService : BackgroundService
    {
        private readonly ILogger<DiscordBotBackgroundService> logger;
        private readonly DiscordSocketClient client;
        private readonly DiscordInteractionHandler interactionHandler;
        private readonly DiscordOptions discordOptions;

        public DiscordBotBackgroundService(ILogger<DiscordBotBackgroundService> logger, IOptions<DiscordOptions> options, DiscordSocketClient client, DiscordInteractionHandler interactionHandler)
        {
            this.logger = logger;
            this.client = client;
            this.interactionHandler = interactionHandler;
            discordOptions = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Configure our logging
            client.Log += logger.LogDiscordMessageAsync;

            // Here we can initialize the service that will register and execute our commands
            await interactionHandler.InitializeAsync();

            // Bot token can be provided from the Configuration object we set up earlier
            if (string.IsNullOrEmpty(discordOptions.BotToken))
                throw new Exception("Bot token is not configured.");

            await client.LoginAsync(TokenType.Bot, discordOptions.BotToken);
            await client.StartAsync();
        }
    }
}
