using CrackersBot.Core.Parameters;
using Discord;
using Discord.WebSocket;

namespace CrackersBot.Core.Actions.Discord;

public class ReactToMessageAction(BotServiceProvider botServices)
    : ActionBase(
        ACTION_ID,
        "React to Discord Message",
        "Adds a reaction to the specified Discord message",
        botServices
    )
{
    public const string ACTION_ID = "CrackersBot.Discord.ReactToMessage";

    public override Dictionary<string, IParameterType> ActionParameters => new() {
        { CommonNames.DISCORD_GUILD_ID, new UInt64ParameterType() },
        { CommonNames.DISCORD_CHANNEL_ID, new UInt64ParameterType() },
        { CommonNames.DISCORD_MESSAGE_ID, new UInt64ParameterType() },
        { CommonNames.DISCORD_EMOTE_NAME, new StringParameterType() }
    };

    public override async Task Run(Dictionary<string, object> parameters, RunContext context)
    {
        var discordClient = BotServices.GetBotService<DiscordSocketClient>();

        if (!parameters.TryGetValue(CommonNames.DISCORD_CHANNEL_ID, out object? channelId) || !parameters.TryGetValue(CommonNames.DISCORD_MESSAGE_ID, out object? messageId)) return;

        var guild = await discordClient.Rest.GetGuildAsync((ulong)parameters[CommonNames.DISCORD_GUILD_ID]);
        var emote = (await guild.GetEmotesAsync())
            .FirstOrDefault(e => e.Name.Equals((string)parameters[CommonNames.DISCORD_EMOTE_NAME], StringComparison.CurrentCultureIgnoreCase));

        if (emote is null) return;

        var channel = (ITextChannel)await guild.GetChannelAsync((ulong)channelId);
        var message = await channel.GetMessageAsync((ulong)messageId);

        await message.AddReactionAsync(emote);
    }
}