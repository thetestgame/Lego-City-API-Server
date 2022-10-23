// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Models.Events
{
    /// <summary>
    /// Event raised when the time of day has changed.
    /// </summary>
    public class TimeOfDayChangedEvent
    {
        public int HourOfDay  { get; set; }
        public bool IsNightTime { get; set; }
        public bool IsDayTime => !this.IsNightTime;
    }
}
