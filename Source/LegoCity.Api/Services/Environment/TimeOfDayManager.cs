// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using LegoCity.Api.Services.Lego;

namespace LegoCity.Api.Services.Environment
{
    /// <summary>Manages the current time of day for the Lego city.</summary>
    public class TimeOfDayManager
    {
        private readonly ILogger logger;
        private readonly LegoTrainService legoTrainService;

        /// <summary>Flag determining if auto time of day is enabled.</summary>
        public bool AutoTimeOfDayEnabled { get; private set; } = true;
        private int autoTimeOfDay = 0;
        private int forcedTimeOfDay = 0;

        /// <summary>Current configured time of day</summary>
        public int HourOfDay => this.AutoTimeOfDayEnabled ? this.autoTimeOfDay : this.forcedTimeOfDay;
        /// <summary>Flag determining if its currently night time</summary>
        public bool IsNightTime => this.HourOfDay > 18 || this.HourOfDay < 6;
        /// <summary>Flag determining if its currently day time</summary>
        public bool IsDayTime   => !this.IsNightTime;

        public TimeOfDayManager(ILogger<TimeOfDayManager> logger, LegoTrainService legoTrainService)
        {
            this.logger = logger;
            this.legoTrainService = legoTrainService;
        }

        /// <summary>Performs a time of day tick. Incrementing the hour of day and adjusting the model lights to match.</summary>
        public async Task TickTimeOfDayAsync()
        {
            // Check if auto time of day is enabled
            if (!this.AutoTimeOfDayEnabled)
                return;

            // Increment the time of day
            this.autoTimeOfDay++;
            if (this.autoTimeOfDay > 23) this.autoTimeOfDay = 0;
            this.logger.LogInformation($"Time of day has changed to {this.autoTimeOfDay}");

            // Update all our lights
            await this.UpdateLegoTrainLightsAsync();
        }

        /// <summary>Updates the lights on all the trains to match the current time of day.</summary>
        private async Task UpdateLegoTrainLightsAsync()
        {
            var trains = this.legoTrainService.GetTrainHubs();
            foreach (var train in trains)
            {
                if (!train.IsConnected)
                    continue;

                await this.legoTrainService.SetTrainLightState(train, this.IsNightTime);
            }
        }

        /// <summary>Enables/Disables auto time of day</summary>
        /// <param name="enabled">Flag determining state</param>
        public async Task SetAutoTimeOfDayAsync(bool enabled)
        {
            this.AutoTimeOfDayEnabled = enabled;
            await this.UpdateLegoTrainLightsAsync();
        }

        /// <summary>Sets the time of day to a specific hour. Only works if <see cref="SetAutoTimeOfDayAsync(bool)"/> is set to false.</summary>
        /// <param name="hourOfDay">Time of day to set.</param>
        public async Task SetForcedTimeOfDayAsync(int hourOfDay)
        {
            this.forcedTimeOfDay = hourOfDay;
            await this.UpdateLegoTrainLightsAsync();
        }
    }
}
