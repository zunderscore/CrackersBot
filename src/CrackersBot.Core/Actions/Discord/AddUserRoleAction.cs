using CrackersBot.Core.Parameters;
using Microsoft.Extensions.Logging;

namespace CrackersBot.Core.Actions.Discord
{
    [ActionId(ACTION_ID)]
    [ActionName("Add User Discord Role")]
    [ActionDescription("Adds a role to a Discord user")]
    public class AddUserRoleAction(IBotCore bot) : ActionBase(bot)
    {
        public const string ACTION_ID = "CrackersBot.Discord.AddUserRole";

        public override Dictionary<string, IParameterType> ActionParameters => new() {
            { CommonNames.DISCORD_USER_ID, new UInt64ParameterType() },
            { CommonNames.DISCORD_GUILD_ID, new UInt64ParameterType() },
            { CommonNames.DISCORD_ROLE_ID, new UInt64ParameterType() },
        };

        public override async Task Run(Dictionary<string, object> parameters, RunContext context)
        {
            try
            {
                var userId = (ulong)parameters[CommonNames.DISCORD_USER_ID];
                var guildId = (ulong)parameters[CommonNames.DISCORD_GUILD_ID];
                var roleId = (ulong)parameters[CommonNames.DISCORD_ROLE_ID];

                Bot.Logger.LogDebug("Attempting to add user {userId} to role {roleId} in guild {guildId}", userId, roleId, guildId);

                await Bot.DiscordClient.GetGuild(guildId).GetUser(userId).AddRoleAsync(roleId);
            }
            catch (Exception ex)
            {
                Bot.Logger.LogError(
                    ex,
                    "Error adding role {roleId} in guild {guildId} to user {userId}",
                    parameters[CommonNames.DISCORD_ROLE_ID],
                    parameters[CommonNames.DISCORD_GUILD_ID],
                    parameters[CommonNames.DISCORD_USER_ID]
                );
            }
        }
    }
}