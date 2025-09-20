namespace CrackersBot.Core.Events.Discord;

public record UserJoinedEvent() : EventDefinition(
    EVENT_ID,
    "Discord User Joined",
    "When a user has joined a Discord server"
)
{
    public const string EVENT_ID = "CrackersBot.Discord.UserJoined";
}