using CrackersBot.Core.Actions;
using CrackersBot.Core.Filters;
using System.Reflection;

namespace CrackersBot.Core.Events
{
    public abstract class EventHandlerBase(IBotCore bot) : IEventHandler
    {
        public IBotCore Bot { get; } = bot;

        public string GetId() => GetType().GetCustomAttribute<EventIdAttribute>()?.Id ?? String.Empty;
        public string GetName() => GetType().GetCustomAttribute<EventNameAttribute>()?.Name ?? String.Empty;
        public string GetDescription() => GetType().GetCustomAttribute<EventDescriptionAttribute>()?.Description ?? String.Empty;

        public virtual async Task Handle(
            IEnumerable<ActionInstance> actions,
            RunContext context,
            IEnumerable<FilterInstance>? filters = null,
            FilterMode filterMode = FilterMode.All
        )
        {
            if (!FilterHelpers.CheckFilters(Bot, filters ?? [], filterMode, context)) return;

            await RunActions(actions, context);
        }

        public async Task RunActions(
            IEnumerable<ActionInstance> actions,
            RunContext context
        ) => await ActionRunner.RunActions(Bot, actions, context);
    }
}