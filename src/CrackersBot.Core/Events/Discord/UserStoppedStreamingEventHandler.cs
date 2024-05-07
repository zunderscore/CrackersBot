namespace CrackersBot.Core.Events.Discord
{
    [EventId(EVENT_ID)]
    [EventName("Discord User Stopped Streaming")]
    [EventDescription("When a Discord user has stopped streaming")]
    public class UserStoppedStreamingEventHandler : EventHandlerBase
    {
        public const string EVENT_ID = "CrackersBot.Discord.UserStoppedStreaming";
    }
}