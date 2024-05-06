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

        public virtual bool Pass(Dictionary<string, object> parameters, FilterDefinition filterDefinition)
            => Pass(parameters, filterDefinition.Conditions, filterDefinition.InclusionType);

        bool Pass(
            Dictionary<string, object> parameters,
            Dictionary<string, string>? rawConditions = null,
            FilterInclusionType inclusionType = FilterInclusionType.Include
        );
    }
}