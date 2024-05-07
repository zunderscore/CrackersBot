namespace CrackersBot.Core.Events.Discord
{
    [EventId(EVENT_ID)]
    [EventName("Discord User Presence Updated")]
    [EventDescription("When a Discord user's presence (status or activities) has changed")]
    public class UserPresenceUpdatedEventHandler : EventHandlerBase
    {
        public const string EVENT_ID = "CrackersBot.Discord.UserPresenceUpdated";
    }
}