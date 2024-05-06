using Newtonsoft.Json;

namespace CrackersBot.Core.Parameters
{
    public class ObjectParameterType(
        Type type,
        bool isOptional = false
    ) : ParameterType<object>(isOptional)
    {
        public override Type Type { get; } = type;

        public override object GetValue(string value)
        {
            ValidateWithThrow(value);

            try
            {
                return JsonConvert.DeserializeObject(value, Type)!;
            }
            catch
            {
                throw new ArgumentException($"{nameof(value)} is not {Type.Name}");
            }
        }
    }
}