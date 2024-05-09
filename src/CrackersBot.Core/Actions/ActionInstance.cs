using CrackersBot.Core.Filters;

namespace CrackersBot.Core.Actions
{
    public record ActionInstance(
        string ActionId,
        Dictionary<string, string>? Parameters = null,
        IEnumerable<FilterInstance>? Filters = null,
        bool Enabled = true
    );
}