namespace CrackersBot.Core.Variables
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VariableTokenAttribute(string token) : Attribute
    {
        public string Token { get; } = token;
    }
}