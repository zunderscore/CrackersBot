namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_TARGET_MESSAGE_ID)]
    [VariableDescription("The target Discord message's ID")]
    public class TargetMessageIdVariable(IBotCore bot) : VariableBase(bot) { }
}