namespace CrackersBot.Core.Events.Discord;

public record UserStoppedStreamingEvent() : EventDefinition(
    EVENT_ID,
    "Discord User Stopped Streaming",
    "When a Discord user has stopped streaming"
)
{
    public const string EVENT_ID = "CrackersBot.Discord.UserStoppedStreaming";
}