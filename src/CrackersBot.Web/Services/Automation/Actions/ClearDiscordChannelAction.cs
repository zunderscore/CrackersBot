using CrackersBot.Core;
using CrackersBot.Core.Actions;
using Discord;

namespace CrackersBot.Web.Services.Automation.Actions
{
    [ActionId("CrackersBot.ClearDiscordChannel")]
    [ActionName("Clear Discord Channel")]
    [ActionDescription("Clears messages from a Discord channel")]
    public class ClearDiscordChannelAction : IAction
    {
        public ClearDiscordChannelAction() { }

        public Dictionary<string, object> ConvertRawParameters(Dictionary<string, string> rawParams)
        {
            throw new NotImplementedException();
        }

        public bool ValidateParameters(Dictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey(CommonNames.DISCORD_CHANNEL_ID) || !(parameters[CommonNames.DISCORD_CHANNEL_ID] is ulong))
                return false;

            if (parameters.ContainsKey(CommonNames.LIMIT) && !(parameters[CommonNames.LIMIT] is ushort))
                return false;

            return true;
        }

        public async Task Run(IBotCore bot, Dictionary<string, object> parameters, Dictionary<string, object> context)
        {
            if (bot is null || bot.DiscordClient is null) return;

            if (!ValidateParameters(parameters)) return;

            ushort limit = 100;
            if (parameters.ContainsKey(CommonNames.LIMIT) && parameters[CommonNames.LIMIT] is ushort)
            {
                limit = (ushort)parameters[CommonNames.LIMIT];
            }

            var channel = await bot.DiscordClient.GetChannelAsync((ulong)parameters[CommonNames.DISCORD_CHANNEL_ID]);

            if (channel is ITextChannel textChannel)
            {
                var messages = await textChannel.GetMessagesAsync(limit).FlattenAsync();
                await textChannel.DeleteMessagesAsync(messages);
            }
        }
    }
}