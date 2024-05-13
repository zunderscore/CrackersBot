using CrackersBot.Core.Actions;
using CrackersBot.Core.Filters;

namespace CrackersBot.Core.Events
{
    public class EventHandlerInstance(
        string eventId,
        IEnumerable<ActionInstance> actions,
        IEnumerable<FilterInstance>? filters = null,
        FilterMode filterMode = FilterMode.All,
        bool enabled = true
    )
    {
        public string EventId { get; } = eventId;
        public bool Enabled { get; } = enabled;
        public IEnumerable<ActionInstance> Actions { get; } = actions;
        public IEnumerable<FilterInstance>? Filters { get; } = filters;
        public FilterMode FilterMode { get; } = filterMode;
    }
}