namespace CrackersBot.Core.Variables;

public interface IVariable : IRegisteredItem, IBotServiceConsumer
{
    string Token { get; }

    string GetValue(RunContext context);
}