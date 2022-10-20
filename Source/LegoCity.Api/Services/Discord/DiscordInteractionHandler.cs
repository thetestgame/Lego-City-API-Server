// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Services.Discord
{
    using global::Discord;
    using global::Discord.Interactions;
    using global::Discord.WebSocket;
    using LegoCity.Api;
    using LegoCity.Api.Models.Options;
    using LegoCity.Api.Utils;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>A service that handles Discord interactions through the configured Discord bot.</summary>
    public class DiscordInteractionHandler
    {
        private readonly ILogger<DiscordInteractionHandler> logger;
        private readonly DiscordOptions discordOptions;
        private readonly DiscordSocketClient client;
        private readonly InteractionService handler;
        private readonly IServiceProvider services;
        private readonly IConfiguration configuration;

        public DiscordInteractionHandler(ILogger<DiscordInteractionHandler> logger, IOptions<DiscordOptions> options, 
            DiscordSocketClient client, InteractionService handler, IServiceProvider services, IConfiguration config)
        {
            this.logger = logger;
            this.client = client;
            this.handler = handler;
            this.services = services;
            this.configuration = config;
            this.discordOptions = options.Value;
        }
        
        public async Task InitializeAsync()
        {
            // Process when the client is ready, so we can register our commands.
            this.client.Ready += this.ReadyAsync;
            this.handler.Log += this.logger.LogDiscordMessageAsync;

            // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await this.handler.AddModulesAsync(Assembly.GetEntryAssembly(), services);

            // Process the InteractionCreated payloads to execute Interactions commands
            this.client.InteractionCreated += HandleInteraction;
        }

        private async Task ReadyAsync()
        {
            // Context & Slash commands can be automatically registered, but this process needs to happen after the client enters the READY state.
            // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands.
            if (Program.IsDebug())
            {
                if (this.discordOptions.TestGuild <= 0)
                    throw new Exception("Invalid Discord TestGuild configured. Please configure a proper Discord guild id for testing");
                
                await this.handler.RegisterCommandsToGuildAsync(this.discordOptions.TestGuild, true);
            }
            else
                await this.handler.RegisterCommandsGloballyAsync(true);
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
                var context = new SocketInteractionContext(this.client, interaction);

                // Execute the incoming command.
                var result = await this.handler.ExecuteCommandAsync(context, this.services);

                if (!result.IsSuccess)
                    switch (result.Error)
                    {
                        case InteractionCommandError.UnmetPrecondition:
                            // implement
                            break;
                        default:
                            break;
                    }
            }
            catch
            {
                // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (interaction.Type is InteractionType.ApplicationCommand)
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}