
using CrackersBot.Core.Parameters;
using System.Reflection;

namespace CrackersBot.Core.Filters
{
    public abstract class FilterBase : IFilter
    {
        public abstract Dictionary<string, IParameterType> FilterConditions { get; }
        public abstract Dictionary<string, IParameterType> FilterParameters { get; }

        public string GetFilterId() => GetType().GetCustomAttribute<FilterIdAttribute>()?.Id ?? String.Empty;
        public string GetFilterName() => GetType().GetCustomAttribute<FilterNameAttribute>()?.Name ?? String.Empty;
        public string GetFilterDescription() => GetType().GetCustomAttribute<FilterDescriptionAttribute>()?.Description ?? String.Empty;

        public virtual bool ValidateParameters(Dictionary<string, string> rawParams)
            => ParameterHelpers.ValidateParameters(FilterParameters, rawParams);

        public abstract bool Pass(
            RunContext context,
            Dictionary<string, string>? rawConditions = null,
            FilterInclusionType inclusionType = FilterInclusionType.Include
        );
    }
}