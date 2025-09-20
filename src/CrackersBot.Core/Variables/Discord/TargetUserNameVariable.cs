namespace CrackersBot.Core.Variables.Discord;

public class TargetUserNameVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_TARGET_USER_NAME,
        "Discord Target User Name",
        "The target Discord user's name",
        botServices
    );