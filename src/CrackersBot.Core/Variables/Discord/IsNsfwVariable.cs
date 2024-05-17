namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.IS_NSFW)]
    [VariableDescription("Whether or not a channel is marked as NSFW")]
    public class IsNsfwVariable(IBotCore bot) : VariableBase(bot) { }
}