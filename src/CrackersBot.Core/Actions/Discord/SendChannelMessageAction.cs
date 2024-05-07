using CrackersBot.Core.Parameters;
using Discord;

namespace CrackersBot.Core.Actions.Discord
{
    [ActionId(ACTION_ID)]
    [ActionName("Send Discord Channel Message")]
    [ActionDescription("Sends a message to the specified Discord channel")]
    public class SendChannelMessageAction : ActionBase
    {
        public const string ACTION_ID = "CrackersBot.Discord.SendChannelMessage";

        public override Dictionary<string, IParameterType> ActionParameters => new() {
            { CommonNames.DISCORD_CHANNEL_ID, new UInt64ParameterType() },
            { CommonNames.MESSAGE_TEXT, new StringParameterType(true) },
            { CommonNames.DISCORD_EMBED, new ObjectParameterType(typeof(EmbedDefinition), true) }
        };

        public override bool ValidateParameters(Dictionary<string, string> parameters)
        {
            var hasMessage = parameters.ContainsKey(CommonNames.MESSAGE_TEXT)
                && ActionParameters[CommonNames.MESSAGE_TEXT].Validate(parameters[CommonNames.MESSAGE_TEXT], true);

            var hasEmbed = parameters.ContainsKey(CommonNames.DISCORD_EMBED)
                && ActionParameters[CommonNames.DISCORD_EMBED].Validate(parameters[CommonNames.DISCORD_EMBED], true);

            return base.ValidateParameters(parameters)
                && (hasMessage || hasEmbed);
        }

        public override async Task Run(IBotCore bot, Dictionary<string, object> parameters, RunContext context)
        {
            var channel = await bot.DiscordClient.GetChannelAsync((ulong)parameters[CommonNames.DISCORD_CHANNEL_ID]);

            if (channel is ITextChannel textChannel)
            {
                var message = !parameters.TryGetValue(CommonNames.MESSAGE_TEXT, out object? value) || value is null
                    ? String.Empty
                    : value.ToString() ?? String.Empty;

                var embed = parameters[CommonNames.DISCORD_EMBED] as EmbedDefinition;

                await textChannel.SendMessageAsync(message,
                    embed: embed?.BuildDiscordEmbed(bot, context));
            }
        }
    }
}