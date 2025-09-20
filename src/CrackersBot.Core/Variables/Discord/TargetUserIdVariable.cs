namespace CrackersBot.Core.Variables.Discord;

public class TargetUserIdVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_TARGET_USER_ID,
        "Discord Target User ID",
        "The target Discord user's ID",
        botServices
    );