namespace CrackersBot.Core.Variables.Bot
{
    [VariableToken(CommonNames.REGISTERED_ACTION_COUNT)]
    [VariableDescription("The total number of actions registered in CrackersBot")]
    public class RegisteredActionCountVariable : VariableBase
    {
        public override string GetValue(IBotCore bot, RunContext context)
            => bot.RegisteredActions.Count.ToString();
    }
}