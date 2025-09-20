using System.Collections.Concurrent;

namespace CrackersBot.Core.Events;

public interface IEventManager
{
    ConcurrentDictionary<string, EventDefinition> RegisteredEvents { get; }

    bool IsEventRegistered(string id);

    void RegisterEvent(EventDefinition eventHandler);

    void UnregisterEvent(string id);

    Task Handle(EventHandler instance, RunContext context);

    Task HandleGuildEvents(
        string eventId,
        GuildConfig guild,
        Func<RunContext, EventHandler, Task<RunContext>>? context
    );
}