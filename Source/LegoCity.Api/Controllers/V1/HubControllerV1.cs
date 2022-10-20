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
    [Tags("Hub Controller")]
    [Route("api/v{version:apiVersion}/hub")]
    public class HubControllerV1 : Controller
    {
        /// <summary>Retrieves a complete list of all connected Lego Powered Up Hubs</summary>
        /// <returns><see cref="IEnumerable{T}"/> containing all connected <see cref="Hub"/> instances.</returns>
        [HttpGet]
        public IEnumerable<Hub> Index([FromServices] PoweredUpHost poweredUpHost) => poweredUpHost.Hubs;

        /// <summary>
        /// Retrieves a single <see cref="Hub"/> instance by its <see cref="Hub.Id"/>.
        /// </summary>
        /// <param name="hubName">Hub name to retrieve if available.</param>
        /// <returns>Hub instance if found. Otherwise 404.</returns>
        [HttpGet("id/{hubName}")]
        public Hub GetById([FromServices] LegoHubService hubService, string hubName)
        {
            var hub = hubService.GetHubByName<Hub>(hubName);
            if (hub == null) throw new HttpObjectNotFoundException($"Hub with name {hubName} not found.");
            return hub;
        }

        /// <summary>Retrieves all connected <see cref="Hub"/> instances matching the requested <see cref="SystemType"/></summary>
        /// <param name="systemType"><see cref="SystemType"/> to query for.</param>
        /// <returns><see cref="IEnumerable{T}"/> containing all connected hubs of the requested type</returns>
        [HttpGet("systemType/{systemType}")]
        public IEnumerable<Hub> GetByType([FromServices] PoweredUpHost poweredUpHost, SystemType systemType) =>
            poweredUpHost.Hubs.Where(hub => hub.SystemType == systemType);
    }
}