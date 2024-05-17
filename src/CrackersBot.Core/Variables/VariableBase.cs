
using System.Reflection;

namespace CrackersBot.Core.Variables
{
    public abstract class VariableBase(IBotCore bot) : IVariable
    {
        public IBotCore Bot { get; } = bot;

        public string Token => GetType().GetCustomAttribute<VariableTokenAttribute>()?.Token ?? String.Empty;

        public virtual string GetValue(RunContext context)
        {
            return DefaultVariableProcessor.GetValue(Token, context);
        }
    }
}