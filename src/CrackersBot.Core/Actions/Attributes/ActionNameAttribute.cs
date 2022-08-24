namespace CrackersBot.Core.Actions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionNameAttribute : Attribute
    {
        public ActionNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}