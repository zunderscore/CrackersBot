namespace CrackersBot.Core.Variables.Discord;

public class TargetUserGlobalDisplayNameVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_TARGET_USER_GLOBAL_DISPLAY_NAME,
        "Discord Target User Global Display Name",
        "The target Discord user's global display name",
        botServices
    );