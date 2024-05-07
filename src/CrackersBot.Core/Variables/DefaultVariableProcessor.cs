using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CrackersBot.Core.Variables
{
    public static partial class DefaultVariableProcessor
    {
        [GeneratedRegex(@"\{{(\w+)}}")]
        public static partial Regex TokenRegex();

        public static List<string> GetVariableTokens(string value)
        {
            var tokenRegex = TokenRegex();
            var tokens = tokenRegex.Matches(value).Cast<Match>()
                .SelectMany(m => m.Groups.Cast<Group>()
                    .Skip(1) // Ignore the complete group with the wrapper
                    .SelectMany(g => g.Captures.Cast<Capture>()
                        .Select(c => c.Value)));

            return tokens.ToList();
        }

        public static string GetValue(string token, Dictionary<string, object> context)
        {
            return context.TryGetValue(token, out object? valueObj) && valueObj is string value
                ? value ?? String.Empty
                : valueObj is null
                    ? String.Empty
                    : valueObj.ToString() ?? String.Empty;
        }

        public static string ProcessVariables(IBotCore bot, string? value, Dictionary<string, object> context)
        {
            if (value is null) return String.Empty;

            foreach (var token in GetVariableTokens(value))
            {
                Debug.WriteLine($"Token: {token}");

                if (bot.RegisteredVariables.TryGetValue(token, out IVariable? variable))
                {
                    value = value.Replace($"{{{{{token}}}}}", variable.GetValue(bot, context));
                }
            }

            return value;
        }
    }
}