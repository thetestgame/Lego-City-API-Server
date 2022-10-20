// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Controllers.V1
{
    using LegoCity.Api.Services.Lego;
    using LegoCity.Api.Utils.Errors;
    using Microsoft.AspNetCore.Mvc;
    using SharpBrick.PoweredUp;

    /// <summary>V1 api controller for interacting with the connected Lego trains.</summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Train Controller")]
    [Route("api/v{version:apiVersion}/train")]
    public class TrainControllerV1 : Controller
    {
        /// <summary>Returns an dictionary containing all the trains that have been discovered and are currently connected.</summary>
        /// <returns>Dictionary containing all discovered trains.</returns>
        [HttpGet()]
        public IEnumerable<string> Index([FromServices] LegoTrainService trainService) =>
            trainService.GetTrainHubs().Select(hub => hub.AdvertisingName);

        /// <summary>Returns the full details of the connected train queried by its id.</summary>
        /// <param name="hubName">Name of the train to command.</param>
        /// <returns><see cref="TwoPortHub"/> containing all the train details if found. Otherwise 404</returns>
        /// <exception cref="HttpObjectNotFoundException">Thrown if the train is not found.</exception>
        [HttpGet("{hubName}")]
        public TwoPortHub GetTrainByhubName([FromServices] LegoTrainService trainService, string hubName)
        {
            var train = trainService.GetTrainHubByName(hubName);
            if (train == null) throw new HttpObjectNotFoundException($"Train with name {hubName} not found.");
            return train;
        }

        [HttpGet("{hubName}/speed")]
        /// <summary>Retrieves the current speed of a specific train</summary>
        /// <param name="hubName">Name of the train to command.</param>
        /// <returns>The current set speed of the train.</returns>
        /// <exception cref="HttpObjectNotFoundException">Thrown if the train is not found.</exception>
        public int GetTrainSpeed([FromServices] LegoTrainService trainService, string hubName)
        {
            var train = trainService.GetTrainHubByName(hubName);
            if (train == null) throw new HttpObjectNotFoundException($"Train with name {hubName} not found.");
            return trainService.GetTrainSpeed(train).FirstOrDefault();
        }

        [HttpPost("{hubName}/speed")]
        /// <summary>Sets the current speed of a specific train.</summary>
        /// <param name="hubName">Name of the train to command.</param>
        /// <inheritdoc cref="LegoTrainService.SetTrainSpeedAsync(Hub, int)"/>
        /// <exception cref="HttpObjectNotFoundException">Thrown if the train is not found.</exception>
        public async Task SetTrainSpeedAsync([FromServices] LegoTrainService trainService, string hubName, int speed)
        {
            var train = trainService.GetTrainHubByName(hubName);
            if (train == null) throw new HttpObjectNotFoundException($"Train with name {hubName} not found.");
            await trainService.SetTrainSpeedAsync(train, speed);
        }

        [HttpPost("{hubName}/lights")]
        /// <summary>Sets the light status of a specific train.</summary>
        /// <param name="hubName">Name of the train to command.</param>
        /// <inheritdoc cref="LegoTrainService.SetTrainLightState(Hub, bool)"/>
        /// <exception cref="HttpObjectNotFoundException">Thrown if the train is not found.</exception>
        public async Task SetTrainLightAsync([FromServices] LegoTrainService trainService, string hubName, bool state)
        {
            var train = trainService.GetTrainHubByName(hubName);
            if (train == null) throw new HttpObjectNotFoundException($"Train with name {hubName} not found.");
            await trainService.SetTrainLightState(train, state);
        }
    }
}
