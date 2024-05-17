namespace CrackersBot.Core.Variables
{
    public interface IVariable : IBotConsumer
    {
        string Token { get; }

        string GetValue(RunContext context);
    }
}