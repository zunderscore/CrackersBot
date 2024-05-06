
using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Filters.Discord
{
    [FilterId(FILTER_ID)]
    [FilterName("Discord Bot")]
    [FilterDescription("Whether or not an action was performed by a Discord bot")]
    public class BotFilter : FilterBase
    {
        public const string FILTER_ID = "CrackersBot.Discord.Bot";

        public const string PARAMTER_IS_BOT = "IsBot";

        public override Dictionary<string, IParameterType> FilterConditions => [];

        public override Dictionary<string, IParameterType> FilterParameters => new()
        {
            { PARAMTER_IS_BOT, new BooleanParameterType() }
        };

        public override bool Pass(
            Dictionary<string, object> parameters,
            Dictionary<string, string>? rawConditions = null,
            FilterInclusionType inclusionType = FilterInclusionType.Include
        )
        {
            if (!parameters.TryGetValue(PARAMTER_IS_BOT, out var isBotObj)
                || isBotObj is not bool isBot)
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