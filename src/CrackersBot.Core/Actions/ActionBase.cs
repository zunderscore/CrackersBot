using System.Reflection;
using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Actions
{
    public abstract class ActionBase : IAction
    {
        public string GetActionId() => GetType().GetCustomAttribute<ActionIdAttribute>()?.Id ?? String.Empty;
        public string GetActionName() => GetType().GetCustomAttribute<ActionNameAttribute>()?.Name ?? String.Empty;
        public string GetActionDescription() => GetType().GetCustomAttribute<ActionDescriptionAttribute>()?.Description ?? String.Empty;

        public abstract Dictionary<string, IParameterType> ActionParameters { get; }

        public bool DoPreRunCheck(IBotCore bot, Dictionary<string, string> rawParams)
        {
            return bot?.DiscordClient is not null && ValidateParameters(rawParams);
        }

        public abstract Task Run(IBotCore bot, Dictionary<string, object> parameters, Dictionary<string, object> context);

        public virtual bool ValidateParameters(Dictionary<string, string> rawParams)
            => ParameterHelpers.ValidateParameters(ActionParameters, rawParams);
    }
}