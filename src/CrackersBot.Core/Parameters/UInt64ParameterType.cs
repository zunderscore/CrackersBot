namespace CrackersBot.Core.Parameters;

public class UInt64ParameterType(
    bool isOptional = false
) : ParameterType<ulong>(isOptional)
{
    public override bool Validate(object value, bool forceRequired = false)
    {
        return base.Validate(value, forceRequired) && UInt64.TryParse(value.ToString(), out var _);
    }

    public override ulong GetValue(string value)
    {
        ValidateWithThrow(value);

        return UInt64.Parse(value);
    }
}