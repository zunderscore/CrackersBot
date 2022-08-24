using CrackersBot.Core;
using CrackersBot.Core.Variables;

namespace CrackersBot.Web.Services.Automation.Variables
{
    public class RegisteredActionCountVariable : IVariable
    {
        public string Token => CommonNames.REGISTERED_ACTION_COUNT;

        public string GetValue(IBotCore bot, Dictionary<string, object> context) => ((BotCore)bot).RegisteredActions.Count.ToString();
    }
}