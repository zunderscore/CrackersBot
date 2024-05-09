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

        public virtual bool Pass(RunContext context, FilterInstance instance)
            => Pass(context, instance.Conditions, instance.InclusionType);

        bool Pass(
            RunContext context,
            Dictionary<string, string>? rawConditions = null,
            FilterInclusionType inclusionType = FilterInclusionType.Include
        );
    }
}