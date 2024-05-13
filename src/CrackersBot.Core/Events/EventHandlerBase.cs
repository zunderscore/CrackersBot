using CrackersBot.Core.Actions;
using CrackersBot.Core.Filters;
using System.Reflection;

namespace CrackersBot.Core.Events
{
    public abstract class EventHandlerBase : IEventHandler
    {
        public string GetEventId() => GetType().GetCustomAttribute<EventIdAttribute>()?.Id ?? String.Empty;
        public string GetEventName() => GetType().GetCustomAttribute<EventNameAttribute>()?.Name ?? String.Empty;
        public string GetEventDescription() => GetType().GetCustomAttribute<EventDescriptionAttribute>()?.Description ?? String.Empty;

        public virtual async Task Handle(
            IBotCore bot,
            IEnumerable<ActionInstance> actions,
            RunContext context,
            IEnumerable<FilterInstance>? filters = null,
            FilterMode filterMode = FilterMode.All
        )
        {
            if (!FilterHelpers.CheckFilters(bot, filters ?? [], filterMode, context)) return;

            await RunActions(bot, actions, context);
        }

        public async Task RunActions(
            IBotCore bot,
            IEnumerable<ActionInstance> actions,
            RunContext context
        ) => await ActionRunner.RunActions(bot, actions, context);
    }
}