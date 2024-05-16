using CrackersBot.Core.Filters;
using CrackersBot.Core.Parameters;
using CrackersBot.Core.Variables;
using Microsoft.Extensions.Logging;

namespace CrackersBot.Core.Actions
{
    public static class ActionRunner
    {
        public static async Task RunActions(
            IBotCore bot,
            IEnumerable<ActionInstance> actions,
            RunContext context
        )
        {
            foreach (var instance in actions)
            {
                try
                {
                    if (!instance.Enabled) return;

                    var processedParams = new Dictionary<string, string>();
                    foreach (var (paramName, paramValue) in instance.Parameters ?? [])
                    {
                        processedParams.Add(paramName, DefaultVariableProcessor.ProcessVariables(bot, paramValue, context));
                    }

                    if (FilterHelpers.CheckFilters(bot, instance.Filters ?? [], instance.FilterMode, context))
                    {
                        var action = bot.GetRegisteredAction(instance.ActionId);

                        if (action.DoPreRunCheck(bot, processedParams))
                        {
                            bot.Logger.LogDebug("Attempting to run action {instance.ActionId}", instance.ActionId);

                            var parsedParams = ParameterHelpers.GetParameterValues(action.ActionParameters, processedParams);
                            await action.Run(bot, parsedParams, context);
                        }
                    }
                }
                catch (Exception ex)
                {
                    bot.Logger.LogError(
                        ex,
                        "Error running action {instance.ActionId}",
                        instance.ActionId
                    );
                }
            }
        }
    }
}