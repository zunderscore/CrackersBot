namespace CrackersBot.Core.Variables.Bot
{
    [VariableToken(CommonNames.REGISTERED_FILTER_COUNT)]
    [VariableDescription("The total number of filters registered in CrackersBot")]
    public class RegisteredFilterCountVariable : VariableBase
    {
        public override string GetValue(IBotCore bot, RunContext context)
            => bot.RegisteredFilters.Count.ToString();
    }
}