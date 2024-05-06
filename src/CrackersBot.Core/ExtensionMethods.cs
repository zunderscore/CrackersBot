using Newtonsoft.Json;

namespace CrackersBot
{
    public static class ExtensionMethods
    {
        public static string ToJsonString(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}