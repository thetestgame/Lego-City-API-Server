// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.SignalR;

namespace LegoCity.Api.Hubs
{
    /// <summary>
    /// Custom <see cref="IHubFilter"/> implementation for logging signalr hub access and exceptions.
    /// </summary>
    public class SignalRHubLoggingFilter : IHubFilter
    {
        private readonly ILogger<SignalRHubLoggingFilter> logger;

        public SignalRHubLoggingFilter(ILogger<SignalRHubLoggingFilter> logger)
        {
            this.logger = logger;
        }

        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
        {
            this.logger.LogDebug($"SignalR client executing hub method: '{invocationContext.HubMethodName}'");
            try{ return await next(invocationContext); }
            catch (Exception ex)
            {
                this.logger.LogError($"Exception calling '{invocationContext.HubMethodName}': {ex}");
                throw;
            }
        }

        public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)                                       => next(context);
        public Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception, Func<HubLifetimeContext, Exception?, Task> next)  => next(context, exception);
    }
}
