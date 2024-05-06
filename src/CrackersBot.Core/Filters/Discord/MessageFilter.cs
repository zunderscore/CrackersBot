using System.Text.RegularExpressions;
using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Filters.Discord
{
    [FilterId(FILTER_ID)]
    [FilterName("Discord Message")]
    [FilterDescription("The contents of a Discord message")]
    public class MessageFilter : FilterBase
    {
        public const string FILTER_ID = "CrackersBot.Discord.Message";

        public const string CONDITION_FILTER_TEXT = "FilterText";
        public const string CONDITION_IS_CASE_SENSITIVE = "IsCaseSensitive";
        public const string CONDITION_IS_REGEX = "IsRegex";

        public override Dictionary<string, IParameterType> FilterConditions => new() {
            { CONDITION_FILTER_TEXT, new StringParameterType() },
            { CONDITION_IS_CASE_SENSITIVE, new BooleanParameterType(true) },
            { CONDITION_IS_REGEX, new BooleanParameterType(true) }
        };

        public override Dictionary<string, IParameterType> FilterParameters => new() {
            { CommonNames.MESSAGE_TEXT, new StringParameterType() }
        };

        public override bool Pass(
            Dictionary<string, object> parameters,
            Dictionary<string, string>? rawConditions = null,
            FilterInclusionType inclusionType = FilterInclusionType.Include
        )
        {
            if (!ParameterHelpers.ValidateParameters(FilterConditions, rawConditions ?? [])) return false;

            var parsedConditions = ParameterHelpers.GetParameterValues(FilterConditions, rawConditions ?? []);

            bool isRegex = parsedConditions.TryGetValue(CONDITION_IS_REGEX, out var isRegexObj)
                && isRegexObj is bool isRegexVal
                && isRegexVal;

            bool caseSensitive = parsedConditions.TryGetValue(CONDITION_IS_CASE_SENSITIVE, out var caseSensitiveObj)
                && caseSensitiveObj is bool caseSensitiveVal
                && caseSensitiveVal;

            bool isMatch;
            var message = parameters.TryGetValue(CommonNames.MESSAGE_TEXT, out object? messageTextObj) && messageTextObj is string messageText
                ? messageText ?? String.Empty
                : String.Empty;

            if (String.IsNullOrEmpty(message)) return inclusionType == FilterInclusionType.Exclude;

            if (isRegex)
            {
                var regexOptions = RegexOptions.None;

                if (caseSensitive) regexOptions |= RegexOptions.IgnoreCase;

                var regex = new Regex((string)parsedConditions[CONDITION_FILTER_TEXT], regexOptions);

                isMatch = regex.IsMatch(message);
            }
            else
            {
                var comparisonType = caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

                isMatch = message.Contains((string)parsedConditions[CONDITION_FILTER_TEXT], comparisonType);
            }

            return inclusionType switch
            {
                FilterInclusionType.Include => isMatch,
                FilterInclusionType.Exclude => !isMatch,
                _ => true,
            };
        }
    }
}