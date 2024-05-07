using CrackersBot.Core.Parameters;
using CrackersBot.Core.Variables;

namespace CrackersBot.Core.Actions
{
    public static class ActionRunner
    {
        public static async Task RunActions(
            IBotCore bot,
            List<KeyValuePair<string, Dictionary<string, string>>> actions,
            RunContext context
        )
        {
            foreach (var (actionType, parameters) in actions)
            {
                try
                {
                    var processedParams = new Dictionary<string, string>();
                    foreach (var (paramName, paramValue) in parameters)
                    {
                        processedParams.Add(paramName, DefaultVariableProcessor.ProcessVariables(bot, paramValue, context));
                    }

                    var action = bot.GetRegisteredAction(actionType);
                    if (action.DoPreRunCheck(bot, processedParams))
                    {
                        var parsedParams = ParameterHelpers.GetParameterValues(action.ActionParameters, processedParams);
                        await action.Run(bot, parsedParams, context);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}