using CrackersBot.Core.Parameters;
using Discord;

namespace CrackersBot.Core.Actions.Discord
{
    [ActionId(ACTION_ID)]
    [ActionName("React to Discord Message")]
    [ActionDescription("Adds a reaction to the specified Discord message")]
    public class ReactToMessageAction : ActionBase
    {
        public const string ACTION_ID = "CrackersBot.Discord.ReactToMessage";

        public override Dictionary<string, IParameterType> ActionParameters => new() {
            { CommonNames.DISCORD_GUILD_ID, new UInt64ParameterType() },
            { CommonNames.DISCORD_CHANNEL_ID, new UInt64ParameterType() },
            { CommonNames.DISCORD_MESSAGE_ID, new UInt64ParameterType() },
            { CommonNames.DISCORD_EMOTE_NAME, new StringParameterType() }
        };

        public override async Task Run(IBotCore bot, Dictionary<string, object> parameters, RunContext context)
        {
            if (!parameters.TryGetValue(CommonNames.DISCORD_CHANNEL_ID, out object? channelId) || !parameters.TryGetValue(CommonNames.DISCORD_MESSAGE_ID, out object? messageId)) return;

            var guild = await bot.DiscordClient.GetGuildAsync((ulong)parameters[CommonNames.DISCORD_GUILD_ID]);
            var emote = (await guild.GetEmotesAsync())
                .FirstOrDefault(e => e.Name.Equals((string)parameters[CommonNames.DISCORD_EMOTE_NAME], StringComparison.CurrentCultureIgnoreCase));

            if (emote is null) return;

            var channel = (ITextChannel)await guild.GetChannelAsync((ulong)channelId);
            var message = await channel.GetMessageAsync((ulong)messageId);

            await message.AddReactionAsync(emote);
        }
    }
}