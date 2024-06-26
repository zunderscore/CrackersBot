using CrackersBot.Core.Parameters;
using Discord;

namespace CrackersBot.Core.Actions.Discord
{
    [ActionId(ACTION_ID)]
    [ActionName("Clear Discord Channel")]
    [ActionDescription("Clears messages from a Discord channel")]
    public class ClearChannelAction(IBotCore bot) : ActionBase(bot)
    {
        public const string ACTION_ID = "CrackersBot.Discord.ClearChannel";

        public override Dictionary<string, IParameterType> ActionParameters => new() {
            { CommonNames.DISCORD_CHANNEL_ID, new UInt64ParameterType() },
            { CommonNames.LIMIT, new UInt16ParameterType() }
        };

        public override async Task Run(Dictionary<string, object> parameters, RunContext context)
        {
            ushort limit = 100;
            if (parameters.TryGetValue(CommonNames.LIMIT, out object? value) && value is ushort)
            {
                limit = (ushort)value;
            }

            var channel = await Bot.DiscordClient.GetChannelAsync(
                (ulong)parameters[CommonNames.DISCORD_CHANNEL_ID]
            );

            if (channel is ITextChannel textChannel)
            {
                var messages = await textChannel.GetMessagesAsync(limit).FlattenAsync();
                try
                {
                    await textChannel.DeleteMessagesAsync(messages.Where(m => DateTime.Now.Subtract(m.CreatedAt.DateTime) < TimeSpan.FromDays(14)));
                }
                finally { }
            }
        }
    }
}