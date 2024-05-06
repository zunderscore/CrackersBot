using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Actions
{
    public interface IAction
    {
        Dictionary<string, IParameterType> ActionParameters { get; }

        string GetActionId();
        string GetActionName();
        string GetActionDescription();

        bool ValidateParameters(Dictionary<string, string> parameters);

        bool DoPreRunCheck(IBotCore bot, Dictionary<string, string> rawParams);

        Task Run(IBotCore bot, Dictionary<string, object> parameters);
    }
}