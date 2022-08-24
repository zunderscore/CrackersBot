using CrackersBot.Core;
using CrackersBot.Core.Filters;
using System.Text.RegularExpressions;

namespace CrackersBot.Web.Services.Automation.Filters
{
    public class DiscordMessageFilter : IFilter
    {
        public DiscordMessageFilter(string filterText, bool caseSensitive = false, bool isRegex = false, bool negateFilter = false)
        {
            FilterText = filterText;
            CaseSensitive = caseSensitive;
            IsRegex = isRegex;
            NegateFilter = negateFilter;
        }

        public string FilterText { get; }

        public bool CaseSensitive { get; }

        public bool IsRegex { get; }

        public bool NegateFilter { get; }

        public bool Pass(Dictionary<string, object> parameters)
        {
            bool isMatch;
            string message = parameters.ContainsKey(CommonNames.MESSAGE_TEXT) && parameters[CommonNames.MESSAGE_TEXT] is string ?
                (string)parameters[CommonNames.MESSAGE_TEXT] ?? String.Empty :
                String.Empty;

            if (String.IsNullOrEmpty(message)) return NegateFilter;

            if (IsRegex)
            {
                var regexOptions = RegexOptions.None;

                if (CaseSensitive) regexOptions |= RegexOptions.IgnoreCase;

                var regex = new Regex(FilterText, regexOptions);

                isMatch = regex.IsMatch(message);
            }
            else
            {
                var comparisonType = CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

                isMatch = message.Contains(FilterText, comparisonType);
            }

            return isMatch == !NegateFilter;
        }
    }
}