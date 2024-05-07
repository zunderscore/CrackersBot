using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Actions.Discord
{
    [ActionId(ACTION_ID)]
    [ActionName("Add User Discord Role")]
    [ActionDescription("Adds a role to a Discord user")]
    public class AddUserRoleAction : ActionBase
    {
        public const string ACTION_ID = "CrackersBot.Discord.AddUserRole";

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

                await bot.DiscordClient.Rest.AddRoleAsync(guildId, userId, roleId);
            }
            finally { }
        }
    }
}