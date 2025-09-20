using System.Collections.Concurrent;
using CrackersBot.Core.Actions;
using CrackersBot.Core.Filters;
using Microsoft.Extensions.Logging;

namespace CrackersBot.Core.Events;

public class EventManager(
    BotServiceProvider botServices
) : IEventManager
{
    private readonly ILogger<EventManager> _logger = botServices.GetLogger<EventManager>();
    private readonly IActionManager _actionManager = botServices.GetBotService<IActionManager>();
    private readonly IFilterManager _filterManager = botServices.GetBotService<IFilterManager>();

    private readonly ConcurrentDictionary<string, EventCacheItem> _cacheItems = [];

    public ConcurrentDictionary<string, EventDefinition> RegisteredEvents { get; } = new();

    public bool IsEventRegistered(string id)
    {
        return RegisteredEvents.Keys.Any(k => k.Equals(id, StringComparison.InvariantCultureIgnoreCase));
    }

    public void RegisterEvent(EventDefinition def)
    {
        if (IsEventRegistered(def.Id))
        {
            _logger.LogDebug("Unable to register Event Handler {id} as it has already been registered", def.Id);
            return;
        }

        if (RegisteredEvents.TryAdd(def.Id, def))
        {
            _logger.LogDebug("Registered Event Handler {id}", def.Id);
        }
        else
        {
            _logger.LogDebug("Unable to register Event Handler {id} (registration failed)", def.Id);
        }
    }

    public void UnregisterEvent(string id)
    {
        if (!IsEventRegistered(id))
        {
            _logger.LogDebug("Unable to unregister Event Handler {id} since it is not currently registered", id);
            return;
        }

        if (RegisteredEvents.TryRemove(id, out _))
        {
            _logger.LogDebug("Unregistered Event Handler {id}", id);
        }
        else
        {
            _logger.LogDebug("Unable to unregister Event Handler {id} (removal failed)", id);
        }
    }

    private static Dictionary<string, string> CreateCacheKeyValues(Dictionary<string, string> metadata, List<string> keysToCache)
    {
        return metadata.Where(m => keysToCache?.Contains(m.Key) == true)
            .ToDictionary();
    }

    private bool CacheEvent(EventHandler instance, Dictionary<string, string> metadata, TimeSpan expireAfter)
    {
        return _cacheItems.TryAdd(Guid.NewGuid().ToString(), new(
            instance.Id,
            metadata,
            DateTimeOffset.UtcNow.Add(expireAfter)
        ));
    }

    private bool IsEventCached(string eventHandlerId, Dictionary<string, string> metadata, out DateTimeOffset expires)
    {
        var matchingEvents = _cacheItems
            .Where(c => c.Value.EventHandlerId == eventHandlerId);

        foreach (var evt in matchingEvents)
        {
            var cacheKeys = evt.Value.CacheKeys.Select(k => k.Key);
            var filteredMetadata = metadata.Where(m => cacheKeys.Contains(m.Key)).ToDictionary();

            if (!filteredMetadata.EqualsDictionary(evt.Value.CacheKeys)) continue;

            // Remove expired events
            if (evt.Value.ExpireTime < DateTimeOffset.UtcNow)
            {
                _cacheItems.Remove(evt.Key, out _);
            }
            else
            {
                expires = evt.Value.ExpireTime;
                return true;
            }
        }

        expires = DateTimeOffset.MinValue;
        return false;
    }

    public async Task HandleGuildEvents(
        string eventId,
        GuildConfig guild,
        Func<RunContext, EventHandler, Task<RunContext>>? contextBuilder
    )
    {
        foreach (var instance in guild.EventHandlers.Where(h => h.EventId == eventId && h.Enabled))
        {
            var context = contextBuilder is not null
                ? await contextBuilder.Invoke(new RunContext(), instance)
                : new RunContext();

            await Handle(instance, context);
        }
    }

    public async Task Handle(EventHandler instance, RunContext context)
    {
        if (instance.Guild is null)
        {
            _logger.LogError($"Event ID {instance.EventId} isn't associated with a guild. This shouldn't happen.");
            return;
        }

        if (instance.CacheEvent && IsEventCached(instance.Id, context.Metadata, out var expires))
        {
            _logger.LogDebug($"Event {instance.EventId} ({instance.Id}) is on cooldown until {expires.LocalDateTime:s}");
            return;
        }

        if (!_filterManager.CheckFilters(
            instance.Filters ?? [],
            instance.FilterMode,
            context))
        {
            return;
        }

        if (instance.ShouldCache)
        {
            var keysToCache = CreateCacheKeyValues(context.Metadata, instance.CacheKeys!);
            CacheEvent(instance, keysToCache, instance.CacheTime!.Value);
        }

        await _actionManager.RunActions(instance.Actions, context);
    }
}