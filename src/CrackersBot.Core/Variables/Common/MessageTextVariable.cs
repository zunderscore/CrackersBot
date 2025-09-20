namespace CrackersBot.Core.Variables.Common;

public class MessageTextVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.MESSAGE_TEXT,
        "Message Text",
        "The message text contents",
        botServices
    );