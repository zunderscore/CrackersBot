namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_GUILD_ID)]
    [VariableDescription("The Discord guild ID")]
    public class GuildIdVariable(IBotCore bot) : VariableBase(bot) { }
}