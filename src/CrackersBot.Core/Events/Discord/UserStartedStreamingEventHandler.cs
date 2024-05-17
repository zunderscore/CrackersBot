namespace CrackersBot.Core.Events.Discord
{
    [EventId(EVENT_ID)]
    [EventName("Discord User Started Streaming")]
    [EventDescription("When a Discord user has started streaming")]
    public class UserStartedStreamingEventHandler(IBotCore bot) : EventHandlerBase(bot)
    {
        public const string EVENT_ID = "CrackersBot.Discord.UserStartedStreaming";
    }
}