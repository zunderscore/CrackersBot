namespace CrackersBot.Core.Variables.Bot
{
    [VariableToken(CommonNames.REGISTERED_EVENT_HANDLER_COUNT)]
    [VariableDescription("The total number of event handlers registered in CrackersBot")]
    public class RegisteredEventHandlerCountVariable : VariableBase
    {
        public override string GetValue(IBotCore bot, Dictionary<string, object> context)
            => bot.RegisteredEventHandlers.Count.ToString();
    }
}