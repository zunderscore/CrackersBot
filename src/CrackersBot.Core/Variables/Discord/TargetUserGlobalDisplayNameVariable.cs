namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_TARGET_USER_GLOBAL_DISPLAY_NAME)]
    [VariableDescription("The target Discord user's global display name")]
    public class TargetUserGlobalDisplayNameVariable(IBotCore bot) : VariableBase(bot) { }
}