using System.Reflection;
using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Actions
{
    public abstract class ActionBase(IBotCore bot) : IAction
    {
        public IBotCore Bot { get; } = bot;

        public abstract Dictionary<string, IParameterType> ActionParameters { get; }

        public string GetId() => GetType().GetCustomAttribute<ActionIdAttribute>()?.Id ?? String.Empty;
        public string GetName() => GetType().GetCustomAttribute<ActionNameAttribute>()?.Name ?? String.Empty;
        public string GetDescription() => GetType().GetCustomAttribute<ActionDescriptionAttribute>()?.Description ?? String.Empty;

        public bool DoPreRunCheck(Dictionary<string, string> rawParams)
            => Bot?.DiscordClient is not null && ValidateParameters(rawParams);

        public abstract Task Run(Dictionary<string, object> parameters, RunContext context);

        public virtual bool ValidateParameters(Dictionary<string, string> rawParams)
            => ParameterHelpers.ValidateParameters(ActionParameters, rawParams);
    }
}