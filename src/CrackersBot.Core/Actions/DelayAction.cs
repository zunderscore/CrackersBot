using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Actions
{
    [ActionId(ACTION_ID)]
    [ActionName("Delay")]
    [ActionDescription("Waits before running the next action")]
    public class DelayAction(IBotCore bot) : ActionBase(bot)
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
}