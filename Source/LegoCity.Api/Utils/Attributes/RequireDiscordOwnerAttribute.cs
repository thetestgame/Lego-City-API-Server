// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Utils.Attributes
{
    using Discord;
    using Discord.Interactions;
    using System;
    using System.Threading.Tasks;
    
    /// <summary></summary>
    public class RequireDiscordOwnerAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync (IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            switch (context.Client.TokenType)
            {
                case TokenType.Bot:
                    var application = await context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
                    if (context.User.Id != application.Owner.Id)
                        return PreconditionResult.FromError(ErrorMessage ?? "Command can only be run by the owner of the bot.");
                    return PreconditionResult.FromSuccess();
                default:
                    return PreconditionResult.FromError($"{nameof(RequireDiscordOwnerAttribute)} is not supported by this {nameof(TokenType)}.");
            }
        }
    }
}
