using CrackersBot.Core.Actions;
using CrackersBot.Core.Filters;
using System.Diagnostics;
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
            if (!CheckFilters(bot, filters ?? [], filterMode, context)) return;

            await RunActions(bot, actions, context);
        }

        public bool CheckFilters(
            IBotCore bot,
            IEnumerable<FilterInstance> filters,
            FilterMode filterMode,
            RunContext context
        )
        {
            var filterPass = false;
            foreach (var filter in filters)
            {
                try
                {
                    var filterResult = bot.RegisteredFilters.ContainsKey(filter.FilterId)
                        && bot.RegisteredFilters[filter.FilterId].Pass(context, filter);

                    if (filterMode == FilterMode.Any && filterResult)
                    {
                        filterPass = true;
                        break;
                    }

                    if (filterMode == FilterMode.All && !filterResult) return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return false;
                }
            }

            return filterMode != FilterMode.Any || filterPass;
        }

        public async Task RunActions(
            IBotCore bot,
            IEnumerable<ActionInstance> actions,
            RunContext context
        ) => await ActionRunner.RunActions(bot, actions, context);
    }
}