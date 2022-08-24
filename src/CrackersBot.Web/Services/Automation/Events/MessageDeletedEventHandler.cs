using CrackersBot.Core;
using CrackersBot.Core.Filters;
using Discord;

namespace CrackersBot.Web.Services.Automation.Events
{

    public class MessageDeletedEventHandler : EventHandlerBase
    {
        public MessageDeletedEventHandler(KeyValuePair<string, Dictionary<string, object>> action) :
            this(new Dictionary<string, Dictionary<string, object>>(new [] { action }), new List<IFilter>(), FilterMode.All)
        { }

        public MessageDeletedEventHandler(Dictionary<string, Dictionary<string, object>> actions) :
            this(actions, new List<IFilter>(), FilterMode.All)
        { }

        public MessageDeletedEventHandler(KeyValuePair<string, Dictionary<string, object>> action, IFilter filter, FilterMode filterMode = FilterMode.All) :
            this(new Dictionary<string, Dictionary<string, object>>(new [] { action }), new List<IFilter>() { filter }, filterMode)
        { }

        public MessageDeletedEventHandler(Dictionary<string, Dictionary<string, object>> actions, IFilter filter, FilterMode filterMode = FilterMode.All) :
            this(actions, new List<IFilter>() { filter }, filterMode)
        { }

        public MessageDeletedEventHandler(KeyValuePair<string, Dictionary<string, object>> action, IEnumerable<IFilter> filters, FilterMode filterMode = FilterMode.All) :
            this(new Dictionary<string, Dictionary<string, object>>(new [] { action }), filters, filterMode)
        { }

        public MessageDeletedEventHandler(Dictionary<string, Dictionary<string, object>> actions, IEnumerable<IFilter> filters, FilterMode filterMode = FilterMode.All) :
            base(actions, filters, filterMode)
        {
        }

        public async Task Handle(IBotCore bot, ulong messageId, ulong channelId, IMessage? message)
        {
            var context = new Dictionary<string, object>()
            {
                { CommonNames.DISCORD_AUTHOR_ID, message?.Author?.Id ?? 0 },
                { CommonNames.DISCORD_CHANNEL_ID, channelId },
                { CommonNames.DISCORD_MESSAGE_ID, messageId },
                { CommonNames.MESSAGE_TEXT, message?.ToString() ?? String.Empty }
            };

            if (!CheckFilters(context)) return;

            await RunActions(bot, context);
        }
    }
}