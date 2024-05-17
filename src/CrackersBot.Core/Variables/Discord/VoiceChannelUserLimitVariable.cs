namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.DISCORD_VOICE_CHANNEL_USER_LIMIT)]
    [VariableDescription("The maximum number of users who can join a Discord voice channel, or nothing if there is no limit")]
    public class VoiceChannelUserLimitVariable(IBotCore bot) : VariableBase(bot) { }
}