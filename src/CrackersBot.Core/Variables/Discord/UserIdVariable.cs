namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_USER_ID)]
    [VariableDescription("The Discord user ID")]
    public class UserIdVariable(IBotCore bot) : VariableBase(bot) { }
}