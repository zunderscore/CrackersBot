using CrackersBot.Core.Actions;

namespace CrackersBot.Core.Variables.Bot;

public class RegisteredActionCountVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.REGISTERED_ACTION_COUNT,
        "Registered Action Count",
        "The total number of actions registered in CrackersBot",
        botServices
    )
{
    public override string GetValue(RunContext context)
        => BotServices.GetBotService<IActionManager>().RegisteredActions.Count.ToString();
}