using CrackersBot.Core.Events;

namespace CrackersBot.Core.Variables.Bot;

public class RegisteredEventCountVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.REGISTERED_EVENT_COUNT,
        "Registered Event Count",
        "The total number of events registered in CrackersBot",
        botServices
    )
{
    public override string GetValue(RunContext context)
        => BotServices.GetBotService<IEventManager>().RegisteredEvents.Count.ToString();
}