namespace CrackersBot.Core.Events.Discord
{
    [EventId(EVENT_ID)]
    [EventName("Discord User Joined")]
    [EventDescription("When a user has joined a Discord server")]
    public class UserJoinedEventHandler : EventHandlerBase
    {
        public const string EVENT_ID = "CrackersBot.Discord.UserJoined";
    }
}