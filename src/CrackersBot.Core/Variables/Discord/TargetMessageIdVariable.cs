namespace CrackersBot.Core.Variables.Discord;

public class TargetMessageIdVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_TARGET_MESSAGE_ID,
        "Discord Target Message ID",
        "The target Discord message's ID",
        botServices
    );