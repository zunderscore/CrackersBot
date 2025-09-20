using Discord.WebSocket;

namespace CrackersBot.Core.Commands;

public interface ICommandManager
{
    Task RunCommandActions(CommandDefinition command, IBotCore bot, SocketCommandBase socketCommand);
}