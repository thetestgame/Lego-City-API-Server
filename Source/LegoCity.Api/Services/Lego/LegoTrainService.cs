// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Services.Lego
{
    using SharpBrick.PoweredUp;

    /// <summary>Singleton service for ineracting with connected Lego train instances.</summary>
    public class LegoTrainService
    {
        private readonly PoweredUpHost poweredUpHost;
        private readonly LegoHubService legoHubService;
        private TwoPortHub? activeTrain;

        public LegoTrainService(PoweredUpHost poweredUpHost, LegoHubService legoHubService)
        {
            this.poweredUpHost = poweredUpHost;
            this.legoHubService = legoHubService;
        }

        /// <summary>Returns an <see cref="IEnumerable{T}"/> containing all the trains that have been discovered.</summary>
        /// <returns><see cref="IEnumerable{T}"/> containing all discovered trains.</returns>
        public IEnumerable<TwoPortHub> GetTrainHubs() =>
            poweredUpHost.Hubs
                .Where(hub => hub.SystemType == SystemType.LegoSystem_TwoPortHub)
                .Where(hub => hub.Ports.Any(port => port.DeviceType == DeviceType.SystemTrainMotor))
                .Select(hub => hub as TwoPortHub ?? throw new InvalidOperationException("Hub is not a TwoPortHub"));

        /// <summary>Retrieves a train <see cref="TwoPortHub"/> instance by its configured <see cref="Hub.AdvertisingName"/></summary>
        /// <param name="name">Name of the train hub to retrieve</param>
        /// <returns>Discovered <see cref="TwoPortHub"/> instance if found. Otherwise <code>null</code></returns>
        public TwoPortHub? GetTrainHubByName(string name) => GetTrainHubs().FirstOrDefault(train => train.AdvertisingName == name);

        /// <summary>
        /// Retrieves a dictionary of all connected train hubs containing its <see cref="Hub.HubId"/> as the key and the <see cref="Hub.AdvertisingName"/> as the value.
        /// </summary>
        /// <returns>Dictionary containing the <see cref="Hub.HubId"/> to <see cref="Hub.AdvertisingName"/> map for all connected trains.</returns>
        public Dictionary<byte, string> GetTrainHubId2Name() => GetTrainHubs().ToDictionary(train => train.HubId, train => train.AdvertisingName);

        /// <summary>Sets the current active Train hub instance.</summary>
        /// <param name="activeHub"><see cref="TwoPortHub"/> instance as active.</param>
        public async Task SetCurrentActiveTrainAsync(TwoPortHub? activeHub)
        {
            activeTrain = activeHub;
            foreach (var hub in GetTrainHubs())
            {
                var greenValue = Convert.ToByte(hub == activeHub ? 255 : 0);
                var redValue = Convert.ToByte(hub != activeHub ? 255: 0);
                await hub.RgbLight.SetRgbColorsAsync(redValue, greenValue, 0);
            }
        }

        /// <inheritdoc cref="SetCurrentActiveTrainAsync(TwoPortHub?)"/>
        /// <param name="name"><see cref="Hub.AdvertisingName"/> of the requested active train.</param>
        public async Task SetCurrentActiveTrainAsync(string name) => await SetCurrentActiveTrainAsync(GetTrainHubByName(name));

        /// <summary>Sets the current speed of a connected Lego Train's connected motors.</summary>
        /// <param name="hub">Train <see cref="Hub"/> instance to set the current movement speed of.</param>
        /// <param name="speed">Speed to set the train too. Must be a value between -100 and 100 with zero being a full stop.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="speed"/> is not between -100 and 100.</exception>
        public async Task SetTrainSpeedAsync(Hub hub, int speed)
        {
            // Verify our speed is within range.
            if (speed < -100 || speed > 100)
                throw new ArgumentOutOfRangeException(nameof(speed), "Speed must be a value between -100 and 100");

            // Retrieve our motors and verify there is at least one motor attached
            var motors = legoHubService.GetTrainMotors(hub);
            if (!motors.Any())
                return;

            // Set the speed of all motors
            foreach (var motor in motors)
            {
                if (speed != 0) await motor.StartPowerAsync(Convert.ToSByte(speed));
                else await motor.StopByBrakeAsync();
            }
        }

        /// <summary>Retrieves the current speed of all connected Lego Train motors on a given <see cref="Hub"/>.</summary>
        /// <param name="hub">Train <see cref="Hub"/> instance to retrieve the current movement speeds from.</param>
        /// <returns><see cref="IEnumerable{T}"/> containing int speed values for all connected Lego <see cref="SystemTrainMotor"/> instances.</returns>
        public IEnumerable<int> GetTrainSpeed(Hub hub)
        {
            // Retrieve our motors and verify there is at least one motor attached
            var motors = legoHubService.GetTrainMotors(hub);
            if (!motors.Any())
                return Enumerable.Empty<int>();

            // Retrieve the speed of all connected train motors
            return motors.Select(motor => Convert.ToInt32(motor.Power));
        }

        /// <summary>Sets the current light power status of a connected Lego Train hub </summary>
        /// <param name="hub">Train <see cref="Hub"/> instance to set the current movement speed of.</param>
        /// <param name="enabled">Flag enabling/disabling the lights on a given train</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="speed"/> is not between -100 and 100.</exception>
        public async Task SetTrainLightState(Hub hub, bool enabled)
        {
            // Retrieve our lights and verify there is at least one light attached
            var trainHub = hub as TwoPortHub;
            if (trainHub == null)
                return;
            
            var light = legoHubService.GetCustomLights(trainHub);
            var powerMode = light.SingleValueMode<sbyte, sbyte>(0);
            if (enabled) await powerMode.WriteDirectModeDataAsync(0x64);
            else  await powerMode.WriteDirectModeDataAsync(0x00); 
        }
    }
}
