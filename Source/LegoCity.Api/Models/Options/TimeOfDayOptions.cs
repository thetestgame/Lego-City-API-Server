// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Models.Options
{
    /// <summary>
    /// Options class for configuring the TimeOfDayManager.
    /// </summary>
    public class TimeOfDayOptions
    {
        /// <summary>Time between time of day ticks defined in minutes</summary>
        public int MinutesBetweenTicks { get; set; } = 5;
    }
}
