namespace CrackersBot.Core.Variables.Shared
{
    [VariableToken(CommonNames.GAME_NAME)]
    [VariableDescription("The name of the game")]
    public class GameNameVariable(IBotCore bot) : VariableBase(bot) { }
}