namespace CrackersBot.Core.Variables.Discord;

public class UserIdVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_USER_ID,
        "Discord User ID",
        "The Discord user ID",
        botServices
    );