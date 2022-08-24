using CrackersBot.Core;
using CrackersBot.Core.Events;
using CrackersBot.Core.Filters;

namespace CrackersBot.Web.Services.Automation.Events
{
    public abstract class EventHandlerBase : IEventHandler
    {
        public EventHandlerBase(Dictionary<string, Dictionary<string, object>> actions, IEnumerable<IFilter> filters, FilterMode filterMode)
        {
            Actions = (Dictionary<string, Dictionary<string, object>>)actions;
            Filters = filters.ToList();
            FilterMode = filterMode;
        }
        
        public Dictionary<string, Dictionary<string, object>> Actions { get; }
        public List<IFilter> Filters { get; }
        public FilterMode FilterMode { get; }

        public bool CheckFilters(Dictionary<string, object> parameters)
        {
            var filterPass = false;
            foreach (var filter in Filters)
            {
                try
                {
                    var filterResult = filter.Pass(parameters);

                    if (FilterMode == FilterMode.Any && filterResult)
                    {
                        filterPass = true;
                        break;
                    }

                    if (FilterMode == FilterMode.All && !filterResult) return false;
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }

            if (FilterMode == FilterMode.Any && !filterPass) return false;

            return true;
        }

        public async Task RunActions(IBotCore bot, Dictionary<string, object> context)
        {
            foreach (var actionEntry in Actions)
            {
                try
                {
                    var action = bot.GetRegisteredAction(actionEntry.Key);
                    await action.Run(bot, actionEntry.Value, context);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}