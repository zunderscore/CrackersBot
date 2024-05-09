using CrackersBot.Core.Actions;
using CrackersBot.Core.Events;
using CrackersBot.Core.Filters;
using CrackersBot.Core.Variables;
using Discord.WebSocket;
using System.Collections.Concurrent;

namespace CrackersBot.Core
{
    public interface IBotCore
    {
        DiscordSocketClient DiscordClient { get; }

        ConcurrentDictionary<ulong, GuildConfig> Guilds { get; }

        Task LoadGuildConfigsAsync();

        // Actions

        ConcurrentDictionary<string, IAction> RegisteredActions { get; }

        bool IsActionRegistered(string id);

        void RegisterAction(IAction action);

        void UnregisterAction(string id);

        IAction GetRegisteredAction(string id);

        // Variables

        ConcurrentDictionary<string, IVariable> RegisteredVariables { get; }

        bool IsVariableRegistered(string token);

        void RegisterVariable(IVariable variable);

        void UnregisterVariable(string token);

        // Event Handler

        ConcurrentDictionary<string, IEventHandler> RegisteredEventHandlers { get; }

        bool IsEventHandlerRegistered(string id);

        void RegisterEventHandler(IEventHandler eventHandler);

        void UnregisterEventHandler(string id);

        // Filters

        ConcurrentDictionary<string, IFilter> RegisteredFilters { get; }

        bool IsFilterRegistered(string id);

        void RegisterFilter(IFilter filter);

        void UnregisterFilter(string id);
    }
}