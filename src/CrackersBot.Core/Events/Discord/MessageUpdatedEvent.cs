namespace CrackersBot.Core.Events.Discord;

public record MessageUpdatedEvent() : EventDefinition(
    EVENT_ID,
    "Discord Message Updated",
    "When a Discord message is updated/edited"
)
{
    public const string EVENT_ID = "CrackersBot.Discord.MessageUpdated";
}