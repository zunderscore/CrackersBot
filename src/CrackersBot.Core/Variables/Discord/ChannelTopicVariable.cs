namespace CrackersBot.Core.Variables.Discord;

public class ChannelTopicVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_CHANNEL_TOPIC,
        "Discotd Channel Topic",
        "The Discord channel topic",
        botServices
    );