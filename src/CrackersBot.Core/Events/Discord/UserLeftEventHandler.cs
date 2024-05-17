namespace CrackersBot.Core.Events.Discord
{
    [EventId(EVENT_ID)]
    [EventName("Discord User Left")]
    [EventDescription("When a user has left a Discord server")]
    public class UserLeftEventHandler(IBotCore bot) : EventHandlerBase(bot)
    {
        public const string EVENT_ID = "CrackersBot.Discord.UserLeft";
    }
}