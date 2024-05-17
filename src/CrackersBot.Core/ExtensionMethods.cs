using Discord;
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
                        components.Add($"{timeSpan.Minutes} minute{(timeSpan.Minutes > 1 ? "s" : "")}");
                    }
                    goto case DatePart.Hour;

                case DatePart.Hour:
                    if (timeSpan.Hours > 0)
                    {
                        components.Add($"{timeSpan.Hours} hour{(timeSpan.Hours > 1 ? "s" : "")}");
                    }
                    goto case DatePart.Day;

                case DatePart.Day:
                default:
                    if (timeSpan.Days > 0)
                    {
                        components.Add($"{timeSpan.Days} day{(timeSpan.Days > 1 ? "s" : "")}");
                    }
                    break;
            }

            components.Reverse();
            return components.Count switch
            {
                0 => $"0 {minUnit.ToString().ToLowerInvariant()}s",
                1 => components[0],
                2 => $"{components[0]} and {components[1]}",
                _ => $"{String.Join(", ", components[..^1])}, and {components[^1]}"
            };
        }

        public static bool IsEphemeral(this IMessage message)
        {
            return message.Flags.HasValue && message.Flags.Value.HasFlag(MessageFlags.Ephemeral);
        }
    }
}