namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_MESSAGE_ID)]
    [VariableDescription("The Discord message ID")]
    public class MessageIdVariable(IBotCore bot) : VariableBase(bot) { }
}