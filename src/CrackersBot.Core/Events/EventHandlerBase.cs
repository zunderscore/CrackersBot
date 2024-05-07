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
            List<KeyValuePair<string, Dictionary<string, string>>> actions,
            Dictionary<string, object>? context = null,
            IEnumerable<FilterDefinition>? filters = null,
            FilterMode filterMode = FilterMode.All
        )
        {
            if (!CheckFilters(bot, filters ?? [], filterMode, context ?? [])) return;

            await RunActions(bot, actions, context ?? []);
        }

        public bool CheckFilters(
            IBotCore bot,
            IEnumerable<FilterDefinition> filterDefinitions,
            FilterMode filterMode,
            Dictionary<string, object> context
        )
        {
            var filterPass = false;
            foreach (var filterDef in filterDefinitions)
            {
                try
                {
                    var filterResult = bot.RegisteredFilters.ContainsKey(filterDef.FilterId)
                        && bot.RegisteredFilters[filterDef.FilterId].Pass(context, filterDef);

                    if (filterMode == FilterMode.Any && filterResult)
                    {
                        filterPass = true;
                        break;
                    }

                    if (filterMode == FilterMode.All && !filterResult) return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }

            return filterMode != FilterMode.Any || filterPass;
        }

        public async Task RunActions(
            IBotCore bot,
            List<KeyValuePair<string, Dictionary<string, string>>> actions,
            Dictionary<string, object> context
        ) => await ActionRunner.RunActions(bot, actions, context);
    }
}