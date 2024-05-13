using CrackersBot.Core.Filters;

namespace CrackersBot.Core.Actions
{
    public record ActionInstance(
        string ActionId,
        Dictionary<string, string>? Parameters = null,
        IEnumerable<FilterInstance>? Filters = null,
        FilterMode FilterMode = FilterMode.All,
        bool Enabled = true
    );
}