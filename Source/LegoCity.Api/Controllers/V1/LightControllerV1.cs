// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Controllers.V1
{
    using LegoCity.Api.Services.Environment;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>V1 api controller for interacting with the model's lighting and time of day.</summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Light Controller")]
    [Route("api/v{version:apiVersion}/light")]
    public class LightControllerV1 : Controller
    {
        /// <summary>Retrieves the current time of day.</summary>
        /// <returns>Current time of day. Represented as a simulated hour between 0 and 23</returns>
        [HttpGet("tod/time")]
        public int GetCurrentTimeOfDay([FromServices] TimeOfDayManager timeOfDayManager) => timeOfDayManager.HourOfDay;

        /// <summary>Sets the time of day to a specific hour. Only works if "light/tod/auto" is set to false.</summary>
        /// <inheritdoc cref="TimeOfDayManager.SetForcedTimeOfDayAsync(int)"/>
        [HttpPost("tod/time")]
        public void SetCurrentTimeOfDay([FromServices] TimeOfDayManager timeOfDayManager, int hourOfDay) => timeOfDayManager.SetForcedTimeOfDay(hourOfDay);

        /// <summary>Retrieves the current auto time of day setting.</summary>
        /// <returns>True if auto time of day management is enabled</returns>
        [HttpGet("tod/auto")]
        public bool GetAutoTimeOfDayEnabled([FromServices] TimeOfDayManager timeOfDayManager) => timeOfDayManager.AutoTimeOfDayEnabled;

        /// <summary>Enables or disables auto time of day management.</summary>
        /// <param name="enabled">New setting for <see cref="TimeOfDayManager.AutoTimeOfDayEnabled"/> to set.</param>
        [HttpPost("tod/auto")]
        public void SetAutoTimeOfDayEnabled([FromServices] TimeOfDayManager timeOfDayManager, bool enabled) => timeOfDayManager.SetAutoTimeOfDay(enabled);
    }
}
