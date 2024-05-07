namespace CrackersBot.Core.Variables
{
    public interface IVariable
    {
        string Token { get; }

        string GetValue(IBotCore bot, RunContext context);
    }
}