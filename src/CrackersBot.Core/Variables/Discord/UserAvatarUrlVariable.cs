namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_USER_AVATAR_URL)]
    [VariableDescription("The Discord user avatar URL")]
    public class UserAvatarUrlVariable(IBotCore bot) : VariableBase(bot) { }
}