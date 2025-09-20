namespace CrackersBot.Core.Variables.Discord;

public class TargetUserDisplayNameVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_TARGET_USER_DISPLAY_NAME,
        "Discord Target User Display Name",
        "The target Discord user's display name",
        botServices
    );