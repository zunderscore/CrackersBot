namespace CrackersBot.Core.Filters;

public record FilterInstance(
    string FilterId,
    FilterInclusionType InclusionType = FilterInclusionType.Include,
    Dictionary<string, string>? Conditions = null
);