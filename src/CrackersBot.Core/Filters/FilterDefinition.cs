namespace CrackersBot.Core.Filters
{
    public record FilterDefinition(
        string FilterId,
        FilterInclusionType InclusionType = FilterInclusionType.Include,
        Dictionary<string, string>? Conditions = null
    );
}