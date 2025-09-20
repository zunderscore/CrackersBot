namespace CrackersBot.Core.Variables.Discord;

public class UserHasCustomStatusVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_USER_HAS_CUSTOM_STATUS,
        "Discord User Has Custom Status",
        "Whether the Discord user has custom status",
        botServices
    );