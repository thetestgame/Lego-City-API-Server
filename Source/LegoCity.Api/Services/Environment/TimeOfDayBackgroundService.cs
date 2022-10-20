// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Services.Environment
{
    /// <summary>
    /// Background service for ticking the time of day on the <see cref="TimeOfDayManager"/> instance.
    /// </summary>
    public class TimeOfDayBackgroundService : BackgroundService
    {
        private readonly TimeOfDayManager timeOfDayManager;

        public TimeOfDayBackgroundService(TimeOfDayManager timeOfDayManager)
        {
            this.timeOfDayManager = timeOfDayManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await this.timeOfDayManager.TickTimeOfDayAsync();
                await Task.Delay(TimeSpan.FromSeconds(1));// TimeSpan.FromMinutes(5));
            }
        }
    }
}
