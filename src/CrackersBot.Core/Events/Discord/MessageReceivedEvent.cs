namespace CrackersBot.Core.Events.Discord;

public record MessageReceivedEvent() : EventDefinition(
    EVENT_ID,
    "Discord Message Received",
    "When a Discord message is received"
)
{
    public const string EVENT_ID = "CrackersBot.Discord.MessageReceived";
}