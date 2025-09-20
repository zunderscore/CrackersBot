namespace CrackersBot.Core.Variables.Discord;

public class VoiceChannelUserLimitVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_VOICE_CHANNEL_USER_LIMIT,
        "Discord Voice Channel User Limit",
        "The maximum number of users who can join a Discord voice channel, or nothing if there is no limit",
        botServices
    );