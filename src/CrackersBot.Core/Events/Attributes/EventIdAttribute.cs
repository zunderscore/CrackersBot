namespace CrackersBot.Core.Events
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventIdAttribute(string id) : Attribute
    {
        public string Id { get; } = id;
    }
}