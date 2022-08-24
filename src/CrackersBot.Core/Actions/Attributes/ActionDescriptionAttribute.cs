namespace CrackersBot.Core.Actions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionDescriptionAttribute : Attribute
    {
        public ActionDescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}