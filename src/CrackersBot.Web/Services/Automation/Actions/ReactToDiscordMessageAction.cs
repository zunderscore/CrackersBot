using CrackersBot.Core;
using CrackersBot.Core.Actions;
using Discord;

namespace CrackersBot.Web.Services.Automation.Actions
{
    [ActionId("CrackersBot.ReactToDiscordMessage")]
    [ActionName("React to Discord Message")]
    [ActionDescription("Adds a reaction to the specified Discord message")]
    public class ReactToDiscordMessageAction : IAction
    {
        public ReactToDiscordMessageAction() { }

        public Dictionary<string, object> ConvertRawParameters(Dictionary<string, string> rawParams)
        {
            throw new NotImplementedException();
        }

        public bool ValidateParameters(Dictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey(CommonNames.DISCORD_GUILD_ID) || !(parameters[CommonNames.DISCORD_GUILD_ID] is ulong))
                return false;

            if (!parameters.ContainsKey(CommonNames.DISCORD_EMOTE_NAME) ||
                !(parameters[CommonNames.DISCORD_EMOTE_NAME] is string) ||
                String.IsNullOrEmpty((string)parameters[CommonNames.DISCORD_EMOTE_NAME]))
                return false;

            return true;
        }

        public async Task Run(IBotCore bot, Dictionary<string, object> parameters, Dictionary<string, object> context)
        {
            if (bot is null || bot.DiscordClient is null) return;

            if (!ValidateParameters(parameters)) return;

            if (!context.ContainsKey(CommonNames.DISCORD_CHANNEL_ID) || !context.ContainsKey(CommonNames.DISCORD_MESSAGE_ID)) return;

            var guild = await bot.DiscordClient.GetGuildAsync((ulong)parameters[CommonNames.DISCORD_GUILD_ID]);
            var emote = (await guild.GetEmotesAsync())
                .FirstOrDefault(e => e.Name.ToLower() == ((string)parameters[CommonNames.DISCORD_EMOTE_NAME]).ToLower());

            if (emote is null) return;

            var channel = (ITextChannel)await guild.GetChannelAsync((ulong)context[CommonNames.DISCORD_CHANNEL_ID]);
            var message = await channel.GetMessageAsync((ulong)context[CommonNames.DISCORD_MESSAGE_ID]);

            await message.AddReactionAsync(emote);
        }
    }
}