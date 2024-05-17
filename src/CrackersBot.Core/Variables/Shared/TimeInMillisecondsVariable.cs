namespace CrackersBot.Core.Variables.Shared
{
    [VariableToken(CommonNames.TIME_IN_MILLISECONDS)]
    [VariableDescription("Time, in milliseconds")]
    public class TimeInMillisecondsVariable(IBotCore bot) : VariableBase(bot) { }
}