using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace CrackersBot.Core.Variables;

public interface IVariableManager
{
    ConcurrentDictionary<string, IVariable> RegisteredVariables { get; }

    bool IsVariableRegistered(string token);

    void RegisterVariable(IVariable variable);

    void UnregisterVariable(string token);

    Regex TokenRegex();

    List<string> GetVariableTokens(string value);

    string GetValue(string token, RunContext context);

    string ProcessVariables(string? value, RunContext context);
}