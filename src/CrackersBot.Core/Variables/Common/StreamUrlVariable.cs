namespace CrackersBot.Core.Variables.Common;

public class StreamUrlVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.STREAM_URL,
        "Stream URL",
        "The stream's URL",
        botServices
    );