namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_CHANNEL_ID)]
    [VariableDescription("The Discord channel ID")]
    public class ChannelIdVariable(IBotCore bot) : VariableBase(bot) { }
}