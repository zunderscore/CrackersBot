namespace CrackersBot.Core.Auditing;

public class AuditSettings
{
    public ulong? AuditChannelId { get; set; } = null;
    public IEnumerable<ulong>? IgnoreChannels { get; set; } = null;
    public bool UserJoined { get; set; }
    public bool UserLeft { get; set; }
    public bool UserStartedStreaming { get; set; }
    public bool UserStoppedStreaming { get; set; }
    public bool MessageUpdated { get; set; }
    public bool MessageDeleted { get; set; }

    public bool ShouldSendAuditMessage(ulong? channelId = null)
    {
        return channelId is null || !(IgnoreChannels?.Contains(channelId.Value) ?? false);
    }
}