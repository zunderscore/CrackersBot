namespace CrackersBot.Core.Variables.Bot
{
    [VariableToken(CommonNames.REGISTERED_FILTER_COUNT)]
    [VariableDescription("The total number of filters registered in CrackersBot")]
    public class RegisteredFilterCountVariable(IBotCore bot) : VariableBase(bot)
    {
        public override string GetValue(RunContext context)
            => Bot.RegisteredFilters.Count.ToString();
    }
}