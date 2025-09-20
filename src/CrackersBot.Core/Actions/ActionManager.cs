using System.Collections.Concurrent;
using CrackersBot.Core.Filters;
using CrackersBot.Core.Parameters;
using CrackersBot.Core.Variables;
using Microsoft.Extensions.Logging;

namespace CrackersBot.Core.Actions;

public class ActionManager(
    BotServiceProvider botServices
) : IActionManager
{
    private readonly ILogger<ActionManager> _logger = botServices.GetLogger<ActionManager>();
    private readonly IFilterManager _filterManager = botServices.GetBotService<IFilterManager>();
    private readonly IVariableManager _variableManager = botServices.GetBotService<IVariableManager>();

    public ConcurrentDictionary<string, IAction> RegisteredActions { get; } = new();

    public bool IsActionRegistered(string id)
    {
        return RegisteredActions.Keys.Any(k => k.Equals(id, StringComparison.InvariantCultureIgnoreCase));
    }

    public void RegisterAction(IAction action)
    {
        if (IsActionRegistered(action.Id))
        {
            _logger.LogDebug("Unable to register Action {id} as it has already been registered", action.Id);
            return;
        }

        if (RegisteredActions.TryAdd(action.Id, action))
        {
            _logger.LogDebug("Registered Action {id}", action.Id);
        }
        else
        {
            _logger.LogDebug("Unable to register Action {id} (registration failed)", action.Id);
        }
    }

    public void UnregisterAction(string id)
    {
        if (!IsActionRegistered(id))
        {
            _logger.LogDebug("Unable to unregister Action {id} since it is not currently registered", id);
            return;
        }

        if (RegisteredActions.TryRemove(id, out _))
        {
            _logger.LogDebug("Unregistered Action {id}", id);
        }
        else
        {
            _logger.LogDebug("Unable to unregister Action {id} (removal failed)", id);
        }
    }

    public IAction GetRegisteredAction(string id)
    {
        if (!IsActionRegistered(id))
        {
            throw new ArgumentException($"Action {id} is not registered");
        }

        return RegisteredActions[id];
    }

    public async Task RunActions(
        IEnumerable<ActionInstance> actions,
        RunContext context
    )
    {
        foreach (var instance in actions)
        {
            try
            {
                if (!instance.Enabled) return;

                var action = GetRegisteredAction(instance.ActionId);
                var processedParams = new Dictionary<string, string>();

                foreach (var (paramName, paramValue) in instance.Parameters ?? [])
                {
                    processedParams.Add(paramName, _variableManager.ProcessVariables(paramValue, context));
                }

                if (_filterManager.CheckFilters(instance.Filters ?? [], instance.FilterMode, context))
                {
                    if (action.DoPreRunCheck(processedParams))
                    {
                        _logger.LogDebug("Attempting to run action {instance.ActionId}", instance.ActionId);

                        var parsedParams = ParameterHelpers.GetParameterValues(action.ActionParameters, processedParams);
                        await action.Run(parsedParams, context);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error running action {instance.ActionId}",
                    instance.ActionId
                );
            }
        }
    }
}