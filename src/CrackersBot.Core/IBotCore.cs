using CrackersBot.Core.Actions;
using CrackersBot.Core.Variables;
using Discord;

namespace CrackersBot.Core
{
    public interface IBotCore
    {
        IDiscordClient DiscordClient { get; }

        // Actions

        bool IsActionRegistered(string id);

        void RegisterAction(IAction action);

        void UnregisterAction(string id);

        IAction GetRegisteredAction(string actionId);

        // Variables

        bool IsVariableRegistered(string token);

        void RegisterVariable(IVariable variable);

        void UnregisterVariable(string token);

        string ProcessVariables(string value, Dictionary<string, object> context);
    }
}