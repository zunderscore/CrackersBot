namespace CrackersBot.Core.Actions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionDescriptionAttribute(string description) : Attribute
    {
        public string Description { get; } = description;
    }
}