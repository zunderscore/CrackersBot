namespace CrackersBot.Core.Variables.Common;

public class TimeInMillisecondsVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.TIME_IN_MILLISECONDS,
        "Time (ms)",
        "Time, in milliseconds",
        botServices
    );