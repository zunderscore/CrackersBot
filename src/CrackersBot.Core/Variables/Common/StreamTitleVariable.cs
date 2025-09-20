namespace CrackersBot.Core.Variables.Common;

public class StreamTitleVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.STREAM_TITLE,
        "Stream Title",
        "The stream's title",
        botServices
    );