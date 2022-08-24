using CrackersBot.Core;
using CrackersBot.Core.Filters;
using Discord;

namespace CrackersBot.Web.Services.Automation.Events
{

    public class MessageReceivedEventHandler : EventHandlerBase
    {
        public MessageReceivedEventHandler(KeyValuePair<string, Dictionary<string, object>> action, bool ignoreBotMessages = true) :
            this(new Dictionary<string, Dictionary<string, object>>(new [] { action }), new List<IFilter>(), FilterMode.All, ignoreBotMessages)
        { }

        public MessageReceivedEventHandler(Dictionary<string, Dictionary<string, object>> actions, bool ignoreBotMessages = true) :
            this(actions, new List<IFilter>(), FilterMode.All, ignoreBotMessages)
        { }

        public MessageReceivedEventHandler(KeyValuePair<string, Dictionary<string, object>> action, IFilter filter, FilterMode filterMode = FilterMode.All, bool ignoreBotMessages = true) :
            this(new Dictionary<string, Dictionary<string, object>>(new [] { action }), new List<IFilter>() { filter }, filterMode, ignoreBotMessages)
        { }

        public MessageReceivedEventHandler(Dictionary<string, Dictionary<string, object>> actions, IFilter filter, FilterMode filterMode = FilterMode.All, bool ignoreBotMessages = true) :
            this(actions, new List<IFilter>() { filter }, filterMode, ignoreBotMessages)
        { }

        public MessageReceivedEventHandler(KeyValuePair<string, Dictionary<string, object>> action, IEnumerable<IFilter> filters, FilterMode filterMode = FilterMode.All, bool ignoreBotMessages = true) :
            this(new Dictionary<string, Dictionary<string, object>>(new [] { action }), filters, filterMode, ignoreBotMessages)
        { }

        public MessageReceivedEventHandler(Dictionary<string, Dictionary<string, object>> actions, IEnumerable<IFilter> filters, FilterMode filterMode = FilterMode.All, bool ignoreBotMessages = true) :
            base(actions, filters, filterMode)
        {
            IgnoreBotMessages = ignoreBotMessages;
        }

        public bool IgnoreBotMessages { get; }

        public async Task Handle(IBotCore bot, IMessage message)
        {
            if (IgnoreBotMessages && message.Author.IsBot) return;

            var context = new Dictionary<string, object>()
            {
                { CommonNames.DISCORD_AUTHOR_ID, message.Author.Id },
                { CommonNames.DISCORD_CHANNEL_ID, message.Channel.Id },
                { CommonNames.DISCORD_MESSAGE_ID, message.Id },
                { CommonNames.MESSAGE_TEXT, message.ToString() ?? String.Empty }
            };

            if (!CheckFilters(context)) return;

            await RunActions(bot, context);
        }
    }
}