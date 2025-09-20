using CrackersBot.Core.Parameters;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace CrackersBot.Core.Actions.Discord;

public class RemoveUserRoleAction(BotServiceProvider botServices)
    : ActionBase(
        ACTION_ID,
        "Remove User Discord Role",
        "Removes a role from a Discord user",
        botServices
    )
{
    public const string ACTION_ID = "CrackersBot.Discord.RemoveUserRole";

    public override Dictionary<string, IParameterType> ActionParameters => new() {
        { CommonNames.DISCORD_USER_ID, new UInt64ParameterType() },
        { CommonNames.DISCORD_GUILD_ID, new UInt64ParameterType() },
        { CommonNames.DISCORD_ROLE_ID, new UInt64ParameterType() },
    };

    public override async Task Run(Dictionary<string, object> parameters, RunContext context)
    {
        var logger = BotServices.GetLogger<RemoveUserRoleAction>();
        var discordClient = BotServices.GetBotService<DiscordSocketClient>();

        try
        {
            var userId = (ulong)parameters[CommonNames.DISCORD_USER_ID];
            var guildId = (ulong)parameters[CommonNames.DISCORD_GUILD_ID];
            var roleId = (ulong)parameters[CommonNames.DISCORD_ROLE_ID];

            logger?.LogDebug("Attempting to remove user {userId} from role {roleId} in guild {guildId}", userId, roleId, guildId);

            await discordClient.GetGuild(guildId).GetUser(userId).RemoveRoleAsync(roleId);
        }
        catch (Exception ex)
        {
            logger?.LogError(
                ex,
                "Error adding role {roleId} in guild {guildId} to user {userId}",
                parameters[CommonNames.DISCORD_ROLE_ID],
                parameters[CommonNames.DISCORD_GUILD_ID],
                parameters[CommonNames.DISCORD_USER_ID]
            );
        }
    }
}