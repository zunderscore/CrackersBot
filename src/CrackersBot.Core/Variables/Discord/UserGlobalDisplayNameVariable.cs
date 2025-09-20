namespace CrackersBot.Core.Variables.Discord;

public class UserGlobalDisplayNameVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_USER_GLOBAL_DISPLAY_NAME,
        "Discord User Global Display Name",
        "The Discord user's global display name",
        botServices
    );