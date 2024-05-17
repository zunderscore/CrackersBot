namespace CrackersBot.Core.Variables.Bot
{
    [VariableToken(CommonNames.REGISTERED_EVENT_HANDLER_COUNT)]
    [VariableDescription("The total number of event handlers registered in CrackersBot")]
    public class RegisteredEventHandlerCountVariable(IBotCore bot) : VariableBase(bot)
    {
        public override string GetValue(RunContext context)
            => Bot.RegisteredEventHandlers.Count.ToString();
    }
}