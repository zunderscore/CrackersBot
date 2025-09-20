namespace CrackersBot.Core.Parameters;

public interface IParameterType
{
    Type Type { get; }

    bool IsOptional { get; }

    bool Validate(object value, bool forceRequired = false);

    bool TryParse(string input, out object? value);
}

public abstract class ParameterType<T>(bool isOptional = false) : IParameterType
{
    public virtual Type Type => typeof(T);

    public bool IsOptional { get; } = isOptional;

    public virtual bool Validate(object value, bool forceRequired = false) =>
        value is not null || (IsOptional && !forceRequired);

    public virtual bool TryParse(string input, out T? value)
    {
        try
        {
            value = GetValue(input);
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }

    bool IParameterType.TryParse(string input, out object? value)
    {
        if (TryParse(input, out var typedValue))
        {
            value = typedValue;
            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }

    public abstract T GetValue(string value);

    protected void ValidateWithThrow(string value)
    {
        if (!Validate(value)) throw new ArgumentException($"{nameof(value)} is not a valid {Type.Name}");
    }
}