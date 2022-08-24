using CrackersBot.Core;
using CrackersBot.Core.Variables;

namespace CrackersBot.Web.Services.Automation.Variables
{
    public class RegisteredVeriableCountVariable : IVariable
    {
        public string Token => CommonNames.REGISTERED_VARIABLE_COUNT;

        public string GetValue(IBotCore bot, Dictionary<string, object> context) => ((BotCore)bot).RegisteredVariables.Count.ToString();
    }
}