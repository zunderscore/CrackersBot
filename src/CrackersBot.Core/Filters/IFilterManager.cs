using System.Collections.Concurrent;

namespace CrackersBot.Core.Filters;

public interface IFilterManager
{
    ConcurrentDictionary<string, IFilter> RegisteredFilters { get; }

    bool IsFilterRegistered(string id);

    void RegisterFilter(IFilter filter);

    void UnregisterFilter(string id);

    public bool CheckFilters(
        IEnumerable<FilterInstance> filters,
        FilterMode filterMode,
        RunContext context
    );
}