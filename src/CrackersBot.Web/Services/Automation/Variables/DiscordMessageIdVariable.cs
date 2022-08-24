using CrackersBot.Core;
using CrackersBot.Core.Variables;

namespace CrackersBot.Web.Services.Automation.Variables
{
    public class DiscordMessageIdVariable : IVariable
    {
        public string Token => CommonNames.DISCORD_MESSAGE_ID;

        public string GetValue(IBotCore bot, Dictionary<string, object> context)
        {
            return DefaultVariableProcessor.GetValue(Token, context);
        }
    }
}