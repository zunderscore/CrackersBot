namespace CrackersBot.Core.Variables.Discord;

public class UserCustomStatusVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_USER_CUSTOM_STATUS,
        "Discord User Custom Status",
        "The Discord user's custom status, if set",
        botServices
    );