// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Utils
{
    using Discord;

    /// <summary>Static extension methods for the .NET <see cref="ILogger"/> object.</summary>
    public static class LoggerExtensions
    {
        /// <summary>Logs a Discord <see cref="LogMessage"/> instance to the <see cref="ILogger"/> instance.</summary>
        /// <param name="logger">ILogger instance to log the Discord message into.</param>
        /// <param name="message">Discord message to log</param>
        public static void LogDiscordMessage(this ILogger logger, LogMessage message)
        {
            switch(message.Severity)
            {
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    logger.LogDebug(message.Message);
                    break;
                case LogSeverity.Info:
                    logger.LogInformation(message.Message);
                    break;
                case LogSeverity.Warning:
                    logger.LogWarning(message.Message);
                    break;
                case LogSeverity.Error:
                case LogSeverity.Critical:
                    logger.LogError(message.Message);
                    break;
            }
        }

        /// <inheritdoc cref="LogDiscordMessage(ILogger, LogMessage)"/>
        public static Task LogDiscordMessageAsync(this ILogger logger, LogMessage message)
        {
            logger.LogDiscordMessage(message);
            return Task.CompletedTask;
        }
    }
}
