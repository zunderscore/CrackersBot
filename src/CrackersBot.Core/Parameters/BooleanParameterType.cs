namespace CrackersBot.Core.Parameters;

public class BooleanParameterType(
    bool isOptional = false
) : ParameterType<bool>(isOptional)
{
    public override bool Validate(object value, bool forceRequired = false)
    {
        return base.Validate(value, forceRequired) && Boolean.TryParse(value.ToString(), out var _);
    }

    public override bool GetValue(string value)
    {
        ValidateWithThrow(value);

        return Boolean.Parse(value);
    }
}