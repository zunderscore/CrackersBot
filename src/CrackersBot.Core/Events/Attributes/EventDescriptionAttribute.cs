namespace CrackersBot.Core.Events
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventDescriptionAttribute(string description) : Attribute
    {
        public string Description { get; } = description;
    }
}