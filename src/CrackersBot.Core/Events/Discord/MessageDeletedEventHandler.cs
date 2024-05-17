namespace CrackersBot.Core.Events.Discord
{
    [EventId(EVENT_ID)]
    [EventName("Discord Message Deleted")]
    [EventDescription("When a Discord message is deleted")]
    public class MessageDeletedEventHandler(IBotCore bot) : EventHandlerBase(bot)
    {
        public const string EVENT_ID = "CrackersBot.Discord.MessageDeleted";
    }
}