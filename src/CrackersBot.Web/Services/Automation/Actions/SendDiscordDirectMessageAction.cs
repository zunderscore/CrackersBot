using CrackersBot.Core;
using CrackersBot.Core.Actions;
using Discord;

namespace CrackersBot.Web.Services.Automation.Actions
{
    [ActionId("CrackersBot.SendDiscordDiscordMessage")]
    [ActionName("Send Discord Direct Message")]
    [ActionDescription("Sends a direct message to the specified Discord user")]
    public class SendDiscordDirectMessageAction : IAction
    {
        public SendDiscordDirectMessageAction() { }

        public Dictionary<string, object> ConvertRawParameters(Dictionary<string, string> rawParams)
        {
            throw new NotImplementedException();
        }

        public bool ValidateParameters(Dictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey(CommonNames.DISCORD_USER_ID) || !(parameters[CommonNames.DISCORD_USER_ID] is ulong))
            {
                return false;
            }
            
            if (!parameters.ContainsKey(CommonNames.MESSAGE_TEXT) ||
                !(parameters[CommonNames.MESSAGE_TEXT] is string) ||
                String.IsNullOrEmpty((string)parameters[CommonNames.MESSAGE_TEXT]))
            {
                return false;
            }

            return true;
        }

        public async Task Run(IBotCore bot, Dictionary<string, object> parameters, Dictionary<string, object> context)
        {
            if (bot is null || bot.DiscordClient is null) return;

            if (!ValidateParameters(parameters)) return;

            var user = await bot.DiscordClient.GetUserAsync((ulong)parameters[CommonNames.DISCORD_USER_ID]);
            await user.SendMessageAsync((string)parameters[CommonNames.MESSAGE_TEXT]);
        }
    }
}