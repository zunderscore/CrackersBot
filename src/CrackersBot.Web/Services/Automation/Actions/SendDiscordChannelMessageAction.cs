using CrackersBot.Core;
using CrackersBot.Core.Actions;
using Discord;

namespace CrackersBot.Web.Services.Automation.Actions
{
    [ActionId("CrackersBot.SendDiscordChannelMessage")]
    [ActionName("Send Discord Channel Message")]
    [ActionDescription("Sends a message to the specified Discord channel")]
    public class SendDiscordChannelMessageAction : IAction
    {
        public SendDiscordChannelMessageAction() { }

        public Dictionary<string, object> ConvertRawParameters(Dictionary<string, string> rawParams)
        {
            throw new NotImplementedException();
        }

        public bool ValidateParameters(Dictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey(CommonNames.DISCORD_CHANNEL_ID) || !(parameters[CommonNames.DISCORD_CHANNEL_ID] is ulong))
            {
                return false;
            }

            if ((!parameters.ContainsKey(CommonNames.MESSAGE_TEXT) && !parameters.ContainsKey(CommonNames.DISCORD_EMBED)))
            {
                return false;
            }

            if(parameters.ContainsKey(CommonNames.MESSAGE_TEXT) &&
                (!(parameters[CommonNames.MESSAGE_TEXT] is string) || String.IsNullOrEmpty((string)parameters[CommonNames.MESSAGE_TEXT])))
            {
                return false;   
            }

            return true;
        }

        public async Task Run(IBotCore bot, Dictionary<string, object> parameters, Dictionary<string, object> context)
        {
            if (bot is null || bot.DiscordClient is null) return;

            if (!ValidateParameters(parameters)) return;

            var channel = await bot.DiscordClient.GetChannelAsync((ulong)parameters[CommonNames.DISCORD_CHANNEL_ID]);

            if (channel is ITextChannel textChannel)
            {
                var message = !parameters.ContainsKey(CommonNames.MESSAGE_TEXT) || parameters[CommonNames.MESSAGE_TEXT] is null ?
                    String.Empty :
                    parameters[CommonNames.MESSAGE_TEXT].ToString() ?? String.Empty;

                message = bot.ProcessVariables(message, context);

                var embed = parameters[CommonNames.DISCORD_EMBED] as Embed;

                await textChannel.SendMessageAsync(message,
                    embed: embed is null ? null : embed.BuildDiscordEmbed(bot, context));
            }
        }
    }
}