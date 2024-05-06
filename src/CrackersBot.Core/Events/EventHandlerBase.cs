using CrackersBot.Core.Filters;
using CrackersBot.Core.Parameters;
using CrackersBot.Core.Variables;
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
            Dictionary<string, Dictionary<string, string>> actions,
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
            Dictionary<string, Dictionary<string, string>> actions,
            Dictionary<string, object> context
        )
        {
            foreach (var (actionType, parameters) in actions)
            {
                try
                {
                    var processedParams = new Dictionary<string, string>();
                    foreach (var (paramName, paramValue) in parameters)
                    {
                        processedParams.Add(paramName, DefaultVariableProcessor.ProcessVariables(bot, paramValue, context));
                    }

                    var action = bot.GetRegisteredAction(actionType);
                    if (action.DoPreRunCheck(bot, processedParams))
                    {
                        var parsedParams = ParameterHelpers.GetParameterValues(action.ActionParameters, processedParams);
                        await action.Run(bot, parsedParams);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}