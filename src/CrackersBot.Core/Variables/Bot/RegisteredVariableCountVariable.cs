namespace CrackersBot.Core.Variables.Bot
{
    [VariableToken(CommonNames.REGISTERED_VARIABLE_COUNT)]
    [VariableDescription("The total number of variables registered in CrackersBot")]
    public class RegisteredVariableCountVariable(IBotCore bot) : VariableBase(bot)
    {
        public override string GetValue(RunContext context)
            => Bot.RegisteredVariables.Count.ToString();
    }
}