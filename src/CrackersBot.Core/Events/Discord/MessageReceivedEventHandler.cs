namespace CrackersBot.Core.Events.Discord
{
    [EventId(EVENT_ID)]
    [EventName("Discord Message Received")]
    [EventDescription("When a Discord message is received")]
    public class MessageReceivedEventHandler : EventHandlerBase
    {
        public const string EVENT_ID = "CrackersBot.Discord.MessageReceived";
    }
}