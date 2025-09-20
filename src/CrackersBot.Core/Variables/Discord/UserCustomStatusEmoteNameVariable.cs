namespace CrackersBot.Core.Variables.Discord;

public class UserCustomStatusEmoteNameVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_USER_CUSTOM_STATUS_EMOTE_NAME,
        "Discord User Custom Status Emote Name",
        "The Discord user's custom status emote name, if set",
        botServices
    );