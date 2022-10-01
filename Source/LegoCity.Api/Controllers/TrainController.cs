// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Controllers
{
    using LegoCity.Api.Services;
    using LegoCity.Api.Utils.Errors;
    using Microsoft.AspNetCore.Mvc;
    using SharpBrick.PoweredUp;

    /// <summary>Controller for interacting with the connected Lego trains.</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TrainController : Controller
    {
        /// <summary>Returns an dictionary containing all the trains that have been discovered and are currently connected.</summary>
        /// <returns>Dictionary containing all discovered trains.</returns>
        [HttpGet()]
        public Dictionary<byte, string> Index([FromServices] LegoTrainService trainService) => trainService.GetTrainHubId2Name();

        /// <summary>Returns the full details of the connected train queried by its id.</summary>
        /// <param name="hubId">Id of the train to command.</param>
        /// <returns><see cref="TwoPortHub"/> containing all the train details if found. Otherwise 404</returns>
        /// <exception cref="HttpObjectNotFoundException">Thrown if the train is not found.</exception>
        [HttpGet("{hubId}")]
        public TwoPortHub GetTrainByHubId([FromServices] LegoTrainService trainService, byte hubId)
        {
            var train = trainService.GetTrainHubById(hubId);
            if (train == null) throw new HttpObjectNotFoundException($"Train with Id {hubId} not found.");
            return train;
        }

        [HttpGet("{hubId}/speed")]
        /// <summary>Retrieves the current speed of a specific train</summary>
        /// <param name="hubId">Id of the train to command.</param>
        /// <returns>The current set speed of the train.</returns>
        /// <exception cref="HttpObjectNotFoundException">Thrown if the train is not found.</exception>
        public int GetTrainSpeed([FromServices] LegoTrainService trainService, byte hubId)
        {
            var train = trainService.GetTrainHubById(hubId);
            if (train == null) throw new HttpObjectNotFoundException($"Train with Id {hubId} not found.");
            return trainService.GetTrainSpeed(train).FirstOrDefault();
        }

        [HttpPost("{hubId}/speed")]
        /// <summary>Sets the current speed of a specific train.</summary>
        /// <param name="hubId">Id of the train to command.</param>
        /// <inheritdoc cref="LegoTrainService.SetTrainSpeedAsync(Hub, int)"/>
        /// <exception cref="HttpObjectNotFoundException">Thrown if the train is not found.</exception>
        public async Task SetTrainSpeedAsync([FromServices] LegoTrainService trainService, byte hubId, int speed)
        {
            var train = trainService.GetTrainHubById(hubId);
            if (train == null) throw new HttpObjectNotFoundException($"Train with Id {hubId} not found.");
            await trainService.SetTrainSpeedAsync(train, speed);
        }
    }
}
