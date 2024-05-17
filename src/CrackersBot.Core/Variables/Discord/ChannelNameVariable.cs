namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_CHANNEL_NAME)]
    [VariableDescription("The Discord channel name")]
    public class ChannelNameVariable(IBotCore bot) : VariableBase(bot) { }
}