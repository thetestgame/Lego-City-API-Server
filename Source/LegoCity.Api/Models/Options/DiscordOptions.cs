// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Models.Options
{
    /// <summary>Options for configuring the included Discord bot</summary>
    public class DiscordOptions
    {
        /// <summary>Configured Bot token for the Discord bot.</summary>
        public string? BotToken { get; set; }
    }
}
