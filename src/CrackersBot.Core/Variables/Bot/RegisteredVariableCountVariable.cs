namespace CrackersBot.Core.Variables.Bot;

public class RegisteredVariableCountVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.REGISTERED_VARIABLE_COUNT,
        "Registered Variable Count",
        "The total number of variables registered in CrackersBot",
        botServices
    )
{
    public override string GetValue(RunContext context)
        => BotServices.GetBotService<IVariableManager>().RegisteredVariables.Count.ToString();
}