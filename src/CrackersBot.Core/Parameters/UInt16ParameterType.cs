namespace CrackersBot.Core.Parameters
{
    public class UInt16ParameterType(
        bool isOptional = false
    ) : ParameterType<ushort>(isOptional)
    {
        public override bool Validate(object value, bool forceRequired = false)
        {
            return base.Validate(value, forceRequired) && UInt16.TryParse(value.ToString(), out var _);
        }

        public override ushort GetValue(string value)
        {
            ValidateWithThrow(value);

            return UInt16.Parse(value);
        }
    }
}