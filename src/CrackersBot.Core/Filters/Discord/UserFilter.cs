using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Filters.Discord
{
    [FilterId(FILTER_ID)]
    [FilterName("Discord User ID")]
    [FilterDescription("The ID of the Discord user")]
    public class UserFilter : FilterBase
    {
        public const string FILTER_ID = "CrackersBot.Discord.User";

        public const string CONDITION_USER_ID = "UserId";

        public override Dictionary<string, IParameterType> FilterConditions => new() {
            { CONDITION_USER_ID, new UInt64ParameterType() }
        };

        public override Dictionary<string, IParameterType> FilterParameters => new() {
            { CommonNames.DISCORD_USER_ID, new UInt64ParameterType() }
        };

        public override bool Pass(
            RunContext context,
            Dictionary<string, string>? rawConditions = null,
            FilterInclusionType inclusionType = FilterInclusionType.Include
        )
        {
            if (!ParameterHelpers.ValidateParameters(FilterConditions, rawConditions ?? [])) return false;

            var parsedConditions = ParameterHelpers.GetParameterValues(FilterConditions, rawConditions ?? []);

            if (!parsedConditions.TryGetValue(CONDITION_USER_ID, out object? userIdObj)
                || userIdObj is not ulong userId)
            {
                return false;
            }

            if (!context.Metadata.TryGetValue(CommonNames.DISCORD_USER_ID, out string? authorIdString)
                || !UInt64.TryParse(authorIdString, out var authorId))
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