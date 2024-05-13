using Newtonsoft.Json;

namespace CrackersBot
{
    public enum DatePart
    {
        Day,
        Hour,
        Minute,
        Second
    }

    public static class ExtensionMethods
    {
        public static string ToJsonString(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static string ToHumanReadableString(this TimeSpan timeSpan, DatePart minUnit = DatePart.Second)
        {
            var components = new List<string>();

            switch (minUnit)
            {
                case DatePart.Second:
                    if (timeSpan.Seconds > 0)
                    {
                        components.Add($"{timeSpan.Seconds} second{(timeSpan.Seconds > 1 ? "s" : "")}");
                    }
                    goto case DatePart.Minute;

                case DatePart.Minute:
                    if (timeSpan.Minutes > 0)
                    {
                        components.Add($"{timeSpan.Minutes} minutes{(timeSpan.Minutes > 1 ? "s" : "")}");
                    }
                    goto case DatePart.Hour;

                case DatePart.Hour:
                    if (timeSpan.Hours > 0)
                    {
                        components.Add($"{timeSpan.Hours} hours{(timeSpan.Hours > 1 ? "s" : "")}");
                    }
                    goto case DatePart.Day;

                case DatePart.Day:
                default:
                    if (timeSpan.Days > 0)
                    {
                        components.Add($"{timeSpan.Days} days{(timeSpan.Days > 1 ? "s" : "")}");
                    }
                    break;
            }

            components.Reverse();
            return $"{String.Join(", ", components[..^1])}, and {components[^1]}";
        }
    }
}