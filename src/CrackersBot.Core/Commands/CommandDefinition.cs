using CrackersBot.Core.Actions;
using CrackersBot.Core.Variables;
using Discord.WebSocket;

namespace CrackersBot.Core.Commands
{
    public class CommandDefinition(
        string name,
        string description,
        IEnumerable<ActionInstance> actions,
        CommandOutput output,
        bool enabled = true
    )
    {
        public string Name { get; } = name;
        public string Description { get; } = description;
        public bool Enabled { get; } = enabled;
        public IEnumerable<ActionInstance> Actions { get; } = actions;
        public CommandOutput Output { get; } = output;

        public async Task RunActions(IBotCore bot, SocketSlashCommand slashCommand)
        {
            var context = new RunContext()
                .WithDiscordUser(slashCommand.User)
                .WithDiscordChannel(slashCommand.Channel);

            await ActionRunner.RunActions(bot, Actions, context);
            await slashCommand.RespondAsync(
                DefaultVariableProcessor.ProcessVariables(bot, Output.Text, context),
                Output.GetParsedEmbeds(bot, context)?.ToArray(),
                ephemeral: Output.Ephemeral
            );
        }
    }
}