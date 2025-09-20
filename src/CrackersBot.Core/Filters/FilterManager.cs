using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace CrackersBot.Core.Filters;

public class FilterManager(
    BotServiceProvider botServiceProvider
) : IFilterManager
{
    private readonly ILogger<FilterManager> _logger = botServiceProvider.GetLogger<FilterManager>();

    public ConcurrentDictionary<string, IFilter> RegisteredFilters { get; } = [];

    public bool IsFilterRegistered(string token)
    {
        return RegisteredFilters.Keys.Any(k => k.Equals(token, StringComparison.InvariantCultureIgnoreCase));
    }

    public void RegisterFilter(IFilter filter)
    {
        if (IsFilterRegistered(filter.Id))
        {
            _logger.LogDebug("Unable to register Filter {id} as it has already been registered", filter.Id);
            return;
        }

        if (RegisteredFilters.TryAdd(filter.Id, filter))
        {
            _logger.LogDebug("Registered Filter {id}", filter.Id);
        }
        else
        {
            _logger.LogDebug("Unable to register Filter {id} (registration failed)", filter.Id);
        }
    }

    public void UnregisterFilter(string id)
    {
        if (!IsFilterRegistered(id))
        {
            _logger.LogDebug("Unable to unregister Filter {id} since it is not currently registered", id);
            return;
        }

        if (RegisteredFilters.TryRemove(id, out _))
        {
            _logger.LogDebug("Unregistered Filter {id}", id);
        }
        else
        {
            _logger.LogDebug("Unable to unregister Filter {id} (removal failed)", id);
        }
    }

    public bool CheckFilters(
        IEnumerable<FilterInstance> filters,
        FilterMode filterMode,
        RunContext context
    )
    {
        var filterPass = false;
        foreach (var filter in filters)
        {
            try
            {
                var filterResult = RegisteredFilters.ContainsKey(filter.FilterId)
                    && RegisteredFilters[filter.FilterId].Pass(context, filter);

                if (filterMode == FilterMode.Any && filterResult)
                {
                    filterPass = true;
                    break;
                }

                if (filterMode == FilterMode.All && !filterResult) return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running filter {filterId}", filter.FilterId);
                return false;
            }
        }

        return filterMode != FilterMode.Any || filterPass;
    }
}