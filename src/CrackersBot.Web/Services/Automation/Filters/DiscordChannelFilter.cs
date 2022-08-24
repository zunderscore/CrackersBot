using CrackersBot.Core;
using CrackersBot.Core.Filters;

namespace CrackersBot.Web.Services.Automation.Filters
{
    public class DiscordChannelFilter : IFilter
    {
        public DiscordChannelFilter(FilterInclusionType filterType, ulong channelId) :
            this(filterType, new List<ulong>() { channelId }) { }

        public DiscordChannelFilter(FilterInclusionType filterType, IEnumerable<ulong> channelList)
        {
            FilterType = filterType;
            ChannelList = channelList.ToList();
        }

        public FilterInclusionType FilterType { get; }

        public List<ulong> ChannelList { get; }

        public bool Pass(Dictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey(CommonNames.DISCORD_CHANNEL_ID) || !(parameters[CommonNames.DISCORD_CHANNEL_ID] is ulong))
                return false;

            switch (FilterType)
            {
                case FilterInclusionType.Include:
                    return ChannelList.Contains((ulong)parameters[CommonNames.DISCORD_CHANNEL_ID]);

                case FilterInclusionType.Exclude:
                    return !ChannelList.Contains((ulong)parameters[CommonNames.DISCORD_CHANNEL_ID]);

                default:
                    return true;
            }
        }
    }
}