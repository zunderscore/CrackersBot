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
        public string EventId { get; set; } = eventId;
        public bool Enabled { get; set; } = enabled;
        public IEnumerable<ActionInstance> Actions { get; set; } = actions;
        public IEnumerable<FilterInstance>? Filters { get; set; } = filters;
        public FilterMode FilterMode { get; set; } = filterMode;
    }
}