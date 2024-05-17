namespace CrackersBot.Core.Variables.Bot
{
    [VariableToken(CommonNames.REGISTERED_ACTION_COUNT)]
    [VariableDescription("The total number of actions registered in CrackersBot")]
    public class RegisteredActionCountVariable(IBotCore bot) : VariableBase(bot)
    {
        public override string GetValue(RunContext context)
            => Bot.RegisteredActions.Count.ToString();
    }
}