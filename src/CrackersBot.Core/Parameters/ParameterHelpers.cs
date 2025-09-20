namespace CrackersBot.Core.Parameters;

public static class ParameterHelpers
{
    public static bool ValidateParameters(
        Dictionary<string, IParameterType> paramTypes,
        Dictionary<string, string> rawParams
    )
    {
        foreach (var (paramName, param) in paramTypes)
        {
            if (!rawParams.ContainsKey(paramName))
            {
                if (!param.IsOptional)
                {
                    return false;
                }
            }
            else
            {
                if (!param.Validate(rawParams[paramName]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static Dictionary<string, object> GetParameterValues(
        Dictionary<string, IParameterType> paramTypes,
        Dictionary<string, string> rawParams
    )
    {
        Dictionary<string, object> parsedParams = [];

        foreach (var (paramName, stringVal) in rawParams)
        {
            if (!paramTypes.TryGetValue(paramName, out IParameterType? value))
            {
                parsedParams.Add(paramName, stringVal);
            }
            else
            {
                parsedParams.Add(paramName, value switch
                {
                    BooleanParameterType bpt => bpt.GetValue(stringVal),
                    UInt16ParameterType u16pt => u16pt.GetValue(stringVal),
                    UInt32ParameterType u32pt => u32pt.GetValue(stringVal),
                    UInt64ParameterType u64pt => u64pt.GetValue(stringVal),
                    StringParameterType spt => spt.GetValue(stringVal),
                    ObjectParameterType opt => Convert.ChangeType(opt.GetValue(stringVal), opt.Type),
                    _ => stringVal
                });
            }
        }

        return parsedParams;
    }
}