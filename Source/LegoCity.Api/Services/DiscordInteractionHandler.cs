// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Services
{
    using Discord;
    using Discord.Interactions;
    using Discord.WebSocket;
    using LegoCity.Api;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>A service that handles Discord interactions through the configured Discord bot.</summary>
    public class DiscordInteractionHandler
    {
        private readonly DiscordSocketClient client;
        private readonly InteractionService handler;
        private readonly IServiceProvider services;
        private readonly IConfiguration configuration;

        public DiscordInteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services, IConfiguration config)
        {
            this.client = client;
            this.handler = handler;
            this.services = services;
            configuration = config;
        }

        public async Task InitializeAsync()
        {
            // Process when the client is ready, so we can register our commands.
            client.Ready += ReadyAsync;
            handler.Log += LogAsync;

            // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await handler.AddModulesAsync(Assembly.GetEntryAssembly(), services);

            // Process the InteractionCreated payloads to execute Interactions commands
            client.InteractionCreated += HandleInteraction;
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            // Context & Slash commands can be automatically registered, but this process needs to happen after the client enters the READY state.
            // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands.
            if (Program.IsDebug())
                await handler.RegisterCommandsToGuildAsync(configuration.GetValue<ulong>("testGuild"), true);
            else
                await handler.RegisterCommandsGloballyAsync(true);
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
                var context = new SocketInteractionContext(client, interaction);

                // Execute the incoming command.
                var result = await handler.ExecuteCommandAsync(context, services);

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