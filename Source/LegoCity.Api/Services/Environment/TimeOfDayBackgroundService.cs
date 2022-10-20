// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using LegoCity.Api.Models.Options;
using Microsoft.Extensions.Options;

namespace LegoCity.Api.Services.Environment
{
    /// <summary>
    /// Background service for ticking the time of day on the <see cref="TimeOfDayManager"/> instance.
    /// </summary>
    public class TimeOfDayBackgroundService : BackgroundService
    {
        private readonly TimeOfDayManager timeOfDayManager;
        private readonly TimeOfDayOptions options;

        public TimeOfDayBackgroundService(TimeOfDayManager timeOfDayManager, IOptions<TimeOfDayOptions> options)
        {
            this.timeOfDayManager = timeOfDayManager;
            this.options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await this.timeOfDayManager.TickTimeOfDayAsync();
                await Task.Delay(TimeSpan.FromMinutes(options.MinutesBetweenTicks));
            }
        }
    }
}
