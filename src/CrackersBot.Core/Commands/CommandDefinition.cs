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
        public string Name { get; set; } = name;
        public string Description { get; set; } = description;
        public bool Enabled { get; set; } = enabled;
        public IEnumerable<ActionInstance> Actions { get; set; } = actions;
        public CommandOutput Output { get; set; } = output;

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