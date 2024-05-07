using CrackersBot.Core.Parameters;
using System.Text.RegularExpressions;

namespace CrackersBot.Core.Filters
{
    [FilterId(FILTER_ID)]
    [FilterName("Message Text")]
    [FilterDescription("The contents of a message")]
    public class MessageTextFilter : FilterBase
    {
        public const string FILTER_ID = "CrackersBot.MessageText";

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
            RunContext context,
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
            var message = context.Metadata.TryGetValue(CommonNames.MESSAGE_TEXT, out string? messageText)
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