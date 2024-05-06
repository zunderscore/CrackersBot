
using System.Reflection;

namespace CrackersBot.Core.Variables
{
    public abstract class VariableBase : IVariable
    {
        public string Token => GetType().GetCustomAttribute<VariableTokenAttribute>()?.Token ?? String.Empty;

        public virtual string GetValue(IBotCore bot, Dictionary<string, object> context)
        {
            return DefaultVariableProcessor.GetValue(Token, context);
        }
    }
}