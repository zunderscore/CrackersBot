namespace CrackersBot.Core.Variables
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VariableDescriptionAttribute(string description) : Attribute
    {
        public string Description { get; } = description;
    }
}