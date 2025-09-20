namespace CrackersBot.Core.Variables.Discord;

public class ChannelNameVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_CHANNEL_NAME,
        "Discord Channel Name",
        "The Discord channel name",
        botServices
    );