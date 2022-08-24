namespace CrackersBot.Core.Filters
{
    public interface IFilter
    {
        bool Pass(Dictionary<string, object> parameters);
    }
}