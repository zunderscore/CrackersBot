namespace CrackersBot.Core.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FilterIdAttribute(string id) : Attribute
    {
        public string Id { get; } = id;
    }
}