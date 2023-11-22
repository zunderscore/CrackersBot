using CrackersBot.Core;
using CrackersBot.Core.Variables;

namespace CrackersBot.Web.Services.Automation.Variables
{
    public class DiscordUserIdVariable : IVariable
    {
        public string Token => CommonNames.DISCORD_USER_ID;

        public string GetValue(IBotCore bot, Dictionary<string, object> context)
        {
            return DefaultVariableProcessor.GetValue(Token, context);
        }
    }
}