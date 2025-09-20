namespace CrackersBot.Core.Variables.Discord;

public class GuildNameVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_GUILD_NAME,
        "Discord Guild Name",
        "The Discord guild name",
        botServices
    );