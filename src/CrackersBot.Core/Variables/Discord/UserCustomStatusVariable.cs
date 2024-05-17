namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_USER_CUSTOM_STATUS)]
    [VariableDescription("The Discord user's custom status, if set")]
    public class UserCustomStatusVariable(IBotCore bot) : VariableBase(bot) { }
}