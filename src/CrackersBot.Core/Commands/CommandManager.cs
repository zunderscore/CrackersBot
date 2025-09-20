using CrackersBot.Core.Actions;
using CrackersBot.Core.Variables;
using Discord.WebSocket;

namespace CrackersBot.Core.Commands;

public class CommandManager(
    BotServiceProvider botServiceProvider
) : ICommandManager
{
    private readonly IActionManager _actionManager = botServiceProvider.GetBotService<IActionManager>();
    private readonly IVariableManager _variableManager = botServiceProvider.GetBotService<IVariableManager>();

    public async Task RunCommandActions(CommandDefinition command, IBotCore bot, SocketCommandBase socketCommand)
    {
        await socketCommand.DeferAsync(command.Output.Ephemeral);

        var context = new RunContext()
            .WithDiscordUser(socketCommand.User)
            .WithDiscordChannel(socketCommand.Channel);

        if (socketCommand is SocketUserCommand userCommand)
        {
            context.WithDiscordTargetUser(userCommand.Data.Member);
        }

        if (socketCommand is SocketMessageCommand messageCommand)
        {
            context.WithDiscordTargetMessage(messageCommand.Data.Message);
        }

        await _actionManager.RunActions(command.Actions, context);

        await socketCommand.FollowupAsync(
            _variableManager.ProcessVariables(command.Output.Text, context),
            command.Output.GetParsedEmbeds()?.ToArray(),
            ephemeral: command.Output.Ephemeral
        );
    }
}