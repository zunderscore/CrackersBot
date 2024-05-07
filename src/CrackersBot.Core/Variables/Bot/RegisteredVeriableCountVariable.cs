namespace CrackersBot.Core.Variables.Bot
{
    [VariableToken(CommonNames.REGISTERED_VARIABLE_COUNT)]
    [VariableDescription("The total number of variables registered in CrackersBot")]
    public class RegisteredVeriableCountVariable : VariableBase
    {
        public override string GetValue(IBotCore bot, RunContext context)
            => bot.RegisteredVariables.Count.ToString();
    }
}