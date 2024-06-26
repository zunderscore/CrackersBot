namespace CrackersBot.Core.Actions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionNameAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }
}