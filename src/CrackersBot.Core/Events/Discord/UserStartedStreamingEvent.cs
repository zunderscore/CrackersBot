namespace CrackersBot.Core.Events.Discord;

public record UserStartedStreamingEvent() : EventDefinition(
    EVENT_ID,
    "Discord User Started Streaming",
    "When a Discord user has started streaming"
)
{
    public const string EVENT_ID = "CrackersBot.Discord.UserStartedStreaming";
}