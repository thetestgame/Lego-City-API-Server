// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Controllers
{
    using LegoCity.Api.Utils.Errors;
    using Microsoft.AspNetCore.Mvc;
    using SharpBrick.PoweredUp;

    /// <summary>Controller for interacting with the connected Lego trains.</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HubController : Controller
    {
        /// <summary>Retrieves a complete list of all connected Lego Powered Up Hubs</summary>
        /// <returns><see cref="IEnumerable{T}"/> containing all connected <see cref="Hub"/> instances.</returns>
        [HttpGet]
        public IEnumerable<Hub> Index([FromServices] PoweredUpHost poweredUpHost) => poweredUpHost.Hubs;

        /// <summary>
        /// Retrieves a single <see cref="Hub"/> instance by its <see cref="Hub.Id"/>.
        /// </summary>
        /// <param name="hubId">Hub Id to retrieve if available.</param>
        /// <returns>Hub instance if found. Otherwise 404.</returns>
        [HttpGet("id/{hubId}")]
        public Hub GetById([FromServices] PoweredUpHost poweredUpHost, byte hubId)
        {
            var hub = poweredUpHost.Hubs.FirstOrDefault(hub => hub.HubId == hubId);
            if (hub == null) throw new HttpObjectNotFoundException($"Train with id {hubId} not found.");
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