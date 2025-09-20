namespace CrackersBot.Core.Variables.Common;

public class PreviousMessageTextVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.PREVIOUS_MESSAGE_TEXT,
        "Previous Message Text",
        "The previous message text contents",
        botServices
    );