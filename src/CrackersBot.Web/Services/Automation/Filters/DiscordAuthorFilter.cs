using CrackersBot.Core;
using CrackersBot.Core.Filters;

namespace CrackersBot.Web.Services.Automation.Filters
{
    public class DiscordAuthorFilter : IFilter
    {
        public DiscordAuthorFilter(FilterInclusionType filterType, ulong userId) :
            this(filterType, new List<ulong>() { userId }) { }

        public DiscordAuthorFilter(FilterInclusionType filterType, IEnumerable<ulong>? userList = null)
        {
            FilterType = filterType;
            UserList = userList is null ? new List<ulong>() : userList.ToList();
        }

        public FilterInclusionType FilterType { get; }

        public List<ulong> UserList { get; }

        public bool Pass(Dictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey(CommonNames.DISCORD_AUTHOR_ID) || !(parameters[CommonNames.DISCORD_AUTHOR_ID] is ulong)) return false;

            switch (FilterType)
            {
                case FilterInclusionType.Include:
                    return UserList.Contains((ulong)parameters[CommonNames.DISCORD_AUTHOR_ID]);

                case FilterInclusionType.Exclude:
                    return !UserList.Contains((ulong)parameters[CommonNames.DISCORD_AUTHOR_ID]);

                default:
                    return true;
            }
        }
    }
}