// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using LegoCity.Api.Models.Events;
using LegoCity.Api.Services.Lego;
using MessagePipe;

namespace LegoCity.Api.Services.Environment
{
    /// <summary>Manages the current time of day for the Lego city.</summary>
    public class TimeOfDayManager
    {
        private readonly ILogger logger;
        private readonly IPublisher<TimeOfDayChangedEvent> todEventPublisher;

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

        public TimeOfDayManager(ILogger<TimeOfDayManager> logger, IPublisher<TimeOfDayChangedEvent> todEventPublisher)
        {
            this.logger = logger;
            this.todEventPublisher = todEventPublisher;
        }

        /// <summary>Performs a time of day tick. Incrementing the hour of day and adjusting the model lights to match.</summary>
        public Task TickTimeOfDayAsync()
        {
            // Check if auto time of day is enabled
            if (!this.AutoTimeOfDayEnabled)
                return Task.CompletedTask;

            // Increment the time of day
            this.autoTimeOfDay++;
            if (this.autoTimeOfDay > 23) this.autoTimeOfDay = 0;
            this.logger.LogDebug("Time of day has changed to {time}", this.autoTimeOfDay);

            // Fire our update event
            this.SendUpdateEvent(this.autoTimeOfDay);

            return Task.CompletedTask;
        }

        /// <summary>Sends an <see cref="TimeOfDayChangedEvent"/> event for other services to respond to the new time of day.</summary>
        /// <param name="current">Current time of day to publish in the message.</param>
        private void SendUpdateEvent(int current)
        {
            this.todEventPublisher.Publish(new TimeOfDayChangedEvent
            {
                HourOfDay = current,
                IsNightTime = current > 18 || current < 6
            });
        }

        /// <summary>Enables/Disables auto time of day</summary>
        /// <param name="enabled">Flag determining state</param>
        public void SetAutoTimeOfDay(bool enabled)
        {
            this.AutoTimeOfDayEnabled = enabled;
            this.SendUpdateEvent(this.autoTimeOfDay);
        }

        /// <summary>Sets the time of day to a specific hour. Only works if <see cref="SetAutoTimeOfDay(bool)"/> is set to false.</summary>
        /// <param name="hourOfDay">Time of day to set.</param>
        public void SetForcedTimeOfDay(int hourOfDay)
        {
            this.forcedTimeOfDay = hourOfDay;
            this.SendUpdateEvent(this.forcedTimeOfDay);
        }
    }
}
