namespace CrackersBot.Core.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FilterNameAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }
}