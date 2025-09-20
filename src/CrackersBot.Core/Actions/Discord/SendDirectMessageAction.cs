using CrackersBot.Core.Parameters;
using Discord;
using Discord.WebSocket;

namespace CrackersBot.Core.Actions.Discord;

public class SendDirectMessageAction(BotServiceProvider botServices)
    : ActionBase(
        ACTION_ID,
        "Send Discord Direct Message",
        "Sends a direct message to the specified Discord user",
        botServices
    )
{
    public const string ACTION_ID = "CrackersBot.Discord.SendDirectMessage";

    public override Dictionary<string, IParameterType> ActionParameters => new() {
        { CommonNames.DISCORD_USER_ID, new UInt64ParameterType() },
        { CommonNames.MESSAGE_TEXT, new StringParameterType() }
    };

    public override async Task Run(Dictionary<string, object> parameters, RunContext context)
    {
        var discordClient = BotServices.GetBotService<DiscordSocketClient>();
        var userId = (ulong)parameters[CommonNames.DISCORD_USER_ID];
        var user = await discordClient.GetUserAsync(userId);

        await user.CreateDMChannelAsync();
        await user.SendMessageAsync((string)parameters[CommonNames.MESSAGE_TEXT]);
    }
}