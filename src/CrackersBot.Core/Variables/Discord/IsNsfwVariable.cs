namespace CrackersBot.Core.Variables.Discord;

public class IsNsfwVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.IS_NSFW,
        "Is NSFW?",
        "Whether or not a channel is marked as NSFW",
        botServices
    );