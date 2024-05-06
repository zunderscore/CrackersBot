namespace CrackersBot.Core.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FilterDescriptionAttribute(string description) : Attribute
    {
        public string Description { get; } = description;
    }
}