namespace CrackersBot.Core.Actions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionIdAttribute(string id) : Attribute
    {
        public string Id { get; } = id;
    }
}