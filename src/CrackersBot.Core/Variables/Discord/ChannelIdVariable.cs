namespace CrackersBot.Core.Variables.Discord;

public class ChannelIdVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_CHANNEL_ID,
        "Discord Channel ID",
        "The Discord channel ID",
        botServices
    );