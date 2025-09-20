using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace CrackersBot.Core.Variables;

public partial class VariableManager(
    BotServiceProvider botServiceProvider
) : IVariableManager
{
    private readonly ILogger<VariableManager> _logger = botServiceProvider.GetLogger<VariableManager>();

    public ConcurrentDictionary<string, IVariable> RegisteredVariables { get; } = new();

    public bool IsVariableRegistered(string token)
    {
        return RegisteredVariables.Keys.Any(k => k.Equals(token, StringComparison.InvariantCultureIgnoreCase));
    }

    public void RegisterVariable(IVariable variable)
    {
        if (IsVariableRegistered(variable.Token))
        {
            _logger.LogDebug("Unable to register Variable {token} as it has already been registered", variable.Token);
            return;
        }

        if (RegisteredVariables.TryAdd(variable.Token, variable))
        {
            _logger.LogDebug("Registered Variable {token}", variable.Token);
        }
        else
        {
            _logger.LogDebug("Unable to register Variable {token} (registration failed)", variable.Token);
        }
    }

    public void UnregisterVariable(string token)
    {
        if (!IsVariableRegistered(token))
        {
            _logger.LogDebug("Unable to unregister Variable {token} since it is not currently registered", token);
            return;
        }

        if (RegisteredVariables.TryRemove(token, out _))
        {
            _logger.LogDebug("Unregistered Variable {token}", token);
        }
        else
        {
            _logger.LogDebug("Unable to unregister Variable {token} (removal failed)", token);
        }
    }

    [GeneratedRegex(@"\{{(\w+)}}")]
    public partial Regex TokenRegex();

    public List<string> GetVariableTokens(string value)
    {
        var tokenRegex = TokenRegex();
        var tokens = tokenRegex.Matches(value).Cast<Match>()
            .SelectMany(m => m.Groups.Cast<Group>()
                .Skip(1) // Ignore the complete group with the wrapper
                .SelectMany(g => g.Captures.Cast<Capture>()
                    .Select(c => c.Value)));

        return tokens.ToList();
    }

    public string GetValue(string token, RunContext context)
    {
        return context.Metadata.TryGetValue(token, out string? value)
            ? value ?? String.Empty
            : String.Empty;
    }

    public string ProcessVariables(string? value, RunContext context)
    {
        if (value is null) return String.Empty;

        foreach (var token in GetVariableTokens(value))
        {
            _logger.LogDebug("Found potential token: {token}", token);

            if (RegisteredVariables.TryGetValue(token, out IVariable? variable))
            {
                value = value.Replace($"{{{{{token}}}}}", variable.GetValue(context));
            }
        }

        return value;
    }
}