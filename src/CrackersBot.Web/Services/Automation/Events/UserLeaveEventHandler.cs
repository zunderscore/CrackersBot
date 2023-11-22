using CrackersBot.Core;
using CrackersBot.Core.Filters;
using Discord;

namespace CrackersBot.Web.Services.Automation.Events
{

    public class UserLeaveEventHandler : EventHandlerBase
    {
        public UserLeaveEventHandler(KeyValuePair<string, Dictionary<string, object>> action) :
            this(new Dictionary<string, Dictionary<string, object>>(new[] { action }), new List<IFilter>(), FilterMode.All)
        { }

        public UserLeaveEventHandler(Dictionary<string, Dictionary<string, object>> actions) :
            this(actions, new List<IFilter>(), FilterMode.All)
        { }

        public UserLeaveEventHandler(KeyValuePair<string, Dictionary<string, object>> action, IFilter filter, FilterMode filterMode = FilterMode.All) :
            this(new Dictionary<string, Dictionary<string, object>>(new[] { action }), new List<IFilter>() { filter }, filterMode)
        { }

        public UserLeaveEventHandler(Dictionary<string, Dictionary<string, object>> actions, IFilter filter, FilterMode filterMode = FilterMode.All) :
            this(actions, new List<IFilter>() { filter }, filterMode)
        { }

        public UserLeaveEventHandler(KeyValuePair<string, Dictionary<string, object>> action, IEnumerable<IFilter> filters, FilterMode filterMode = FilterMode.All) :
            this(new Dictionary<string, Dictionary<string, object>>(new[] { action }), filters, filterMode)
        { }

        public UserLeaveEventHandler(Dictionary<string, Dictionary<string, object>> actions, IEnumerable<IFilter> filters, FilterMode filterMode = FilterMode.All) :
            base(actions, filters, filterMode)
        { }

        public async Task Handle(IBotCore bot, IUser user)
        {
            var context = new Dictionary<string, object>(){
                { CommonNames.DISCORD_USER_ID, user.Id }
            };

            if (!CheckFilters(context)) return;

            await RunActions(bot, context);
        }
    }
}