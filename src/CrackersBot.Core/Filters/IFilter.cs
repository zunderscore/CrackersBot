using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Filters;

public interface IFilter : IRegisteredItem, IBotServiceConsumer
{
    Dictionary<string, IParameterType> FilterConditions { get; }
    Dictionary<string, IParameterType> FilterParameters { get; }

    public virtual bool Pass(RunContext context, FilterInstance instance)
        => Pass(context, instance.Conditions, instance.InclusionType);

    bool Pass(
        RunContext context,
        Dictionary<string, string>? rawConditions = null,
        FilterInclusionType inclusionType = FilterInclusionType.Include
    );
}