namespace CrackersBot.Core.Events.Discord
{
    [EventId(EVENT_ID)]
    [EventName("Discord Message Updated")]
    [EventDescription("When a Discord message is updated/edited")]
    public class MessageUpdatedEventHandler(IBotCore bot) : EventHandlerBase(bot)
    {
        public const string EVENT_ID = "CrackersBot.Discord.MessageUpdated";
    }
}