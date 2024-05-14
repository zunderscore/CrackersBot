using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Actions
{
    [ActionId(ACTION_ID)]
    [ActionName("Delay")]
    [ActionDescription("Waits before running the next action")]
    public class DelayAction : ActionBase
    {
        public const string ACTION_ID = "CrackersBot.Delay";

        public override Dictionary<string, IParameterType> ActionParameters => new() {
            { CommonNames.TIME_IN_MILLISECONDS, new UInt32ParameterType() }
        };

        public override async Task Run(IBotCore bot, Dictionary<string, object> parameters, RunContext context)
        {
            var delay = (uint)parameters[CommonNames.TIME_IN_MILLISECONDS];

            await Task.Delay((int)delay);
        }
    }
}