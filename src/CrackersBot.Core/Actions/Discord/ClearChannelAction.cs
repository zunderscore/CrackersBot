using CrackersBot.Core.Parameters;
using Discord;
using Discord.WebSocket;

namespace CrackersBot.Core.Actions.Discord;

public class ClearChannelAction(BotServiceProvider botServices)
    : ActionBase(
        ACTION_ID,
        "Clear Discord Channel",
        "Clears messages from a Discord channel",
        botServices
    )
{
    public const string ACTION_ID = "CrackersBot.Discord.ClearChannel";

    public override Dictionary<string, IParameterType> ActionParameters => new() {
        { CommonNames.DISCORD_CHANNEL_ID, new UInt64ParameterType() },
        { CommonNames.LIMIT, new UInt16ParameterType() }
    };

    public override async Task Run(Dictionary<string, object> parameters, RunContext context)
    {
        var discordClient = BotServices.GetBotService<DiscordSocketClient>();
        ushort limit = 100;

        if (parameters.TryGetValue(CommonNames.LIMIT, out object? value) && value is ushort)
        {
            limit = (ushort)value;
        }

        var channel = await discordClient.GetChannelAsync(
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