using CrackersBot.Core.Filters;

namespace CrackersBot.Core.Events
{
    public class EventHandlerDefinition(
        string eventId,
        List<KeyValuePair<string, Dictionary<string, string>>> actions,
        IEnumerable<FilterDefinition>? filters = null,
        FilterMode filterMode = FilterMode.All
    )
    {
        public string EventId { get; } = eventId;
        public List<KeyValuePair<string, Dictionary<string, string>>> Actions { get; } = actions;
        public IEnumerable<FilterDefinition>? Filters { get; } = filters;
        public FilterMode FilterMode { get; } = filterMode;
    }
}