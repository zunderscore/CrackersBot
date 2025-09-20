using CrackersBot.Core.Filters;

namespace CrackersBot.Core.Variables.Bot;

public class RegisteredFilterCountVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.REGISTERED_FILTER_COUNT,
        "Registered Filter Count",
        "The total number of filters registered in CrackersBot",
        botServices
    )
{
    public override string GetValue(RunContext context)
        => BotServices.GetBotService<IFilterManager>().RegisteredFilters.Count.ToString();
}