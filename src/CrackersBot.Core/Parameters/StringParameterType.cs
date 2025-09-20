namespace CrackersBot.Core.Parameters;

public class StringParameterType(
    bool isOptional = false,
    bool allowEmptyString = false
) : ParameterType<string>(isOptional)
{
    public bool AllowEmptyString => allowEmptyString;

    public override bool Validate(object value, bool forceRequired = false)
    {
        return base.Validate(value, forceRequired)
            && (AllowEmptyString || !String.IsNullOrEmpty(value.ToString()));
    }

    public override string GetValue(string value)
    {
        ValidateWithThrow(value);

        return value;
    }
}