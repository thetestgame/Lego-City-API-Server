// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Models.Options
{
    public class CityLightingOptions
    {
        /// <summary>The GPIO pin number to use for the city lighting power management.</summary>
        public int GpioPin { get; set; } = 18;
    }
}
