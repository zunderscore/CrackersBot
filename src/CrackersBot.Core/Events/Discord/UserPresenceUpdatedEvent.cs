namespace CrackersBot.Core.Events.Discord;

public record UserPresenceUpdatedEvent() : EventDefinition(
    EVENT_ID,
    "Discord User Presence Updated",
    "When a Discord user's presence (status or activities) has changed"
)
{
    public const string EVENT_ID = "CrackersBot.Discord.UserPresenceUpdated";
}