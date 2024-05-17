namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_TARGET_USER_DISPLAY_NAME)]
    [VariableDescription("The target Discord user's display name")]
    public class TargetUserDisplayNameVariable(IBotCore bot) : VariableBase(bot) { }
}