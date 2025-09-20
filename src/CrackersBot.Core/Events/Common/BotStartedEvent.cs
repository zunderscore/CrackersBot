namespace CrackersBot.Core.Events.Common;

public record BotStartedEvent() : EventDefinition(
    EVENT_ID,
    "CrackersBot Started",
    "When CrackersBot first starts up and connects to Discord"
)
{
    public const string EVENT_ID = "CrackersBot.BotStarted";
}