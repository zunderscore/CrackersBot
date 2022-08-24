using CrackersBot.Core.Filters;

namespace CrackersBot.Core.Events
{
    public interface IEventHandler
    {
        Dictionary<string, Dictionary<string, object>> Actions { get; }
        List<IFilter> Filters { get; }
        FilterMode FilterMode { get; }

        bool CheckFilters(Dictionary<string, object> parameters);

        Task RunActions(IBotCore bot, Dictionary<string, object> context);
    }
}