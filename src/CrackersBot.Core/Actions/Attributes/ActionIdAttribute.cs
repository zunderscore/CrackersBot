namespace CrackersBot.Core.Actions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionIdAttribute : Attribute
    {
        public ActionIdAttribute(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}