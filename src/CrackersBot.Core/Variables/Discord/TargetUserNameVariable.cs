namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_TARGET_USER_NAME)]
    [VariableDescription("The target Discord user's name")]
    public class TargetUserNameVariable(IBotCore bot) : VariableBase(bot) { }
}