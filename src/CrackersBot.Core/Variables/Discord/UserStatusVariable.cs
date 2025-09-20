namespace CrackersBot.Core.Variables.Discord;

public class UserStatusVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_USER_STATUS,
        "Discord User Status",
        "The Discord user's status",
        botServices
    );