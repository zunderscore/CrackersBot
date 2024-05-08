using System.Diagnostics;
using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Actions.Discord
{
    [ActionId(ACTION_ID)]
    [ActionName("Remove User Discord Role")]
    [ActionDescription("Removes a role from a Discord user")]
    public class RemoveUserRoleAction : ActionBase
    {
        public const string ACTION_ID = "CrackersBot.Discord.RemoveUserRole";

        public override Dictionary<string, IParameterType> ActionParameters => new() {
            { CommonNames.DISCORD_USER_ID, new UInt64ParameterType() },
            { CommonNames.DISCORD_GUILD_ID, new UInt64ParameterType() },
            { CommonNames.DISCORD_ROLE_ID, new UInt64ParameterType() },
        };

        public override async Task Run(IBotCore bot, Dictionary<string, object> parameters, RunContext context)
        {
            try
            {
                var userId = (ulong)parameters[CommonNames.DISCORD_USER_ID];
                var guildId = (ulong)parameters[CommonNames.DISCORD_GUILD_ID];
                var roleId = (ulong)parameters[CommonNames.DISCORD_ROLE_ID];

                Debug.WriteLine($"Attempting to remove user {userId} from role {roleId} in guild {guildId}");

                await bot.DiscordClient.GetGuild(guildId).GetUser(userId).RemoveRoleAsync(roleId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error removing role {parameters[CommonNames.DISCORD_ROLE_ID]} in guild {parameters[CommonNames.DISCORD_GUILD_ID]} from user {parameters[CommonNames.DISCORD_USER_ID]}: {ex.Message}");
            }
        }
    }
}