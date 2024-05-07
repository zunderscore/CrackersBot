using CrackersBot.Core.Actions;
using CrackersBot.Core.Variables;
using Discord.WebSocket;

namespace CrackersBot.Core.Commands
{
    public class CommandDefinition(
        string name,
        string description,
        List<KeyValuePair<string, Dictionary<string, string>>> actions,
        CommandOutput output
    )
    {
        public string Name { get; } = name;
        public string Description { get; } = description;
        public List<KeyValuePair<string, Dictionary<string, string>>> Actions { get; } = actions;
        public CommandOutput Output { get; } = output;

        public async Task RunActions(IBotCore bot, SocketSlashCommand slashCommand)
        {
            var context = new Dictionary<string, object>()
            {
                { CommonNames.DISCORD_USER_ID, slashCommand.User.Id },
                { CommonNames.DISCORD_USER_NAME, slashCommand.User.Username },
                { CommonNames.DISCORD_USER_AVATAR_URL, slashCommand.User.GetAvatarUrl() },
            };

            await ActionRunner.RunActions(bot, Actions, context);
            await slashCommand.RespondAsync(
                DefaultVariableProcessor.ProcessVariables(bot, Output.Text, context),
                Output.GetParsedEmbeds(bot, context)?.ToArray(),
                ephemeral: Output.Ephemeral
            );
        }
    }
}