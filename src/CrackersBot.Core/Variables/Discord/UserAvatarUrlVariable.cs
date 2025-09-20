namespace CrackersBot.Core.Variables.Discord;

public class UserAvatarUrlVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_USER_AVATAR_URL,
        "Discord User Avatar URL",
        "The Discord user avatar URL",
        botServices
    );