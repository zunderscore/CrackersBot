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
            RunContext context
        );

        public virtual Task Handle(
            IBotCore bot,
            EventHandlerDefinition definition,
            RunContext context
        ) => Handle(bot, definition.Actions, context, definition.Filters, definition.FilterMode);

        Task Handle(
            IBotCore bot,
            List<KeyValuePair<string, Dictionary<string, string>>> actions,
            RunContext context,
            IEnumerable<FilterDefinition>? filters = null,
            FilterMode filterMode = FilterMode.All
        );

        Task RunActions(
            IBotCore bot,
            List<KeyValuePair<string, Dictionary<string, string>>> actions,
            RunContext context
        );
    }
}