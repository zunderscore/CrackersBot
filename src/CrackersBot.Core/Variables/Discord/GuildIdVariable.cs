namespace CrackersBot.Core.Variables.Discord;

public class GuildIdVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.DISCORD_GUILD_ID,
        "Discord Guild ID",
        "The Discord guild ID",
        botServices
    );