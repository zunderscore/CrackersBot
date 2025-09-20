using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Actions;

public interface IAction : IRegisteredItem, IBotServiceConsumer
{
    Dictionary<string, IParameterType> ActionParameters { get; }

    bool ValidateParameters(Dictionary<string, string> parameters);

    bool DoPreRunCheck(Dictionary<string, string> rawParams);

    Task Run(Dictionary<string, object> parameters, RunContext context);
}