using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Filters.Discord
{
    [FilterId(FILTER_ID)]
    [FilterName("Discord Author")]
    [FilterDescription("The author of a Discord post")]
    public class AuthorFilter : FilterBase
    {
        public const string FILTER_ID = "CrackersBot.Discord.Author";

        public const string CONDITION_USER = "User";

        public override Dictionary<string, IParameterType> FilterConditions => new() {
            { CONDITION_USER, new UInt64ParameterType() }
        };

        public override Dictionary<string, IParameterType> FilterParameters => new() {
            { CommonNames.DISCORD_AUTHOR_ID, new UInt64ParameterType() }
        };

        public override bool Pass(
            Dictionary<string, object> parameters,
            Dictionary<string, string>? rawConditions = null,
            FilterInclusionType inclusionType = FilterInclusionType.Include
        )
        {
            if (!ParameterHelpers.ValidateParameters(FilterConditions, rawConditions ?? [])) return false;

            var parsedConditions = ParameterHelpers.GetParameterValues(FilterConditions, rawConditions ?? []);

            if (!parsedConditions.TryGetValue(CONDITION_USER, out object? userIdObj)
                || userIdObj is not ulong userId)
            {
                return false;
            }

            if (!parameters.TryGetValue(CommonNames.DISCORD_AUTHOR_ID, out object? authorIdObj)
                || authorIdObj is not ulong authorId)
            {
                return false;
            }

            return inclusionType switch
            {
                FilterInclusionType.Include => authorId == userId,
                FilterInclusionType.Exclude => authorId != userId,
                _ => true,
            };
        }
    }
}