using CrackersBot.Core.Parameters;

namespace CrackersBot.Core.Filters
{
    public interface IFilter
    {
        Dictionary<string, IParameterType> FilterConditions { get; }
        Dictionary<string, IParameterType> FilterParameters { get; }

        string GetFilterId();
        string GetFilterName();
        string GetFilterDescription();

        public virtual bool Pass(RunContext context, FilterDefinition filterDefinition)
            => Pass(context, filterDefinition.Conditions, filterDefinition.InclusionType);

        bool Pass(
            RunContext context,
            Dictionary<string, string>? rawConditions = null,
            FilterInclusionType inclusionType = FilterInclusionType.Include
        );
    }
}