namespace CrackersBot.Core.Events.Discord;

public record MessageDeletedEvent() : EventDefinition(
    EVENT_ID,
    "Discord Message Deleted",
    "When a Discord message is deleted"
)
{
    public const string EVENT_ID = "CrackersBot.Discord.MessageDeleted";
}