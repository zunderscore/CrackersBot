using CrackersBot.Core.Parameters;
using Discord;

namespace CrackersBot.Core.Actions.Discord
{
    [ActionId(ACTION_ID)]
    [ActionName("Send Discord Direct Message")]
    [ActionDescription("Sends a direct message to the specified Discord user")]
    public class SendDirectMessageAction(IBotCore bot) : ActionBase(bot)
    {
        public const string ACTION_ID = "CrackersBot.Discord.SendDirectMessage";

        public override Dictionary<string, IParameterType> ActionParameters => new() {
            { CommonNames.DISCORD_USER_ID, new UInt64ParameterType() },
            { CommonNames.MESSAGE_TEXT, new StringParameterType() }
        };

        public override async Task Run(Dictionary<string, object> parameters, RunContext context)
        {
            var userId = (ulong)parameters[CommonNames.DISCORD_USER_ID];
            var user = await Bot.DiscordClient.GetUserAsync(userId);

            await user.CreateDMChannelAsync();
            await user.SendMessageAsync((string)parameters[CommonNames.MESSAGE_TEXT]);
        }
    }
}