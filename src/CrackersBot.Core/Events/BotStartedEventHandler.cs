namespace CrackersBot.Core.Events
{
    [EventId(EVENT_ID)]
    [EventName("CrackersBot Started")]
    [EventDescription("When CrackersBot first starts up and connects to Discord")]
    public class BotStartedEventHandler(IBotCore bot) : EventHandlerBase(bot)
    {
        public const string EVENT_ID = "CrackersBot.BotStarted";
    }
}