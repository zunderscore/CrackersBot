namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_USER_HAS_CUSTOM_STATUS)]
    [VariableDescription("Whether the Discord user has custom status")]
    public class UserHasCustomStatusVariable(IBotCore bot) : VariableBase(bot) { }
}