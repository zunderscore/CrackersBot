namespace CrackersBot.Core.Variables.Common;

public class GameNameVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.GAME_NAME,
        "Game Name",
        "The name of the game",
        botServices
    );