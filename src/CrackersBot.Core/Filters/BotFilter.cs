
using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Filters
{
    [FilterId(FILTER_ID)]
    [FilterName("Bot User")]
    [FilterDescription("Whether or not an action was performed by a bot user")]
    public class BotFilter : FilterBase
    {
        public const string FILTER_ID = "CrackersBot.Bot";

        public override Dictionary<string, IParameterType> FilterConditions => [];

        public override Dictionary<string, IParameterType> FilterParameters => new()
        {
            { CommonNames.IS_BOT, new BooleanParameterType() }
        };

        public override bool Pass(
            RunContext context,
            Dictionary<string, string>? rawConditions = null,
            FilterInclusionType inclusionType = FilterInclusionType.Include
        )
        {
            if (!context.Metadata.TryGetValue(CommonNames.IS_BOT, out var isBotString)
                || !Boolean.TryParse(isBotString, out bool isBot))
            {
                return false;
            }

            return inclusionType switch
            {
                FilterInclusionType.Include => isBot,
                FilterInclusionType.Exclude => !isBot,
                _ => true,
            };
        }
    }
}