using System.Collections.Concurrent;

namespace CrackersBot.Core.Actions;

public interface IActionManager
{
    ConcurrentDictionary<string, IAction> RegisteredActions { get; }

    bool IsActionRegistered(string id);

    void RegisterAction(IAction action);

    void UnregisterAction(string id);

    IAction GetRegisteredAction(string id);

    Task RunActions(
        IEnumerable<ActionInstance> actions,
        RunContext context
    );
}