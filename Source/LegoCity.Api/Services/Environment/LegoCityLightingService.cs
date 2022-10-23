// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using LegoCity.Api.Models.Events;
using LegoCity.Api.Models.Options;
using MessagePipe;
using Microsoft.Extensions.Options;
using System.Device.Gpio;
using System.Runtime.InteropServices;

namespace LegoCity.Api.Services.Environment
{
    /// <summary>
    /// Manages the current lighting status of the Lego city LEDs
    /// </summary>
    public class LegoCityLightingService : IHostedService
    {
        private readonly ILogger<LegoCityLightingService> logger;
        private readonly CityLightingOptions options;
        private readonly IDisposable disposable;

        public LegoCityLightingService(ILogger<LegoCityLightingService> logger, IOptions<CityLightingOptions> options, ISubscriber<TimeOfDayChangedEvent> todChangedEvent)
        {
            this.logger = logger;
            this.options = options.Value;
            
            var bag = DisposableBag.CreateBuilder();
            todChangedEvent.Subscribe(this.OnTimeOfDayUpdated).AddTo(bag);
            this.disposable = bag.Build();
        }

        /// <summary>Changes the current state of the gpio pin controlling the city lighting. 
        /// <see cref="PinValue.High"/> when the city should be lit.
        /// <see cref="PinValue.Low"/> when the city lighting should be off.
        /// </summary>
        /// <param name="state">State to set the city lighting.</param>
        private void SetLightingPowerState(bool state)
        {
            var pin = this.options.GpioPin;
            this.logger.LogDebug("Setting lighting power state to {state} on pin {pin}", state, pin);

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                this.logger.LogWarning("Cannot set lighting power state on {platform}", RuntimeInformation.OSDescription);
                return;
            }

            using var controller = new GpioController();
            controller.OpenPin(pin, PinMode.Output);
            controller.Write(pin, state ? PinValue.High : PinValue.Low);
        }

        /// <summary>Handles time of day updates from the time of day manager</summary>
        /// <param name="todEvent">Event fired from the time of day manager.</param>
        private void OnTimeOfDayUpdated(TimeOfDayChangedEvent todEvent) => 
            this.SetLightingPowerState(todEvent.IsNightTime);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.disposable.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
