namespace CrackersBot.Core.Events;

public record EventCacheItem(
    string EventHandlerId,
    Dictionary<string, string> CacheKeys,
    DateTimeOffset ExpireTime
);