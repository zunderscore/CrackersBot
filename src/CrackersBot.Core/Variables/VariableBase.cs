namespace CrackersBot.Core.Variables;

public abstract class VariableBase(
    string token,
    string name,
    string description,
    BotServiceProvider botServices
) : IVariable
{
    public BotServiceProvider BotServices { get; } = botServices;

    public string Token => Id;
    public string Id { get; } = token ?? String.Empty;
    public string Name { get; } = name ?? String.Empty;
    public string Description { get; } = description ?? String.Empty;

    public virtual string GetValue(RunContext context)
    {
        return BotServices.GetBotService<IVariableManager>().GetValue(Token, context);
    }
}