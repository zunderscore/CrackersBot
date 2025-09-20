namespace CrackersBot.Core.Variables.Discord;

public class UserDisplayNameVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_USER_DISPLAY_NAME,
        "Discord User Display Name",
        "The Discord user's display name",
        botServices
    );