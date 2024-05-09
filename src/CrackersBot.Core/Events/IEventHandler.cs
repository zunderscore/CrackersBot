using CrackersBot.Core.Actions;
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
            IEnumerable<FilterInstance> filters,
            FilterMode filterMode,
            RunContext context
        );

        public virtual Task Handle(
            IBotCore bot,
            EventHandlerInstance instance,
            RunContext context
        ) => Handle(bot, instance.Actions, context, instance.Filters, instance.FilterMode);

        Task Handle(
            IBotCore bot,
            IEnumerable<ActionInstance> actions,
            RunContext context,
            IEnumerable<FilterInstance>? filters = null,
            FilterMode filterMode = FilterMode.All
        );

        Task RunActions(
            IBotCore bot,
            IEnumerable<ActionInstance> actions,
            RunContext context
        );
    }
}