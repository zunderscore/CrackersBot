using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Actions.Common;

public class DelayAction(BotServiceProvider botServices)
    : ActionBase(
        ACTION_ID,
        "Delay",
        "Waits before running the next action",
        botServices
    )
{
    public const string ACTION_ID = "CrackersBot.Delay";

    public override Dictionary<string, IParameterType> ActionParameters => new() {
        { CommonNames.TIME_IN_MILLISECONDS, new UInt32ParameterType() }
    };

    public override async Task Run(Dictionary<string, object> parameters, RunContext context)
    {
        var delay = (uint)parameters[CommonNames.TIME_IN_MILLISECONDS];

        await Task.Delay((int)delay);
    }
}