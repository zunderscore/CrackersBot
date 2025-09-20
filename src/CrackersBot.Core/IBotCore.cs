using System.Collections.Concurrent;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace CrackersBot.Core;

public interface IBotCore
{
    ILogger Logger { get; }

    DiscordSocketClient DiscordClient { get; }

    ConcurrentDictionary<ulong, GuildConfig> Guilds { get; }

    Task LoadGuildConfigsAsync();
}