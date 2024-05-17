namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_CHANNEL_TOPIC)]
    [VariableDescription("The Discord channel topic")]
    public class ChannelTopicVariable(IBotCore bot) : VariableBase(bot) { }
}