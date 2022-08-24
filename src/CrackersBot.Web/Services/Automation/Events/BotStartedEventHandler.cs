using CrackersBot.Core;
using CrackersBot.Core.Filters;
using Discord;

namespace CrackersBot.Web.Services.Automation.Events
{

    public class BotStartedEventHandler : EventHandlerBase
    {
        public BotStartedEventHandler(KeyValuePair<string, Dictionary<string, object>> action) :
            this(new Dictionary<string, Dictionary<string, object>>(new [] { action }), new List<IFilter>(), FilterMode.All)
        { }

        public BotStartedEventHandler(Dictionary<string, Dictionary<string, object>> actions) :
            this(actions, new List<IFilter>(), FilterMode.All)
        { }

        public BotStartedEventHandler(KeyValuePair<string, Dictionary<string, object>> action, IFilter filter, FilterMode filterMode = FilterMode.All) :
            this(new Dictionary<string, Dictionary<string, object>>(new [] { action }), new List<IFilter>() { filter }, filterMode)
        { }

        public BotStartedEventHandler(Dictionary<string, Dictionary<string, object>> actions, IFilter filter, FilterMode filterMode = FilterMode.All) :
            this(actions, new List<IFilter>() { filter }, filterMode)
        { }

        public BotStartedEventHandler(KeyValuePair<string, Dictionary<string, object>> action, IEnumerable<IFilter> filters, FilterMode filterMode = FilterMode.All) :
            this(new Dictionary<string, Dictionary<string, object>>(new [] { action }), filters, filterMode)
        { }

        public BotStartedEventHandler(Dictionary<string, Dictionary<string, object>> actions, IEnumerable<IFilter> filters, FilterMode filterMode = FilterMode.All) :
            base(actions, filters, filterMode)
        { }

        public async Task Handle(IBotCore bot)
        {
            var context = new Dictionary<string, object>();

            if (!CheckFilters(context)) return;

            await RunActions(bot, context);
        }
    }
}