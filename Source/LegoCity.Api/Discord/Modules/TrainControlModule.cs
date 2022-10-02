// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Discord.Modules
{
    using global::Discord.Interactions;
    using LegoCity.Api.Services.Discord;
    using LegoCity.Api.Services.Lego;

    /// <summary></summary>
    [Group("train", "Commands for controlling Lego Trains")]
    public class TrainControlModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DiscordInteractionHandler handler;
        private readonly LegoTrainService trainService;

        public TrainControlModule(DiscordInteractionHandler handler, LegoTrainService trainService)
        {
            this.handler = handler;
            this.trainService = trainService;
        }

        /// <summary></summary>
        [SlashCommand("connected", "Returns a list of connected Lego trains")]
        public async Task GetConnectedTrains()
        {
            var trains = this.trainService.GetTrainHubs();
            var trainNames = trains.Select(t => t.AdvertisingName);
            var trainNamesString = string.Join(", ", trainNames);

            await this.RespondAsync($"Connected trains: {trainNamesString}");
        }

        /// <summary></summary>
        /// <param name="hubId"></param>
        /// <param name="speed"></param>
        [SlashCommand("speed", "Sets the speed of a specific lego train")]
        public async Task SetTrainSpeedAsync(string trainName, int speed)
        {
            var trainHub = this.trainService.GetTrainHubByName(trainName);
            if (trainHub == null)
            {
                await RespondAsync($"Requested train name {trainName} does not exist");
                return;
            }
            
            await this.trainService.SetTrainSpeedAsync(trainHub, speed);
            await RespondAsync($"Set train {trainName} speed to {speed}");
        }
    }
}
