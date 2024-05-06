namespace CrackersBot.Core.Events
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventNameAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }
}