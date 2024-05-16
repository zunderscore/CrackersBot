using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

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

        public static string GetValue(string token, RunContext context)
        {
            return context.Metadata.TryGetValue(token, out string? value)
                ? value ?? String.Empty
                : String.Empty;
        }

        public static string ProcessVariables(IBotCore bot, string? value, RunContext context)
        {
            if (value is null) return String.Empty;

            foreach (var token in GetVariableTokens(value))
            {
                bot.Logger.LogDebug("Found potential token: {token}", token);

                if (bot.RegisteredVariables.TryGetValue(token, out IVariable? variable))
                {
                    value = value.Replace($"{{{{{token}}}}}", variable.GetValue(bot, context));
                }
            }

            return value;
        }
    }
}