namespace CrackersBot.Core.Variables.Discord;

public class MessageIdVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_MESSAGE_ID,
        "Discord Message ID",
        "The Discord message ID",
        botServices
    );