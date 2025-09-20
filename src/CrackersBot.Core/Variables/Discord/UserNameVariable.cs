namespace CrackersBot.Core.Variables.Discord;

public class UserNameVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_USER_NAME,
        "Discord Username",
        "The Discord user's username",
        botServices
    );