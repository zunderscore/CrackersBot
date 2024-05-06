using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Filters.Discord
{
    [FilterId(FILTER_ID)]
    [FilterName("Discord Channel")]
    [FilterDescription("The Discord channel an action was performed in")]
    public class ChannelFilter : FilterBase
    {
        public const string FILTER_ID = "CrackersBot.Discord.Channel";

        public override Dictionary<string, IParameterType> FilterConditions => new() {
            { CommonNames.DISCORD_CHANNEL_ID, new UInt64ParameterType() }
        };

        public override Dictionary<string, IParameterType> FilterParameters => new() {
            { CommonNames.DISCORD_CHANNEL_ID, new UInt64ParameterType() }
        };

        public override bool Pass(
            Dictionary<string, object> parameters,
            Dictionary<string, string>? rawConditions = null,
            FilterInclusionType inclusionType = FilterInclusionType.Include)
        {
            if (!ParameterHelpers.ValidateParameters(FilterConditions, rawConditions ?? [])) return false;

            var parsedConditions = ParameterHelpers.GetParameterValues(FilterConditions, rawConditions ?? []);

            if (!parsedConditions.TryGetValue(CommonNames.DISCORD_CHANNEL_ID, out object? channelIdConditionObj)
                || channelIdConditionObj is not ulong channelIdCondition)
            {
                return false;
            }

            if (!parameters.TryGetValue(CommonNames.DISCORD_CHANNEL_ID, out object? channelIdParamObj)
                || channelIdParamObj is not ulong channelIdParam)
            {
                return false;
            }

            return inclusionType switch
            {
                FilterInclusionType.Include => channelIdCondition == channelIdParam,
                FilterInclusionType.Exclude => channelIdCondition != channelIdParam,
                _ => true,
            };
        }
    }
}