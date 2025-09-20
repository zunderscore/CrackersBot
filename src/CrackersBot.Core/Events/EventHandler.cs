using CrackersBot.Core.Actions;
using CrackersBot.Core.Filters;

namespace CrackersBot.Core.Events;

public record EventHandler(
    string Id,
    string EventId,
    IEnumerable<ActionInstance> Actions,
    IEnumerable<FilterInstance>? Filters = null,
    FilterMode FilterMode = FilterMode.All,
    bool Enabled = true,
    bool CacheEvent = false,
    List<string>? CacheKeys = null,
    TimeSpan? CacheTime = null
)
{
    public GuildConfig? Guild { get; private set; }

    public void SetGuild(GuildConfig guild) => Guild ??= guild;

    public bool ShouldCache => CacheEvent
        && CacheTime is not null;
}