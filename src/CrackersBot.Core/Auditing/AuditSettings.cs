namespace CrackersBot.Core.Auditing
{
    public class AuditSettings
    {
        public ulong? AuditChannelId { get; } = null;
        public bool UserJoined { get; }
        public bool UserLeft { get; }
        public bool UserStartedStreaming { get; }
        public bool UserStoppedStreaming { get; }
    }
}