using CrackersBot.Core.Filters;

namespace CrackersBot.Core.Events
{
    public interface IEventHandler
    {
        string GetEventId();
        string GetEventName();
        string GetEventDescription();

        bool CheckFilters(
            IBotCore bot,
            IEnumerable<FilterDefinition> filterDefinitions,
            FilterMode filterMode,
            Dictionary<string, object> context
        );

        public virtual Task Handle(
            IBotCore bot,
            EventHandlerDefinition definition,
            Dictionary<string, object>? context = null
        ) => Handle(bot, definition.Actions, context, definition.Filters, definition.FilterMode);

        Task Handle(
            IBotCore bot,
            Dictionary<string, Dictionary<string, string>> actions,
            Dictionary<string, object>? context = null,
            IEnumerable<FilterDefinition>? filters = null,
            FilterMode filterMode = FilterMode.All
        );

        Task RunActions(
            IBotCore bot,
            Dictionary<string, Dictionary<string, string>> actions,
            Dictionary<string, object> context
        );
    }
}