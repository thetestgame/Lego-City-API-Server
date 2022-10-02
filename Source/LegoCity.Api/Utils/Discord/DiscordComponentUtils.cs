// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Utils.Discord
{
    using global::Discord;

    /// <summary>Static helper methods for working with Discord.NET message components.</summary>
    public static class DiscordComponentUtils
    {
        /// <summary>Creates a new multi option dropdown component for use in Discord messages.</summary>
        /// <param name="id">Id of the component used in result identification</param>
        /// <param name="description">Context description to describe to the user what needs to be inserted.</param>
        /// <param name="maxOptions">Maximum number of options that can be selected.</param>
        /// <param name="options">Array containing all the options to be added to the dropdown</param>
        /// <returns>Newly built <see cref="IMessageComponent"/> instance for use in Discord messages.</returns>
        public static IMessageComponent BuildDropdownComponent(string id, string description, string[] options, int maxOptions = 1, int minOptions = 1)
        {
            var componentBuilder = new SelectMenuBuilder()
                .WithCustomId(id)
                .WithMaxValues(maxOptions)
                .WithMinValues(minOptions)
                .WithPlaceholder(description);

            foreach (var option in options)
                componentBuilder.AddOption(option, option);

            return componentBuilder.Build();
        }

        /// <inheritdoc cref="BuildDropdownComponent(string, string, IEnumerable{string}, int, int)"/>
        /// <param name="options"><see cref="IEnumerable{T}"/> containing all the options to be added to the dropdown</param>
        public static IMessageComponent BuildDropdownComponent(string id, string description, IEnumerable<string> options, int maxOptions = 1, int minOptions = 1) =>
            BuildDropdownComponent(id, description, options.ToArray(), maxOptions, minOptions);
    }
}
