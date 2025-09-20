namespace CrackersBot.Core.Variables.Common;

public class IsBotVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.IS_BOT,
        "Is Bot",
        "Whether or not a user is a bot",
        botServices
    );