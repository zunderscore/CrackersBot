namespace CrackersBot.Core.Events.Discord;

public record UserLeftEvent() : EventDefinition(
    EVENT_ID,
    "Discord User Left",
    "When a user has left a Discord server"
)
{
    public const string EVENT_ID = "CrackersBot.Discord.UserLeft";
}