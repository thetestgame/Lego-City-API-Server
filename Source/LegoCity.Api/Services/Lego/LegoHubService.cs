﻿// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Services.Lego
{
    using SharpBrick.PoweredUp;

    /// <summary></summary>
    public class LegoHubService
    {
        private readonly ILogger<LegoHubService> logger;
        private readonly PoweredUpHost poweredUpHost;

        public LegoHubService(ILogger<LegoHubService> logger, PoweredUpHost poweredUpHost)
        {
            this.logger = logger;
            this.poweredUpHost = poweredUpHost;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="THub"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public THub? GetHubByName<THub>(string name) where THub : Hub => this.poweredUpHost.Hubs.FirstOrDefault(h => h.AdvertisingName == name) as THub;

        /// <summary>
        /// Retrieves all the connected <see cref="SystemTrainMotor"/> instances connected to a Lego <see cref="Hub"/> instance.
        /// </summary>
        /// <param name="trainHub">Hub to query for connected train motors.</param>
        /// <returns><see cref="IEnumerable{T}"/> containing all discovered <see cref="SystemTrainMotor"/> instances.</returns>
        public IEnumerable<SystemTrainMotor> GetTrainMotors(Hub hub) => hub.Ports
            .Where(port => port.DeviceType == DeviceType.SystemTrainMotor)
            .Select(port => port.GetDevice<SystemTrainMotor>());
    }
}