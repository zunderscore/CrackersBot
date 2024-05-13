using System.Diagnostics;

namespace CrackersBot.Core.Filters
{
    public static class FilterHelpers
    {
        public static bool CheckFilters(
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
    }
}