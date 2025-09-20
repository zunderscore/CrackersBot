namespace CrackersBot.Core.Variables.Common;

public class IsStreamingVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.IS_STREAMING,
        "Is Streaming",
        "Whether or not a user is streaming",
        botServices
    );