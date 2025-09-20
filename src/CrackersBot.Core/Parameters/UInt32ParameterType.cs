namespace CrackersBot.Core.Parameters;

public class UInt32ParameterType(
    bool isOptional = false
) : ParameterType<uint>(isOptional)
{
    public override bool Validate(object value, bool forceRequired = false)
    {
        return base.Validate(value, forceRequired) && UInt32.TryParse(value.ToString(), out var _);
    }

    public override uint GetValue(string value)
    {
        ValidateWithThrow(value);

        return UInt32.Parse(value);
    }
}