
using System.Reflection;
using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Filters;

public abstract class FilterBase(
    string id,
    string name,
    string description,
    BotServiceProvider botServices
) : IFilter
{
    public BotServiceProvider BotServices { get; } = botServices;

    public abstract Dictionary<string, IParameterType> FilterConditions { get; }
    public abstract Dictionary<string, IParameterType> FilterParameters { get; }

    public string Id { get; } = id ?? String.Empty;
    public string Name { get; } = name ?? String.Empty;
    public string Description { get; } = description ?? String.Empty;

    public virtual bool ValidateParameters(Dictionary<string, string> rawParams)
        => ParameterHelpers.ValidateParameters(FilterParameters, rawParams);

    public abstract bool Pass(
        RunContext context,
        Dictionary<string, string>? rawConditions = null,
        FilterInclusionType inclusionType = FilterInclusionType.Include
    );
}